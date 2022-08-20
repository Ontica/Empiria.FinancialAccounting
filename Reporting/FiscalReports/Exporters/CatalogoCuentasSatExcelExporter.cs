/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                           Component : Excel Exporters                       *
*  Assembly : FinancialAccounting.Reporting.dll            Pattern   : IExcelExporter                        *
*  Type     : CatalogoCuentasSatExcelExporter              License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Creates a Microsoft Excel file with operational report from balance information.               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.FinancialAccounting.Reporting.Builders;
using Empiria.FinancialAccounting.Reporting.Exporters;
using Empiria.FinancialAccounting.Reporting.Exporters.Excel;

namespace Empiria.FinancialAccounting.Reporting {

  /// <summary>Creates a Microsoft Excel file with operational report from balance information.</summary>
  internal class CatalogoCuentasSatExcelExporter : IExcelExporter {

    private readonly ReportDataDto _reportData;
    private readonly FileTemplateConfig _template;

    public CatalogoCuentasSatExcelExporter(ReportDataDto reportData, FileTemplateConfig template) {
      Assertion.Require(reportData, "reportData");
      Assertion.Require(template, "template");

      _reportData = reportData;
      _template = template;
    }


    public FileReportDto CreateExcelFile() {
      var excelFile = new ExcelFile(_template);

      excelFile.Open();

      SetHeader(excelFile);

      FillOutRows(excelFile, _reportData.Entries.Select(x => (CatalogoCuentasSatEntry) x));

      excelFile.Save();

      excelFile.Close();

      return excelFile.ToFileReportDto();
    }


    #region Private methods

    private void FillOutRows(ExcelFile excelFile, IEnumerable<CatalogoCuentasSatEntry> entries) {
      int i = 5;

      foreach (var entry in entries) {
        excelFile.SetCell($"A{i}", entry.CodigoAgrupacion);
        excelFile.SetCell($"B{i}", entry.NumeroCuenta);
        excelFile.SetCell($"C{i}", entry.Descripcion);
        excelFile.SetCell($"D{i}", entry.SubcuentaDe);
        excelFile.SetCell($"E{i}", entry.Nivel);
        excelFile.SetCell($"F{i}", entry.Naturaleza);
        excelFile.SetCell($"G{i}", entry.FechaModificacion);
        excelFile.SetCell($"H{i}", entry.Baja);

        i++;
      }
    }


    private void SetHeader(ExcelFile excelFile) {
      excelFile.SetCell($"A2", _template.Title);

      DateTime toDate = _reportData.Query.ToDate;

      var subTitle = $"Al {toDate.ToString("dd/MMM/yyyy")}";

      excelFile.SetCell($"A3", subTitle);
    }

    #endregion Private methods

  } // class CatalogoCuentasSatExcelExporter

} // Empiria.FinancialAccounting.Reporting.Exporters.Excel
