/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                           Component : Excel Exporters                       *
*  Assembly : FinancialAccounting.Reporting.dll            Pattern   : IExcelExporter                        *
*  Type     : VouchersExporter                             License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Main service used to export vouchers to Microsoft Excel.                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.FinancialAccounting.Vouchers.Adapters;

namespace Empiria.FinancialAccounting.Reporting {

  /// <summary>Main service used to export vouchers to Microsoft Excel.</summary>
  internal class VouchersExporter {

    private readonly FileTemplateConfig _templateConfig;
    private ExcelFile _excelFile;

    public VouchersExporter(FileTemplateConfig templateConfig) {
      Assertion.Require(templateConfig, "templateConfig");

      _templateConfig = templateConfig;
    }


    internal ExcelFile CreateExcelFile(VoucherDto voucher) {
      Assertion.Require(voucher, "voucher");

      _excelFile = new ExcelFile(_templateConfig);

      _excelFile.Open();

      SetHeader(voucher);

      SetTable(voucher);

      _excelFile.Save();

      _excelFile.Close();

      return _excelFile;
    }


    #region Private methods


    private void SetHeader(VoucherDto voucher) {
      _excelFile.SetCell($"A2", _templateConfig.Title);

      var subTitle = $"Elaboración {voucher.RecordingDate.ToString("dd/MMM/yyyy")} ";

      _excelFile.SetCell($"A3", subTitle);
    }


    private void SetTable(VoucherDto voucher) {
      int i = 5;

      foreach (var entry in voucher.Entries) {

        _excelFile.SetCell($"A{i}", $"{entry.AccountNumber} {entry.SubledgerAccountNumber}");
        _excelFile.SetCell($"B{i}", entry.Sector);
        _excelFile.SetCell($"C{i}", $"{entry.AccountName} {entry.SubledgerAccountName}");
        _excelFile.SetCell($"D{i}", entry.VerificationNumber);
        _excelFile.SetCell($"E{i}", entry.ResponsibilityArea);
        _excelFile.SetCell($"F{i}", entry.Currency);
        _excelFile.SetCell($"G{i}", entry.ExchangeRate);
        _excelFile.SetCell($"H{i}", entry.Debit);
        _excelFile.SetCell($"I{i}", entry.Credit);

        i++;
      }
    }


    #endregion Private methods


  }
}
