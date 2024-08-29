/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                         Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Reporting.dll          Pattern   : Builder                                 *
*  Type     : PolizasPorCuentaSqlClausesBuilder          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Builds Sql clauses used for retrive data for 'Pólizas por cuenta' report.                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Linq;


namespace Empiria.FinancialAccounting.Reporting.Data {

  /// <summary>Builds Sql clauses used for retrive data for 'Pólizas por cuenta' report.</summary>
  internal class PolizasPorCuentaSqlClausesBuilder {

    private readonly ReportBuilderQuery _buildQuery;

    internal PolizasPorCuentaSqlClausesBuilder(ReportBuilderQuery buildQuery) {
      Assertion.Require(buildQuery, nameof(buildQuery));

      _buildQuery = buildQuery;
    }


    internal ListadoPolizasSqlClauses Build() {
      var accountsChart = AccountsChart.Parse(_buildQuery.AccountsChartUID);
      var commandData = new ListadoPolizasSqlClauses();

      commandData.AccountsChart = accountsChart;
      commandData.FromDate = _buildQuery.FromDate;
      commandData.ToDate = _buildQuery.ToDate;
      commandData.Filters = GetFilters();
      commandData.Fields = GetFields();
      commandData.Grouping = GetGroupingClause();

      return commandData;
    }


    #region Private methods

    private string GetAccountFilter() {
      if (_buildQuery.AccountNumber != string.Empty) {
        return $"NUMERO_CUENTA_ESTANDAR LIKE '{_buildQuery.AccountNumber}%'";
      }
      return string.Empty;
    }


    private string GetFields() {

      if (_buildQuery.ReportType == ReportTypes.MovimientosPorNumeroDeVerificacion) {

        return "NUMERO_VERIFICACION, ID_MAYOR, ID_MONEDA, ID_CUENTA_ESTANDAR, ID_SECTOR, ID_TRANSACCION, " +
               "ID_ELABORADA_POR, ID_AUTORIZADA_POR, ID_MOVIMIENTO, NUMERO_CUENTA_ESTANDAR, " +
               "NOMBRE_CUENTA_ESTANDAR, '-1' AS NUMERO_CUENTA_AUXILIAR, NUMERO_TRANSACCION, NATURALEZA, " +
               "FECHA_AFECTACION, FECHA_REGISTRO, CONCEPTO_TRANSACCION,  SUM(DEBE) AS DEBE, SUM(HABER) AS HABER";
      } else {
        
        if (_buildQuery.WithSubledgerAccount) {
          return "ID_MAYOR, ID_MONEDA, ID_CUENTA_ESTANDAR, ID_SECTOR, ID_TRANSACCION, ID_ELABORADA_POR, " +
               "ID_AUTORIZADA_POR, ID_MOVIMIENTO, NUMERO_CUENTA_ESTANDAR, NOMBRE_CUENTA_ESTANDAR, " +
               "NUMERO_CUENTA_AUXILIAR, NUMERO_TRANSACCION, NATURALEZA, FECHA_AFECTACION, FECHA_REGISTRO, " +
               "CONCEPTO_TRANSACCION,  SUM(DEBE) AS DEBE, SUM(HABER) AS HABER";
        } else {
          return "'-1' AS NUMERO_VERIFICACION, ID_MAYOR, ID_MONEDA, ID_CUENTA_ESTANDAR, ID_SECTOR, ID_TRANSACCION, ID_ELABORADA_POR, " +
               "ID_AUTORIZADA_POR, ID_MOVIMIENTO, NUMERO_CUENTA_ESTANDAR, NOMBRE_CUENTA_ESTANDAR, " +
               "'-1' AS NUMERO_CUENTA_AUXILIAR, NUMERO_TRANSACCION, NATURALEZA, FECHA_AFECTACION, FECHA_REGISTRO, " +
               "CONCEPTO_TRANSACCION,  SUM(DEBE) AS DEBE, SUM(HABER) AS HABER";
        }
      }
    }


    private string GetFilters() {

      string account = GetAccountFilter();
      string ledgers = GetLedgerFilter();
      string verificationNumber = GetVerificationNumber();

      var filter = new Filter(account);

      filter.AppendAnd(ledgers);
      filter.AppendAnd(verificationNumber);

      return filter.ToString().Length > 0 ? $"AND {filter}" : "";
    }


    private string GetGroupingClause() {

      if (_buildQuery.ReportType == ReportTypes.MovimientosPorNumeroDeVerificacion) {

        return "NUMERO_VERIFICACION, ID_MAYOR, ID_MONEDA, ID_CUENTA_ESTANDAR, ID_SECTOR, ID_TRANSACCION, ID_ELABORADA_POR, " +
               "ID_AUTORIZADA_POR, ID_MOVIMIENTO, NUMERO_CUENTA_ESTANDAR, NOMBRE_CUENTA_ESTANDAR, " +
               "NUMERO_TRANSACCION, NATURALEZA, FECHA_AFECTACION, FECHA_REGISTRO, " +
               "CONCEPTO_TRANSACCION";
      } else {

        if (_buildQuery.WithSubledgerAccount) {
          return "ID_MAYOR, ID_MONEDA, ID_CUENTA_ESTANDAR, ID_SECTOR, ID_TRANSACCION, ID_ELABORADA_POR, " +
               "ID_AUTORIZADA_POR, ID_MOVIMIENTO, NUMERO_CUENTA_ESTANDAR, NOMBRE_CUENTA_ESTANDAR, " +
               "NUMERO_CUENTA_AUXILIAR, NUMERO_TRANSACCION, NATURALEZA, FECHA_AFECTACION, FECHA_REGISTRO, " +
               "CONCEPTO_TRANSACCION";
        } else {
          return "NUMERO_VERIFICACION, ID_MAYOR, ID_MONEDA, ID_CUENTA_ESTANDAR, ID_SECTOR, ID_TRANSACCION, ID_ELABORADA_POR, " +
               "ID_AUTORIZADA_POR, ID_MOVIMIENTO, NUMERO_CUENTA_ESTANDAR, NOMBRE_CUENTA_ESTANDAR, " +
               "NUMERO_TRANSACCION, NATURALEZA, FECHA_AFECTACION, FECHA_REGISTRO, " +
               "CONCEPTO_TRANSACCION";
        }
      }
      
    }


    private string GetLedgerFilter() {
      if (_buildQuery.Ledgers.Length > 0) {
        int[] ledgerIds = _buildQuery.Ledgers
                      .Select(uid => Ledger.Parse(uid).Id).ToArray();

        return $"ID_MAYOR IN ({String.Join(", ", ledgerIds)})";
      }
      return string.Empty;
    }


    private string GetSubledgerAccountFilter() {
      if (_buildQuery.SubledgerAccountNumber != string.Empty) {
        return $"NUMERO_CUENTA_AUXILIAR = '{_buildQuery.SubledgerAccountNumber}'";
      }

      return string.Empty;
    }


    private string GetVerificationNumber() {

      if (_buildQuery.ReportType == ReportTypes.MovimientosPorNumeroDeVerificacion &&
          _buildQuery.VerificationNumbers.Length > 0) {

        return $"NUMERO_VERIFICACION IN ({String.Join(", ", _buildQuery.VerificationNumbers)})";
      }
      return string.Empty;
    }


    #endregion Private methods

  } // class PolizasPorCuentaSqlClausesBuilder

} // namespace Empiria.FinancialAccounting.Reporting.Data
