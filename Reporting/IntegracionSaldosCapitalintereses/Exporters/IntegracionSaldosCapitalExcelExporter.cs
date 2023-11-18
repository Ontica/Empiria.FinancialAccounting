/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                                 Component : Excel Exporters                 *
*  Assembly : FinancialAccounting.Reporting.dll                  Pattern   : IExcelExporter                  *
*  Type     : IntegracionSaldosCapitalExcelExporter              License   : Read LICENSE.txt file           *
*                                                                                                            *
*  Summary  : Service used to export 'Integración de saldos de capital' to Microsoft Excel.                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Office;
using Empiria.Storage;

using System.Collections.Generic;

using Empiria.FinancialAccounting.Reporting.IntegracionSaldosCapitalIntereses.Adapters;

namespace Empiria.FinancialAccounting.Reporting {

  /// <summary>Service used to export 'Integración de saldos de capital' to Microsoft Excel.</summary>
  internal class IntegracionSaldosCapitalExcelExporter : IExcelExporter {

    private readonly ReportDataDto _reportData;
    private readonly FileTemplateConfig _template;

    public IntegracionSaldosCapitalExcelExporter(ReportDataDto reportData,
                                                 FileTemplateConfig template) {
      Assertion.Require(reportData, nameof(reportData));
      Assertion.Require(template, nameof(template));

      _reportData = reportData;
      _template = template;
    }


    public FileReportDto CreateExcelFile() {
      var excelFile = new ExcelFile(_template);

      excelFile.Open();

      SetHeader(excelFile);

      FillOutRows(excelFile, _reportData.Entries.Select(x => (IntegracionSaldosCapitalDto) x));

      excelFile.Save();

      excelFile.Close();

      return excelFile.ToFileReportDto();
    }

    #region Private methods

    private void FillOutRows(ExcelFile excelFile, IEnumerable<IntegracionSaldosCapitalDto> entries) {
      int i = 5;

      foreach (var entry in entries) {
        if (entry is IntegracionSaldosCapitalEntryDto) {
          FillOutRows(i, excelFile, (IntegracionSaldosCapitalEntryDto) entry);
        } else if (entry is IntegracionSaldosCapitalTitleDto) {
          FillOutRows(i, excelFile, (IntegracionSaldosCapitalTitleDto) entry);
        }
        i++;
      }
    }

    private void FillOutRows(int row, ExcelFile excelFile, IntegracionSaldosCapitalTitleDto entry) {
      excelFile.SetCell($"A{row}", entry.SubledgerAccount);

      if (entry.ItemType == "Total") {
        excelFile.SetRowStyleBold(row);
        excelFile.SetRowFontFamily(row, "Courier New");
      }
    }

    private void FillOutRows(int row, ExcelFile excelFile, IntegracionSaldosCapitalEntryDto entry) {
      excelFile.SetCell($"A{row}", entry.PrestamoName);
      excelFile.SetCell($"B{row}", entry.CurrencyCode);
      excelFile.SetCell($"C{row}", $"'{entry.SubledgerAccount}");
      excelFile.SetCell($"D{row}", entry.SubledgerAccountName);
      excelFile.SetCellWrapText($"D{row}");
      excelFile.SetCell($"E{row}", entry.SectorCode);
      excelFile.SetCell($"F{row}", entry.CapitalCortoPlazoMonedaOrigen);
      excelFile.SetCell($"G{row}", entry.CapitalLargoPlazoMonedaOrigen);
      excelFile.SetCell($"H{row}", entry.CapitalMonedaOrigenTotal);
      if (entry.Vencimiento.HasValue) {
        excelFile.SetCell($"I{row}", entry.Vencimiento.Value);
      }

      if (entry.ItemType == "Total") {
        excelFile.SetRowStyleBold(row);
        excelFile.SetRowFontFamily(row, "Courier New");
      }
    }

    private void SetHeader(ExcelFile excelFile) {
      excelFile.SetCell($"A2", _template.Title);

      var subTitle = $"Cifras al {_reportData.Query.ToDate.ToString("dd/MMM/yyyy")}";

      excelFile.SetCell($"A3", subTitle);
    }

    #endregion Private methods

  }  // class IntegracionSaldosCapitalExcelExporter

}  // namespace Empiria.FinancialAccounting.Reporting
