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

      FixedList<BalanceExplorerEntry> entries = helper.GetBalanceExplorerEntries();

      entries = helper.GetSummaryToParentEntries(entries);

      FixedList<BalanceExplorerEntry> subledgerAccounts = GetSubledgerAccounts(entries);

      EmpiriaHashTable<BalanceExplorerEntry> totalByAccountAndCurrency = GetTotalBalanceByAccountAndCurrency(
                                                                  subledgerAccounts);

      FixedList<BalanceExplorerEntry> balancesAndtotalByAccountAndCurrency =
                                CombineBalanceAndtotalByAccountAndCurrency(
                                  subledgerAccounts, totalByAccountAndCurrency);

      EmpiriaHashTable<BalanceExplorerEntry> balanceHeader = GetBalanceHeaderByAccount(subledgerAccounts);

      FixedList<BalanceExplorerEntry> balancesWithHeader = CombineBalanceAndBalanceHeader(
                                                    balancesAndtotalByAccountAndCurrency, balanceHeader);

      return new BalanceExplorerResult(_query, balancesWithHeader);
    }

    #region Private methods


    private FixedList<BalanceExplorerEntry> CombineBalanceAndBalanceHeader(FixedList<BalanceExplorerEntry> balancesAndtotalByLedger,
                                                                           EmpiriaHashTable<BalanceExplorerEntry> balanceHeader) {
      var balanceWithHeader = new List<BalanceExplorerEntry>();

      foreach (var header in balanceHeader.ToFixedList()) {
        var balance = balancesAndtotalByLedger.Where(a =>
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
                                      EmpiriaHashTable<BalanceExplorerEntry> totalByAccountAndCurrency) {
      var combinedEntries = new List<BalanceExplorerEntry>();

      foreach (var totalByAccount in totalByAccountAndCurrency.ToFixedList()) {
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


    private EmpiriaHashTable<BalanceExplorerEntry> GetBalanceHeaderByAccount(
                                            FixedList<BalanceExplorerEntry> subledgerAccounts) {
      var helper = new BalanceExplorerHelper(_query);

      var headerByAccount = new EmpiriaHashTable<BalanceExplorerEntry>();

      foreach (var entry in subledgerAccounts) {
        helper.GetHeaderAccountName(headerByAccount, entry, TrialBalanceItemType.Total);
      }

      return headerByAccount;
    }


    private List<BalanceExplorerEntry> GetOrderBySubledgerAccount(List<BalanceExplorerEntry> subledgerAccounts) {
      var orderBySubledger = subledgerAccounts.OrderBy(a => a.Ledger.Number)
                                              .ThenBy(a => a.Currency.Code)
                                              .ThenBy(a => a.Account.Number)
                                              .ThenBy(a => a.Sector.Code)
                                              .ThenBy(a => a.SubledgerAccountNumber).ToList();
      return orderBySubledger;
    }


    private FixedList<BalanceExplorerEntry> GetSubledgerAccounts(FixedList<BalanceExplorerEntry> balance) {
      if (!_query.WithSubledgerAccount) {
        return balance;
      }

      var balanceWithSubledgerAccounts = new List<BalanceExplorerEntry>();

      foreach (var entry in balance.Where(a => a.SubledgerAccountId > 0)) {
        SubledgerAccount subledgerAccount = SubledgerAccount.Parse(entry.SubledgerAccountId);
        entry.SubledgerAccountNumber = subledgerAccount.Number;
        entry.SubledgerAccountName = subledgerAccount.Name;

        balanceWithSubledgerAccounts.Add(entry);
      }

      balanceWithSubledgerAccounts = GetOrderBySubledgerAccount(balanceWithSubledgerAccounts);

      return balanceWithSubledgerAccounts.ToFixedList();
    }


    private EmpiriaHashTable<BalanceExplorerEntry> GetTotalBalanceByAccountAndCurrency(
                                            FixedList<BalanceExplorerEntry> subledgerAccounts) {
      var helper = new BalanceExplorerHelper(_query);

      var totalByCurrencies = new EmpiriaHashTable<BalanceExplorerEntry>();

      foreach (var entry in subledgerAccounts) {
        helper.SummaryEntriesByAccountAndCurrency(totalByCurrencies, entry, TrialBalanceItemType.Group);
      }

      return totalByCurrencies;
    }


    #endregion Private methods

  } // class SaldosPorCuentaBuilder

} // Empiria.FinancialAccounting.BalanceEngine.BalanceExplorer
