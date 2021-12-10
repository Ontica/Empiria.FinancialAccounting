/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Type Extension methods                  *
*  Type     : VouchersByAccountCommandExtensions         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Type extension methods for VouchersByAccountCommand.                                           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Linq;
using Empiria.FinancialAccounting.BalanceEngine.Data;

namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {

  /// <summary>Type extension methods for VouchersByAccountCommand.</summary>
  internal class VouchersByAccountCommandExtensions {

    private BalanceCommand _command;
    internal VouchersByAccountCommandExtensions(BalanceCommand command) {
      _command = command;
    }

    #region Public methods

    internal VouchersByAccountCommandData MapToVouchersByAccountCommandData() {
      var commandData = new VouchersByAccountCommandData();

      var accountsChart = AccountsChart.Parse(_command.AccountsChartUID);


      commandData.Fields = GetFields();
      commandData.AccountsChartId = accountsChart.Id;
      commandData.FromDate = _command.InitialPeriod.FromDate;
      commandData.ToDate = _command.InitialPeriod.ToDate;
      commandData.Filters = GetFilters();
      commandData.Grouping = GetGroupingClause();
      return commandData;
    }

    private string GetGroupingClause() {
      return string.Empty;
    }

    #endregion


    #region Private methods

    private string GetFilters() {
      string ledgerFilter = GetLedgerFilter();
      string currencyFilter = GetCurrencyFilter();
      string accountRangeFilter = GetAccountFilter();
      string subledgerAccountFilter = GetSubledgerAccountFilter();

      var filter = new Filter(ledgerFilter);

      filter.AppendAnd(currencyFilter);
      filter.AppendAnd(accountRangeFilter);
      filter.AppendAnd(subledgerAccountFilter);

      return filter.ToString().Length > 0 ? $"AND ({filter})" : "";
    }

    private string GetSubledgerAccountFilter() {
      if (_command.SubledgerAccount.Length == 0) {
        return string.Empty;
      }
      return $"NUMERO_CUENTA_AUXILIAR LIKE '%{_command.SubledgerAccount}'";
    }

    private string GetAccountFilter() {
      if (_command.FromAccount.Length != 0) {
        return $"NUMERO_CUENTA_ESTANDAR LIKE '{_command.FromAccount}%'";
      } else {
        return string.Empty;
      }
    }

    private string GetCurrencyFilter() {
      if (_command.Currencies.Length == 0) {
        return string.Empty;
      }
      int[] currencyIds = _command.Currencies.Select(uid => Currency.Parse(uid).Id).ToArray();

      return $"ID_MONEDA IN ({String.Join(", ", currencyIds)})";
    }

    private string GetLedgerFilter() {
      if (_command.Ledgers.Length == 0) {
        return string.Empty;
      }

      int[] ledgerIds = _command.Ledgers.Select(uid => Ledger.Parse(uid).Id)
                                        .ToArray();

      return $"ID_MAYOR IN ({String.Join(", ", ledgerIds)})";
    }

    static private string GetFields() {
      return "ID_MAYOR, ID_MONEDA, ID_CUENTA_ESTANDAR, ID_SECTOR, ID_CUENTA_AUXILIAR, " +
             "ID_TRANSACCION, SUM(DEBE) AS DEBE, SUM(HABER) AS HABER";
    }

    #endregion



  } // class VouchersByAccountCommandExtensions

} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
