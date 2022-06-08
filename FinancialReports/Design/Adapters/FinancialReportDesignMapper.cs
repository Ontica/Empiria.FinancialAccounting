/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Interface adapters                      *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Mapper class                            *
*  Type     : FinancialReportDesignMapper                License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Methods used to map financial reports design data.                                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.FinancialReports.Adapters {

  /// <summary>Methods used to map financial reports design data.</summary>
  static internal class FinancialReportDesignMapper {

    #region Public mappers

    static internal FinancialReportDesignDto Map(FinancialReportType reportType) {
      return new FinancialReportDesignDto {
        Columns = reportType.DataColumns,
        Rows = MapRows(reportType),
      };
    }


    #endregion Public mappers

    #region Helpers

    static private FixedList<FinancialReportRowDto> MapRows(FinancialReportType reportType) {
      var rows = reportType.GetRows();

      return rows.Select((x) => MapRow(x))
                 .ToFixedList();
    }

    static private FinancialReportRowDto MapRow(FinancialReportRow row) {
      return new FinancialReportRowDto {
        UID = row.UID,
        ConceptCode = row.Code,
        Concept = row.Label,
        Format = row.Format,
        Position = row.Position,
        FinancialConceptUID = row.FinancialConcept.UID
      };
    }

    #endregion Helpers

  } // class FinancialReportDesignMapper

} // namespace Empiria.FinancialAccounting.FinancialReports.Adapters
