﻿/* Empiria Financial *****************************************************************************************
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


    #region Helper methods

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
      var helper = new TrialBalanceHelper(_command);

      List<TrialBalanceEntry> returnedEntries = new List<TrialBalanceEntry>();

      foreach (var entry in subsidiaryEntries) {

        List<TrialBalanceEntry> summaryEntries = new List<TrialBalanceEntry>();
        var summaryParentEntries = new EmpiriaHashTable<TrialBalanceEntry>();

        StandardAccount account = entry.Account;

        while (true) {
          var parent = returnedEntries.FirstOrDefault(a => a.Ledger.Number == entry.Ledger.Number &&
                            a.Currency.Code == entry.Currency.Code && a.SubledgerAccountId == 0 &&
                            a.SubledgerAccountIdParent == entry.SubledgerAccountId &&
                            a.Account.Number == account.Number && a.Sector.Code == entry.Sector.Code);

          if (parent == null) {
            helper.SummaryByEntry(summaryParentEntries, entry, account, entry.Sector,
                                  TrialBalanceItemType.BalanceSummary);

            if (!account.HasParent && entry.HasSector && entry.SubledgerAccountId > 0) {
              CreateOrAccumulateParentWithoutSector(returnedEntries, entry,
                                                    summaryParentEntries,
                                                    account);
              break;
            } else if (!account.HasParent) {
              break;
            } else {
              account = account.GetParent();
            }
          } else {
            parent.Sum(entry);

            if (!account.HasParent) {
              AccumulateSubledgerAccount(returnedEntries, entry, account);
              break;
            } else {
              account = account.GetParent();
            }
          }

        } // while

        summaryEntries.AddRange(summaryParentEntries.Values.ToList());
        returnedEntries.AddRange(summaryEntries);

      } // foreach

      return returnedEntries.OrderBy(a => a.Ledger.Number)
                            .ThenBy(a => a.Currency.Code)
                            .ThenByDescending(a => a.SubledgerAccountIdParent)
                            .ThenBy(a => a.Account.Number).ThenBy(a => a.SubledgerAccountId)
                            .ThenBy(a => a.Sector.Code)
                            .ToList();
    }


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


    private void CreateOrAccumulateParentWithoutSector(List<TrialBalanceEntry> returnedEntries,
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

    private List<TrialBalanceEntry> CombineTotalSubsidiaryEntriesWithSummaryAccounts(
                                         List<TrialBalanceEntry> summaryEntries) {

      List<TrialBalanceEntry> returnedEntries = new List<TrialBalanceEntry>();

      var totaBySubsidiaryAccountList = summaryEntries.Where(a => a.Level == 1 && a.NotHasSector).ToList();

      foreach (var entry in totaBySubsidiaryAccountList.OrderBy(a => a.Currency.Code)) {

        entry.SubledgerAccountId = entry.SubledgerAccountIdParent;
        var summaryAccounts = summaryEntries.Where(
                               a => a.SubledgerAccountIdParent == entry.SubledgerAccountIdParent &&
                                    a.Ledger.Number == entry.Ledger.Number &&
                                    a.Currency.Code == entry.Currency.Code).ToList();

        if (summaryAccounts.Count > 0) {
          returnedEntries.AddRange(summaryAccounts);
        }
      }

      return returnedEntries;
    }
    #endregion Helper methods

  }  // class SaldosPorAuxiliar

}  // namespace Empiria.FinancialAccounting.BalanceEngine