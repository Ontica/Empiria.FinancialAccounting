/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Service provider                        *
*  Type     : SaldosPorCuentaYMayores                    License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Genera los datos para el reporte de saldos por cuenta y mayores.                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;

using Empiria.Collections;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Genera los datos para el reporte de saldos por cuenta y mayores.</summary>
  internal class SaldosPorCuentaYMayores {

    private readonly TrialBalanceCommand _command;

    public SaldosPorCuentaYMayores(TrialBalanceCommand command) {
      _command = command;
    }


    internal TrialBalance Build() {
      var helper = new TrialBalanceHelper(_command);

      FixedList<TrialBalanceEntry> postingEntries = helper.GetPostingEntries();

      List<TrialBalanceEntry> trialBalance = new List<TrialBalanceEntry>(postingEntries);

      List<TrialBalanceEntry> orderingTrialBalance = OrderingLedgersByAccount(trialBalance);

      FixedList<TrialBalanceEntry> summaryByAccountEntries = GenerateTotalSummaryByGroup(orderingTrialBalance);

      trialBalance = CombineGroupEntriesAndBalanceEntries(trialBalance,
                                                          summaryByAccountEntries);

      List<TrialBalanceEntry> summaryDebtorCreditorEntries =
                                GetSummaryByDebtorCreditor(orderingTrialBalance);

      trialBalance = CombineTotalDebtorsCreditorsWithBalanceEntries(trialBalance,
                                                                    summaryDebtorCreditorEntries);

      FixedList<TrialBalanceEntry> summaryTotalByCurrency = GenerateTotalSummaryByCurrency(
                                                            summaryDebtorCreditorEntries);

      trialBalance = CombineTotalByCurrencyAndSummaryEntries(trialBalance, summaryTotalByCurrency);

      List<TrialBalanceEntry> summaryTotalConsolidated = helper.GenerateTotalSummaryConsolidated(
                                                                       trialBalance.ToList());

      trialBalance = helper.CombineTotalConsolidatedAndPostingEntries(trialBalance, summaryTotalConsolidated);

      trialBalance = helper.GenerateAverageDailyBalance(trialBalance, _command.InitialPeriod);

      trialBalance = helper.RestrictLevels(trialBalance);

      var returnBalance = new FixedList<ITrialBalanceEntry>(trialBalance.Select(x => (ITrialBalanceEntry) x));

      return new TrialBalance(_command, returnBalance);
    }

    

    #region Helper methods

    private List<TrialBalanceEntry> CombineGroupEntriesAndBalanceEntries(
                                      List<TrialBalanceEntry> balanceEntries,
                                      FixedList<TrialBalanceEntry> summaryByAccountEntries) {
      var returnedEntries = new List<TrialBalanceEntry>();

      summaryByAccountEntries = OrderingTotalsByGroup(summaryByAccountEntries).ToFixedList();

      foreach (var totalGroupDebtorEntry in summaryByAccountEntries) {
        var entries = balanceEntries.Where(
                                  a => a.Account.Number == totalGroupDebtorEntry.GroupNumber &&
                                  a.Currency.Id == totalGroupDebtorEntry.Currency.Id &&
                                  a.Sector.Code == totalGroupDebtorEntry.Sector.Code).ToList();
        foreach (var entry in entries) {
          if (entry.LastChangeDate > totalGroupDebtorEntry.LastChangeDate) {
            totalGroupDebtorEntry.LastChangeDate = entry.LastChangeDate;
          }
        }
        totalGroupDebtorEntry.GroupNumber = "";
        entries.Add(totalGroupDebtorEntry);
        returnedEntries.AddRange(entries);
      }

      return returnedEntries;
    }


    private List<TrialBalanceEntry> CombineTotalByCurrencyAndSummaryEntries(
                                    List<TrialBalanceEntry> trialBalance,
                                     FixedList<TrialBalanceEntry> summaryTotalByCurrency) {

      var returnedEntries = new List<TrialBalanceEntry>();

      foreach (var currencyEntry in summaryTotalByCurrency
                    .Where(a => a.ItemType == TrialBalanceItemType.BalanceTotalCurrency)) {

        var listSummaryByCurrency = trialBalance.Where(a => a.Currency.Code == currencyEntry.Currency.Code).ToList();
        if (listSummaryByCurrency.Count > 0) {
          listSummaryByCurrency.Add(currencyEntry);
          returnedEntries.AddRange(listSummaryByCurrency);
        }
      }
      return returnedEntries;
    }


    private List<TrialBalanceEntry> CombineTotalDebtorsCreditorsWithBalanceEntries(
                                    List<TrialBalanceEntry> balanceEntries,
                                     List<TrialBalanceEntry> summaryDebtorCreditorEntries) {

      var returnedEntries = new List<TrialBalanceEntry>();

      foreach (var totalGroupDebtorEntry in summaryDebtorCreditorEntries) {
        var entries = balanceEntries.Where(a => a.Currency.Id == totalGroupDebtorEntry.Currency.Id &&
                                                 a.DebtorCreditor == totalGroupDebtorEntry.DebtorCreditor
                                                 ).ToList();

        entries.Add(totalGroupDebtorEntry);
        returnedEntries.AddRange(entries);
      }

      return returnedEntries;
    }


    private FixedList<TrialBalanceEntry> GenerateTotalSummaryByCurrency(
                                          List<TrialBalanceEntry> entries) {
      var helper = new TrialBalanceHelper(_command);
      var totalByCurrencies = new EmpiriaHashTable<TrialBalanceEntry>(entries.Count);

      foreach (var entry in entries.Where(a => a.ItemType == TrialBalanceItemType.BalanceTotalDebtor ||
                                               a.ItemType == TrialBalanceItemType.BalanceTotalCreditor)) {

        helper.SummaryByCurrencyEntries(totalByCurrencies, entry, StandardAccount.Empty,
                            Sector.Empty, TrialBalanceItemType.BalanceTotalCurrency);
      }
      
      return totalByCurrencies.ToFixedList();
    }


    private FixedList<TrialBalanceEntry> GenerateTotalSummaryByGroup(List<TrialBalanceEntry> trialBalance) {
      var helper = new TrialBalanceHelper(_command);
      var totalsListByGroupEntries = new EmpiriaHashTable<TrialBalanceEntry>(trialBalance.Count);
      
      foreach (var entry in trialBalance) {
        helper.SummaryByLedgersGroupEntries(totalsListByGroupEntries, entry);
      }
      
      return totalsListByGroupEntries.ToFixedList();
    }


    private List<TrialBalanceEntry> GetSummaryByDebtorCreditor(List<TrialBalanceEntry> trialBalance) {
      var helper = new TrialBalanceHelper(_command);
      var totalSummaryDebtorCredtor = new EmpiriaHashTable<TrialBalanceEntry>(trialBalance.Count);

      foreach (var entry in trialBalance) {

        if (entry.DebtorCreditor == DebtorCreditorType.Deudora) {
          helper.SummaryDebtorCreditorLedgersByAccount(totalSummaryDebtorCredtor, entry,
                                                       TrialBalanceItemType.BalanceTotalDebtor);
        }
        if (entry.DebtorCreditor == DebtorCreditorType.Acreedora) {
          helper.SummaryDebtorCreditorLedgersByAccount(totalSummaryDebtorCredtor, entry,
                                                       TrialBalanceItemType.BalanceTotalCreditor);
        }
      }

      return OrderingDeptorCreditorEntries(totalSummaryDebtorCredtor.Values.ToList());
    }


    private List<TrialBalanceEntry> OrderingDeptorCreditorEntries(
                                     List<TrialBalanceEntry> trialBalanceEntries) {
      return trialBalanceEntries.OrderBy(a => a.Currency.Code)
                                .ThenByDescending(a => a.DebtorCreditor)
                                .ToList();
    }


    private List<TrialBalanceEntry> OrderingLedgersByAccount(List<TrialBalanceEntry> trialBalance) {
      List<TrialBalanceEntry> returnedList = new List<TrialBalanceEntry>(trialBalance);
      
      foreach (var entry in trialBalance) {
        entry.GroupName = entry.Ledger.Name;
        entry.DebtorCreditor = entry.Account.DebtorCreditor;
      }

      returnedList = returnedList.OrderBy(a => a.Currency.Code)
                                 .ThenBy(a => a.Account.Number)
                                 .ThenBy(a => a.Sector.Code)
                                 .ThenByDescending(a => a.Account.DebtorCreditor)
                                 .ThenBy(a => a.Ledger.Number)
                                 .ToList();
      return returnedList;
    }


    private List<TrialBalanceEntry> OrderingTotalsByGroup(FixedList<TrialBalanceEntry> summaryGroupEntries) {
      var returnedList = new List<TrialBalanceEntry>(summaryGroupEntries);
      return returnedList.OrderBy(a => a.Currency.Code)
                         .ThenBy(a => a.GroupNumber)
                         .ThenBy(a => a.Sector.Code)
                         .ToList();
    }

    #endregion Helper methods

  }  // class AnaliticoDeCuentas

}  // namespace Empiria.FinancialAccounting.BalanceEngine
