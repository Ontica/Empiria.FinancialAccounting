/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                            Component : Interface adapters                   *
*  Assembly : FinancialAccounting.Reporting.dll             Pattern   : Query payload                        *
*  Type     : ReportBuilderQueryExtensions                  License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Class extension used to generate clauses from query for financial accounting reports.          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Linq;
using Empiria.Data;

namespace Empiria.FinancialAccounting.Reporting {


    /// <summary>Class extension used to generate clauses from query for financial accounting reports.</summary>
    static internal class ReportBuilderQueryExtensions {


        #region Public methods


        static internal string MapToFilterString(this ReportBuilderQuery query) {

            string accountChart = GetAccountsChartFilter(query);
            string dateFilter = GetDateFilter(query);
            string account = GetAccountFilter(query);
            string ledgers = GetLedgerFilter(query);
            string verificationNumber = GetVerificationNumber(query);
            string voucherRangeFilter = GetVoucherRangeFilter(query);

            var filter = new Filter(accountChart);

            filter.AppendAnd(dateFilter);
            filter.AppendAnd(account);
            filter.AppendAnd(ledgers);
            filter.AppendAnd(verificationNumber);
            filter.AppendAnd(voucherRangeFilter);

            return filter.ToString().Length > 0 ? $"{filter}" : "";
        }


        static internal string MapToSortString(this ReportBuilderQuery query) {

            if (query.ReportType == ReportTypes.ListadoMovimientosPorPolizas) {

                return $"ID_MAYOR, NUMERO_TRANSACCION, ID_MOVIMIENTO";
            }

            return string.Empty;
        }


        #endregion Public methods


        #region Helpers

        static private string GetAccountFilter(ReportBuilderQuery query) {
            if (query.AccountNumber != string.Empty) {
                return $"NUMERO_CUENTA_ESTANDAR LIKE '{query.AccountNumber}%'";
            }
            return string.Empty;
        }


        static private string GetAccountsChartFilter(ReportBuilderQuery query) {

            if (query.ReportType == ReportTypes.ListadoDePolizasPorCuenta ||
                query.ReportType == ReportTypes.MovimientosPorNumeroDeVerificacion) {

                return $"ID_TIPO_CUENTAS_STD = {AccountsChart.Parse(query.AccountsChartUID).Id}";
            }
            return string.Empty;
        }


        static private string GetDateFilter(ReportBuilderQuery query) {
            if (query.ReportType == ReportTypes.ListadoDePolizasPorCuenta ||
                query.ReportType == ReportTypes.MovimientosPorNumeroDeVerificacion) {

                return $"FECHA_AFECTACION >= {DataCommonMethods.FormatSqlDbDate(query.FromDate)} " +
                       $"AND FECHA_AFECTACION <= {DataCommonMethods.FormatSqlDbDate(query.ToDate)}";
            }
            return string.Empty;
        }


        static private string GetLedgerFilter(ReportBuilderQuery query) {
            if (query.Ledgers.Length > 0) {
                int[] ledgerIds = query.Ledgers.Select(uid => Ledger.Parse(uid).Id).ToArray();

                return $"ID_MAYOR IN ({String.Join(", ", ledgerIds)})";
            }
            return string.Empty;
        }


        static private string GetVerificationNumber(ReportBuilderQuery query) {

            if (query.ReportType == ReportTypes.MovimientosPorNumeroDeVerificacion) {

                if (query.VerificationNumbers.Length > 0) {

                    return $"NUMERO_VERIFICACION IN ({String.Join(", ", query.VerificationNumbers)})";

                } else if (query.VerificationNumbers.Length == 0 && query.AccountNumber != string.Empty) {

                    return $"NUMERO_VERIFICACION > 0";
                }
            }

            return string.Empty;
        }


        static private string GetVoucherRangeFilter(ReportBuilderQuery query) {

            if (query.ReportType == ReportTypes.ListadoMovimientosPorPolizas) {

                return $"ID_TRANSACCION IN ({String.Join(", ", query.VoucherIds)})";
            }
            return string.Empty;
        }

        #endregion Helpers

    } // class ReportBuilderQueryExtensions

} // namespace Empiria.FinancialAccounting.Reporting
