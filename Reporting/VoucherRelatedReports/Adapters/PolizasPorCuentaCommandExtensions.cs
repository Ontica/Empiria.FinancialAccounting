/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                         Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Reporting.dll          Pattern   : Type Extension methods                  *
*  Type     : PolizasPorCuentaCommandExtensions                    License   : Please read LICENSE.txt file  *
*                                                                                                            *
*  Summary  : Type extension methods for PolizasPorCuentaCommand.                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Linq;

namespace Empiria.FinancialAccounting.Reporting.Adapters {

  /// <summary>Type extension methods for PolizasPorCuentaCommand.</summary>
  internal class PolizasPorCuentaCommandExtensions {


    internal PolizaCommandData MapToPolizaCommandData(BuildReportCommand command) {
      var clauses = new ListadoPolizasPorCuentaClausesHelper(command);

      return clauses.GetPolizasCommandData();
    }


    private class ListadoPolizasPorCuentaClausesHelper {

      private readonly BuildReportCommand _command;

      internal ListadoPolizasPorCuentaClausesHelper(BuildReportCommand command) {
        _command = command;
      }


      #region Public methods


      internal PolizaCommandData GetPolizasCommandData() {
        var accountsChart = AccountsChart.Parse(_command.AccountsChartUID);
        var commandData = new PolizaCommandData();

        commandData.AccountsChart = accountsChart;
        commandData.FromDate = _command.FromDate;
        commandData.ToDate = _command.ToDate;
        commandData.Filters = GetFilters();
        commandData.Fields = GetFields();
        commandData.Grouping = GetGroupingClause();

        return commandData;
      }


      #endregion Public methods


      #region Private methods


      private string GetAccountFilter() {
        if (_command.AccountNumber != string.Empty) {
          return $"NUMERO_CUENTA_ESTANDAR LIKE '{_command.AccountNumber}%'";
        }
        return string.Empty;
      }


      private string GetFields() {
        if (_command.WithSubledgerAccount) {
          return "ID_MAYOR, ID_MONEDA, ID_CUENTA_ESTANDAR, ID_SECTOR, ID_TRANSACCION, ID_ELABORADA_POR, " +
               "ID_AUTORIZADA_POR, ID_MOVIMIENTO, NUMERO_CUENTA_ESTANDAR, NOMBRE_CUENTA_ESTANDAR, " +
               "NUMERO_CUENTA_AUXILIAR, NUMERO_TRANSACCION, NATURALEZA, FECHA_AFECTACION, FECHA_REGISTRO, " +
               "CONCEPTO_TRANSACCION,  SUM(DEBE) AS DEBE, SUM(HABER) AS HABER";
        } else {
          return "ID_MAYOR, ID_MONEDA, ID_CUENTA_ESTANDAR, ID_SECTOR, ID_TRANSACCION, ID_ELABORADA_POR, " +
               "ID_AUTORIZADA_POR, ID_MOVIMIENTO, NUMERO_CUENTA_ESTANDAR, NOMBRE_CUENTA_ESTANDAR, " +
               "'-1' AS NUMERO_CUENTA_AUXILIAR, NUMERO_TRANSACCION, NATURALEZA, FECHA_AFECTACION, FECHA_REGISTRO, " +
               "CONCEPTO_TRANSACCION,  SUM(DEBE) AS DEBE, SUM(HABER) AS HABER";
        }
      }


      private string GetFilters() {
        string account = GetAccountFilter();
        //string subledgerAccount = GetSubledgerAccountFilter();
        string ledgers = GetLedgerFilter();

        var filter = new Filter(account);
        //filter.AppendAnd(subledgerAccount);
        filter.AppendAnd(ledgers);


        return filter.ToString().Length > 0 ? $"AND {filter}" : "";
      }


      private string GetGroupingClause() {
        if (_command.WithSubledgerAccount) {
          return "ID_MAYOR, ID_MONEDA, ID_CUENTA_ESTANDAR, ID_SECTOR, ID_TRANSACCION, ID_ELABORADA_POR, " +
               "ID_AUTORIZADA_POR, ID_MOVIMIENTO, NUMERO_CUENTA_ESTANDAR, NOMBRE_CUENTA_ESTANDAR, " +
               "NUMERO_CUENTA_AUXILIAR, NUMERO_TRANSACCION, NATURALEZA, FECHA_AFECTACION, FECHA_REGISTRO, " +
               "CONCEPTO_TRANSACCION";
        } else {
          return "ID_MAYOR, ID_MONEDA, ID_CUENTA_ESTANDAR, ID_SECTOR, ID_TRANSACCION, ID_ELABORADA_POR, " +
               "ID_AUTORIZADA_POR, ID_MOVIMIENTO, NUMERO_CUENTA_ESTANDAR, NOMBRE_CUENTA_ESTANDAR, " +
               "NUMERO_TRANSACCION, NATURALEZA, FECHA_AFECTACION, FECHA_REGISTRO, " +
               "CONCEPTO_TRANSACCION";
        }
      }


      private string GetLedgerFilter() {
        if (_command.Ledgers.Length > 0) {
          int[] ledgerIds = _command.Ledgers
                        .Select(uid => Ledger.Parse(uid).Id).ToArray();

          return $"ID_MAYOR IN ({String.Join(", ", ledgerIds)})";
        }
        return string.Empty;
      }


      private string GetSubledgerAccountFilter() {
        if (_command.SubledgerAccountNumber != string.Empty) {
          return $"NUMERO_CUENTA_AUXILIAR = '{_command.SubledgerAccountNumber}'";
        }

        return string.Empty;
      }

      #endregion Private methods

    }

  } // class PolizasPorCuentaCommandExtensions

} // namespace Empiria.FinancialAccounting.Reporting.Adapters
