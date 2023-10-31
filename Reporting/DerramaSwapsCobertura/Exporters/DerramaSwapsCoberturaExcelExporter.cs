/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                           Component : Excel Exporters                       *
*  Assembly : FinancialAccounting.Reporting.dll            Pattern   : IExcelExporter                        *
*  Type     : DerramaSwapsCoberturaExcelExporter           License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Service used to export 'Derrama de intereses de swaps de cobertura' to Microsoft Excel.        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Office;
using Empiria.Storage;

using System.Collections.Generic;

using Empiria.FinancialAccounting.Reporting.DerramaSwapsCobertura.Adapters;

namespace Empiria.FinancialAccounting.Reporting {

  /// <summary>Service used to export 'Derrama de intereses de swaps de cobertura' to Microsoft Excel.</summary>
  internal class DerramaSwapsCoberturaExcelExporter : IExcelExporter {

    private readonly ReportDataDto _reportData;
    private readonly FileTemplateConfig _template;

    public DerramaSwapsCoberturaExcelExporter(ReportDataDto reportData, FileTemplateConfig template) {
      Assertion.Require(reportData, nameof(reportData));
      Assertion.Require(template, nameof(template));

      _reportData = reportData;
      _template = template;
    }


    public FileReportDto CreateExcelFile() {
      var excelFile = new ExcelFile(_template);

      excelFile.Open();

      SetHeader(excelFile);

      FillOutRows(excelFile, _reportData.Entries.Select(x => (DerramaSwapsCoberturaEntryDto) x));

      excelFile.Save();

      excelFile.Close();

      return excelFile.ToFileReportDto();
    }

    #region Private methods

    private void FillOutRows(ExcelFile excelFile, IEnumerable<DerramaSwapsCoberturaEntryDto> entries) {
      int i = 5;

      foreach (var entry in entries) {

        if (entry.ItemType == "Total") {
          excelFile.SetRowStyleBold(i);
        }

        excelFile.SetCell($"A{i}", $"'{entry.SubledgerAccount}");
        excelFile.SetCell($"B{i}", entry.SubledgerAccountName);
        excelFile.SetCell($"C{i}", entry.IncomeAccountTotal);
        excelFile.SetCell($"D{i}", entry.ExpensesAccountTotal);
        excelFile.SetCell($"E{i}", entry.Total);
        excelFile.SetCell($"F{i}", entry.Classification);

        i++;
      }
    }


    private void SetHeader(ExcelFile excelFile) {
      excelFile.SetCell($"A2", _template.Title);

      var subTitle = $"Cifras al {_reportData.Query.ToDate.ToString("dd/MMM/yyyy")}";

      excelFile.SetCell($"A3", subTitle);
    }

    #endregion Private methods

  }  // class DerramaSwapsCoberturaExcelExporter

}  // namespace Empiria.FinancialAccounting.Reporting
