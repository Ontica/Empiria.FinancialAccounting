/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                           Component : Excel Exporters                       *
*  Assembly : FinancialAccounting.Reporting.dll            Pattern   : IExcelExporter                        *
*  Type     : MovimientosNumeroVerificacionExcelExporter   License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Genera los datos de las polizas por número de verificación en un archivo Excel.                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using Empiria.FinancialAccounting.Reporting.VoucherRelatedReports.Domain;
using Empiria.Office;
using Empiria.Storage;

namespace Empiria.FinancialAccounting.Reporting {

  /// <summary>Genera los datos de las polizas por número de verificación en un archivo Excel.</summary>
  internal class MovimientosNumeroVerificacionExcelExporter : IExcelExporter {


    private readonly ReportDataDto _reportData;
    private readonly FileTemplateConfig _template;


    public MovimientosNumeroVerificacionExcelExporter(ReportDataDto reportData,
                                                      FileTemplateConfig template) {
      Assertion.Require(reportData, "reportData");
      Assertion.Require(template, "template");

      _reportData = reportData;
      _template = template;
    }


    public FileDto CreateExcelFile() {
      var excelFile = new ExcelFile(_template);

      excelFile.Open();

      SetHeader(excelFile);

      FillOutRows(excelFile, _reportData.Entries.Select(x => (VoucherByAccountEntry) x));

      excelFile.Save();

      excelFile.Close();

      return excelFile.ToFileDto();
    }


    #region Private methods

    private void SetHeader(ExcelFile excelFile) {
      excelFile.SetCell($"A2", _template.Title);

      var subTitle = $"Del {_reportData.Query.FromDate.ToString("dd/MMM/yyyy")} al " +
                     $"{_reportData.Query.ToDate.ToString("dd/MMM/yyyy")}";

      excelFile.SetCell($"A3", subTitle);
    }


    private void FillOutRows(ExcelFile excelFile, IEnumerable<VoucherByAccountEntry> vouchers) {
      int i = 5;

      foreach (var voucher in vouchers) {
        excelFile.SetCell($"A{i}", voucher.VerificationNumber);
        excelFile.SetCell($"B{i}", voucher.LedgerNumber);
        excelFile.SetCell($"C{i}", voucher.LedgerName);
        excelFile.SetCell($"D{i}", voucher.CurrencyCode);
        excelFile.SetCell($"E{i}", voucher.VoucherNumber);
        excelFile.SetCell($"F{i}", voucher.AccountNumber);
        excelFile.SetCell($"G{i}", voucher.SectorCode);
        excelFile.SetCell($"H{i}", (decimal) voucher.Debit);
        excelFile.SetCell($"I{i}", (decimal) voucher.Credit);
        excelFile.SetCell($"J{i}", voucher.AccountingDate);
        excelFile.SetCell($"K{i}", voucher.RecordingDate);
        excelFile.SetCell($"L{i}", EmpiriaString.Clean(voucher.Concept));
        excelFile.SetCell($"M{i}", voucher.AuthorizedBy);
        excelFile.SetCell($"N{i}", voucher.ElaboratedBy);

        i++;
      }

    }


    #endregion


  } // class MovimientosNumeroVerificacionExcelExporter

} // namespace Empiria.FinancialAccounting.Reporting
