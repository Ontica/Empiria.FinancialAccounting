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
using System.Linq;
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

      SetVoucherData(voucher);

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

      int i = 13;
      foreach (var entry in voucher.Entries) {

        if (entry == voucher.Entries.Last()) {
          _excelFile.SetCell($"C{i}", $"{entry.AccountName}");
          _excelFile.SetCell($"F{i}", entry.Currency);
          _excelFile.SetCell($"H{i}", entry.Debit);
          _excelFile.SetCell($"I{i}", entry.Credit);
          _excelFile.SetRowStyleBold(i);
        } else {
          string withSubledger = entry.SubledgerAccountName != "" ?
                                  $"\n{entry.SubledgerAccountName}" : "";

          _excelFile.SetCell($"A{i}", $"{entry.AccountNumber} {entry.SubledgerAccountNumber}");
          _excelFile.SetCell($"B{i}", entry.Sector.ToString());
          _excelFile.SetCell($"C{i}", $"{entry.AccountName}{withSubledger}");
          _excelFile.SetCell($"D{i}", entry.VerificationNumber);
          _excelFile.SetCell($"E{i}", entry.ResponsibilityArea.ToString());
          _excelFile.SetCell($"F{i}", entry.Currency);
          _excelFile.SetCell($"G{i}", entry.ExchangeRate);
          _excelFile.SetCell($"H{i}", entry.Debit);
          _excelFile.SetCell($"I{i}", entry.Credit);
        }

        i++;
      }

    }


    private void SetVoucherData(VoucherDto voucher) {

      _excelFile.SetCell($"G3", $"{voucher.Number}");
      _excelFile.SetCell($"G4", $"id: {voucher.Id}");

      _excelFile.SetCell($"A5", $"{voucher.Concept}");

      _excelFile.SetCell($"B6", $"{voucher.AccountsChart.Name}");
      _excelFile.SetCell($"B7", $"{voucher.VoucherType.Name}");
      _excelFile.SetCell($"B8", $"{voucher.FunctionalArea.Name}");
      _excelFile.SetCell($"B9", $"{voucher.ElaboratedBy}");
      _excelFile.SetCell($"B10", $"{voucher.AuthorizedBy}");

      _excelFile.SetCell($"E6", $"{voucher.Ledger.Name}");
      _excelFile.SetCell($"E7", $"{voucher.TransactionType.Name}");
      _excelFile.SetCell($"E8", $"{voucher.AccountingDate.ToString("dd/MMM/yyyy")}");
      _excelFile.SetCell($"E9", $"{voucher.RecordingDate.ToString("dd/MMM/yyyy")}");
      _excelFile.SetCell($"E10", $"{voucher.Status}");

    }

    #endregion Private methods


  }
}
