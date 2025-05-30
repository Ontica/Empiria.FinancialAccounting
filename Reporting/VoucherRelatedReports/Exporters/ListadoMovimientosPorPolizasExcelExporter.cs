﻿/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                           Component : Excel Exporters                       *
*  Assembly : FinancialAccounting.Reporting.dll            Pattern   : IExcelExporter                        *
*  Type     : ListadoPolizasPorCuentaExcelExporter         License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Genera los datos de los movimientos por pólizas en un archivo Excel.                           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System.Collections.Generic;

using Empiria.Office;
using Empiria.Storage;

using Empiria.FinancialAccounting.Reporting.VoucherRelatedReports.Domain;

namespace Empiria.FinancialAccounting.Reporting {

  /// <summary>Genera los datos de los movimientos por pólizas en un archivo Excel.</summary>
  public class ListadoMovimientosPorPolizasExcelExporter : IExcelExporter {


    private readonly ReportDataDto _reportData;
    private readonly FileTemplateConfig _template;

    #region Public methods

    public ListadoMovimientosPorPolizasExcelExporter(ReportDataDto reportData,
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

    #endregion Public methods


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
        excelFile.SetCell($"A{i}", voucher.LedgerNumber);
        excelFile.SetCell($"B{i}", voucher.LedgerName);
        excelFile.SetCell($"C{i}", voucher.VoucherId);
        excelFile.SetCell($"D{i}", voucher.VoucherNumber);
        excelFile.SetCell($"E{i}", voucher.AccountNumber);
        excelFile.SetCell($"F{i}", voucher.AccountName);
        excelFile.SetCell($"G{i}", voucher.SectorCode);
        excelFile.SetCell($"H{i}", voucher.SubledgerAccountNumber);
        excelFile.SetCell($"I{i}", voucher.SubledgerAccountNumber.Length != 0 ? voucher.SubledgerAccountName : string.Empty);
        excelFile.SetCell($"J{i}", voucher.VerificationNumber);
        excelFile.SetCell($"K{i}", voucher.CurrencyCode);
        excelFile.SetCell($"L{i}", (decimal) voucher.Debit);
        excelFile.SetCell($"M{i}", (decimal) voucher.Credit);
        excelFile.SetCell($"N{i}", voucher.AccountingDate);
        excelFile.SetCell($"O{i}", voucher.RecordingDate);
        excelFile.SetCell($"P{i}", voucher.Concept);
        excelFile.SetCell($"Q{i}", voucher.AuthorizedBy);
        excelFile.SetCell($"R{i}", voucher.ElaboratedBy);

        i++;
      }
    }

    #endregion Private methods


  } // class ListadoMovimientosPorPolizasExcelExporter

} // namespace Empiria.FinancialAccounting.Reporting
