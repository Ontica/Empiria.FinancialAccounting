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
using System.Collections.Generic;

using Empiria.Data;

namespace Empiria.FinancialAccounting.FinancialReports.Data {

  /// <summary>Data services for financial reports configuration objects.</summary>
  static internal class FinancialReportsData {

    static internal List<FinancialReportItemDefinition> GetItems(FinancialReportType reportType) {
      var sql = "SELECT * " +
                "FROM COF_CONCEPTOS_REPORTES " +
                $"WHERE ID_REPORTE = {reportType.Id} AND STATUS <> 'X' " +
                $"ORDER BY FILA, COLUMNA";

      var op = DataOperation.Parse(sql);

      return DataReader.GetList<FinancialReportItemDefinition>(op, true);
    }


    static internal void Write(FinancialReportCell o) {
      var op = DataOperation.Parse("write_cof_concepto_reporte",
                               o.Id, o.UID, o.GetEmpiriaType().Id,
                               o.FinancialReportType.Id, o.FinancialConcept.Id,
                               o.Label, o.ExtendedData.ToString(), o.Format,
                               o.Section, o.DataField, o.RowIndex, o.Column,
                               (char) o.Status);

      DataWriter.Execute(op);
    }


    static internal void Write(FinancialReportRow o) {
      if (o.IsNew) {

        WriteRow(o);
        UpdateRowsPositions(o, -1, o.RowIndex);

      } else if (o.Status == StateEnums.EntityStatus.Deleted) {

        int oldRowIndex = GetRowIndex(o);

        WriteRow(o);
        UpdateRowsPositions(o, oldRowIndex, -1);

      } else {

        int oldRowIndex = GetRowIndex(o);

        WriteRow(o);
        UpdateRowsPositions(o, oldRowIndex, o.RowIndex);

      }
    }

    #region Helpers

    static private int GetRowIndex(FinancialReportRow o) {
      var sql = "SELECT FILA " +
                "FROM COF_CONCEPTOS_REPORTES " +
                $"WHERE ID_ELEMENTO_REPORTE = {o.Id}";

      var op = DataOperation.Parse(sql);

      return DataReader.GetScalar<int>(op);
    }


    static private void UpdateRowsPositions(FinancialReportRow o, int oldRowIndex, int newRowIndex) {
      var op = DataOperation.Parse("do_reposicionar_filas_reporte",
                              o.FinancialReportType.Id, o.Id,
                              oldRowIndex, newRowIndex);

      DataWriter.Execute(op);
    }

    static private void WriteRow(FinancialReportRow o) {
      var op = DataOperation.Parse("write_cof_concepto_reporte",
                             o.Id, o.UID, o.GetEmpiriaType().Id,
                             o.FinancialReportType.Id, o.FinancialConcept.Id,
                             o.Label, o.ExtendedData.ToString(), o.Format,
                             o.Section, string.Empty, o.RowIndex, string.Empty,
                             (char) o.Status);
      DataWriter.Execute(op);
    }

    #endregion Helpers

  }  // class FinancialReportsData

}  // namespace Empiria.FinancialAccounting.FinancialReports.Data
