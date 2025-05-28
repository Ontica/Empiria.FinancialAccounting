/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                            Component : Interface adapters                   *
*  Assembly : FinancialAccounting.Reporting.dll             Pattern   : Query type extension methods         *
*  Type     : ReportBuilderQueryExtensions                  License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Extension methods for ReportBuilderQuery type.                                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System;
using System.Linq;

using Empiria.Data;

namespace Empiria.FinancialAccounting.Reporting {

  /// <summary>Extension methods for ReportBuilderQuery type.</summary>
  static internal class ReportBuilderQueryExtensions {

    #region Methods

    static internal string MapToFilterString(this ReportBuilderQuery query) {

      string accountChartFilter = BuildAccountsChartFilter(query);
      string dateFilter = BuildDateRangeFilter(query);
      string accountNumberFilter = BuildAccountNumberFilter(query);
      string ledgersFilter = BuildLedgersFilter(query);
      string verificationNumberFilter = BuildVerificationNumberFilter(query);
      string voucherRangeFilter = BuildVoucherRangeFilter(query);

      var filter = new Filter(accountChartFilter);

      filter.AppendAnd(dateFilter);
      filter.AppendAnd(accountNumberFilter);
      filter.AppendAnd(ledgersFilter);
      filter.AppendAnd(verificationNumberFilter);
      filter.AppendAnd(voucherRangeFilter);

      return filter.ToString();
    }


    static internal string MapToSortString(this ReportBuilderQuery query) {

      if (query.ReportType != ReportTypes.ListadoMovimientosPorPolizas) {
        return string.Empty;
      }

      return $"ID_MAYOR, NUMERO_TRANSACCION, ID_MOVIMIENTO";
    }

    #endregion Methods

    #region Helpers

    static private string BuildAccountNumberFilter(ReportBuilderQuery query) {
      if (query.AccountNumber == string.Empty) {
        return string.Empty;
      }

      return $"NUMERO_CUENTA_ESTANDAR LIKE '{query.AccountNumber}%'";
    }


    static private string BuildAccountsChartFilter(ReportBuilderQuery query) {

      if (query.ReportType != ReportTypes.ListadoDePolizasPorCuenta &&
          query.ReportType != ReportTypes.MovimientosPorNumeroDeVerificacion) {
        return string.Empty;
      }

      return $"ID_TIPO_CUENTAS_STD = {AccountsChart.Parse(query.AccountsChartUID).Id}";
    }


    static private string BuildDateRangeFilter(ReportBuilderQuery query) {
      if (query.ReportType != ReportTypes.ListadoDePolizasPorCuenta &&
          query.ReportType != ReportTypes.MovimientosPorNumeroDeVerificacion) {
        return string.Empty;
      }

      return $"FECHA_AFECTACION >= {DataCommonMethods.FormatSqlDbDate(query.FromDate)} " +
             $"AND FECHA_AFECTACION <= {DataCommonMethods.FormatSqlDbDate(query.ToDate)}";
    }


    static private string BuildLedgersFilter(ReportBuilderQuery query) {
      if (query.Ledgers.Length == 0) {
        return string.Empty;
      }

      int[] ledgerIds = query.Ledgers.Select(uid => Ledger.Parse(uid).Id).ToArray();

      return $"ID_MAYOR IN ({String.Join(", ", ledgerIds)})";
    }


    static private string BuildVerificationNumberFilter(ReportBuilderQuery query) {
      if (query.VerificationNumbers.Length == 0) {
        return string.Empty;
      }

      return $"NUMERO_VERIFICACION IN ({String.Join(", ", query.VerificationNumbers.Select(x => $"'{x}'"))})";

    }


    static private string BuildVoucherRangeFilter(ReportBuilderQuery query) {
      if (query.ReportType != ReportTypes.ListadoMovimientosPorPolizas) {
        return string.Empty;
      }

      return SearchExpression.ParseInSet("ID_TRANSACCION", query.VouchersIdGroup);
    }

    #endregion Helpers

  } // class ReportBuilderQueryExtensions

} // namespace Empiria.FinancialAccounting.Reporting
