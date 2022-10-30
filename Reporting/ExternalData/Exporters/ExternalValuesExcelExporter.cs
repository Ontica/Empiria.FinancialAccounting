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


    private void SetTable(ExternalValuesDto externalValues) {
      int i = _templateConfig.FirstRowIndex;

      foreach (var externalValue in externalValues.Entries) {
        _excelFile.SetCell($"A{i}", externalValue.VariableCode);
        _excelFile.SetCell($"B{i}", externalValue.VariableName);
        i++;
      }
    }


    private void SetHeader(ExternalValuesDto externalValues) {
      _excelFile.SetCell(_templateConfig.ReportDateCell, externalValues.Query.Date);
    }


  }  // class ExternalValuesExporter

}  // namespace Empiria.FinancialAccounting.Reporting.ExternalData
