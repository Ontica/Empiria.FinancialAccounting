/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                           Component : Excel Exporters                       *
*  Assembly : FinancialAccounting.Reporting.dll            Pattern   : Helper class                          *
*  Type     : FinancialReportCellFiller                    License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Helper class that fills out an Excel file cell described by a FinancialReportCell instance.    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.DynamicData;
using Empiria.Office;

using Empiria.FinancialAccounting.FinancialReports;
using Empiria.FinancialAccounting.FinancialReports.Adapters;

namespace Empiria.FinancialAccounting.Reporting.FinancialReports.Exporters {

  /// <summary>Helper class that fills out an Excel file cell
  /// described by a FinancialReportCell instance.</summary>
  internal class FinancialReportCellFiller {

    private readonly FixedList<DataTableColumn> _columns;

    private readonly ExcelFile _excelFile;

    public FinancialReportCellFiller(FixedList<DataTableColumn> columns,
                                     ExcelFile excelFile) {
      _columns = columns;
      _excelFile = excelFile;
    }


    internal void FillOutCell(FinancialReportCell cell, FinancialReportEntryDto entry) {
      if (cell.DataField == "conceptCode") {

        _excelFile.SetCell($"{cell.Column}{cell.RowIndex}", entry.ConceptCode);

      } else if (cell.DataField == "concept") {

        _excelFile.SetCell($"{cell.Column}{cell.RowIndex}", entry.Concept);

      } else if (cell.DataField.Length != 0) {

        decimal cellValue = entry.GetTotalField(cell.DataField);

        _excelFile.SetCell($"{cell.Column}{cell.RowIndex}", cellValue);

      } else {

        _excelFile.SetCell($"{cell.Column}{cell.RowIndex}", cell.Label);

      }

    }

  }  // class FinancialReportCellFiller

}  // namespace Empiria.FinancialAccounting.Reporting.FinancialReports.Exporters
