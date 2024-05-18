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

using Empiria.Data;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine.Data {

  sealed internal partial class BalancesSqlClauses {

    /// <summary>Builds BalancesSqlClauses instances.</summary>
    sealed private class BalancesSqlClausesBuilder {

      private readonly TrialBalanceQuery _query;

      internal BalancesSqlClausesBuilder(TrialBalanceQuery query) {
        Assertion.Require(query, nameof(query));

        this._query = PrepareQuery(query);
      }


      #region Methods

      internal BalancesSqlClauses Build() {
        var sqlClauses = new BalancesSqlClauses();

        var accountsChart = AccountsChart.Parse(_query.AccountsChartUID);

        StoredBalanceSet balanceSet = StoredBalanceSet.GetBestBalanceSet(accountsChart,
                                                                         _query.InitialPeriod.FromDate);

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


      private TrialBalanceQuery PrepareQuery(TrialBalanceQuery query) {
        if (query.Ledgers.Length == 1) {
          query.ShowCascadeBalances = true;
        }
        return query;
      }


      private void SetDatesAccordingToRules(BalancesSqlClauses sqlClauses) {
        if (_query.TrialBalanceType == TrialBalanceType.BalanzaValorizadaComparativa) {
          sqlClauses.FromDate = _query.InitialPeriod.ToDate.AddDays(1);
          sqlClauses.ToDate = _query.FinalPeriod.ToDate;
        } else {
          sqlClauses.FromDate = _query.InitialPeriod.FromDate;
          sqlClauses.ToDate = _query.InitialPeriod.ToDate;
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
        if (_query.Currencies.Length == 0 &&
            _query.TrialBalanceType == TrialBalanceType.BalanzaDolarizada) {

          return $"ID_MONEDA <> {Currency.MXN.Id} AND ID_MONEDA <> {Currency.UDI.Id}";

        }
        //else if (_query.TrialBalanceType == TrialBalanceType.ValorizacionEstimacionPreventiva) {

        //  return $"ID_MONEDA <> {Currency.MXN.Id}";

        //}
        else if (_query.Currencies.Length == 0) {
          return string.Empty;

        }

        var currencyIds = _query.Currencies.Select(uid => Currency.Parse(uid).Id);

        return $"ID_MONEDA IN ({String.Join(", ", currencyIds)})";
      }


      private string GetWhereClause() {

        if (_query.BalancesType == BalancesType.AllAccounts) {
          return string.Empty;
        }

        if (_query.BalancesType == BalancesType.WithCurrentBalance) {
          return "WHERE SALDO_ACTUAL <> 0";
        }

        if (_query.BalancesType == BalancesType.WithCurrentBalanceOrMovements) {
          return "WHERE SALDO_ACTUAL <> 0 OR DEBE <> 0 OR HABER <> 0";
        }

        if (_query.BalancesType == BalancesType.WithMovements) {
          return "WHERE DEBE <> 0 OR HABER <> 0";
        }

        throw Assertion.EnsureNoReachThisCode();
      }


      private string GetInitialFields() {

        if (_query.DoNotReturnSubledgerAccounts && _query.Consolidated) {
          return "-1 AS ID_MAYOR, ID_MONEDA, ID_CUENTA_ESTANDAR, ID_SECTOR, -1 AS ID_CUENTA_AUXILIAR, ";
        }

        if (_query.DoNotReturnSubledgerAccounts && _query.ShowCascadeBalances) {
          return "ID_MAYOR, ID_MONEDA, ID_CUENTA_ESTANDAR, ID_SECTOR, -1 AS ID_CUENTA_AUXILIAR, ";
        }

        if (_query.WithSubledgerAccount && _query.Consolidated) {
          return "-1 AS ID_MAYOR, ID_MONEDA, ID_CUENTA_ESTANDAR, ID_SECTOR, ID_CUENTA_AUXILIAR, ";
        }

        if (_query.WithSubledgerAccount && _query.ShowCascadeBalances) {
          return "ID_MAYOR, ID_MONEDA, ID_CUENTA_ESTANDAR, ID_SECTOR, ID_CUENTA_AUXILIAR, ";
        }

        throw Assertion.EnsureNoReachThisCode();
      }


      private string GetInitialGroupingClause() {

        if (_query.DoNotReturnSubledgerAccounts && _query.Consolidated) {
          return "GROUP BY ID_MONEDA, ID_CUENTA_ESTANDAR, ID_SECTOR";

        }

        if (_query.DoNotReturnSubledgerAccounts && _query.ShowCascadeBalances) {
          return "GROUP BY ID_MAYOR, ID_MONEDA, ID_CUENTA_ESTANDAR, ID_SECTOR";
        }

        if (_query.WithSubledgerAccount && _query.Consolidated) {
          return "GROUP BY ID_MONEDA, ID_CUENTA_ESTANDAR, ID_SECTOR, ID_CUENTA_AUXILIAR";
        }

        if (_query.WithSubledgerAccount && _query.ShowCascadeBalances) {
          return "GROUP BY ID_MAYOR, ID_MONEDA, ID_CUENTA_ESTANDAR, ID_SECTOR, ID_CUENTA_AUXILIAR";
        }

        throw Assertion.EnsureNoReachThisCode();
      }


      private string GetOrderClause() {
        if (_query.Consolidated) {
          return "ORDER BY ID_MONEDA, NUMERO_CUENTA_ESTANDAR, ID_SECTOR";
        }

        if (_query.ShowCascadeBalances) {
          return "ORDER BY ID_MAYOR, ID_MONEDA, NUMERO_CUENTA_ESTANDAR, ID_SECTOR";
        }

        throw Assertion.EnsureNoReachThisCode();
      }


      private string GetOutputFields() {

        if (_query.DoNotReturnSubledgerAccounts && _query.Consolidated) {

          return "-1 AS ID_MAYOR, ID_MONEDA, ID_CUENTA_ESTANDAR, " +
                 "NUMERO_CUENTA_ESTANDAR, ID_SECTOR, -1 AS ID_CUENTA_AUXILIAR, " +
                 "SALDO_ANTERIOR, DEBE, HABER, SALDO_ACTUAL, FECHA_ULTIMO_MOVIMIENTO, " +
                 "SALDO_PROMEDIO";
        }

        if (_query.DoNotReturnSubledgerAccounts && _query.ShowCascadeBalances) {

          return "ID_MAYOR, ID_MONEDA, ID_CUENTA_ESTANDAR, NUMERO_CUENTA_ESTANDAR, ID_SECTOR, " +
                 "-1 AS ID_CUENTA_AUXILIAR, SALDO_ANTERIOR, DEBE, HABER, SALDO_ACTUAL, FECHA_ULTIMO_MOVIMIENTO, " +
                 "SALDO_PROMEDIO";
        }

        if (_query.WithSubledgerAccount && _query.Consolidated) {

          return "-1 AS ID_MAYOR, ID_MONEDA, ID_CUENTA_ESTANDAR, NUMERO_CUENTA_ESTANDAR, ID_SECTOR, " +
                 "ID_CUENTA_AUXILIAR, SALDO_ANTERIOR, DEBE, HABER, SALDO_ACTUAL, FECHA_ULTIMO_MOVIMIENTO, " +
                 "SALDO_PROMEDIO";

        }

        if (_query.WithSubledgerAccount && _query.ShowCascadeBalances) {

          return "ID_MAYOR, ID_MONEDA, ID_CUENTA_ESTANDAR, NUMERO_CUENTA_ESTANDAR, ID_SECTOR, " +
                 "ID_CUENTA_AUXILIAR, SALDO_ANTERIOR, DEBE, HABER, SALDO_ACTUAL, FECHA_ULTIMO_MOVIMIENTO, " +
                 "SALDO_PROMEDIO";

        }

        throw Assertion.EnsureNoReachThisCode();
      }


      private string GetAverageBalance() {
        if (!_query.WithAverageBalance) {
          return ", 0 AS SALDO_PROMEDIO";
        }
        return $", SUM((({DataCommonMethods.FormatSqlDbDate(_query.InitialPeriod.ToDate)} - " +
                        $"FECHA_AFECTACION + 1) * MOVIMIENTO) / {_query.InitialPeriod.ToDate.Day}) " +
                $" AS SALDO_PROMEDIO";
      }


      private string GetAccountRangeFilter() {

        if (_query.FromAccount.Length == 0 && _query.ToAccount.Length == 0) {
          return string.Empty;
        }

        if (_query.FromAccount.Length != 0 && _query.ToAccount.Length == 0) {
          return $"NUMERO_CUENTA_ESTANDAR LIKE '{_query.FromAccount}%'";
        }

        if (_query.FromAccount.Length == 0 && _query.ToAccount.Length != 0) {
          return $"NUMERO_CUENTA_ESTANDAR <= '{_query.ToAccount}' AND " +
                 $"NUMERO_CUENTA_ESTANDAR LIKE '{_query.ToAccount}%'";

        }

        if (_query.FromAccount.Length != 0 && _query.ToAccount.Length != 0) {

          return $"NUMERO_CUENTA_ESTANDAR >= '{_query.FromAccount}' AND " +
                 $"(NUMERO_CUENTA_ESTANDAR <= '{_query.ToAccount}' OR " +
                 $"NUMERO_CUENTA_ESTANDAR LIKE '{_query.ToAccount}%')";
        }

        throw Assertion.EnsureNoReachThisCode();
      }


      private string GetSubledgerAccountFilter() {
        if (_query.SubledgerAccount.Length == 0) {
          return string.Empty;
        }

        return $"NUMERO_CUENTA_AUXILIAR LIKE '%{_query.SubledgerAccount}'";
      }


      private string GetLedgerFilter() {
        if (_query.Ledgers.Length == 0) {
          return string.Empty;
        }

        var ledgerIds = _query.Ledgers.Select(uid => Ledger.Parse(uid).Id);

        return $"ID_MAYOR IN ({String.Join(", ", ledgerIds)})";
      }


      private string GetSectorFilter() {
        if (_query.Sectors.Length == 0) {
          return string.Empty;
        }

        var sectorIds = _query.Sectors.Select(uid => Sector.Parse(uid).Id);

        return $"ID_SECTOR IN ({String.Join(", ", sectorIds)})";
      }

      #endregion Helpers

    } // private class BalancesDataServiceSqlFilters

  }  // partial class BalancesSqlClauses

} // namespace Empiria.FinancialAccounting.BalanceEngine.Data
