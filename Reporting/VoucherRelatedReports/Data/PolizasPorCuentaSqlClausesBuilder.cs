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
using Empiria.Data;


namespace Empiria.FinancialAccounting.Reporting.Data {

    /// <summary>Builds Sql clauses used for retrive data for 'Pólizas por cuenta' report.</summary>
    internal class PolizasPorCuentaSqlClausesBuilder {

        private readonly ReportBuilderQuery _buildQuery;

        internal PolizasPorCuentaSqlClausesBuilder(ReportBuilderQuery buildQuery) {
            Assertion.Require(buildQuery, nameof(buildQuery));

            _buildQuery = buildQuery;
        }



        internal ListadoPolizasSqlClauses Build() {
            var commandData = new ListadoPolizasSqlClauses();
            commandData.Fields = GetFields();
            commandData.Filters = GetFilters();
            commandData.Grouping = GetGroupingClause();
            commandData.Ordering = string.Empty;
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
                       "FECHA_AFECTACION, FECHA_REGISTRO, CONCEPTO_TRANSACCION, " +
                       "SUM(DEBE) AS DEBE, SUM(HABER) AS HABER";

            } else {

                return "NUMERO_VERIFICACION, ID_MAYOR, ID_MONEDA, ID_CUENTA_ESTANDAR, ID_SECTOR, " +
                           "ID_TRANSACCION, ID_ELABORADA_POR, ID_AUTORIZADA_POR, ID_MOVIMIENTO, " +
                           "NUMERO_CUENTA_ESTANDAR, NOMBRE_CUENTA_ESTANDAR, NUMERO_CUENTA_AUXILIAR, NOMBRE_CUENTA_AUXILIAR, " +
                           "NUMERO_TRANSACCION, NATURALEZA, FECHA_AFECTACION, FECHA_REGISTRO, CONCEPTO_TRANSACCION, " +
                           "SUM(DEBE) AS DEBE, SUM(HABER) AS HABER";
            }
        }


        private string GetFilters() {

            string accountChart = GetAccountsChartFilter();
            string dateFilter = GetDateFilter();
            string account = GetAccountFilter();
            string ledgers = GetLedgerFilter();
            string verificationNumber = GetVerificationNumber();
            string voucherRangeFilter = GetVoucherRangeFilter();

            var filter = new Filter(accountChart);

            filter.AppendAnd(dateFilter);
            filter.AppendAnd(account);
            filter.AppendAnd(ledgers);
            filter.AppendAnd(verificationNumber);
            filter.AppendAnd(voucherRangeFilter);

            return filter.ToString().Length > 0 ? $"{filter}" : "";
        }

        private string GetVoucherRangeFilter() {

            if (_buildQuery.ReportType == ReportTypes.ListadoMovimientosPorPolizas) {

                return $"ID_TRANSACCION IN ({String.Join(", ", _buildQuery.VoucherIds)})";
            } else {

                return string.Empty;
            }

        }

        private string GetAccountsChartFilter() {

            if (_buildQuery.ReportType == ReportTypes.ListadoDePolizasPorCuenta ||
                _buildQuery.ReportType == ReportTypes.MovimientosPorNumeroDeVerificacion) {

                return $"ID_TIPO_CUENTAS_STD = {AccountsChart.Parse(_buildQuery.AccountsChartUID).Id}";
            } else {

                return string.Empty;
            }
        }


        private string GetDateFilter() {
            if (_buildQuery.ReportType == ReportTypes.ListadoDePolizasPorCuenta ||
                _buildQuery.ReportType == ReportTypes.MovimientosPorNumeroDeVerificacion) {

                return $"FECHA_AFECTACION >= {DataCommonMethods.FormatSqlDbDate(_buildQuery.FromDate)} " +
                       $"AND FECHA_AFECTACION <= {DataCommonMethods.FormatSqlDbDate(_buildQuery.ToDate)}";
            } else {

                return string.Empty;
            }
        }

        private string GetGroupingClause() {

            if (_buildQuery.ReportType == ReportTypes.MovimientosPorNumeroDeVerificacion) {

                return "NUMERO_VERIFICACION, ID_MAYOR, ID_MONEDA, ID_CUENTA_ESTANDAR, ID_SECTOR, ID_TRANSACCION, ID_ELABORADA_POR, " +
                       "ID_AUTORIZADA_POR, ID_MOVIMIENTO, NUMERO_CUENTA_ESTANDAR, NOMBRE_CUENTA_ESTANDAR, " +
                       "NUMERO_TRANSACCION, NATURALEZA, FECHA_AFECTACION, FECHA_REGISTRO, " +
                       "CONCEPTO_TRANSACCION";

            } else {

                return "NUMERO_VERIFICACION, ID_MAYOR, ID_MONEDA, ID_CUENTA_ESTANDAR, ID_SECTOR, ID_TRANSACCION, " +
                           "ID_ELABORADA_POR, ID_AUTORIZADA_POR, ID_MOVIMIENTO, NUMERO_CUENTA_ESTANDAR, " +
                           "NOMBRE_CUENTA_ESTANDAR, NUMERO_CUENTA_AUXILIAR, NOMBRE_CUENTA_AUXILIAR, NUMERO_TRANSACCION, NATURALEZA, " +
                           "FECHA_AFECTACION, FECHA_REGISTRO, CONCEPTO_TRANSACCION";
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

            if (_buildQuery.ReportType == ReportTypes.MovimientosPorNumeroDeVerificacion) {

                if (_buildQuery.VerificationNumbers.Length > 0) {

                    return $"NUMERO_VERIFICACION IN ({String.Join(", ", _buildQuery.VerificationNumbers)})";

                } else if (_buildQuery.VerificationNumbers.Length == 0 && _buildQuery.AccountNumber != string.Empty) {

                    return $"NUMERO_VERIFICACION > 0";
                }
            }

            return string.Empty;
        }


        #endregion Private methods

    } // class PolizasPorCuentaSqlClausesBuilder

} // namespace Empiria.FinancialAccounting.Reporting.Data
