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

    static internal FixedList<FinancialReportRow> GetRows(FinancialReportType financialReportType) {
      var sql = "SELECT * " +
                "FROM COF_REPORTES_ELEMENTOS " +
                $"WHERE ID_REPORTE = {financialReportType.Id} " +
                $"ORDER BY POSICION";

      var dataOperation = DataOperation.Parse(sql);

      return DataReader.GetFixedList<FinancialReportRow>(dataOperation);
    }

  }  // class FinancialReportsRowData

}  // namespace Empiria.FinancialAccounting.FinancialReports.Data
