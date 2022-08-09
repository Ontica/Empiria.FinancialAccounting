/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Builder                                 *
*  Type     : SaldosPorCuentaBuilder                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Genera los datos para el reporte de saldos por cuenta para el explorador de saldos.            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;

using Empiria.Collections;

using Empiria.FinancialAccounting.BalanceEngine.BalanceExplorer.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine.BalanceExplorer {

  /// <summary>Genera los datos para el reporte de saldos por cuenta para el explorador de saldos.</summary>
  internal class SaldosPorCuentaBuilder {

    private readonly BalanceExplorerQuery _query;

    internal SaldosPorCuentaBuilder(BalanceExplorerQuery query) {
      _query = query;
    }


    internal BalanceExplorerResult Build() {
      var helper = new BalanceExplorerHelper(_query);

      FixedList<BalanceExplorerEntry> balanceEntries = helper.GetBalanceExplorerEntries();

      if (balanceEntries.Count == 0) {
        return new BalanceExplorerResult(_query, new FixedList<BalanceExplorerEntry>());
      }

      balanceEntries = helper.GetSummaryToParentEntries(balanceEntries);

      FixedList<BalanceExplorerEntry> subledgerAccounts = GetSubledgerAccounts(balanceEntries);

      FixedList<BalanceExplorerEntry> totalByAccountAndCurrency =
                    GetTotalBalanceByAccountAndCurrency(subledgerAccounts);

      FixedList<BalanceExplorerEntry> accountEntries =
                                CombineBalanceAndtotalByAccountAndCurrency(
                                  subledgerAccounts, totalByAccountAndCurrency);

      FixedList<BalanceExplorerEntry> balanceHeader = GetBalanceHeaderByAccount(subledgerAccounts);

      FixedList<BalanceExplorerEntry> balancesWithHeader = CombineBalanceAndBalanceHeader(
                                                    accountEntries, balanceHeader);

      return new BalanceExplorerResult(_query, balancesWithHeader);
    }


    #region Private methods


    private FixedList<BalanceExplorerEntry> CombineBalanceAndBalanceHeader(
      FixedList<BalanceExplorerEntry> accountEntries,
      FixedList<BalanceExplorerEntry> balanceHeader) {

      if (balanceHeader.Count == 0) {
        return accountEntries;
      }

      var balanceWithHeader = new List<BalanceExplorerEntry>();

      foreach (var header in balanceHeader) {
        var balance = accountEntries.Where(a =>
                                    a.Currency.Code == header.Currency.Code &&
                                    a.Account.Number == header.Account.Number).ToList();
        balanceWithHeader.Add(header);
        if (balance.Count > 0) {

          var totalByCurrency = balance
              .FirstOrDefault(a => a.ItemType == TrialBalanceItemType.BalanceTotalCurrency);

          if (totalByCurrency != null && totalByCurrency.LastChangeDate > header.LastChangeDate) {
            header.LastChangeDate = totalByCurrency.LastChangeDate;
          }
          balanceWithHeader.AddRange(balance);
        }
      }
      return balanceWithHeader.ToFixedList();
    }


    private FixedList<BalanceExplorerEntry> CombineBalanceAndtotalByAccountAndCurrency(
                                      FixedList<BalanceExplorerEntry> subledgerAccounts,
                                      FixedList<BalanceExplorerEntry> totalByAccountAndCurrency) {
      if (totalByAccountAndCurrency.Count == 0) {
        return subledgerAccounts;
      }

      var combinedEntries = new List<BalanceExplorerEntry>();

      foreach (var totalByAccount in totalByAccountAndCurrency) {
        var balancesByLedger = subledgerAccounts.Where(a => a.Currency.Code == totalByAccount.Currency.Code &&
                                                    a.Account.Number == totalByAccount.Account.Number).ToList();
        if (balancesByLedger.Count > 0) {

          foreach (var balance in balancesByLedger) {
            balance.DebtorCreditor = balance.Account.DebtorCreditor;
            if (balance.LastChangeDate > totalByAccount.LastChangeDate) {
              totalByAccount.LastChangeDate = balance.LastChangeDate;
            }
          }

          combinedEntries.AddRange(balancesByLedger);
        }

        combinedEntries.Add(totalByAccount);
      }

      return combinedEntries.ToFixedList();
    }


    private FixedList<BalanceExplorerEntry> GetBalanceHeaderByAccount(
                                            FixedList<BalanceExplorerEntry> subledgerAccounts) {
      if (subledgerAccounts.Count == 0) {
        return new FixedList<BalanceExplorerEntry>();
      }

      var helper = new BalanceExplorerHelper(_query);

      var headerByAccount = new EmpiriaHashTable<BalanceExplorerEntry>();

      foreach (var entry in subledgerAccounts) {
        helper.GetHeaderAccountName(headerByAccount, entry, TrialBalanceItemType.Total);
      }

      return headerByAccount.ToFixedList();
    }


    private List<BalanceExplorerEntry> GetOrderBySubledgerAccount(List<BalanceExplorerEntry> subledgerAccounts) {
      var orderBySubledger = subledgerAccounts.OrderBy(a => a.Ledger.Number)
                                              .ThenBy(a => a.Currency.Code)
                                              .ThenBy(a => a.Account.Number)
                                              .ThenBy(a => a.Sector.Code)
                                              .ThenBy(a => a.SubledgerAccountNumber).ToList();
      return orderBySubledger;
    }


    private FixedList<BalanceExplorerEntry> GetSubledgerAccounts(
                                      FixedList<BalanceExplorerEntry> balanceEntries) {

      if (balanceEntries.Count == 0) {
        return new FixedList<BalanceExplorerEntry>();
      }

      if (!_query.WithSubledgerAccount) {
        return balanceEntries;
      }

      var balanceWithSubledgerAccounts = new List<BalanceExplorerEntry>();

      foreach (var entry in balanceEntries.Where(a => a.SubledgerAccountId > 0)) {
        SubledgerAccount subledgerAccount = SubledgerAccount.Parse(entry.SubledgerAccountId);
        entry.SubledgerAccountNumber = subledgerAccount.Number;
        entry.SubledgerAccountName = subledgerAccount.Name;

        balanceWithSubledgerAccounts.Add(entry);
      }

      balanceWithSubledgerAccounts = GetOrderBySubledgerAccount(balanceWithSubledgerAccounts);

      return balanceWithSubledgerAccounts.ToFixedList();
    }


    private FixedList<BalanceExplorerEntry> GetTotalBalanceByAccountAndCurrency(
                                            FixedList<BalanceExplorerEntry> subledgerAccounts) {
      if (subledgerAccounts.Count == 0) {
        return new FixedList<BalanceExplorerEntry>();
      }

      var helper = new BalanceExplorerHelper(_query);

      var totalByCurrencies = new EmpiriaHashTable<BalanceExplorerEntry>();

      foreach (var entry in subledgerAccounts) {
        helper.SummaryEntriesByAccountAndCurrency(totalByCurrencies, entry, TrialBalanceItemType.Group);
      }

      return totalByCurrencies.ToFixedList();
    }


    #endregion Private methods

  } // class SaldosPorCuentaBuilder

} // Empiria.FinancialAccounting.BalanceEngine.BalanceExplorer
