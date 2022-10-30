/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                           Component : Excel Exporters                       *
*  Assembly : FinancialAccounting.Reporting.dll            Pattern   : IExcelExporter                        *
*  Type     : ExternalValuesExporter                       License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Exports financial external data values to Excel files.                                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.ExternalData.Adapters;

namespace Empiria.FinancialAccounting.Reporting.ExternalData.Exporters {

  /// <summary>Exports financial external data values to Excel files.</summary>
  internal class ExternalValuesExcelExporter {

    private readonly FileTemplateConfig _templateConfig;
    private ExcelFile _excelFile;

    public ExternalValuesExcelExporter(FileTemplateConfig templateConfig) {
      Assertion.Require(templateConfig, "templateConfig");

      _templateConfig = templateConfig;
    }


    internal ExcelFile CreateExcelFile(ExternalValuesDto externalValues) {
      Assertion.Require(externalValues, nameof(externalValues));

      _excelFile = new ExcelFile(_templateConfig);

      _excelFile.Open();

      SetHeader(externalValues);

      SetTable(externalValues);

      _excelFile.Save();

      _excelFile.Close();

      return _excelFile;
    }


    private void SetDynamicColumns(FixedList<DataTableColumn> dynamicColumns,
                                   ExternalValuesEntryDto entry, int row) {
      foreach (var totalColumn in dynamicColumns) {
        decimal totalField = entry.GetTotalField(totalColumn.Field);

        _excelFile.SetCell($"{totalColumn.Column}{row}", totalField);
      }
    }


    private void SetTable(ExternalValuesDto externalValues) {
      int row = _templateConfig.FirstRowIndex;

      FixedList<DataTableColumn> dynamicColumns = externalValues.Columns.FindAll(x => x.Type == "decimal");

      foreach (var externalValue in externalValues.Entries) {
        _excelFile.SetCell($"A{row}", externalValue.VariableCode);
        _excelFile.SetCell($"B{row}", externalValue.VariableName);

        SetDynamicColumns(dynamicColumns, externalValue, row);

        row++;
      }
    }

    private void SetHeader(ExternalValuesDto externalValues) {
      _excelFile.SetCell(_templateConfig.ReportDateCell, externalValues.Query.Date);
    }


  }  // class ExternalValuesExporter

}  // namespace Empiria.FinancialAccounting.Reporting.ExternalData
