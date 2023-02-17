/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                           Component : Excel Exporters                       *
*  Assembly : FinancialAccounting.Reporting.dll            Pattern   : Helper class                          *
*  Type     : FinancialReportRowFiller                     License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Helper class that fills out an Excel row described by a FinancialReportRow instance.           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.FinancialReports;
using Empiria.FinancialAccounting.FinancialReports.Adapters;

namespace Empiria.FinancialAccounting.Reporting.FinancialReports.Exporters {

  /// <summary>Helper class that fills out an Excel row described by a FinancialReportRow instance.</summary>
  internal class FinancialReportRowFiller {

    private readonly FixedList<DataTableColumn> _columns;

    private readonly DataTableColumn _conceptCodeColumn;
    private readonly DataTableColumn _conceptNameColumn;
    private readonly FixedList<DataTableColumn> _totalsColumns;

    private readonly ExcelFile _excelFile;

    public FinancialReportRowFiller(FixedList<DataTableColumn> columns,
                                    ExcelFile excelFile) {
      _columns = columns;
      _excelFile = excelFile;

      _conceptCodeColumn = _columns.Find(x => x.Field == "conceptCode" && !x.Column.StartsWith("_"));
      _conceptNameColumn = _columns.Find(x => x.Field == "concept" && !x.Column.StartsWith("_"));
      _totalsColumns = _columns.FindAll(x => x.Type == "decimal" && !x.Column.StartsWith("_"));
    }


    internal void FillOutRow(FinancialReportRow row, FinancialReportEntryDto entry) {
      FillOutRow(row, entry, row.RowIndex);
    }


    internal void FillOutRow(FinancialReportRow row, FinancialReportEntryDto entry, int rowIndex) {
      if (_conceptCodeColumn != null) {
        _excelFile.SetCell($"{_conceptCodeColumn.Column}{rowIndex}", entry.ConceptCode);
      }
      if (_conceptNameColumn != null) {
        _excelFile.SetCell($"{_conceptNameColumn.Column}{rowIndex}", entry.Concept);
      }

      if (entry.FinancialConceptUID.Length == 0) {
        return;
      }

      foreach (var totalColumn in _totalsColumns) {

        decimal totalField = entry.GetTotalField(totalColumn.Field);

        _excelFile.SetCell($"{totalColumn.Column}{rowIndex}", totalField);
      }
    }


  }  // class FinancialReportRowFiller

}  // namespace Empiria.FinancialAccounting.Reporting.FinancialReports.Exporters
