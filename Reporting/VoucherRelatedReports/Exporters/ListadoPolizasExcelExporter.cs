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

using Empiria.FinancialAccounting.Reporting.VoucherRelatedReports.Domain;

namespace Empiria.FinancialAccounting.Reporting {

  /// <summary></summary>
  internal class ListadoPolizasExcelExporter : IExcelExporter {

    private readonly ReportDataDto _reportData;
    private readonly FileTemplateConfig _template;

    public ListadoPolizasExcelExporter(ReportDataDto reportData, FileTemplateConfig template) {
      Assertion.Require(reportData, "reportData");
      Assertion.Require(template, "template");

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

    private void FillOutRows(ExcelFile excelFile, IEnumerable<PolizaReturnedEntry> vouchers) {
      int i = 5;

      foreach (var voucher in vouchers) {

        excelFile.SetCell($"A{i}", voucher.LedgerName);
        excelFile.SetCell($"B{i}", voucher.VoucherNumber);
        excelFile.SetCell($"C{i}", voucher.AccountingDate);
        excelFile.SetCell($"D{i}", voucher.RecordingDate);
        excelFile.SetCell($"E{i}", voucher.ElaboratedBy);
        excelFile.SetCell($"F{i}", EmpiriaString.Clean(voucher.Concept));
        excelFile.SetCell($"G{i}", voucher.Debit);
        excelFile.SetCell($"H{i}", voucher.Credit);

        if (voucher.ItemType != ItemType.Entry) {
          excelFile.SetRowStyleBold(i);
        }

        i++;

      }
    }


    private void SetHeader(ExcelFile excelFile) {
      excelFile.SetCell($"A2", _template.Title);

      var subTitle = $"Del {_reportData.Query.FromDate.ToString("dd/MMM/yyyy")} al " +
                     $"{_reportData.Query.ToDate.ToString("dd/MMM/yyyy")}";

      excelFile.SetCell($"A3", subTitle);
    }

    #endregion Private methods


  } // class PolizaExcelExporter

} // namespace Empiria.FinancialAccounting.Reporting.Exporters.Excel
