/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Data Access Layer                       *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Data Service                            *
*  Type     : FinancialReportsData                       License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Data services for financial reports configuration objects.                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Data;

namespace Empiria.FinancialAccounting.FinancialReports.Data {

  /// <summary>Data services for financial reports configuration objects.</summary>
  static internal class FinancialReportsData {

    static internal FixedList<FinancialReportItemDefinition> GetItems(FinancialReportType reportType) {
      var sql = "SELECT * " +
                "FROM COF_CONCEPTOS_REPORTES " +
                $"WHERE ID_REPORTE = {reportType.Id} AND STATUS <> 'X' " +
                $"ORDER BY FILA, COLUMNA";

      var op = DataOperation.Parse(sql);

      return DataReader.GetFixedList<FinancialReportItemDefinition>(op, true);
    }


    static internal void Write(FinancialReportCell o) {
      var op = DataOperation.Parse("write_cof_concepto_reporte",
                               o.Id, o.UID, o.GetEmpiriaType().Id,
                               o.FinancialReportType.Id, o.FinancialConcept.Id,
                               o.Label, o.ExtendedData.ToString(), o.Format,
                               o.Section, o.DataField,
                               o.RowIndex.ToString(), o.ColumnIndex,
                               (char) o.Status);

      DataWriter.Execute(op);
    }


    static internal void Write(FinancialReportRow o) {
      var op = DataOperation.Parse("write_cof_concepto_reporte",
                               o.Id, o.UID, o.GetEmpiriaType().Id,
                               o.FinancialReportType.Id, o.FinancialConcept.Id,
                               o.Label, o.ExtendedData.ToString(), o.Format,
                               o.Section, string.Empty,
                               o.RowIndex.ToString(), string.Empty,
                               (char) o.Status);

      DataWriter.Execute(op);
    }

  }  // class FinancialReportsData

}  // namespace Empiria.FinancialAccounting.FinancialReports.Data
