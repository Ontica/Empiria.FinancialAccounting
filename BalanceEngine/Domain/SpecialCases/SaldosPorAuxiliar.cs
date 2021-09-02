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

      List<TrialBalanceEntry> trialBalance = helper.GetPostingEntries().ToList();

      List<TrialBalanceEntry> summarySubsidiaryEntries = BalancesBySubsidiaryAccounts(trialBalance);

      trialBalance = CombineTotalSubsidiaryEntriesWithSummaryAccounts(summarySubsidiaryEntries);

      trialBalance = helper.RestrictLevels(trialBalance);

      var returnBalance = new FixedList<ITrialBalanceEntry>(trialBalance.Select(x => (ITrialBalanceEntry) x));

      return new TrialBalance(_command, returnBalance);
    }


    internal TrialBalance BuildForBalancesGeneration() {
      var helper = new TrialBalanceHelper(_command);

      _command.WithSubledgerAccount = true;

      FixedList<TrialBalanceEntry> trialBalance = helper.GetPostingEntries();

      var returnBalance = new FixedList<ITrialBalanceEntry>(trialBalance.Select(x => (ITrialBalanceEntry) x));

      return new TrialBalance(_command, returnBalance);
    }


    #region Helper methods


    private void AccumulateSubledgerAccount(EmpiriaHashTable<TrialBalanceEntry> returnedEntries,
                                            TrialBalanceEntry entry,
                                            StandardAccount currentParent) {

      string hash = $"{entry.Currency.Code}||{currentParent.Number}";

      if (returnedEntries.ContainsKey(hash)) {
        if (returnedEntries[hash].SubledgerAccountIdParent == entry.SubledgerAccountId &&
            returnedEntries[hash].NotHasSector) {
          returnedEntries[hash].Sum(entry);
        }
      }
    }


    private List<TrialBalanceEntry> AddSummaryAccounts(List<TrialBalanceEntry> summaryEntries,
                                                       List<TrialBalanceEntry> accumulatedEntries,
                                                       TrialBalanceEntry entry) {

      var summaryAccounts = summaryEntries.Where(
              a => a.Account.GroupNumber == entry.Account.GroupNumber &&
              a.SubledgerAccountId == 0 &&
              a.SubledgerAccountIdParent == entry.SubledgerAccountIdParent &&
              a.Ledger.Number == entry.Ledger.Number &&
              a.Currency.Code == entry.Currency.Code &&
              a.ItemType == TrialBalanceItemType.BalanceEntry).ToList();

      var hashReturnedEntries = new EmpiriaHashTable<TrialBalanceEntry>();
      string hashEntry = string.Empty;

      foreach (var item in accumulatedEntries) {
        hashEntry = $"{item.Ledger.Number}||{item.Currency.Code}||{item.Account.Number}||" +
                    $"{item.Sector.Code}||{item.SubledgerAccountIdParent}";

        hashReturnedEntries.Insert(hashEntry, item);
      }

      foreach (var summary in summaryAccounts) {
        hashEntry = $"{summary.SubledgerAccountIdParent}||" +
                    $"{summary.Account.Number}||{summary.Ledger.Number}||" +
                    $"{summary.Currency.Code}||{summary.Sector.Code}||";

        if (!hashReturnedEntries.ContainsKey(hashEntry)) {
          hashReturnedEntries.Insert(hashEntry, summary);
        }
      }
      var returnedEntries = new FixedList<TrialBalanceEntry>(hashReturnedEntries.ToFixedList());

      return returnedEntries.ToList();
    }


    private List<TrialBalanceEntry> BalancesBySubsidiaryAccounts(List<TrialBalanceEntry> trialBalance) {

      List<TrialBalanceEntry> subsidiaryEntries = new List<TrialBalanceEntry>(trialBalance);

      subsidiaryEntries = subsidiaryEntries.Where(a => a.SubledgerAccountId > 0).ToList();

      List<TrialBalanceEntry> returnedSubsidiaryEntries = GenerateOrIncreaseParentEntries(subsidiaryEntries);
      
      return returnedSubsidiaryEntries;
    }


    private void CreateOrAccumulateParentWithoutSector(
                  EmpiriaHashTable<TrialBalanceEntry> summaryParentEntries,
                  EmpiriaHashTable<TrialBalanceEntry> hashReturnedEntries,
                  TrialBalanceEntry entry,
                  StandardAccount currentParent) {

      var helper = new TrialBalanceHelper(_command);

      string hash = $"{entry.Ledger.Number}||{entry.Currency.Code}||" +
                    $"{currentParent.Number}||{Sector.Empty.Code}||" +
                    $"{entry.SubledgerAccountIdParent}";

      if (hashReturnedEntries.ContainsKey(hash)) {
        if (hashReturnedEntries[hash].SubledgerAccountIdParent == entry.SubledgerAccountId &&
            hashReturnedEntries[hash].NotHasSector) {
          hashReturnedEntries[hash].Sum(entry);
        }
      } else {
        helper.SummaryByEntry(summaryParentEntries, entry, currentParent, Sector.Empty,
                             TrialBalanceItemType.BalanceSummary);
      }

    }


    private List<TrialBalanceEntry> CreateOrAccumulateTotalBySubsidiaryEntry(
                                      List<TrialBalanceEntry> returnedEntries,
                                      TrialBalanceEntry entry) {
      var helper = new TrialBalanceHelper(_command);

      var parentEntries = new EmpiriaHashTable<TrialBalanceEntry>();
      var hashReturnedEntries = new EmpiriaHashTable<TrialBalanceEntry>();

      foreach (var item in returnedEntries) {
        string hashEntry = $"{item.Ledger.Number}||{item.Currency.Code}||" +
                        $"{item.Account.Number}||{item.Sector.Code}||" +
                        $"{item.SubledgerAccountIdParent}";

        hashReturnedEntries.Insert(hashEntry, item);
      }

      string hash = $"{entry.Ledger.Number}||{entry.Currency.Code}||" +
                    $"{StandardAccount.Empty.Number}||{Sector.Empty.Code}||" +
                    $"{entry.SubledgerAccountIdParent}";

      if (hashReturnedEntries.ContainsKey(hash)) {

        if (hashReturnedEntries[hash].SubledgerAccountId == entry.SubledgerAccountIdParent) {

          if (entry.LastChangeDate > hashReturnedEntries[hash].LastChangeDate) {
            hashReturnedEntries[hash].LastChangeDate = entry.LastChangeDate;
          }
          hashReturnedEntries[hash].Sum(entry);
        }

      } else {
        helper.SummaryBySubsidiaryEntry(parentEntries, entry, TrialBalanceItemType.BalanceSummary);

        var parent = parentEntries.Values.FirstOrDefault();
        parent.SubledgerAccountId = parent.SubledgerAccountIdParent;
        parent.SubledgerAccountNumber = entry.SubledgerAccountNumber;
        parent.LastChangeDate = entry.LastChangeDate;

        string hashParent = $"{parent.Ledger.Number}||{parent.Currency.Code}||" +
                            $"{parent.Account.Number}||{parent.Sector.Code}||" +
                            $"{parent.SubledgerAccountIdParent}";

        hashReturnedEntries.Insert(hashParent, parent);
      }

      return hashReturnedEntries.Values.ToList();
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


    private List<TrialBalanceEntry> GenerateOrIncreaseParentEntries(List<TrialBalanceEntry> subsidiaryEntries) {
      var helper = new TrialBalanceHelper(_command);

      EmpiriaHashTable<TrialBalanceEntry> hashReturnedEntries = new EmpiriaHashTable<TrialBalanceEntry>();

      
      foreach (var entry in subsidiaryEntries) {
        entry.DebtorCreditor = entry.Account.DebtorCreditor;
        entry.SubledgerAccountIdParent = entry.SubledgerAccountId;
        
        var summaryParentEntries = new EmpiriaHashTable<TrialBalanceEntry>();

        StandardAccount account = entry.Account;

        int count = 0;
        string hash = string.Empty;
        while (true) {

          hash = $"{entry.Ledger.Number}||{entry.Currency.Code}||" +
                        $"{account.Number}||{entry.Sector.Code}";

          if (hashReturnedEntries.ContainsKey(hash)) {
            if (hashReturnedEntries[hash].SubledgerAccountIdParent == entry.SubledgerAccountId &&
                hashReturnedEntries[hash].SubledgerAccountId == 0){
              hashReturnedEntries[hash].Sum(entry);
            }
            
            if (!account.HasParent) {
              if (entry.HasSector) {
                AccumulateSubledgerAccount(hashReturnedEntries, entry, account);
              }
              break;
            } else {
              account = account.GetParent();
            }
          } else {
            count++;
            TrialBalanceItemType itemType = count == 1 ? TrialBalanceItemType.BalanceEntry :
                                                         TrialBalanceItemType.BalanceSummary;
            helper.SummaryByEntry(summaryParentEntries, entry, account, entry.Sector, itemType);

            if (!account.HasParent && entry.HasSector && entry.SubledgerAccountId > 0) {
              CreateOrAccumulateParentWithoutSector(summaryParentEntries, hashReturnedEntries, entry, account);
              break;
            } else if (!account.HasParent) {
              break;
            } else {
              account = account.GetParent();
            }
          }

        } // while
        
        foreach (var item in summaryParentEntries.Values.ToList()) {
          hash = $"{item.Ledger.Number}||{item.Currency.Code}||" +
                        $"{item.Account.Number}||{item.Sector.Code}||"+
                        $"{item.SubledgerAccountIdParent}";

          if (hashReturnedEntries.ContainsKey(hash)) {
            hashReturnedEntries[hash].Sum(item);
          } else {
            hashReturnedEntries.Insert(hash, item);
          }
          
        }
      } // foreach

      return hashReturnedEntries.Values.ToList();
    }

    private List<TrialBalanceEntry> OrderingSubsidiaryAccountsByNumber(
                    List<TrialBalanceEntry> totaBySubsidiaryAccountList) {
      var returnedEntries = totaBySubsidiaryAccountList.Where(a => a.Level == 1 && a.NotHasSector).ToList();

      returnedEntries = AssignSubsidiaryNumber(returnedEntries).ToList();
      
      returnedEntries = returnedEntries.Where(a => !a.SubledgerAccountNumber.Contains("undefined"))
                                       .OrderBy(a => a.Currency.Code)
                                       .ThenBy(a => a.SubledgerNumberOfDigits)
                                       .ThenBy(a => a.SubledgerAccountNumber)
                                       .ToList();
      return returnedEntries;
    }

    private List<TrialBalanceEntry> AssignSubsidiaryNumber(List<TrialBalanceEntry> returnedEntries) {
      SubsidiaryAccount subsidiary;

      foreach (var entry in returnedEntries) {

        subsidiary = SubsidiaryAccount.Parse(entry.SubledgerAccountIdParent);
        entry.SubledgerAccountNumber = subsidiary.Number ?? "";
        entry.GroupName = subsidiary.Name ?? "";
        entry.SubledgerNumberOfDigits = entry.SubledgerAccountNumber != "" ?
                                 entry.SubledgerAccountNumber.Count() : 0;
      }

      return returnedEntries;
    }

    #endregion Helper methods

  }  // class SaldosPorAuxiliar

}  // namespace Empiria.FinancialAccounting.BalanceEngine
