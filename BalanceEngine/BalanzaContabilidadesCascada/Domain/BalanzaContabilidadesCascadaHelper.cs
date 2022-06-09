/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Helper methods                          *
*  Type     : BalanceCascadeAccountingHelper             License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Helper methods to build balance with cascade accounting.                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;

using Empiria.Collections;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Helper methods to build balance with cascade accounting.</summary>
  internal class BalanzaContabilidadesCascadaHelper {

    private readonly TrialBalanceQuery _query;

    public BalanzaContabilidadesCascadaHelper(TrialBalanceQuery query) {
      _query = query;
    }


    internal List<TrialBalanceEntry> CombineGroupAndBalanceEntries(
                                      List<TrialBalanceEntry> balanceEntries,
                                      FixedList<TrialBalanceEntry> summaryByAccountEntries) {

      var returnedEntries = new List<TrialBalanceEntry>();

      summaryByAccountEntries = OrderingTotalsByGroup(summaryByAccountEntries).ToFixedList();

      foreach (var totalGroupDebtorEntry in summaryByAccountEntries) {

        var entries = balanceEntries.Where(a => a.Account.Number == totalGroupDebtorEntry.GroupNumber &&
                                           a.Currency.Id == totalGroupDebtorEntry.Currency.Id &&
                                           a.Sector.Code == totalGroupDebtorEntry.Sector.Code)
                                    .ToList();
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


    internal List<TrialBalanceEntry> CombineTotalByCurrencyAndBalanceEntries(
                                    List<TrialBalanceEntry> balanceEntries,
                                     FixedList<TrialBalanceEntry> summaryTotalByCurrency) {

      var returnedEntries = new List<TrialBalanceEntry>();

      foreach (var totalByCurrency in summaryTotalByCurrency
                    .Where(a => a.ItemType == TrialBalanceItemType.BalanceTotalCurrency)) {

        var entriesByCurrency = balanceEntries.Where(
                                a => a.Currency.Code == totalByCurrency.Currency.Code).ToList();

        if (entriesByCurrency.Count > 0) {
          entriesByCurrency.Add(totalByCurrency);
          returnedEntries.AddRange(entriesByCurrency);
        }
      }
      return returnedEntries;
    }


    internal List<TrialBalanceEntry> CombineTotalDebtorCreditorAndEntries(
                                    List<TrialBalanceEntry> balanceEntries,
                                     List<TrialBalanceEntry> summaryDebtorCreditorEntries) {

      var returnedEntries = new List<TrialBalanceEntry>();

      foreach (var totalGroupDebtorEntry in summaryDebtorCreditorEntries) {
        var entries = balanceEntries.Where(a => a.Currency.Id == totalGroupDebtorEntry.Currency.Id &&
                                           a.DebtorCreditor == totalGroupDebtorEntry.DebtorCreditor)
                                    .ToList();

        entries.Add(totalGroupDebtorEntry);
        returnedEntries.AddRange(entries);
      }
      return returnedEntries;
    }


    internal List<TrialBalanceEntry> GenerateTotalReport(
                                      List<TrialBalanceEntry> balanceEntries) {
      var totalReport = new EmpiriaHashTable<TrialBalanceEntry>();

      foreach (var currencyEntry in balanceEntries.Where(
                a => a.ItemType == TrialBalanceItemType.BalanceTotalCurrency)) {

        var entry = currencyEntry.CreatePartialCopy();

        entry.GroupNumber = "";
        entry.GroupName = "TOTAL DEL REPORTE";
        string hash = $"{entry.GroupName}";

        GetOrIncreaseEntries(totalReport, entry, StandardAccount.Empty, Sector.Empty,
                             TrialBalanceItemType.BalanceTotalConsolidated, hash);
      } // foreach

      balanceEntries.AddRange(totalReport.Values.ToList());

      return balanceEntries;
    }


    internal FixedList<TrialBalanceEntry> GenerateTotalSummaryByCurrency(
                                          List<TrialBalanceEntry> entries) {

      var totalByCurrencies = new EmpiriaHashTable<TrialBalanceEntry>(entries.Count);

      foreach (var entry in entries.Where(a => !a.HasParentPostingEntry &&
                            (a.ItemType == TrialBalanceItemType.BalanceTotalDebtor ||
                            a.ItemType == TrialBalanceItemType.BalanceTotalCreditor))) {

        SummaryByCurrency(totalByCurrencies, entry);
      }
      return totalByCurrencies.ToFixedList();
    }


    internal FixedList<TrialBalanceEntry> GenerateTotalSummaryByGroup(List<TrialBalanceEntry> trialBalance) {

      var totalsByGroupEntries = new EmpiriaHashTable<TrialBalanceEntry>(trialBalance.Count);

      foreach (var entry in trialBalance) {
        SummaryByLedgerGroupEntries(totalsByGroupEntries, entry);
      }

      return totalsByGroupEntries.ToFixedList();
    }


    internal List<TrialBalanceEntry> GetAverageBalance(List<TrialBalanceEntry> trialBalance) {
      var returnedEntries = new List<TrialBalanceEntry>(trialBalance);

      if (_query.WithAverageBalance) {

        foreach (var entry in returnedEntries.Where(a =>
                     a.ItemType == TrialBalanceItemType.BalanceTotalGroupDebtor ||
                     a.ItemType == TrialBalanceItemType.BalanceTotalGroupCreditor)) {

          decimal debtorCreditor = 0;

          if (entry.DebtorCreditor == DebtorCreditorType.Deudora) {
            debtorCreditor = entry.Debit - entry.Credit;

          }

          if (entry.DebtorCreditor == DebtorCreditorType.Acreedora) {
            debtorCreditor = entry.Credit - entry.Debit;

          }

          TimeSpan timeSpan = _query.InitialPeriod.ToDate - entry.LastChangeDate;
          int numberOfDays = timeSpan.Days + 1;

          entry.AverageBalance = ((numberOfDays * debtorCreditor) /
                                   _query.InitialPeriod.ToDate.Day) +
                                   entry.InitialBalance;
        }
      }

      return returnedEntries;
    }


    internal List<TrialBalanceEntry> GetSummaryByDebtorCreditor(List<TrialBalanceEntry> trialBalance) {

      var totalSummaryDebtorCreditor = new EmpiriaHashTable<TrialBalanceEntry>(trialBalance.Count);

      foreach (var entry in trialBalance.Where(a => !a.HasParentPostingEntry)) {

        if (entry.DebtorCreditor == DebtorCreditorType.Deudora) {
          SummaryDebtorCreditorByAccount(totalSummaryDebtorCreditor, entry,
                                         TrialBalanceItemType.BalanceTotalDebtor);
        }

        if (entry.DebtorCreditor == DebtorCreditorType.Acreedora) {
          SummaryDebtorCreditorByAccount(totalSummaryDebtorCreditor, entry,
                                         TrialBalanceItemType.BalanceTotalCreditor);
        }
      }

      return OrderingDebtorCreditorEntries(totalSummaryDebtorCreditor.Values.ToList());
    }


    internal List<TrialBalanceEntry> OrderingLedgersByAccount(List<TrialBalanceEntry> trialBalance) {
      List<TrialBalanceEntry> returnedList = new List<TrialBalanceEntry>(trialBalance);

      foreach (var entry in returnedList) {
        entry.GroupName = entry.Ledger.Name;
        entry.DebtorCreditor = entry.Account.DebtorCreditor;
      }

      return returnedList.OrderBy(a => a.Currency.Code)
                         .ThenBy(a => a.Account.Number)
                         .ThenBy(a => a.Sector.Code)
                         .ThenByDescending(a => a.Account.DebtorCreditor)
                         .ThenBy(a => a.Ledger.Number)
                         .ToList();
    }


    #region Private methods

    private List<TrialBalanceEntry> OrderingDebtorCreditorEntries(
                                     List<TrialBalanceEntry> trialBalanceEntries) {
      return trialBalanceEntries.OrderBy(a => a.Currency.Code)
                                .ThenByDescending(a => a.DebtorCreditor)
                                .ToList();
    }


    private List<TrialBalanceEntry> OrderingTotalsByGroup(FixedList<TrialBalanceEntry> summaryGroupEntries) {

      var returnedList = new List<TrialBalanceEntry>(summaryGroupEntries);
      return returnedList.OrderBy(a => a.Currency.Code)
                         .ThenBy(a => a.GroupNumber)
                         .ThenBy(a => a.Sector.Code)
                         .ToList();
    }


    private void GetOrIncreaseEntries(EmpiriaHashTable<TrialBalanceEntry> summaryEntries,
                                           TrialBalanceEntry entry,
                                           StandardAccount targetAccount, Sector targetSector,
                                           TrialBalanceItemType itemType, string hash) {

      TrialBalanceEntry summaryEntry;

      summaryEntries.TryGetValue(hash, out summaryEntry);

      if (summaryEntry == null) {

        summaryEntry = new TrialBalanceEntry {
          Ledger = entry.Ledger,
          Currency = entry.Currency,
          Sector = targetSector,
          Account = targetAccount,
          ItemType = itemType,
          GroupNumber = entry.GroupNumber,
          GroupName = entry.GroupName,
          DebtorCreditor = entry.DebtorCreditor,
          SubledgerAccountIdParent = entry.SubledgerAccountIdParent,
          LastChangeDate = entry.LastChangeDate
        };

        summaryEntry.Sum(entry);

        summaryEntries.Insert(hash, summaryEntry);

      } else {

        summaryEntry.Sum(entry);
      }
    }


    private void SummaryByCurrency(EmpiriaHashTable<TrialBalanceEntry> summaryEntries,
                                          TrialBalanceEntry balanceEntry) {

      TrialBalanceEntry entry = balanceEntry.CreatePartialCopy();

      TrialBalanceItemType itemType = TrialBalanceItemType.BalanceTotalCurrency;

      if (entry.ItemType == TrialBalanceItemType.BalanceTotalCreditor) {
        entry.InitialBalance = -1 * entry.InitialBalance;
        entry.CurrentBalance = -1 * entry.CurrentBalance;
      }

      entry.GroupName = "TOTAL MONEDA " + entry.Currency.FullName;
      entry.GroupNumber = "";

      string hash = $"{entry.GroupName}||{entry.Currency.Id}";

      GetOrIncreaseEntries(summaryEntries, entry, StandardAccount.Empty, Sector.Empty, itemType, hash);
    }


    private void SummaryByLedgerGroupEntries(EmpiriaHashTable<TrialBalanceEntry> totalsListByGroupEntries,
                                                TrialBalanceEntry balanceEntry) {

      TrialBalanceEntry groupEntry = balanceEntry.CreatePartialCopy();

      groupEntry.GroupName = $"SUMA DE DELEGACIONES";
      groupEntry.GroupNumber = balanceEntry.Account.Number;
      groupEntry.Account = balanceEntry.Account;
      groupEntry.Sector = balanceEntry.Sector;
      groupEntry.DebtorCreditor = balanceEntry.Account.DebtorCreditor;
      groupEntry.Ledger = Ledger.Empty;

      var itemType = new TrialBalanceItemType();

      if (balanceEntry.DebtorCreditor == DebtorCreditorType.Deudora) {
        itemType = TrialBalanceItemType.BalanceTotalGroupDebtor;

      }

      if (balanceEntry.DebtorCreditor == DebtorCreditorType.Acreedora) {
        itemType = TrialBalanceItemType.BalanceTotalGroupCreditor;

      }

      string hash = $"{balanceEntry.Currency.Id}||{groupEntry.GroupNumber}||" +
                    $"{groupEntry.Sector.Code}||{groupEntry.DebtorCreditor}";

      GetOrIncreaseEntries(totalsListByGroupEntries, groupEntry, groupEntry.Account,
                                groupEntry.Sector, itemType, hash);
    }


    private void SummaryDebtorCreditorByAccount(EmpiriaHashTable<TrialBalanceEntry> summaryEntries,
                                                TrialBalanceEntry balanceEntry,
                                                TrialBalanceItemType itemType) {

      TrialBalanceEntry entry = balanceEntry.CreatePartialCopy();


      if (itemType == TrialBalanceItemType.BalanceTotalDebtor) {
        entry.GroupName = $"TOTAL DEUDORAS {entry.Currency.FullName}";
      } else {
        entry.GroupName = $"TOTAL ACREEDORAS {entry.Currency.FullName}";
      }

      entry.Ledger = Ledger.Empty;
      entry.DebtorCreditor = balanceEntry.DebtorCreditor;

      string hash = $"{entry.GroupName}||{entry.Currency.Id}";

      GetOrIncreaseEntries(summaryEntries, entry, StandardAccount.Empty, Sector.Empty, itemType, hash);
    }


    #endregion Private methods


  } // class BalanceCascadeAccountingHelper

} // class BalanceCascadeAccountingHelper
