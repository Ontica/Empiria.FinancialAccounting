/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Service provider                        *
*  Type     : SaldosPorCuentaConsultaRapida              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Genera los datos para el reporte de saldos por cuenta de consulta rápida.                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;

using Empiria.Collections;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Genera los datos para el reporte de saldos por cuenta de consulta rápida.</summary>
  internal class SaldosPorCuentaConsultaRapida {

    private readonly BalanceCommand _command;

    public SaldosPorCuentaConsultaRapida(BalanceCommand command) {
      _command = command;
    }

    internal Balance Build() {
      var helper = new BalanceHelper(_command);

      FixedList<BalanceEntry> balance = helper.GetBalanceEntries();

      FixedList<BalanceEntry> subledgerAccounts = GetSubledgerAccounts(balance);

      EmpiriaHashTable<BalanceEntry> totalByLedger = GetTotalBalanceByLedgerAndCurrency(subledgerAccounts);

      FixedList<BalanceEntry> balancesAndtotalByLedger = CombineBalanceAndtotalByLedger(
                                                          subledgerAccounts, totalByLedger);

      EmpiriaHashTable<BalanceEntry> balanceHeader = GetBalanceHeaderByAccount(subledgerAccounts);

      FixedList<BalanceEntry> balanceWithHeader = CombineBalanceAndBalanceHeader(
                                                    balancesAndtotalByLedger, balanceHeader);

      var returnedBalance = new FixedList<IBalanceEntry>(
                                balanceWithHeader.Select(x => (IBalanceEntry) x));

      return new Balance(_command, returnedBalance);
    }


    #region Private methods


    private FixedList<BalanceEntry> CombineBalanceAndBalanceHeader(
                                     FixedList<BalanceEntry> balancesAndtotalByLedger,
                                     EmpiriaHashTable<BalanceEntry> balanceHeader) {
      var balanceWithHeader = new List<BalanceEntry>();

      foreach (var header in balanceHeader.ToFixedList()) {
        var balance = balancesAndtotalByLedger.Where(a => a.Ledger.Number == header.Ledger.Number &&
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


    private FixedList<BalanceEntry> CombineBalanceAndtotalByLedger(
                                      FixedList<BalanceEntry> balanceEntries,
                                      EmpiriaHashTable<BalanceEntry> totalByLedger) {
      var combinedEntries = new List<BalanceEntry>();

      foreach (var totalLedger in totalByLedger.ToFixedList()) {
        var balancesByLedger = balanceEntries.Where(a => a.Ledger.Number == totalLedger.Ledger.Number &&
                                                    a.Currency.Code == totalLedger.Currency.Code &&
                                                    a.Account.Number == totalLedger.Account.Number).ToList();
        if (balancesByLedger.Count > 0) {

          foreach (var balance in balancesByLedger) {
            if (balance.LastChangeDate > totalLedger.LastChangeDate) {
              totalLedger.LastChangeDate = balance.LastChangeDate;
            }
          }

          combinedEntries.AddRange(balancesByLedger);
        }
        combinedEntries.Add(totalLedger);
      }
      return combinedEntries.ToFixedList();
    }


    private EmpiriaHashTable<BalanceEntry> GetBalanceHeaderByAccount(
                                            FixedList<BalanceEntry> subledgerAccounts) {
      var helper = new BalanceHelper(_command);

      var headerByAccount = new EmpiriaHashTable<BalanceEntry>();

      foreach (var entry in subledgerAccounts) {
        helper.GetHeaderAccountName(headerByAccount, entry, TrialBalanceItemType.BalanceTotalConsolidated);
      }
      return headerByAccount;
    }


    private List<BalanceEntry> GetOrderBySubledgerAccount(List<BalanceEntry> subledgerAccounts) {
      var orderBySubledger = subledgerAccounts.OrderBy(a => a.Ledger.Number)
                                              .ThenBy(a => a.Currency.Code)
                                              .ThenBy(a => a.Account.Number)
                                              .ThenBy(a => a.Sector.Code)
                                              .ThenBy(a => a.SubledgerAccountNumber).ToList();
      return orderBySubledger;
    }


    private FixedList<BalanceEntry> GetSubledgerAccounts(FixedList<BalanceEntry> balance) {

      var balanceWithSubledgerAccounts = new List<BalanceEntry>();

      if (_command.WithSubledgerAccount) {

        foreach (var entry in balance.Where(a => a.SubledgerAccountId > 0)) {
          SubledgerAccount subledgerAccount = SubledgerAccount.Parse(entry.SubledgerAccountId);
          entry.SubledgerAccountNumber = subledgerAccount.Number;
          balanceWithSubledgerAccounts.Add(entry);
        }

        balanceWithSubledgerAccounts = GetOrderBySubledgerAccount(balanceWithSubledgerAccounts);
        return balanceWithSubledgerAccounts.ToFixedList();

      } else {
        return balance;
      }

    }


    private EmpiriaHashTable<BalanceEntry> GetTotalBalanceByLedgerAndCurrency(
                                            FixedList<BalanceEntry> balanceList) {
      var helper = new BalanceHelper(_command);

      var totalByCurrencies = new EmpiriaHashTable<BalanceEntry>();

      foreach (var entry in balanceList) {
        helper.SummaryEntriesByCurrency(totalByCurrencies, entry, TrialBalanceItemType.BalanceTotalCurrency);
      }
      return totalByCurrencies;
    }

    #endregion

  } // class SaldosPorCuentaConsultaRapida

} // Empiria.FinancialAccounting.BalanceEngine.Domain.SpecialCases
