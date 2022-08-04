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
using Empiria.FinancialAccounting.BalanceEngine.Data;

namespace Empiria.FinancialAccounting.BalanceEngine.Helpers {

  /// <summary>Helper methods to build valorized balances.</summary>
  internal class SaldosPorAuxiliarHelper {

    private readonly TrialBalanceQuery _query;

    internal SaldosPorAuxiliarHelper(TrialBalanceQuery query) {
      _query = query;
    }


    internal void GenerateAverageBalance(List<TrialBalanceEntry> accountEntries) {

      if (_query.WithAverageBalance) {

        foreach (var entry in accountEntries.Where(a => a.ItemType == TrialBalanceItemType.Summary)) {

          decimal debtorCreditor = entry.DebtorCreditor == DebtorCreditorType.Deudora ?
                                   entry.Debit - entry.Credit : entry.Credit - entry.Debit;

          TimeSpan timeSpan = _query.InitialPeriod.ToDate - entry.LastChangeDate;
          int numberOfDays = timeSpan.Days + 1;

          entry.AverageBalance = ((numberOfDays * debtorCreditor) /
                                   _query.InitialPeriod.ToDate.Day) +
                                   entry.InitialBalance;
        }
      }
    }


    internal EmpiriaHashTable<TrialBalanceEntry> GetBalancesBySubledgerAccounts(
                                                FixedList<TrialBalanceEntry> accountEntries) {

      if (accountEntries.Count == 0) {
        return new EmpiriaHashTable<TrialBalanceEntry>();
      }

      var subledgerAccountsEntries = accountEntries.Where(a => a.SubledgerAccountId > 0).ToList();

      var subledgerAccountsEntriesHashTable = new EmpiriaHashTable<TrialBalanceEntry>();

      foreach (var entry in subledgerAccountsEntries) {
        string hash = $"{entry.Ledger.Number}||{entry.Currency.Code}||" +
                      $"{entry.Account.Number}||{entry.Sector.Code}||" +
                      $"{entry.SubledgerAccountId}";

        subledgerAccountsEntriesHashTable.Insert(hash, entry);
      }

      return GenerateEntries(subledgerAccountsEntriesHashTable);
    }


    internal List<TrialBalanceEntry> GetTotalsAndCombineWithAccountEntries(
                                    List<TrialBalanceEntry> orderedParentAccounts,
                                    FixedList<TrialBalanceEntry> accountEntries) {

      if (orderedParentAccounts.Count == 0 || accountEntries.Count == 0) {
        return new List<TrialBalanceEntry>();
      }

      var returnedOrdering = new List<TrialBalanceEntry>();

      foreach (var parentAccountEntry in orderedParentAccounts) {

        List<TrialBalanceEntry> parentAccountsWithLastChangeDate =
                                AssingnLastChangeDateToParentAccounts(parentAccountEntry, accountEntries);
        
        returnedOrdering.Add(parentAccountEntry);
        returnedOrdering.AddRange(parentAccountsWithLastChangeDate);

        GenerateTotalBySubledgerAccount(parentAccountEntry, returnedOrdering);
      }

      return returnedOrdering;
    }


    internal List<TrialBalanceEntry> OrderingAndAssingnSubledgerAccountInfoToParent(
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


    private List<TrialBalanceEntry> AssingnLastChangeDateToParentAccounts(
            TrialBalanceEntry parentAccountEntry, FixedList<TrialBalanceEntry> accountEntries) {

      var accountEntriesByParentAccount = accountEntries.Where(
                      a => a.SubledgerAccountId == parentAccountEntry.SubledgerAccountIdParent &&
                      a.Ledger.Number == parentAccountEntry.Ledger.Number &&
                      a.Currency.Code == parentAccountEntry.Currency.Code &&
                      a.ItemType == TrialBalanceItemType.Entry).ToList();

      foreach (var entryToCompare in accountEntriesByParentAccount) {

        if (entryToCompare.LastChangeDate > parentAccountEntry.LastChangeDate) {
          parentAccountEntry.LastChangeDate = entryToCompare.LastChangeDate;

        } else {
          parentAccountEntry.LastChangeDate = parentAccountEntry.LastChangeDate;

        }
        entryToCompare.SubledgerAccountId = 0;
      }

      return accountEntriesByParentAccount;
    }


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


    private void GenerateTotalBySubledgerAccount(TrialBalanceEntry entry,
                                                 List<TrialBalanceEntry> returnedOrdering) {

      var totalBySubledgerAccountEntry = new EmpiriaHashTable<TrialBalanceEntry>();

      SummaryBySubledgerEntry(totalBySubledgerAccountEntry, entry, TrialBalanceItemType.Total);
      returnedOrdering.Add(totalBySubledgerAccountEntry.ToFixedList().First());
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
