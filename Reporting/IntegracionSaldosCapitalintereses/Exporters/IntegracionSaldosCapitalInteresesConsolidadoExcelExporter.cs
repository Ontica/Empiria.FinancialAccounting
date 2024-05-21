/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                                        Component : Excel Exporters          *
*  Assembly : FinancialAccounting.Reporting.dll                         Pattern   : IExcelExporter           *
*  Type     : IntegracionSaldosCapitalInteresesConsolidadoExcelExporter License   : Read LICENSE.txt file    *
*                                                                                                            *
*  Summary  : Service used to export 'Integración de saldos de capital e intereses' to Microsoft Excel.      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Globalization;

using Empiria.Office;
using Empiria.Storage;

using Empiria.FinancialAccounting.Reporting.IntegracionSaldosCapitalIntereses.Adapters;

namespace Empiria.FinancialAccounting.Reporting {

  /// <summary>Service used to export 'Integración de saldos de capital e intereses' to Microsoft Excel.</summary>
  internal class IntegracionSaldosCapitalInteresesConsolidadoExcelExporter : IExcelExporter {

    private const int LAST_COLUMN_INDEX = 14;

    private readonly ReportDataDto _reportData;
    private readonly FileTemplateConfig _template;

    public IntegracionSaldosCapitalInteresesConsolidadoExcelExporter(ReportDataDto reportData,
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

      FillOutRows(excelFile, _reportData.Entries.Select(x => (IIntegracionSaldosCapitalInteresesConsolidadoDto) x)
                                                .ToFixedList());

      excelFile.Save();

      excelFile.Close();

      return excelFile.ToFileReportDto();
    }

    #region Private methods

    private void FillOutRows(ExcelFile excelFile, FixedList<IIntegracionSaldosCapitalInteresesConsolidadoDto> entries) {
      int i = 15;
      int backOfficeStartIndex = 0;

      foreach (var entry in entries) {
        if (entry is IntegracionSaldosCapitalInteresesConsolidadoEntryDto e) {
          FillOutRows(i, excelFile, e);
        } else if (entry is IntegracionSaldosCapitalInteresesConsolidadoTotalDto total) {
          FillOutRows(i, excelFile, total);
        } else if (entry is IntegracionSaldosCapitalInteresesConsolidadoTitleDto title) {
          if (title.Title == "REGISTROS BACK OFFICE (1)") {
            backOfficeStartIndex = entries.IndexOf(title) + 1;
            break;
          }
          FillOutRows(i, excelFile, title);
        }
        i++;
        excelFile.InsertRow(i);
      }

      i = excelFile.SearchColumnValue("C", "REGISTROS BACK OFFICE (1)", i, i + 50);

      excelFile.SetCell($"J{i}", _reportData.Query.ToDate.ToString("dd/MM/yyyy"));

      i += 2;
      foreach (var entry in entries.Sublist(backOfficeStartIndex)) {
        if (entry is IntegracionSaldosCapitalInteresesConsolidadoEntryDto e) {
          FillOutRows(i, excelFile, e);
        } else if (entry is IntegracionSaldosCapitalInteresesConsolidadoTotalDto total) {
          FillOutRows(i, excelFile, total);
        } else if (entry is IntegracionSaldosCapitalInteresesConsolidadoTitleDto title) {
          FillOutRows(i, excelFile, title);
        }
        i++;
        excelFile.InsertRow(i);
      }
    }

    private void FillOutRows(int row, ExcelFile excelFile, IntegracionSaldosCapitalInteresesConsolidadoTitleDto entry) {
      excelFile.SetCell($"C{row}", $"'{entry.Title}");

      if (entry.ItemType == "Total") {
        excelFile.SetRowBold(row, LAST_COLUMN_INDEX);
        excelFile.SetRowFontName(row, LAST_COLUMN_INDEX, "Arial", 10);
      }
    }

    private void FillOutRows(int row, ExcelFile excelFile, IntegracionSaldosCapitalInteresesConsolidadoTotalDto entry) {
      excelFile.SetCell($"C{row}", $"'{entry.Title}");
      excelFile.SetCell($"L{row}", entry.CapitalMonedaNacional);
      excelFile.SetCell($"M{row}", entry.InteresesMonedaNacional);
      excelFile.SetCell($"N{row}", entry.TotalMonedaNacional);

      if (entry.ItemType == "Total") {
        excelFile.SetRowBold(row, LAST_COLUMN_INDEX);
        excelFile.SetRowFontName(row, LAST_COLUMN_INDEX, "Arial", 10);
      }
    }

    private void FillOutRows(int row, ExcelFile excelFile, IntegracionSaldosCapitalInteresesConsolidadoEntryDto entry) {
      excelFile.SetCell($"C{row}", entry.Banco);
      excelFile.SetCell($"D{row}", entry.PrestamoID.Length == 0 ? entry.Title : entry.PrestamoID);
      excelFile.SetCell($"E{row}", entry.CurrencyName);
      excelFile.SetCell($"F{row}", entry.CapitalMonedaOrigenTotal);
      excelFile.SetCell($"G{row}", entry.InteresesMonedaOrigenTotal);
      excelFile.SetCell($"H{row}", entry.TotalMonedaOrigen);
      excelFile.SetCell($"J{row}", entry.TipoCambio);
      excelFile.SetCell($"L{row}", entry.CapitalMonedaNacional);
      excelFile.SetCell($"M{row}", entry.InteresesMonedaNacional);
      excelFile.SetCell($"N{row}", entry.TotalMonedaNacional);

      if (entry.ItemType == "Total") {
        excelFile.SetRowBold(row, LAST_COLUMN_INDEX);
        excelFile.SetRowFontName(row, LAST_COLUMN_INDEX,"Arial", 10);
      }
    }

    private void SetHeader(ExcelFile excelFile) {
      CultureInfo esUS = new CultureInfo("es-US");

      var subTitle = $"INTEGRACIÓN DE SALDOS DE CAPITAL E INTERESES AL {_reportData.Query.ToDate.ToString("dd \\de MMMM \\de yyyy", esUS)}".ToUpper();

      excelFile.SetCell($"B7", subTitle);

      excelFile.SetCell($"J13", _reportData.Query.ToDate.ToString("dd/MM/yyyy"));
    }

    #endregion Private methods

  }  // class IntegracionSaldosCapitalInteresesConsolidadoExcelExporter

}  // namespace Empiria.FinancialAccounting.Reporting
