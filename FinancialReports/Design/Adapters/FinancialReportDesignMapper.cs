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
        Config = MapConfig(reportType),
        Columns = reportType.DataColumns,
        Rows = MapRows(reportType),
      };
    }


    static private ReportDesignConfigDto MapConfig(FinancialReportType reportType) {
      return new ReportDesignConfigDto {
        Type = reportType.DesignType,
        AccountsChart = reportType.AccountsChart.MapToNamedEntity(),
        Grid = new ReportGridDto {
          Columns = MapToReportColumn(reportType.DataColumns),
          StartRow = 1,
          EndRow = 64
        }
      };
    }


    static private FixedList<string> MapToReportColumn(FixedList<DataTableColumn> dataColumns) {
      return dataColumns.Select(x => x.Column)
                        .ToFixedList();
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
        FinancialConceptGroupUID = row.FinancialConcept.Group.UID,
        ConceptCode = row.FinancialConcept.Code,
        Concept = row.FinancialConcept.Name,
        Format = row.Format,
        Row = row.Row,
        FinancialConceptUID = row.FinancialConcept.UID
      };
    }

    #endregion Helpers

  } // class FinancialReportDesignMapper

} // namespace Empiria.FinancialAccounting.FinancialReports.Adapters
