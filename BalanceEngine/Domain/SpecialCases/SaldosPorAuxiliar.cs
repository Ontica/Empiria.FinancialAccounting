/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Service provider                        *
*  Type     : SaldosPorAuxiliar                          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Genera los datos para el reporte de saldos por auxiliar.                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;

using Empiria.Collections;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Genera los datos para el reporte de saldos por auxiliar.</summary>
  internal class SaldosPorAuxiliar {

    private readonly TrialBalanceCommand _command;

    public SaldosPorAuxiliar(TrialBalanceCommand command) {
      _command = command;
    }


    internal TrialBalance Build() {
      var helper = new TrialBalanceHelper(_command);

      List<TrialBalanceEntry> trialBalance = helper.GetSummaryAndPostingEntries();

      List<TrialBalanceEntry> summarySubsidiaryEntries = BalancesBySubsidiaryAccounts(trialBalance);

      trialBalance = CombineTotalSubsidiaryEntriesWithSummaryAccounts(summarySubsidiaryEntries);

      trialBalance = helper.RestrictLevels(trialBalance);

      var returnBalance = new FixedList<ITrialBalanceEntry>(trialBalance.Select(x => (ITrialBalanceEntry) x));

      return new TrialBalance(_command, returnBalance);
    }


    internal TrialBalance BuildForBalancesGeneration() {
      var helper = new TrialBalanceHelper(_command);

      FixedList<TrialBalanceEntry> trialBalance = helper.GetPostingEntries();

      var returnBalance = new FixedList<ITrialBalanceEntry>(trialBalance.Select(x => (ITrialBalanceEntry) x));

      return new TrialBalance(_command, returnBalance);
    }


    #region Helper methods

    private void AccumulateSubledgerAccount(List<TrialBalanceEntry> returnedEntries,
                                            TrialBalanceEntry entry,
                                            StandardAccount currentParent) {
      var existTotalBySubledger = returnedEntries.FirstOrDefault(
                                         a => a.SubledgerAccountIdParent == entry.SubledgerAccountId &&
                                         a.Currency.Code == entry.Currency.Code &&
                                         a.Account.Number == currentParent.Number &&
                                         a.NotHasSector);

      if (existTotalBySubledger != null) {
        existTotalBySubledger.Sum(entry);
      }
    }


    private List<TrialBalanceEntry> AddSummaryAccounts(List<TrialBalanceEntry> summaryEntries,
                                                       List<TrialBalanceEntry> returnedEntries,
                                                       TrialBalanceEntry entry) {
      var summaryAccounts = summaryEntries.Where(
                               a => a.Account.GroupNumber == entry.Account.GroupNumber &&
                                    a.SubledgerAccountId == 0 &&
                                    a.SubledgerAccountIdParent == entry.SubledgerAccountIdParent &&
                                    a.Ledger.Number == entry.Ledger.Number &&
                                    a.Currency.Code == entry.Currency.Code &&
                                    a.ItemType == TrialBalanceItemType.BalanceEntry).ToList();

      foreach (var summary in summaryAccounts) {
        var existSummaryAccount = returnedEntries.FirstOrDefault(
                                    a => a.SubledgerAccountIdParent == entry.SubledgerAccountIdParent &&
                                         a.Account.Number == summary.Account.Number &&
                                         a.Ledger.Number == summary.Ledger.Number &&
                                         a.Currency.Code == summary.Currency.Code &&
                                         a.Sector.Code == summary.Sector.Code);
        if (existSummaryAccount == null) {
          returnedEntries.Add(summary);
        }
      }

      return returnedEntries;
    }


    private List<TrialBalanceEntry> BalancesBySubsidiaryAccounts(List<TrialBalanceEntry> trialBalance) {
      List<TrialBalanceEntry> returnedSubsidiaryEntries = new List<TrialBalanceEntry>();

      foreach (var entry in trialBalance) {
        if (entry.SubledgerAccountId > 0) {
          returnedSubsidiaryEntries.Add(entry);
        }
      }

      returnedSubsidiaryEntries = returnedSubsidiaryEntries.OrderBy(a => a.Ledger.Number)
                                                           .ThenBy(a => a.Currency.Code)
                                                           .ThenBy(a => a.Account.Number)
                                                           .ThenBy(a => a.Sector.Code)
                                                           .ToList();

      returnedSubsidiaryEntries = CombineSubsidiaryEntriesAndSummaryAccounts(returnedSubsidiaryEntries);

      return returnedSubsidiaryEntries;
    }


    private List<TrialBalanceEntry> CombineSubsidiaryEntriesAndSummaryAccounts(
                                List<TrialBalanceEntry> subsidiaryEntries) {
      List<TrialBalanceEntry> returnedEntries = new List<TrialBalanceEntry>();
      GenerateOrIncreaseParentEntries(subsidiaryEntries, returnedEntries);

      return returnedEntries.OrderBy(a => a.Ledger.Number)
                            .ThenBy(a => a.Currency.Code)
                            .ThenByDescending(a => a.SubledgerAccountIdParent)
                            .ThenBy(a => a.Account.Number).ThenBy(a => a.SubledgerAccountId)
                            .ThenBy(a => a.Sector.Code)
                            .ToList();
    }


    private void CreateOrAccumulateParentWithoutSector(
                  List<TrialBalanceEntry> returnedEntries,
                  TrialBalanceEntry entry,
                  EmpiriaHashTable<TrialBalanceEntry> summaryParentEntries,
                  StandardAccount currentParent) {
      var helper = new TrialBalanceHelper(_command);

      var entryWithoutSector = returnedEntries.FirstOrDefault(
                                       a => a.Ledger.Number == entry.Ledger.Number &&
                                       a.Currency.Code == entry.Currency.Code &&
                                       a.SubledgerAccountIdParent == entry.SubledgerAccountId &&
                                       a.Account.Number == currentParent.Number &&
                                       a.NotHasSector);


      if (entryWithoutSector == null) {
        helper.SummaryByEntry(summaryParentEntries, entry, currentParent, Sector.Empty,
                             TrialBalanceItemType.BalanceSummary);
      } else {
        entryWithoutSector.Sum(entry);
      }
    }

    private List<TrialBalanceEntry> CreateOrAccumulateTotalBySubsidiaryEntry(
                                      List<TrialBalanceEntry> returnedEntries,
                                      TrialBalanceEntry entry) {
      var helper = new TrialBalanceHelper(_command);

      var parentEntries = new EmpiriaHashTable<TrialBalanceEntry>();

      var existTotalByAccount = returnedEntries.FirstOrDefault(
                                  a => a.SubledgerAccountId == entry.SubledgerAccountIdParent &&
                                  a.Ledger.Number == entry.Ledger.Number &&
                                  a.Currency.Code == entry.Currency.Code);

      if (existTotalByAccount == null) {
        helper.SummaryBySubsidiaryEntry(parentEntries, entry, TrialBalanceItemType.BalanceSummary);

        var parent = parentEntries.Values.FirstOrDefault();
        parent.SubledgerAccountId = parent.SubledgerAccountIdParent;
        parent.SubledgerAccountNumber = SubsidiaryAccount.Parse(entry.SubledgerAccountIdParent).Number ?? "";
        returnedEntries.Add(parent);
      } else {
        existTotalByAccount.Sum(entry);
      }

      return returnedEntries;
    }


    private List<TrialBalanceEntry> CombineTotalSubsidiaryEntriesWithSummaryAccounts(
                                         List<TrialBalanceEntry> summaryEntries) {
      List<TrialBalanceEntry> returnedEntries = new List<TrialBalanceEntry>();
      var totaBySubsidiaryAccountList = OrderingSubsidiaryAccountsByNumber(summaryEntries);

      foreach (var entry in totaBySubsidiaryAccountList) {
        returnedEntries = CreateOrAccumulateTotalBySubsidiaryEntry(returnedEntries, entry);
        returnedEntries = AddSummaryAccounts(summaryEntries, returnedEntries, entry);
      }

      return returnedEntries;
    }


    private void GenerateOrIncreaseParentEntries(List<TrialBalanceEntry> subsidiaryEntries,
                                                  List<TrialBalanceEntry> returnedEntries) {
      var helper = new TrialBalanceHelper(_command);

      foreach (var entry in subsidiaryEntries) {
        int count = 0;
        List<TrialBalanceEntry> summaryEntries = new List<TrialBalanceEntry>();
        var summaryParentEntries = new EmpiriaHashTable<TrialBalanceEntry>();
        StandardAccount account = entry.Account;

        while (true) {
          var parent = returnedEntries.FirstOrDefault(a => a.Ledger.Number == entry.Ledger.Number &&
                            a.Currency.Code == entry.Currency.Code && a.SubledgerAccountId == 0 &&
                            a.SubledgerAccountIdParent == entry.SubledgerAccountId &&
                            a.Account.Number == account.Number && a.Sector.Code == entry.Sector.Code);

          if (parent == null) {
            count++;
            TrialBalanceItemType itemType = count == 1 ? TrialBalanceItemType.BalanceEntry :
                                                         TrialBalanceItemType.BalanceSummary;
            helper.SummaryByEntry(summaryParentEntries, entry, account, entry.Sector, itemType);

            if (!account.HasParent && entry.HasSector && entry.SubledgerAccountId > 0) {
              CreateOrAccumulateParentWithoutSector(returnedEntries, entry, summaryParentEntries, account);
              break;
            } else if (!account.HasParent) {
              break;
            } else {
              account = account.GetParent();
            }
          } else {
            parent.Sum(entry);

            if (!account.HasParent) {
              if (entry.HasSector) {
                AccumulateSubledgerAccount(returnedEntries, entry, account);
              }
              break;
            } else {
              account = account.GetParent();
            }
          }
        } // while
        summaryEntries.AddRange(summaryParentEntries.Values.ToList());
        returnedEntries.AddRange(summaryEntries);
      } // foreach

    }


    private List<TrialBalanceEntry> OrderingSubsidiaryAccountsByNumber(
                    List<TrialBalanceEntry> totaBySubsidiaryAccountList) {
      var returnedEntries = new List<TrialBalanceEntry>();

      foreach (var entry in totaBySubsidiaryAccountList.Where(a => a.Level == 1 && a.NotHasSector)) {
        entry.SubledgerAccountNumber = SubsidiaryAccount.Parse(entry.SubledgerAccountIdParent).Number ?? "";
        entry.SubledgerNumber = entry.SubledgerAccountNumber != "" ?
                                 Convert.ToInt64(entry.SubledgerAccountNumber) : 0;
        returnedEntries.Add(entry);
      }

      returnedEntries = returnedEntries.OrderBy(a => a.Currency.Code)
                                       .ThenBy(a => a.SubledgerNumber)
                                       .ToList();
      return returnedEntries;
    }

    #endregion Helper methods

  }  // class SaldosPorAuxiliar

}  // namespace Empiria.FinancialAccounting.BalanceEngine
