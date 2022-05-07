/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Helper methods                          *
*  Type     : BalanceBySubledgerAccountHelper            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Helper methods to build valorized balances.                                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;
using Empiria.Collections;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine.Helpers {

  /// <summary>Helper methods to build valorized balances.</summary>
  internal class BalanceBySubledgerAccountHelper {

    private readonly TrialBalanceCommand _command;

    internal BalanceBySubledgerAccountHelper(TrialBalanceCommand command) {
      _command = command;
    }


    internal EmpiriaHashTable<TrialBalanceEntry> BalancesBySubledgerAccounts(
                                                List<TrialBalanceEntry> trialBalance) {

      var subledgerAccountsEntries = trialBalance.Where(a => a.SubledgerAccountId > 0).ToList();

      var subledgerAccountsEntriesHashTable = new EmpiriaHashTable<TrialBalanceEntry>();

      foreach (var entry in subledgerAccountsEntries) {
        string hash = $"{entry.Ledger.Number}||{entry.Currency.Code}||" +
                      $"{entry.Account.Number}||{entry.Sector.Code}||" +
                      $"{entry.SubledgerAccountId}";

        subledgerAccountsEntriesHashTable.Insert(hash, entry);
      }

      return GenerateEntries(subledgerAccountsEntriesHashTable);
    }

    
    internal List<TrialBalanceEntry> CombineTotalAndSummaryEntries(
                                    List<TrialBalanceEntry> orderingtTialBalance,
                                    List<TrialBalanceEntry> trialBalance) {

      var returnedOrdering = new List<TrialBalanceEntry>();

      foreach (var entry in orderingtTialBalance) {

        var summaryAccounts = trialBalance.Where(
                      a => a.SubledgerAccountId == entry.SubledgerAccountIdParent &&
                      a.Ledger.Number == entry.Ledger.Number &&
                      a.Currency.Code == entry.Currency.Code &&
                      a.ItemType == TrialBalanceItemType.Entry).ToList();

        foreach (var summary in summaryAccounts) {
          entry.LastChangeDate = summary.LastChangeDate > entry.LastChangeDate ?
                                 summary.LastChangeDate : entry.LastChangeDate;
          summary.SubledgerAccountId = 0;
        }

        returnedOrdering.Add(entry);

        if (summaryAccounts.Count > 0) {
          returnedOrdering.AddRange(summaryAccounts);
        }

        var hashTotalEntry = new EmpiriaHashTable<TrialBalanceEntry>();
        SummaryBySubledgerEntry(hashTotalEntry, entry, TrialBalanceItemType.Total);
        returnedOrdering.Add(hashTotalEntry.ToFixedList().First());
      }

      return returnedOrdering;
    }


    internal List<TrialBalanceEntry> GenerateAverageBalance(List<TrialBalanceEntry> trialBalance) {
      var returnedEntries = new List<TrialBalanceEntry>(trialBalance);

      if (_command.WithAverageBalance) {

        foreach (var entry in returnedEntries.Where(a => a.ItemType == TrialBalanceItemType.Summary)) {

          decimal debtorCreditor = entry.DebtorCreditor == DebtorCreditorType.Deudora ?
                                   entry.Debit - entry.Credit : entry.Credit - entry.Debit;

          TimeSpan timeSpan = _command.InitialPeriod.ToDate - entry.LastChangeDate;
          int numberOfDays = timeSpan.Days + 1;

          entry.AverageBalance = ((numberOfDays * debtorCreditor) /
                                   _command.InitialPeriod.ToDate.Day) +
                                   entry.InitialBalance;
        }
      }

      return returnedEntries;
    }


    internal List<TrialBalanceEntry> OrderByAccountNumber(
                                      EmpiriaHashTable<TrialBalanceEntry> summaryEntries) {

      var returnedCombineOrdering = new List<TrialBalanceEntry>();

      foreach (var entry in summaryEntries.ToFixedList()) {
        SubledgerAccount subledgerAccount = SubledgerAccount.Parse(entry.SubledgerAccountIdParent);
        if (subledgerAccount != null) {
          entry.SubledgerAccountNumber = subledgerAccount.Number;
          entry.GroupName = subledgerAccount.Name;
          entry.SubledgerNumberOfDigits = entry.SubledgerAccountNumber.Count();
          entry.SubledgerAccountId = entry.SubledgerAccountIdParent;
        }
        returnedCombineOrdering.Add(entry);
      }

      return returnedCombineOrdering.Where(a => !a.SubledgerAccountNumber.Contains("undefined"))
                                    .OrderBy(a => a.Currency.Code)
                                    .ThenBy(a => a.SubledgerNumberOfDigits)
                                    .ThenBy(a => a.SubledgerAccountNumber)
                                    .ToList();
    }


    #region Private methods

    private EmpiriaHashTable<TrialBalanceEntry> GenerateEntries(
                        EmpiriaHashTable<TrialBalanceEntry> subledgerAccountEntriesHashTable) {

      var hashReturnedEntries = new EmpiriaHashTable<TrialBalanceEntry>();

      foreach (var entry in subledgerAccountEntriesHashTable.ToFixedList()) {
        entry.DebtorCreditor = entry.Account.DebtorCreditor;
        entry.SubledgerAccountIdParent = entry.SubledgerAccountId;

        SummaryBySubledgerEntry(hashReturnedEntries, entry, TrialBalanceItemType.Summary);
      }

      return hashReturnedEntries;
    }


    private void GenerateOrIncreaseEntries(EmpiriaHashTable<TrialBalanceEntry> summaryEntries,
                                           TrialBalanceEntry entry,
                                           TrialBalanceItemType itemType,
                                           string hash) {

      TrialBalanceEntry summaryEntry;

      summaryEntries.TryGetValue(hash, out summaryEntry);

      if (summaryEntry == null) {

        summaryEntry = new TrialBalanceEntry {
          Ledger = entry.Ledger,
          Currency = entry.Currency,
          Sector = Sector.Empty,
          Account = StandardAccount.Empty,
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


    private void SummaryBySubledgerEntry(EmpiriaHashTable<TrialBalanceEntry> summaryEntries,
                                     TrialBalanceEntry entry,
                                     TrialBalanceItemType itemType) {

      string hash = $"{entry.Ledger.Number}||{entry.Currency.Code}||" +
                    $"{entry.SubledgerAccountIdParent}||{Sector.Empty.Code}";

      if (itemType == TrialBalanceItemType.Total) {
        entry.GroupName = $"TOTAL DEL AUXILIAR: {entry.SubledgerAccountNumber}";
      }

      GenerateOrIncreaseEntries(summaryEntries, entry, itemType, hash);
    }

    #endregion Private methods

  } // class BalanceBySubledgerAccountHelper

} // namespace Empiria.FinancialAccounting.BalanceEngine.Helpers
