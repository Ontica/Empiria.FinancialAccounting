/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                           Component : Excel Exporters                       *
*  Assembly : FinancialAccounting.Reporting.dll            Pattern   : IExcelExporter                        *
*  Type     : BalanzaSatExcelExporter                      License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Genera los datos de la balanza de comprobación en un archivo Excel.                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.FinancialAccounting.Reporting.Builders;

namespace Empiria.FinancialAccounting.Reporting.Exporters.Excel {

  /// <summary>Genera los datos de la balanza de comprobación en un archivo Excel.</summary>
  internal class BalanzaSatExcelExporter : IExcelExporter {

    private readonly ReportDataDto _reportData;
    private readonly FileTemplateConfig _template;

    public BalanzaSatExcelExporter(ReportDataDto reportData, FileTemplateConfig template) {
      Assertion.Require(reportData, "reportData");
      Assertion.Require(template, "template");

      _reportData = reportData;
      _template = template;
    }


    public FileReportDto CreateExcelFile() {
      var excelFile = new ExcelFile(_template);

      excelFile.Open();

      SetHeader(excelFile);

      FillOutRows(excelFile, _reportData.Entries.Select(x => (BalanzaSatEntry) x));

      excelFile.Save();

      excelFile.Close();

      return excelFile.ToFileReportDto();
    }


    #region Private methods

    private void FillOutRows(ExcelFile excelFile, IEnumerable<BalanzaSatEntry> entries) {
      int i = 5;

      foreach (var entry in entries) {
        excelFile.SetCell($"A{i}", entry.Cuenta);
        excelFile.SetCell($"B{i}", entry.SaldoInicial);
        excelFile.SetCell($"C{i}", entry.Debe);
        excelFile.SetCell($"D{i}", entry.Haber);
        excelFile.SetCell($"E{i}", entry.SaldoFinal);
        excelFile.SetCell($"F{i}", entry.FechaModificacion);
        i++;
      }

      if (_reportData.Query.SendType == SendType.N) {
        excelFile.RemoveColumn("F");
      }
    }


    private void SetHeader(ExcelFile excelFile) {
      excelFile.SetCell($"A2", _template.Title);

      DateTime fromDate = new DateTime(_reportData.Query.ToDate.Year, _reportData.Query.ToDate.Month, 1);
      DateTime toDate = _reportData.Query.ToDate;

      var subTitle = $"Del {fromDate.ToString("dd/MMM/yyyy")} al {toDate.ToString("dd/MMM/yyyy")}";

      excelFile.SetCell($"A3", subTitle);
    }

    #endregion Private methods

  } // class BalanzaSatExcelExporter

} // Empiria.FinancialAccounting.Reporting.Exporters.Excel
