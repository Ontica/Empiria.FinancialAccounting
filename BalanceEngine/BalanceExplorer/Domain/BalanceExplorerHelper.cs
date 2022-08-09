/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Helper methods                          *
*  Type     : BalanceExplorerHelper                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Helper methods to build balances for the balances explorer.                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Linq;
using System.Collections.Generic;

using Empiria.Collections;

using Empiria.FinancialAccounting.BalanceEngine.Data;

using Empiria.FinancialAccounting.BalanceEngine.BalanceExplorer.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine.BalanceExplorer {

  /// <summary>Helper methods to build balances for the balances explorer.</summary>
  internal class BalanceExplorerHelper {

    private readonly BalanceExplorerQuery _query;

    internal BalanceExplorerHelper(BalanceExplorerQuery query) {
      _query = query;
    }


    internal FixedList<BalanceExplorerEntry> GetBalanceExplorerEntries() {
      FixedList<TrialBalanceEntry> entries = BalancesDataService.GetTrialBalanceForBalancesExplorer(_query);

      return BalanceExplorerMapper.MapToBalance(entries);
    }


    #region Public methods


    internal FixedList<BalanceExplorerEntry> CombineSubledgerAccountsWithBalanceEntries(
                                    List<BalanceExplorerEntry> orderingBalance,
                                    FixedList<BalanceExplorerEntry> balanceEntries) {
      var returnedEntries = new List<BalanceExplorerEntry>();

      foreach (var entry in orderingBalance) {
        var summaryAccounts = balanceEntries.Where(
                      a => a.SubledgerAccountId == entry.SubledgerAccountIdParent &&
                      a.Ledger.Number == entry.Ledger.Number &&
                      a.Currency.Code == entry.Currency.Code &&
                      a.ItemType == TrialBalanceItemType.Entry).ToList();

        foreach (var summary in summaryAccounts) {
          entry.LastChangeDate = summary.LastChangeDate > entry.LastChangeDate ?
                                 summary.LastChangeDate : entry.LastChangeDate;
          summary.SubledgerAccountId = 0;
          summary.SubledgerAccountNumber = entry.SubledgerAccountNumber;
          summary.SubledgerAccountName = entry.SubledgerAccountName;
        }

        returnedEntries.Add(entry);

        if (summaryAccounts.Count > 0) {
          returnedEntries.AddRange(summaryAccounts);
        }
      }

      return returnedEntries.ToFixedList();
    }


    internal void GetHeaderAccountName(EmpiriaHashTable<BalanceExplorerEntry> headerByAccount,
                                        BalanceExplorerEntry entry, TrialBalanceItemType balanceType) {
      BalanceExplorerEntry newEntry = BalanceExplorerMapper.MapToBalanceEntry(entry);
      newEntry.GroupName = $"{newEntry.Account.Name} [{newEntry.Currency.FullName}]";
      newEntry.InitialBalance = entry.InitialBalance;
      newEntry.Ledger = Ledger.Empty;
      string hash = $"{newEntry.Currency.Code}||{newEntry.Account.Number}"; //{newEntry.Ledger.Number}||

      GenerateOrIncreaseBalances(headerByAccount, newEntry, newEntry.Account,
                                 Sector.Empty, balanceType, hash);

    }


    internal FixedList<BalanceExplorerEntry> GetSubledgerAccounts(FixedList<BalanceExplorerEntry> balance) {

      var subledgerAccountList = balance.Where(a => a.SubledgerAccountId > 0).ToList();

      var subledgerAccountListHashTable = new EmpiriaHashTable<BalanceExplorerEntry>();

      foreach (var entry in subledgerAccountList) {
        string hash = $"{entry.Ledger.Number}||{entry.Currency.Code}||" +
                      $"{entry.Account.Number}||{entry.Sector.Code}||" +
                      $"{entry.SubledgerAccountId}";

        subledgerAccountListHashTable.Insert(hash, entry);
      }

      return GenerateSubledgerAccount(subledgerAccountListHashTable).ToFixedList();
    }


    internal FixedList<BalanceExplorerEntry> GetSummaryToParentEntries(
                                            FixedList<BalanceExplorerEntry> postingEntries) {
      var returnedEntries = new List<BalanceExplorerEntry>(postingEntries);
      foreach (var entry in postingEntries) {
        StandardAccount currentParent = entry.Account.GetParent();

        var entryParent = returnedEntries.FirstOrDefault(a => a.Account.Number == currentParent.Number &&
                                                a.Currency.Code == entry.Currency.Code &&
                                                a.Ledger.Number == entry.Ledger.Number &&
                                                a.Sector.Code == entry.Sector.Code &&
                                                a.Account.DebtorCreditor == entry.Account.DebtorCreditor);
        if (entryParent != null) {
          entry.HasParentPostingEntry = true;
          entryParent.IsParentPostingEntry = true;
          entryParent.Sum(entry);
        }
      }

      return returnedEntries.ToFixedList();
    }


    internal List<BalanceExplorerEntry> OrderBySubledgerAccounts(
                                    FixedList<BalanceExplorerEntry> subledgerAccounts) {

      var returnedCombineOrdering = new List<BalanceExplorerEntry>();

      foreach (var entry in subledgerAccounts) {
        SubledgerAccount subledgerAccount = SubledgerAccount.Parse(entry.SubledgerAccountIdParent);
        if (subledgerAccount != null) {
          entry.SubledgerAccountNumber = subledgerAccount.Number;
          entry.SubledgerAccountName = subledgerAccount.Name;
          entry.GroupName = subledgerAccount.Name;
          entry.SubledgerNumberOfDigits = entry.SubledgerAccountNumber.Count();
          entry.SubledgerAccountId = entry.SubledgerAccountIdParent;
        }
        returnedCombineOrdering.Add(entry);
      }
      return returnedCombineOrdering.OrderBy(a => a.Currency.Code)
                                    .ThenBy(a => a.SubledgerNumberOfDigits)
                                    .ThenBy(a => a.SubledgerAccountNumber)
                                    .ToList();
    }


    internal void SummaryBySubledgerAccount(EmpiriaHashTable<BalanceExplorerEntry> entries,
                                    BalanceExplorerEntry entry, TrialBalanceItemType balanceType) {

      string hash = $"{entry.Ledger.Number}||{entry.Currency.Code}||" +
                    $"{entry.SubledgerAccountIdParent}||{Sector.Empty.Code}";

      GenerateOrIncreaseBalances(entries, entry, StandardAccount.Empty, Sector.Empty, balanceType, hash);
    }


    internal void SummaryEntriesByCurrency(EmpiriaHashTable<BalanceExplorerEntry> totalByCurrencies,
                                           BalanceExplorerEntry entry, TrialBalanceItemType balanceType) {

      BalanceExplorerEntry newEntry = BalanceExplorerMapper.MapToBalanceEntry(entry);
      newEntry.GroupName = $"TOTAL DE LA CUENTA {newEntry.Account.Number} EN {newEntry.Currency.FullName}";

      string hash = $"{newEntry.Ledger.Number}||{newEntry.Currency.Code}||{newEntry.Account.Number}";

      GenerateOrIncreaseBalances(totalByCurrencies, newEntry, newEntry.Account,
                                 Sector.Empty, balanceType, hash);

    }

    internal void SummaryEntriesByAccountAndCurrency(EmpiriaHashTable<BalanceExplorerEntry> totalByCurrencies,
                                           BalanceExplorerEntry entry, TrialBalanceItemType balanceType) {

      BalanceExplorerEntry newEntry = BalanceExplorerMapper.MapToBalanceEntry(entry);
      newEntry.GroupName = $"TOTAL DE LA CUENTA {newEntry.Account.Number} EN {newEntry.Currency.FullName}";
      newEntry.Ledger = Ledger.Empty;

      string hash = $"{newEntry.Currency.Code}||{newEntry.Account.Number}";

      GenerateOrIncreaseBalances(totalByCurrencies, newEntry, newEntry.Account,
                                 Sector.Empty, balanceType, hash);

    }

    #endregion

    #region Private methods


    private void GenerateOrIncreaseBalances(EmpiriaHashTable<BalanceExplorerEntry> entries,
                                            BalanceExplorerEntry entry, StandardAccount account,
                                            Sector sector, TrialBalanceItemType balanceType, string hash) {
      BalanceExplorerEntry returnedEntry;

      entries.TryGetValue(hash, out returnedEntry);

      if (returnedEntry == null) {

        returnedEntry = new BalanceExplorerEntry {
          Ledger = entry.Ledger,
          Currency = entry.Currency,
          Sector = sector,
          Account = account,
          ItemType = balanceType,
          GroupNumber = entry.GroupNumber,
          GroupName = entry.GroupName,
          DebtorCreditor = entry.DebtorCreditor,
          SubledgerAccountIdParent = entry.SubledgerAccountIdParent,
          LastChangeDate = entry.LastChangeDate
        };
        returnedEntry.InitialBalance += entry.InitialBalance;
        returnedEntry.CurrentBalance += entry.CurrentBalance;

        entries.Insert(hash, returnedEntry);

      } else {
        returnedEntry.InitialBalance += entry.InitialBalance;
        returnedEntry.CurrentBalance += entry.CurrentBalance;
        if (entry.LastChangeDate > returnedEntry.LastChangeDate) {
          returnedEntry.LastChangeDate = entry.LastChangeDate;
        }
      }
    }


    private EmpiriaHashTable<BalanceExplorerEntry> GenerateSubledgerAccount(
                        EmpiriaHashTable<BalanceExplorerEntry> subledgerAccountListHash) {
      
      var returnedEntries = new EmpiriaHashTable<BalanceExplorerEntry>();

      foreach (var entry in subledgerAccountListHash.ToFixedList()) {

        entry.SubledgerAccountIdParent = entry.SubledgerAccountId;
        entry.DebtorCreditor = entry.Account.DebtorCreditor;

        SummaryBySubledgerAccount(returnedEntries, entry, TrialBalanceItemType.Summary);
      }

      return returnedEntries;
    }


    #endregion

  } // class BalanceExplorerHelper

} // Empiria.FinancialAccounting.BalanceEngine.BalanceExplorer
