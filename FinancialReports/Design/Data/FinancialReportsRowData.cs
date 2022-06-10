/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Data Access Layer                       *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Data Service                            *
*  Type     : FinancialReportsRowData                    License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Data services for financial reports row definition objects.                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Data;

namespace Empiria.FinancialAccounting.FinancialReports.Data {

  /// <summary>Data services for financial reports row definition objects.</summary>
  static internal class FinancialReportsRowData {

    static internal FixedList<FinancialReportRow> GetRows(FinancialReportType reportType) {
      var sql = "SELECT * " +
                "FROM COF_CONCEPTOS_REPORTES " +
                $"WHERE ID_REPORTE = {reportType.Id} AND " +
                $"ID_TIPO_ELEMENTO_REPORTE = 3085 " +
                $"ORDER BY FILA";

      var op = DataOperation.Parse(sql);

      return DataReader.GetFixedList<FinancialReportRow>(op);
    }

    internal static FixedList<FinancialReportCell> GetCells(FinancialReportType reportType) {
      var sql = "SELECT * " +
                "FROM COF_CONCEPTOS_REPORTES " +
                $"WHERE ID_REPORTE = {reportType.Id} AND " +
                $"ID_TIPO_ELEMENTO_REPORTE = 3088 " +
                $"ORDER BY FILA";

      var op = DataOperation.Parse(sql);

      return DataReader.GetFixedList<FinancialReportCell>(op);
    }

  }  // class FinancialReportsRowData

}  // namespace Empiria.FinancialAccounting.FinancialReports.Data
