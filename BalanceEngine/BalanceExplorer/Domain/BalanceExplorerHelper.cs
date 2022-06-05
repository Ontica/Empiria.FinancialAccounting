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

    #endregion

  } // class BalanceExplorerHelper

} // Empiria.FinancialAccounting.BalanceEngine.BalanceExplorer
