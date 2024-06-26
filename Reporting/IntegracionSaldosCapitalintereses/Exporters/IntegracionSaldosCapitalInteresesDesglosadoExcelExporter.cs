﻿/* Empiria Financial *****************************************************************************************
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

    private const int LAST_COLUMN_INDEX = 13;

    private readonly ReportDataDto _reportData;
    private readonly FileTemplateConfig _template;

    public IntegracionSaldosCapitalInteresesDesglosadoExcelExporter(ReportDataDto reportData,
                                                                    FileTemplateConfig template) {
      Assertion.Require(reportData, nameof(reportData));
      Assertion.Require(template, nameof(template));

      _reportData = reportData;
      _template = template;
    }


    public FileDto CreateExcelFile() {
      var excelFile = new ExcelFile(_template);

      excelFile.Open();

      SetHeader(excelFile);

      FillOutRows(excelFile, _reportData.Entries.Select(x => (IIntegracionSaldosCapitalInteresesDto) x));

      excelFile.Save();

      excelFile.Close();

      return excelFile.ToFileDto();
    }

    #region Private methods

    private void FillOutRows(ExcelFile excelFile, IEnumerable<IIntegracionSaldosCapitalInteresesDto> entries) {
      int i = 5;

      foreach (var entry in entries) {
        if (entry is IntegracionSaldosCapitalInteresesEntryDto) {
          FillOutRows(i, excelFile, (IntegracionSaldosCapitalInteresesEntryDto) entry);
        } else if (entry is IntegracionSaldosCapitalInteresesTotalDto) {
          FillOutRows(i, excelFile, (IntegracionSaldosCapitalInteresesTotalDto) entry);
        } else if (entry is IntegracionSaldosCapitalInteresesTitleDto) {
          FillOutRows(i, excelFile, (IntegracionSaldosCapitalInteresesTitleDto) entry);
        }
        i++;
      }
    }

    private void FillOutRows(int row, ExcelFile excelFile, IntegracionSaldosCapitalInteresesTitleDto entry) {
      excelFile.SetCell($"A{row}", $"'{entry.SubledgerAccount}");

      if (entry.ItemType == "Total") {
        excelFile.SetRowBold(row, LAST_COLUMN_INDEX);
        excelFile.SetRowFontName(row, LAST_COLUMN_INDEX, "Courier New");
      }
    }

    private void FillOutRows(int row, ExcelFile excelFile, IntegracionSaldosCapitalInteresesTotalDto entry) {
      excelFile.SetCell($"A{row}", $"'{entry.SubledgerAccount}");
      excelFile.SetCell($"J{row}", entry.CapitalMonedaNacional);
      excelFile.SetCell($"K{row}", entry.InteresesMonedaNacional);
      excelFile.SetCell($"L{row}", entry.TotalMonedaNacional);

      if (entry.ItemType == "Total") {
        excelFile.SetRowBold(row, LAST_COLUMN_INDEX);
        excelFile.SetRowFontName(row, LAST_COLUMN_INDEX, "Courier New");
      }
    }

    private void FillOutRows(int row, ExcelFile excelFile, IntegracionSaldosCapitalInteresesEntryDto entry) {
      excelFile.SetCell($"A{row}", entry.PrestamoName);
      excelFile.SetCell($"B{row}", entry.CurrencyCode);
      excelFile.SetCell($"C{row}", $"'{entry.SubledgerAccount}");
      excelFile.SetCell($"D{row}", entry.SubledgerAccountName);
      excelFile.SetCellWrapText($"D{row}");
      excelFile.SetCell($"E{row}", entry.SectorCode);
      excelFile.SetCell($"F{row}", entry.CapitalMonedaOrigenTotal);
      excelFile.SetCell($"G{row}", entry.InteresesMonedaOrigenTotal);
      excelFile.SetCell($"H{row}", entry.TotalMonedaOrigen);
      excelFile.SetCell($"I{row}", entry.TipoCambio);
      excelFile.SetCell($"J{row}", entry.CapitalMonedaNacional);
      excelFile.SetCell($"K{row}", entry.InteresesMonedaNacional);
      excelFile.SetCell($"L{row}", entry.TotalMonedaNacional);
      if (entry.Vencimiento.HasValue) {
        excelFile.SetCell($"M{row}", entry.Vencimiento.Value);
      }
      if (entry.ItemType == "Total") {
        excelFile.SetRowBold(row, LAST_COLUMN_INDEX);
        excelFile.SetRowFontName(row, LAST_COLUMN_INDEX, "Courier New");
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
