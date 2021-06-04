/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Type Extension methods                  *
*  Type     : TrialBalanceCommandExtensions              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Type extension methods for TrialBalanceCommand.                                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Linq;

namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {

  /// <summary>Type extension methods for TrialBalanceCommand.</summary>
  static internal class TrialBalanceCommandExtensions {

    #region Public methods

    static internal TrialBalanceCommandData MapToTrialBalanceCommandData(this TrialBalanceCommand command) {
      var helper = new TrialBalanceClausesHelper(command);

      return helper.GetTrialBalanceCommandData();
    }

    #endregion Public methods



    /// <summary>Private inner class that provides services used to build trial balance sql clauses.</summary>
    private class TrialBalanceClausesHelper {

      private readonly TrialBalanceCommand _command;

      internal TrialBalanceClausesHelper(TrialBalanceCommand command) {
        this._command = command;
      }


      #region Public methods

      internal TrialBalanceCommandData GetTrialBalanceCommandData() {
        var commandData = new TrialBalanceCommandData();

        var accountsChart = AccountsChart.Parse(_command.AccountsChartUID);

        var _dummy = accountsChart.Accounts;

        StoredBalanceSet balanceSet = StoredBalanceSet.GetBestBalanceSet(accountsChart, _command.FromDate);

        commandData.StoredInitialBalanceSet = balanceSet;
        commandData.FromDate = _command.FromDate;
        commandData.ToDate = _command.ToDate;
        commandData.InitialFields = GetInitialFields();
        commandData.Fields = GetOutputFields();
        commandData.Filters = GetFilterString();
        commandData.AccountFilters = GetAccountFilterString();
        commandData.InitialGrouping = GetInitialGroupingClause();
        commandData.Grouping = GetGroupingClause();
        commandData.Having = GetHavingClause();
        commandData.Ordering = GetOrderClause();
        commandData.AccountsChart = accountsChart;

        return commandData;
      }

      #endregion Public methods


      #region Private methods

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

        var filter = new Filter(ledgerFilter);

        filter.AppendAnd(sectorFilter);
        filter.AppendAnd(accountRangeFilter);
        filter.AppendAnd(currencyFilter);

        return filter.ToString().Length > 0 ? $"AND ({filter})" : "";
      }


      private string GetCurrencyFilter() {
        if (_command.Currencies.Length == 0) {
          return string.Empty;
        }
        int[] currencyIds = _command.Currencies.Select(uid => Currency.Parse(uid).Id)
                                          .ToArray();

        return $"ID_MONEDA IN ({String.Join(", ", currencyIds)})";
      }


      private string GetGroupingClause() {
        if (_command.TrialBalanceType == TrialBalanceType.Traditional &&
            _command.Consolidated) {

          return "GROUP BY ID_MONEDA, ID_CUENTA_ESTANDAR_HIST, NUMERO_CUENTA_ESTANDAR, ID_SECTOR, " +
                 "SALDO_ANTERIOR, DEBE, HABER, SALDO_ACTUAL";

        } else if (_command.TrialBalanceType == TrialBalanceType.Traditional &&
                   !_command.Consolidated) {

          return "GROUP BY ID_MAYOR, ID_MONEDA, ID_CUENTA_ESTANDAR_HIST, NUMERO_CUENTA_ESTANDAR, ID_SECTOR, " +
                 "ID_CUENTA, SALDO_ANTERIOR, DEBE, HABER, SALDO_ACTUAL";

        } else if (_command.TrialBalanceType == TrialBalanceType.BalancesByAccount &&
                  _command.Consolidated) {

          return "GROUP BY ID_MONEDA, ID_CUENTA_ESTANDAR_HIST, NUMERO_CUENTA_ESTANDAR, ID_SECTOR, " +
                 "ID_CUENTA_AUXILIAR, NUMERO_CUENTA_AUXILIAR, SALDO_ANTERIOR, DEBE, HABER, SALDO_ACTUAL";

        } else if (_command.TrialBalanceType == TrialBalanceType.BalancesByAccount &&
                   !_command.Consolidated) {

          return "GROUP BY ID_MAYOR, ID_MONEDA, ID_CUENTA_ESTANDAR_HIST, NUMERO_CUENTA_ESTANDAR, ID_SECTOR, " +
                 "ID_CUENTA, ID_CUENTA_AUXILIAR, NUMERO_CUENTA_AUXILIAR, SALDO_ANTERIOR, DEBE, HABER, SALDO_ACTUAL";

        } else {
          throw Assertion.AssertNoReachThisCode();
        }
      }


      private string GetHavingClause() {
        if (_command.BalancesType == BalancesType.AllAccounts) {
          return string.Empty;
        }

        string clause;

        if (_command.BalancesType == BalancesType.WithCurrentBalance) {
          clause = "SALDO_ACTUAL <> 0";

        } else if (_command.BalancesType == BalancesType.WithCurrenBalanceOrMovements) {
          clause = "SALDO_ACTUAL <> 0 OR DEBE <> 0 OR HABER <> 0";


        } else if (_command.BalancesType == BalancesType.WithMovements) {
          clause = "DEBE <> 0 OR HABER <> 0";

        } else {
          throw Assertion.AssertNoReachThisCode();
        }

        return $"HAVING {clause}";
      }


      private string GetInitialFields() {
        if (_command.TrialBalanceType == TrialBalanceType.Traditional &&
            _command.Consolidated) {

          return "-1 AS ID_MAYOR, ID_MONEDA, ID_CUENTA_ESTANDAR, ID_SECTOR, -1 AS ID_CUENTA, -1 AS ID_CUENTA_AUXILIAR, ";

        } else if (_command.TrialBalanceType == TrialBalanceType.Traditional &&
                   !_command.Consolidated) {

          return "ID_MAYOR, ID_MONEDA, ID_CUENTA_ESTANDAR, ID_SECTOR, ID_CUENTA, -1 AS ID_CUENTA_AUXILIAR, ";

        } else if (_command.TrialBalanceType == TrialBalanceType.BalancesByAccount &&
                   _command.Consolidated) {

          return "-1 AS ID_MAYOR, ID_MONEDA, ID_CUENTA_ESTANDAR, ID_SECTOR, -1 AS ID_CUENTA, ID_CUENTA_AUXILIAR, ";

        } else if (_command.TrialBalanceType == TrialBalanceType.BalancesByAccount &&
                   !_command.Consolidated) {

          return "ID_MAYOR, ID_MONEDA, ID_CUENTA_ESTANDAR, ID_SECTOR, ID_CUENTA, ID_CUENTA_AUXILIAR, ";

        } else {
          throw Assertion.AssertNoReachThisCode();
        }
      }


      private string GetInitialGroupingClause() {
        if (_command.TrialBalanceType == TrialBalanceType.Traditional &&
            _command.Consolidated) {

          return "GROUP BY ID_MONEDA, ID_CUENTA_ESTANDAR, ID_SECTOR";

        } else if (_command.TrialBalanceType == TrialBalanceType.Traditional &&
                   !_command.Consolidated) {

          return "GROUP BY ID_MAYOR, ID_MONEDA, ID_CUENTA_ESTANDAR, ID_SECTOR, ID_CUENTA";

        } else if (_command.TrialBalanceType == TrialBalanceType.BalancesByAccount &&
                   _command.Consolidated) {

          return "GROUP BY ID_MONEDA, ID_CUENTA_ESTANDAR, ID_SECTOR, ID_CUENTA_AUXILIAR";

        } else if (_command.TrialBalanceType == TrialBalanceType.BalancesByAccount &&
                   !_command.Consolidated) {

          return "GROUP BY ID_MAYOR, ID_MONEDA, ID_CUENTA_ESTANDAR, ID_SECTOR, ID_CUENTA, ID_CUENTA_AUXILIAR";

        } else {
          throw Assertion.AssertNoReachThisCode();
        }
      }


      private string GetOrderClause() {
        if (_command.Consolidated) {
          return "ORDER BY ID_MONEDA, NUMERO_CUENTA_ESTANDAR, ID_SECTOR";
        } else {
          return "ORDER BY ID_MAYOR, ID_MONEDA, NUMERO_CUENTA_ESTANDAR, ID_SECTOR, ID_CUENTA";
        }
      }


      private string GetOutputFields() {
        if (_command.TrialBalanceType == TrialBalanceType.Traditional &&
            _command.Consolidated) {

          return "-1 AS ID_MAYOR, ID_MONEDA, ID_CUENTA_ESTANDAR_HIST, -1 AS ID_CUENTA, " +
                 "NUMERO_CUENTA_ESTANDAR, ID_SECTOR, -1 AS ID_CUENTA_AUXILIAR, " +
                 "SALDO_ANTERIOR, DEBE, HABER, SALDO_ACTUAL";

        } else if (_command.TrialBalanceType == TrialBalanceType.Traditional &&
                   !_command.Consolidated) {

          return "ID_MAYOR, ID_MONEDA, ID_CUENTA_ESTANDAR_HIST, NUMERO_CUENTA_ESTANDAR, ID_SECTOR, " +
                 "ID_CUENTA, -1 AS ID_CUENTA_AUXILIAR, SALDO_ANTERIOR, DEBE, HABER, SALDO_ACTUAL";

        } else if (_command.TrialBalanceType == TrialBalanceType.BalancesByAccount &&
                   _command.Consolidated) {

          return "-1 AS ID_MAYOR, ID_MONEDA, ID_CUENTA_ESTANDAR_HIST, NUMERO_CUENTA_ESTANDAR, ID_SECTOR, " +
                 "-1 AS ID_CUENTA, ID_CUENTA_AUXILIAR, SALDO_ANTERIOR, DEBE, HABER, SALDO_ACTUAL";

        } else if (_command.TrialBalanceType == TrialBalanceType.BalancesByAccount &&
                   !_command.Consolidated) {

          return "ID_MAYOR, ID_MONEDA, ID_CUENTA_ESTANDAR_HIST, NUMERO_CUENTA_ESTANDAR, ID_SECTOR, " +
                 "ID_CUENTA, ID_CUENTA_AUXILIAR, SALDO_ANTERIOR, DEBE, HABER, SALDO_ACTUAL";

        } else {
          throw Assertion.AssertNoReachThisCode();
        }
      }


      private string GetAccountRangeFilter() {
        string filter = String.Empty;

        if (_command.FromAccount.Length != 0) {
          filter = $"NUMERO_CUENTA_ESTANDAR >= '{_command.FromAccount}'";
        }
        if (_command.ToAccount.Length != 0) {
          if (filter.Length != 0) {
            filter += " AND ";
          }
          filter += $"NUMERO_CUENTA_ESTANDAR <= '{_command.ToAccount}'";
        }

        return filter;
      }


      private string GetLedgerFilter() {
        if (_command.Ledgers.Length == 0) {
          return string.Empty;
        }

        int[] ledgerIds = _command.Ledgers.Select(uid => Ledger.Parse(uid).Id)
                                          .ToArray();

        return $"ID_MAYOR IN ({String.Join(", ", ledgerIds)})";
      }


      private string GetSectorFilter() {
        if (_command.Sectors.Length == 0) {
          return string.Empty;
        }

        int[] sectorIds = _command.Sectors.Select(uid => Sector.Parse(uid).Id)
                                             .ToArray();

        return $"ID_SECTOR IN ({String.Join(", ", sectorIds)})";
      }


      #endregion Private methods


    } // private inner class TrialBalanceClausesHelper


  }  // class TrialBalanceCommandExtensions

} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
