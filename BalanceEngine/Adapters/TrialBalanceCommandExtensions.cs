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

    static internal TrialBalanceCommandData MapToTrialBalanceCommandData(
                                              this TrialBalanceCommand command,
                                              TrialBalanceCommandPeriod commandPeriod) {

      var helper = new TrialBalanceClausesHelper(command);

      return helper.GetTrialBalanceCommandData(commandPeriod);
    }


    static internal void Prepare(this TrialBalanceCommand command) {
      if (command.UseDefaultValuation) {
        command.InitialPeriod.UseDefaultValuation = true;
        command.FinalPeriod.UseDefaultValuation = true;
      }
    }

    #endregion Public methods


    /// <summary>Private inner class that provides services used to build trial balance sql clauses.</summary>
    private class TrialBalanceClausesHelper {

      private readonly TrialBalanceCommand _command;

      internal TrialBalanceClausesHelper(TrialBalanceCommand command) {
        this._command = command;
      }


      #region Public methods

      internal TrialBalanceCommandData GetTrialBalanceCommandData(TrialBalanceCommandPeriod commandPeriod) {
        var commandData = new TrialBalanceCommandData();

        var accountsChart = AccountsChart.Parse(_command.AccountsChartUID);

        StoredBalanceSet balanceSet = StoredBalanceSet.GetBestBalanceSet(
                                        accountsChart, commandPeriod.FromDate);

        commandData.AccountsChart = accountsChart;
        commandData.StoredInitialBalanceSet = balanceSet;
        commandData.FromDate = _command.TrialBalanceType == TrialBalanceType.BalanzaValorizadaComparativa ?
                               commandPeriod.ToDate.AddDays(1) : commandPeriod.FromDate;

        if (commandData.FromDate == new DateTime(2022, 1, 1) &&
            commandData.AccountsChart.Equals(AccountsChart.IFRS)) {
          commandData.FromDate = new DateTime(2022, 1, 2);
        }

        commandData.ToDate = _command.TrialBalanceType == TrialBalanceType.BalanzaValorizadaComparativa ?
                               _command.FinalPeriod.ToDate : commandPeriod.ToDate;
        commandData.InitialFields = GetInitialFields();
        commandData.Fields = GetOutputFields();
        commandData.Filters = GetFilterString();
        commandData.AccountFilters = GetAccountFilterString();
        commandData.InitialGrouping = GetInitialGroupingClause();
        commandData.Where = GetWhereClause();
        commandData.Ordering = GetOrderClause();

        commandData.AverageBalance = GetAverageBalance();

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
        string subledgerAccountFilter = GetSubledgerAccountFilter();

        var filter = new Filter(ledgerFilter);

        filter.AppendAnd(sectorFilter);
        filter.AppendAnd(accountRangeFilter);
        filter.AppendAnd(subledgerAccountFilter);
        filter.AppendAnd(currencyFilter);

        return filter.ToString().Length > 0 ? $"AND ({filter})" : "";
      }


      private string GetCurrencyFilter() {
        if (_command.Currencies.Length == 0) {
          if (_command.TrialBalanceType == TrialBalanceType.BalanzaDolarizada) {
            return $"ID_MONEDA <> 1 AND ID_MONEDA <> 28";
          }
          return string.Empty;
        }
        int[] currencyIds = _command.Currencies.Select(uid => Currency.Parse(uid).Id)
                                          .ToArray();

        return $"ID_MONEDA IN ({String.Join(", ", currencyIds)})";
      }


      private string GetWhereClause() {
        if (_command.BalancesType == BalancesType.AllAccounts) {
          return string.Empty;
        }

        string clause;

        if (_command.BalancesType == BalancesType.WithCurrentBalance) {
          clause = "SALDO_ACTUAL <> 0";

        } else if (_command.BalancesType == BalancesType.WithCurrentBalanceOrMovements) {
          clause = "SALDO_ACTUAL <> 0 OR DEBE <> 0 OR HABER <> 0";


        } else if (_command.BalancesType == BalancesType.WithMovements) {
          clause = "DEBE <> 0 OR HABER <> 0";

        } else {
          throw Assertion.AssertNoReachThisCode();
        }

        return $"WHERE {clause}";
      }


      private string GetInitialFields() {
        if (_command.DoNotReturnSubledgerAccounts && _command.Consolidated) {

          return "-1 AS ID_MAYOR, ID_MONEDA, ID_CUENTA_ESTANDAR, ID_SECTOR, -1 AS ID_CUENTA_AUXILIAR, ";

        } else if (_command.DoNotReturnSubledgerAccounts && _command.ShowCascadeBalances) {

          return "ID_MAYOR, ID_MONEDA, ID_CUENTA_ESTANDAR, ID_SECTOR, -1 AS ID_CUENTA_AUXILIAR, ";

        } else if (_command.WithSubledgerAccount && _command.Consolidated) {

          return "-1 AS ID_MAYOR, ID_MONEDA, ID_CUENTA_ESTANDAR, ID_SECTOR, ID_CUENTA_AUXILIAR, ";

        } else if (_command.WithSubledgerAccount && _command.ShowCascadeBalances) {

          return "ID_MAYOR, ID_MONEDA, ID_CUENTA_ESTANDAR, ID_SECTOR, ID_CUENTA_AUXILIAR, ";

        } else {
          throw Assertion.AssertNoReachThisCode();
        }
      }


      private string GetInitialGroupingClause() {
        if (_command.DoNotReturnSubledgerAccounts && _command.Consolidated) {

          return "GROUP BY ID_MONEDA, ID_CUENTA_ESTANDAR, ID_SECTOR";

        } else if (_command.DoNotReturnSubledgerAccounts && _command.ShowCascadeBalances) {

          return "GROUP BY ID_MAYOR, ID_MONEDA, ID_CUENTA_ESTANDAR, ID_SECTOR";

        } else if (_command.WithSubledgerAccount && _command.Consolidated) {

          return "GROUP BY ID_MONEDA, ID_CUENTA_ESTANDAR, ID_SECTOR, ID_CUENTA_AUXILIAR";

        } else if (_command.WithSubledgerAccount && _command.ShowCascadeBalances) {

          return "GROUP BY ID_MAYOR, ID_MONEDA, ID_CUENTA_ESTANDAR, ID_SECTOR, ID_CUENTA_AUXILIAR";

        } else {
          throw Assertion.AssertNoReachThisCode();
        }
      }


      private string GetOrderClause() {
        if (_command.Consolidated) {
          return "ORDER BY ID_MONEDA, NUMERO_CUENTA_ESTANDAR, ID_SECTOR";
        } else if (_command.ShowCascadeBalances) {
          return "ORDER BY ID_MAYOR, ID_MONEDA, NUMERO_CUENTA_ESTANDAR, ID_SECTOR";
        } else {
          throw Assertion.AssertNoReachThisCode();
        }
      }


      private string GetOutputFields() {

        if (_command.DoNotReturnSubledgerAccounts && _command.Consolidated) {

          return "-1 AS ID_MAYOR, ID_MONEDA, ID_CUENTA_ESTANDAR, " +
                 "NUMERO_CUENTA_ESTANDAR, ID_SECTOR, -1 AS ID_CUENTA_AUXILIAR, " +
                 "SALDO_ANTERIOR, DEBE, HABER, SALDO_ACTUAL, FECHA_ULTIMO_MOVIMIENTO, " +
                 "SALDO_PROMEDIO";

        } else if (_command.DoNotReturnSubledgerAccounts && _command.ShowCascadeBalances) {

          return "ID_MAYOR, ID_MONEDA, ID_CUENTA_ESTANDAR, NUMERO_CUENTA_ESTANDAR, ID_SECTOR, " +
                 "-1 AS ID_CUENTA_AUXILIAR, SALDO_ANTERIOR, DEBE, HABER, SALDO_ACTUAL, FECHA_ULTIMO_MOVIMIENTO, " +
                 "SALDO_PROMEDIO";

        } else if (_command.WithSubledgerAccount && _command.Consolidated) {

          return "-1 AS ID_MAYOR, ID_MONEDA, ID_CUENTA_ESTANDAR, NUMERO_CUENTA_ESTANDAR, ID_SECTOR, " +
                 "ID_CUENTA_AUXILIAR, SALDO_ANTERIOR, DEBE, HABER, SALDO_ACTUAL, FECHA_ULTIMO_MOVIMIENTO, " +
                 "SALDO_PROMEDIO";

        } else if (_command.WithSubledgerAccount && _command.ShowCascadeBalances) {

          return "ID_MAYOR, ID_MONEDA, ID_CUENTA_ESTANDAR, NUMERO_CUENTA_ESTANDAR, ID_SECTOR, " +
                 "ID_CUENTA_AUXILIAR, SALDO_ANTERIOR, DEBE, HABER, SALDO_ACTUAL, FECHA_ULTIMO_MOVIMIENTO, " +
                 "SALDO_PROMEDIO";

        } else {
          throw Assertion.AssertNoReachThisCode();
        }

      }

      private string GetAverageBalance() {

        if (_command.WithAverageBalance) {
          return $", SUM(CASE WHEN NATURALEZA = 'D' THEN (((TO_DATE( " +
                 $"'{CommonMethods.FormatSqlDate(_command.InitialPeriod.ToDate)}','DD/MM/YYYY') - " +
                 $"FECHA_AFECTACION + 1) * MOVIMIENTO) / {_command.InitialPeriod.ToDate.Day}) " +

                 $"WHEN NATURALEZA = 'A' THEN (((TO_DATE( " +
                 $"'{CommonMethods.FormatSqlDate(_command.InitialPeriod.ToDate)}','DD/MM/YYYY') - " +
                 $"FECHA_AFECTACION + 1) * MOVIMIENTO) / {_command.InitialPeriod.ToDate.Day}) " +

                 $"END) AS SALDO_PROMEDIO";
        } else {
          return ", 0 AS SALDO_PROMEDIO";
        }

      }


      private string GetAccountRangeFilter() {
        if (_command.FromAccount.Length != 0 && _command.ToAccount.Length != 0) {
          return $"NUMERO_CUENTA_ESTANDAR >= '{_command.FromAccount}' AND " +
                 $"(NUMERO_CUENTA_ESTANDAR <= '{_command.ToAccount}' OR " +
                 $"NUMERO_CUENTA_ESTANDAR LIKE '{_command.ToAccount}%')";
        } else if (_command.FromAccount.Length != 0 && _command.ToAccount.Length == 0) {
          return $"NUMERO_CUENTA_ESTANDAR LIKE '{_command.FromAccount}%'";
        } else if (_command.FromAccount.Length == 0 && _command.ToAccount.Length != 0) {
          return $"NUMERO_CUENTA_ESTANDAR <= '{_command.ToAccount}' AND " +
                 $"NUMERO_CUENTA_ESTANDAR LIKE '{_command.ToAccount}%'";
        } else {
          return string.Empty;
        }
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
