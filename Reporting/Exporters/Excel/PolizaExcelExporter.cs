/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                           Component : Excel Exporters                       *
*  Assembly : FinancialAccounting.Reporting.dll            Pattern   : IExcelExporter                        *
*  Type     : PolizaExcelExporter                          License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Genera los datos de la poliza en un archivo Excel.                                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using Empiria.FinancialAccounting.Reporting.Builders;

namespace Empiria.FinancialAccounting.Reporting.Exporters.Excel {
  
  /// <summary></summary>
  internal class PolizaExcelExporter : IExcelExporter {

    private readonly ReportDataDto _reportData;
    private readonly ExcelTemplateConfig _template;

    public PolizaExcelExporter(ReportDataDto reportData, ExcelTemplateConfig template) {
      Assertion.AssertObject(reportData, "reportData");
      Assertion.AssertObject(template, "template");

      _reportData = reportData;
      _template = template;
    }


    public FileReportDto CreateExcelFile() {
      var excelFile = new ExcelFile(_template);

      excelFile.Open();

      SetHeader(excelFile);

      FillOutRows(excelFile, _reportData.Entries.Select(x => (PolizaReturnedEntry) x));

      excelFile.Save();

      excelFile.Close();

      return excelFile.ToFileReportDto();
    }


    #region Private methods

    private void FillOutRows(ExcelFile excelFile, IEnumerable<PolizaReturnedEntry> entries) {
      int i = 5;

      foreach (var entry in entries) {
        excelFile.SetCell($"A{i}", entry.LedgerName);
        excelFile.SetCell($"B{i}", entry.VoucherNumber);
        excelFile.SetCell($"C{i}", entry.AccountingDate);
        excelFile.SetCell($"D{i}", entry.RecordingDate);
        excelFile.SetCell($"E{i}", entry.ElaboratedBy);
        excelFile.SetCell($"F{i}", entry.Concept);
        excelFile.SetCell($"G{i}", entry.Debit);
        excelFile.SetCell($"H{i}", entry.Credit);

        i++;
      }
    }


    private void SetHeader(ExcelFile excelFile) {
      excelFile.SetCell($"A2", _template.Title);

      var subTitle = $"Del {_reportData.Command.FromDate.ToString("dd/MMM/yyyy")} al " +
                     $"{_reportData.Command.ToDate.ToString("dd/MMM/yyyy")}";

      excelFile.SetCell($"A3", subTitle);
    }

    #endregion Private methods


  } // class PolizaExcelExporter

} // namespace Empiria.FinancialAccounting.Reporting.Exporters.Excel
