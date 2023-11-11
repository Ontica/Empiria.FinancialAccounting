/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                                        Component : Excel Exporters          *
*  Assembly : FinancialAccounting.Reporting.dll                         Pattern   : IExcelExporter           *
*  Type     : IntegracionSaldosCapitalInteresesDesglosadoExcelExporter  License   : Read LICENSE.txt file    *
*                                                                                                            *
*  Summary  : Service used to export 'Integración de saldos de capital e intereses' to Microsoft Excel.      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Office;
using Empiria.Storage;

using System.Collections.Generic;

using Empiria.FinancialAccounting.Reporting.IntegracionSaldosCapitalIntereses.Adapters;

namespace Empiria.FinancialAccounting.Reporting {

  /// <summary>Service used to export 'Integración de saldos de capital e intereses' to Microsoft Excel.</summary>
  internal class IntegracionSaldosCapitalInteresesDesglosadoExcelExporter : IExcelExporter {

    private readonly ReportDataDto _reportData;
    private readonly FileTemplateConfig _template;

    public IntegracionSaldosCapitalInteresesDesglosadoExcelExporter(ReportDataDto reportData,
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

      FillOutRows(excelFile, _reportData.Entries.Select(x => (IntegracionSaldosCapitalInteresesEntryDto) x));

      excelFile.Save();

      excelFile.Close();

      return excelFile.ToFileReportDto();
    }

    #region Private methods

    private void FillOutRows(ExcelFile excelFile, IEnumerable<IntegracionSaldosCapitalInteresesEntryDto> entries) {
      int i = 5;

      foreach (var entry in entries) {

        excelFile.SetCell($"A{i}", entry.PrestamoName);
        excelFile.SetCell($"B{i}", entry.CurrencyCode);
        excelFile.SetCell($"C{i}", $"'{entry.SubledgerAccount}");
        excelFile.SetCell($"D{i}", entry.SubledgerAccountName);
        excelFile.SetCell($"E{i}", entry.SectorCode);
        excelFile.SetCell($"F{i}", entry.CapitalMonedaOrigenTotal);
        excelFile.SetCell($"G{i}", entry.InteresesMonedaOrigenTotal);
        excelFile.SetCell($"H{i}", entry.TotalMonedaOrigen);
        excelFile.SetCell($"I{i}", entry.TipoCambio);
        excelFile.SetCell($"J{i}", entry.CapitalMonedaNacional);
        excelFile.SetCell($"K{i}", entry.InteresesMonedaNacional);
        excelFile.SetCell($"L{i}", entry.TotalMonedaNacional);

        if (entry.ItemType == "Total") {
          excelFile.SetRowStyleBold(i);
          excelFile.SetRowFontFamily(i, "Courier New");
        }


        i++;
      }
    }


    private void SetHeader(ExcelFile excelFile) {
      excelFile.SetCell($"A2", _template.Title);

      var subTitle = $"Cifras al {_reportData.Query.ToDate.ToString("dd/MMM/yyyy")}";

      excelFile.SetCell($"A3", subTitle);
    }

    #endregion Private methods

  }  // class IntegracionSaldosCapitalInteresesDesglosadoExcelExporter

}  // namespace Empiria.FinancialAccounting.Reporting
