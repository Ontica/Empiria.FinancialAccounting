/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Data Layer                              *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Builder                                 *
*  Type     : BalancesSqlClausesBuilder                  License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Builds BalancesSqlClauses instances.                                                           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Linq;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine.Data {

  sealed internal partial class BalancesSqlClauses {

    /// <summary>Builds BalancesSqlClauses instances.</summary>
    sealed private class BalancesSqlClausesBuilder {

      private readonly TrialBalanceCommand _command;

      internal BalancesSqlClausesBuilder(TrialBalanceCommand command) {
        Assertion.AssertObject(command, nameof(command));

        this._command = PrepareCommand(command);
      }


      #region Methods

      internal BalancesSqlClauses Build() {
        var sqlClauses = new BalancesSqlClauses();

        var accountsChart = AccountsChart.Parse(_command.AccountsChartUID);

        StoredBalanceSet balanceSet = StoredBalanceSet.GetBestBalanceSet(accountsChart,
                                                                         _command.InitialPeriod.FromDate);

        sqlClauses.AccountsChart = accountsChart;
        sqlClauses.StoredInitialBalanceSet = balanceSet;

        SetDatesAccordingToRules(sqlClauses);

        sqlClauses.InitialFields = GetInitialFields();
        sqlClauses.Fields = GetOutputFields();
        sqlClauses.Filters = GetFilterString();
        sqlClauses.AccountFilters = GetAccountFilterString();
        sqlClauses.InitialGrouping = GetInitialGroupingClause();
        sqlClauses.Where = GetWhereClause();
        sqlClauses.Ordering = GetOrderClause();

        sqlClauses.AverageBalance = GetAverageBalance();

        return sqlClauses;
      }


      private TrialBalanceCommand PrepareCommand(TrialBalanceCommand command) {
        if (command.Ledgers.Length == 1) {
          command.ShowCascadeBalances = true;
        }
        return command;
      }


      private void SetDatesAccordingToRules(BalancesSqlClauses sqlClauses) {
        if (_command.TrialBalanceType == TrialBalanceType.BalanzaValorizadaComparativa) {
          sqlClauses.FromDate = _command.InitialPeriod.ToDate.AddDays(1);
          sqlClauses.ToDate = _command.FinalPeriod.ToDate;
        } else {
          sqlClauses.FromDate = _command.InitialPeriod.FromDate;
          sqlClauses.ToDate = _command.InitialPeriod.ToDate;
        }

        // Start on Jan 2nd because Jan 1st 2022 was used for initial balances loading
        if (sqlClauses.FromDate == new DateTime(2022, 1, 1) &&
            sqlClauses.AccountsChart.Equals(AccountsChart.IFRS)) {
          sqlClauses.FromDate = new DateTime(2022, 1, 2);
        }
      }

      #endregion Methods

      #region Helpers

      private string GetAccountFilterString() {
        string rangeFilter = GetAccountRangeFilter();

        var filter = new Filter(rangeFilter);

        return filter.ToString().Length > 0 ? $"AND {filter}" : "";
      }


      private string GetFilterString() {
        string ledgerFilter = GetLedgerFilter();
        string sectorFilter = GetSectorFilter();
        string currencyFilter = GetCurrencyFilter();
        string accountRangeFilter = GetAccountRangeFilter();
        string subledgerAccountFilter = GetSubledgerAccountFilter();

        var filter = new Filter(ledgerFilter);

        filter.AppendAnd(sectorFilter);
        filter.AppendAnd(accountRangeFilter);
        filter.AppendAnd(subledgerAccountFilter);
        filter.AppendAnd(currencyFilter);

        return filter.ToString().Length > 0 ? $"AND ({filter})" : "";
      }


      private string GetCurrencyFilter() {
        if (_command.Currencies.Length == 0 &&
            _command.TrialBalanceType == TrialBalanceType.BalanzaDolarizada) {

          return $"ID_MONEDA <> {Currency.MXN.Id} AND ID_MONEDA <> {Currency.UDI.Id}";

        } else if (_command.Currencies.Length == 0) {
          return string.Empty;

        }

        var currencyIds = _command.Currencies.Select(uid => Currency.Parse(uid).Id);

        return $"ID_MONEDA IN ({String.Join(", ", currencyIds)})";
      }


      private string GetWhereClause() {

        if (_command.BalancesType == BalancesType.AllAccounts) {
          return string.Empty;
        }

        if (_command.BalancesType == BalancesType.WithCurrentBalance) {
          return "WHERE SALDO_ACTUAL <> 0";
        }

        if (_command.BalancesType == BalancesType.WithCurrentBalanceOrMovements) {
          return "WHERE SALDO_ACTUAL <> 0 OR DEBE <> 0 OR HABER <> 0";
        }

        if (_command.BalancesType == BalancesType.WithMovements) {
          return "WHERE DEBE <> 0 OR HABER <> 0";
        }

        throw Assertion.AssertNoReachThisCode();
      }


      private string GetInitialFields() {

        if (_command.DoNotReturnSubledgerAccounts && _command.Consolidated) {
          return "-1 AS ID_MAYOR, ID_MONEDA, ID_CUENTA_ESTANDAR, ID_SECTOR, -1 AS ID_CUENTA_AUXILIAR, ";
        }

        if (_command.DoNotReturnSubledgerAccounts && _command.ShowCascadeBalances) {
          return "ID_MAYOR, ID_MONEDA, ID_CUENTA_ESTANDAR, ID_SECTOR, -1 AS ID_CUENTA_AUXILIAR, ";
        }

        if (_command.WithSubledgerAccount && _command.Consolidated) {
          return "-1 AS ID_MAYOR, ID_MONEDA, ID_CUENTA_ESTANDAR, ID_SECTOR, ID_CUENTA_AUXILIAR, ";
        }

        if (_command.WithSubledgerAccount && _command.ShowCascadeBalances) {
          return "ID_MAYOR, ID_MONEDA, ID_CUENTA_ESTANDAR, ID_SECTOR, ID_CUENTA_AUXILIAR, ";
        }

        throw Assertion.AssertNoReachThisCode();
      }


      private string GetInitialGroupingClause() {

        if (_command.DoNotReturnSubledgerAccounts && _command.Consolidated) {
          return "GROUP BY ID_MONEDA, ID_CUENTA_ESTANDAR, ID_SECTOR";

        }

        if (_command.DoNotReturnSubledgerAccounts && _command.ShowCascadeBalances) {
          return "GROUP BY ID_MAYOR, ID_MONEDA, ID_CUENTA_ESTANDAR, ID_SECTOR";
        }

        if (_command.WithSubledgerAccount && _command.Consolidated) {
          return "GROUP BY ID_MONEDA, ID_CUENTA_ESTANDAR, ID_SECTOR, ID_CUENTA_AUXILIAR";
        }

        if (_command.WithSubledgerAccount && _command.ShowCascadeBalances) {
          return "GROUP BY ID_MAYOR, ID_MONEDA, ID_CUENTA_ESTANDAR, ID_SECTOR, ID_CUENTA_AUXILIAR";
        }

        throw Assertion.AssertNoReachThisCode();
      }


      private string GetOrderClause() {
        if (_command.Consolidated) {
          return "ORDER BY ID_MONEDA, NUMERO_CUENTA_ESTANDAR, ID_SECTOR";
        }

        if (_command.ShowCascadeBalances) {
          return "ORDER BY ID_MAYOR, ID_MONEDA, NUMERO_CUENTA_ESTANDAR, ID_SECTOR";
        }

        throw Assertion.AssertNoReachThisCode();
      }


      private string GetOutputFields() {

        if (_command.DoNotReturnSubledgerAccounts && _command.Consolidated) {

          return "-1 AS ID_MAYOR, ID_MONEDA, ID_CUENTA_ESTANDAR, " +
                 "NUMERO_CUENTA_ESTANDAR, ID_SECTOR, -1 AS ID_CUENTA_AUXILIAR, " +
                 "SALDO_ANTERIOR, DEBE, HABER, SALDO_ACTUAL, FECHA_ULTIMO_MOVIMIENTO, " +
                 "SALDO_PROMEDIO";
        }

        if (_command.DoNotReturnSubledgerAccounts && _command.ShowCascadeBalances) {

          return "ID_MAYOR, ID_MONEDA, ID_CUENTA_ESTANDAR, NUMERO_CUENTA_ESTANDAR, ID_SECTOR, " +
                 "-1 AS ID_CUENTA_AUXILIAR, SALDO_ANTERIOR, DEBE, HABER, SALDO_ACTUAL, FECHA_ULTIMO_MOVIMIENTO, " +
                 "SALDO_PROMEDIO";
        }

        if (_command.WithSubledgerAccount && _command.Consolidated) {

          return "-1 AS ID_MAYOR, ID_MONEDA, ID_CUENTA_ESTANDAR, NUMERO_CUENTA_ESTANDAR, ID_SECTOR, " +
                 "ID_CUENTA_AUXILIAR, SALDO_ANTERIOR, DEBE, HABER, SALDO_ACTUAL, FECHA_ULTIMO_MOVIMIENTO, " +
                 "SALDO_PROMEDIO";

        }

        if (_command.WithSubledgerAccount && _command.ShowCascadeBalances) {

          return "ID_MAYOR, ID_MONEDA, ID_CUENTA_ESTANDAR, NUMERO_CUENTA_ESTANDAR, ID_SECTOR, " +
                 "ID_CUENTA_AUXILIAR, SALDO_ANTERIOR, DEBE, HABER, SALDO_ACTUAL, FECHA_ULTIMO_MOVIMIENTO, " +
                 "SALDO_PROMEDIO";

        }

        throw Assertion.AssertNoReachThisCode();
      }


      private string GetAverageBalance() {
        if (!_command.WithAverageBalance) {
          return ", 0 AS SALDO_PROMEDIO";
        }
        return $", SUM((({CommonMethods.FormatSqlDbDate(_command.InitialPeriod.ToDate)} - " +
                        $"FECHA_AFECTACION + 1) * MOVIMIENTO) / {_command.InitialPeriod.ToDate.Day}) " +
                $" AS SALDO_PROMEDIO";
      }


      private string GetAccountRangeFilter() {

        if (_command.FromAccount.Length == 0 && _command.ToAccount.Length == 0) {
          return string.Empty;
        }

        if (_command.FromAccount.Length != 0 && _command.ToAccount.Length == 0) {
          return $"NUMERO_CUENTA_ESTANDAR LIKE '{_command.FromAccount}%'";
        }

        if (_command.FromAccount.Length == 0 && _command.ToAccount.Length != 0) {
          return $"NUMERO_CUENTA_ESTANDAR <= '{_command.ToAccount}' AND " +
                 $"NUMERO_CUENTA_ESTANDAR LIKE '{_command.ToAccount}%'";

        }

        if (_command.FromAccount.Length != 0 && _command.ToAccount.Length != 0) {

          return $"NUMERO_CUENTA_ESTANDAR >= '{_command.FromAccount}' AND " +
                 $"(NUMERO_CUENTA_ESTANDAR <= '{_command.ToAccount}' OR " +
                 $"NUMERO_CUENTA_ESTANDAR LIKE '{_command.ToAccount}%')";
        }

        throw Assertion.AssertNoReachThisCode();
      }


      private string GetSubledgerAccountFilter() {
        if (_command.SubledgerAccount.Length == 0) {
          return string.Empty;
        }

        return $"NUMERO_CUENTA_AUXILIAR LIKE '%{_command.SubledgerAccount}'";
      }


      private string GetLedgerFilter() {
        if (_command.Ledgers.Length == 0) {
          return string.Empty;
        }

        var ledgerIds = _command.Ledgers.Select(uid => Ledger.Parse(uid).Id);

        return $"ID_MAYOR IN ({String.Join(", ", ledgerIds)})";
      }


      private string GetSectorFilter() {
        if (_command.Sectors.Length == 0) {
          return string.Empty;
        }

        var sectorIds = _command.Sectors.Select(uid => Sector.Parse(uid).Id);

        return $"ID_SECTOR IN ({String.Join(", ", sectorIds)})";
      }

      #endregion Helpers

    } // private class BalancesDataServiceSqlFilters

  }  // partial class BalancesSqlClauses

} // namespace Empiria.FinancialAccounting.BalanceEngine.Data
