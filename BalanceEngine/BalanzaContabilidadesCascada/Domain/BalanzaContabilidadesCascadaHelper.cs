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


    internal List<TrialBalanceEntry> CombineTotalsWithAccountEntries(
                                      FixedList<TrialBalanceEntry> accountEntries,
                                      FixedList<TrialBalanceEntry> totalByAccountEntries) {

      var returnedEntries = new List<TrialBalanceEntry>();

      totalByAccountEntries = OrderingTotalsByAccountEntries(totalByAccountEntries).ToFixedList();

      foreach (var totalByAccountEntry in totalByAccountEntries) {

        var filteredEntries = accountEntries.Where(a => a.Account.Number == totalByAccountEntry.GroupNumber &&
                                           a.Currency.Id == totalByAccountEntry.Currency.Id &&
                                           a.Sector.Code == totalByAccountEntry.Sector.Code)
                                    .ToList();

        foreach (var entry in filteredEntries) {

          if (entry.LastChangeDate > totalByAccountEntry.LastChangeDate) {
            totalByAccountEntry.LastChangeDate = entry.LastChangeDate;

          }
        }

        totalByAccountEntry.GroupNumber = "";
        filteredEntries.Add(totalByAccountEntry);
        returnedEntries.AddRange(filteredEntries);
      }
      return returnedEntries;
    }


    internal List<TrialBalanceEntry> CombineTotalsByCurrencyAndAccountEntries(
                                     List<TrialBalanceEntry> accountEntries,
                                     FixedList<TrialBalanceEntry> totalsByCurrency) {

      var returnedEntries = new List<TrialBalanceEntry>();

      foreach (var totalByCurrency in totalsByCurrency
                    .Where(a => a.ItemType == TrialBalanceItemType.BalanceTotalCurrency)) {

        var entriesByCurrency = accountEntries.Where(
                                a => a.Currency.Code == totalByCurrency.Currency.Code).ToList();

        if (entriesByCurrency.Count > 0) {
          entriesByCurrency.Add(totalByCurrency);
          returnedEntries.AddRange(entriesByCurrency);
        }
      }
      return returnedEntries;
    }

    internal List<TrialBalanceEntry> CombineAccountEntriesAndTotalReport(
                                     List<TrialBalanceEntry> accountEntriesAndTotalsByCurrency,
                                     TrialBalanceEntry totalReport) {

      var accountEntriesAndTotal = new List<TrialBalanceEntry>(accountEntriesAndTotalsByCurrency);

      

      throw new NotImplementedException();
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


    internal TrialBalanceEntry GenerateTotalReport(
                               FixedList<TrialBalanceEntry> totalsByCurrency) {

      var totalReport = new EmpiriaHashTable<TrialBalanceEntry>();

      foreach (var currencyEntry in totalsByCurrency) {

        var entry = currencyEntry.CreatePartialCopy();

        entry.GroupNumber = "";
        entry.GroupName = "TOTAL DEL REPORTE";
        string hash = $"{entry.GroupName}";

        GetOrIncreaseEntries(totalReport, entry, StandardAccount.Empty, Sector.Empty,
                             TrialBalanceItemType.BalanceTotalConsolidated, hash);
      } // foreach

      return totalReport.Values.FirstOrDefault();
    }


    internal FixedList<TrialBalanceEntry> GenerateTotalsByCurrency(
                                          List<TrialBalanceEntry> entries) {

      var totalByCurrencies = new EmpiriaHashTable<TrialBalanceEntry>();

      foreach (var entry in entries.Where(a => !a.HasParentPostingEntry &&
                            (a.ItemType == TrialBalanceItemType.BalanceTotalDebtor ||
                            a.ItemType == TrialBalanceItemType.BalanceTotalCreditor))) {

        SummaryByCurrency(totalByCurrencies, entry);
      }
      return totalByCurrencies.ToFixedList();
    }


    internal FixedList<TrialBalanceEntry> GenerateTotalsByAccountAndLedger(
                                          List<TrialBalanceEntry> trialBalance) {

      var totalsByAccountAndLedger = new EmpiriaHashTable<TrialBalanceEntry>();

      foreach (var entry in trialBalance) {
        SummaryAccountEntryByLedger(totalsByAccountAndLedger, entry);
      }

      return totalsByAccountAndLedger.ToFixedList();
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


    internal List<TrialBalanceEntry> GenerateTotalsByDebtorCreditor(List<TrialBalanceEntry> trialBalance) {

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


    internal List<TrialBalanceEntry> OrderingAccountEntries(
                                     FixedList<TrialBalanceEntry> accountEntries) {

      List<TrialBalanceEntry> accountEntriesOrdered = new List<TrialBalanceEntry>(accountEntries);

      foreach (var entry in accountEntriesOrdered) {
        entry.GroupName = entry.Ledger.Name;
        entry.DebtorCreditor = entry.Account.DebtorCreditor;
      }

      return accountEntriesOrdered.OrderBy(a => a.Currency.Code)
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


    private List<TrialBalanceEntry> OrderingTotalsByAccountEntries(
                                    FixedList<TrialBalanceEntry> totalsByAccountEntries) {

      var returnedList = new List<TrialBalanceEntry>(totalsByAccountEntries);

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


    private void SummaryAccountEntryByLedger(EmpiriaHashTable<TrialBalanceEntry> totalsByAccountAndLedger,
                                                TrialBalanceEntry accountEntry) {

      TrialBalanceEntry groupEntry = accountEntry.CreatePartialCopy();

      groupEntry.GroupName = $"SUMA DE DELEGACIONES";
      groupEntry.GroupNumber = accountEntry.Account.Number;
      groupEntry.Account = accountEntry.Account;
      groupEntry.Sector = accountEntry.Sector;
      groupEntry.DebtorCreditor = accountEntry.Account.DebtorCreditor;
      groupEntry.Ledger = Ledger.Empty;

      var itemType = new TrialBalanceItemType();

      if (accountEntry.DebtorCreditor == DebtorCreditorType.Deudora) {
        itemType = TrialBalanceItemType.BalanceTotalGroupDebtor;

      }

      if (accountEntry.DebtorCreditor == DebtorCreditorType.Acreedora) {
        itemType = TrialBalanceItemType.BalanceTotalGroupCreditor;

      }

      string hash = $"{accountEntry.Currency.Id}||{groupEntry.GroupNumber}||" +
                    $"{groupEntry.Sector.Code}||{groupEntry.DebtorCreditor}";

      GetOrIncreaseEntries(totalsByAccountAndLedger, groupEntry, groupEntry.Account,
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
