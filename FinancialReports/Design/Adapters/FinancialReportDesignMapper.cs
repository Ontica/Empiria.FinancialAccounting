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
        Columns = reportType.DesignType == FinancialReportDesignType.FixedRows
                                        ? reportType.DataColumns : null,
        Rows = MapRows(reportType),
        Cells = MapCells(reportType),
      };
    }


    static internal FinancialReportRowDto Map(FinancialReportRow row) {
      return new FinancialReportRowDto {
        UID = row.UID,
        FinancialConceptGroupUID = row.FinancialConcept.Group.UID,
        ConceptCode = row.FinancialConcept.IsEmptyInstance ?
                                    string.Empty : row.FinancialConcept.Code,
        Concept = row.FinancialConcept.IsEmptyInstance ?
                        "Línea en blanco" : row.FinancialConcept.Name,
        Format = row.Format,
        Row = row.RowIndex + row.FinancialReportType.RowsOffset,
        FinancialConceptUID = row.FinancialConcept.UID
      };
    }


    static internal FinancialReportCellDto Map(FinancialReportCell cell) {
      return new FinancialReportCellDto {
        UID = cell.UID,
        Label = MapCellLabel(cell),
        DataField = cell.DataField,
        Column = cell.Column,
        Row = cell.RowIndex,
        Format = cell.Format,
        FinancialConceptUID = cell.FinancialConcept.UID,
        FinancialConceptGroupUID = cell.FinancialConcept.Group.UID
      };
    }


    #endregion Public mappers

    #region Helpers


    private static FixedList<FinancialReportCellDto> MapCells(FinancialReportType reportType) {
      FixedList<FinancialReportCell> cells = reportType.GetCells();

      return cells.Select((x) => Map(x))
                 .ToFixedList();
    }


    static private string MapCellLabel(FinancialReportCell cell) {
      if (cell.DataField.Length != 0) {
        return $"= CONCEPTO('{cell.FinancialConcept.Code}').{cell.DataField}";
      } else {
        return cell.Label;
      }
    }


    static private ReportDesignConfigDto MapConfig(FinancialReportType reportType) {
      return new ReportDesignConfigDto {
        DesignType = reportType.DesignType,
        RowsOffset = reportType.RowsOffset,
        AccountsChart = reportType.AccountsChart.MapToNamedEntity(),
        ReportType = reportType.MapToNamedEntity(),
        FinancialConceptGroups = reportType.FinancialConceptGroups.MapToNamedEntityList(),
        DataFields = reportType.DataFields.MapToNamedEntityList(),
        Grid = new ReportGridDto {
          Columns = reportType.GridColumns,
          StartRow = reportType.GridStartRow,
          EndRow = reportType.GridEndRow
        }
      };
    }


    static private FixedList<FinancialReportRowDto> MapRows(FinancialReportType reportType) {
      FixedList<FinancialReportRow> rows = reportType.GetRows();

      return rows.Select((x) => Map(x))
                 .ToFixedList();
    }


    #endregion Helpers

  } // class FinancialReportDesignMapper

} // namespace Empiria.FinancialAccounting.FinancialReports.Adapters
