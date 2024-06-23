/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                             Component : Excel Exporters                     *
*  Assembly : FinancialAccounting.Reporting.dll              Pattern   : IExcelExporter                      *
*  Type     : DerramaSwapsCoberturaConsolidadoExcelExporter  License   : Please read LICENSE.txt file        *
*                                                                                                            *
*  Summary  : Service used to export 'Derrama de intereses de swaps de cobertura' consolidado to Excel.      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Office;
using Empiria.Storage;

using Empiria.FinancialAccounting.Reporting.DerramaSwapsCobertura.Adapters;
using System.Globalization;

namespace Empiria.FinancialAccounting.Reporting {

  /// <summary>Service used to export 'Derrama de intereses de swaps de cobertura' consolidado to Excel.</summary>
  internal class DerramaSwapsCoberturaConsolidadoExcelExporter : IExcelExporter {

    private readonly ReportDataDto _reportData;
    private readonly FileTemplateConfig _template;

    public DerramaSwapsCoberturaConsolidadoExcelExporter(ReportDataDto reportData, FileTemplateConfig template) {
      Assertion.Require(reportData, nameof(reportData));
      Assertion.Require(template, nameof(template));

      _reportData = reportData;
      _template = template;
    }


    public FileDto CreateExcelFile() {
      var excelFile = new ExcelFile(_template);

      excelFile.Open();

      SetHeader(excelFile);

      FillOutRows(excelFile, _reportData.Entries.Select(x => (DerramaSwapsCoberturaConsolidadoEntryDto) x)
                                                .ToFixedList());

      excelFile.Save();

      excelFile.Close();

      return excelFile.ToFileDto();
    }

    #region Private methods


    private void FillOutRows(ExcelFile excelFile, FixedList<DerramaSwapsCoberturaConsolidadoEntryDto> entries) {

      foreach (var entry in entries) {

        excelFile.SetCell($"C{entry.Row}", entry.IncomeTotal);
        excelFile.SetCell($"D{entry.Row}", entry.ExpensesTotal);
        excelFile.SetCell($"E{entry.Row}", entry.Total);

      }

      var unclassified = entries.Find(x => x.Classification == "Sin clasificar");

      if (unclassified != null) {
        excelFile.SetCell($"B{unclassified.Row}", unclassified.Classification);
      }

    }


    private void SetHeader(ExcelFile excelFile) {
      CultureInfo esUS = new CultureInfo("es-US");

      var subTitle = $"Cifras al {_reportData.Query.ToDate.ToString("dd \\de MMMM \\de yyyy", esUS)}".ToUpper();

      excelFile.SetCell($"B9", subTitle);
    }

    #endregion Private methods

  }  // class DerramaSwapsCoberturaExcelExporter

}  // namespace Empiria.FinancialAccounting.Reporting
