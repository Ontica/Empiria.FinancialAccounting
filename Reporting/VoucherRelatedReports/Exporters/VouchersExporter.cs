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

using Empiria.Office;
using Empiria.Storage;

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


    internal ExcelFile CreateExcelFile(FixedList<VoucherDto> vouchers) {
      Assertion.Require(vouchers, "vouchers");

      _excelFile = new ExcelFile(_templateConfig);

      _excelFile.Open();

      SetVouchersListInfo(vouchers);

      _excelFile.Save();

      _excelFile.Close();

      return _excelFile;
    }


    internal ExcelFile CreateExcelFile(VoucherDto voucher) {
      Assertion.Require(voucher, "voucher");

      _excelFile = new ExcelFile(_templateConfig);

      _excelFile.Open();

      SetExcelHeader(voucher);

      SetVoucherData(voucher);

      SetTable(voucher);

      _excelFile.Save();

      _excelFile.Close();

      return _excelFile;
    }


    #region Private methods


    private void SetDataByVoucher(VoucherDto voucher, int inc, out int i) {
      
      i = inc + 1;
      
      _excelFile.SetCell($"B{i}", $"{voucher.AccountsChart.Name}");
      _excelFile.SetCell($"E{i}", $"{voucher.Ledger.Name}");
      i++;
      _excelFile.SetCell($"B{i}", $"{voucher.VoucherType.Name}");
      _excelFile.SetCell($"E{i}", $"{voucher.TransactionType.Name}");
      i++;
      _excelFile.SetCell($"B{i}", $"{voucher.FunctionalArea.Name}");
      _excelFile.SetCell($"E{i}", $"{voucher.AccountingDate.ToString("dd/MMM/yyyy")}");
      i++;
      _excelFile.SetCell($"B{i}", $"{voucher.ElaboratedBy}");
      _excelFile.SetCell($"E{i}", $"{voucher.RecordingDate.ToString("dd/MMM/yyyy")}");
      i++;
      _excelFile.SetCell($"B{i}", $"{voucher.AuthorizedBy}");
      _excelFile.SetCell($"E{i}", $"{voucher.Status}");

    }


    private void SetDataTitlesByVoucher(int i) {
      i = i + 1;

      _excelFile.SetCell($"A{i}", "Tipo de contabilidad:");
      _excelFile.SetCell($"D{i}", "Contabilidad:");
      i++;
      _excelFile.SetCell($"A{i}", "Tipo de póliza:");
      _excelFile.SetCell($"D{i}", "Tipo de transacción:");
      i++;
      _excelFile.SetCell($"A{i}", "Originada en:");
      _excelFile.SetCell($"D{i}", "Afectación:");
      i++;
      _excelFile.SetCell($"A{i}", "Elaboró:");
      _excelFile.SetCell($"D{i}", "Elaboración:");
      i++;
      _excelFile.SetCell($"A{i}", "Autorizó:");
      _excelFile.SetCell($"D{i}", "Envió al diario:");

    }


    private void SetDataMovementesByVoucher(VoucherDto voucher, int inc, out int i) {
      i = inc+1;

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
          _excelFile.SetCell($"C{i}", $"{entry.AccountName} {withSubledger}");
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


    private void SetExcelHeader(VoucherDto voucher) {
      _excelFile.SetCell($"A2", _templateConfig.Title);

      var subTitle = $"Elaboración {voucher.RecordingDate.ToString("dd/MMM/yyyy")} ";

      _excelFile.SetCell($"A3", subTitle);

    }


    private void SetExcelHeaderByVoucher(VoucherDto voucher, int inc,  out int i) {
      
      i = inc;
      _excelFile.SetCell($"A{i}", _templateConfig.Title);
      _excelFile.SetCell($"I{i}", $"Póliza:");
      _excelFile.SetCellFontColorStyle($"I{i}", System.Drawing.Color.Green);
      _excelFile.SetRowStyleBold(i);
      i++;

      var subTitle = $"Elaboración {voucher.RecordingDate.ToString("dd/MMM/yyyy")} ";
      _excelFile.SetCell($"A{i}", subTitle);
      _excelFile.SetCell($"I{i}", $"{voucher.Number}");
      _excelFile.SetRowStyleBold(i);
      i++;
      _excelFile.SetCell($"I{i}", $"id: {voucher.Id}");
      i++;
      _excelFile.SetCell($"A{i}", $"{voucher.Concept}");
      _excelFile.SetRowBackgroundStyle(i, DocumentFormat.OpenXml.Spreadsheet.PatternValues.MediumGray);
      _excelFile.SetRowStyleBold(i);
      
    }


    private void SetHeaderMovementesByVoucher(int inc, out int i) {
      i = inc + 2;

      _excelFile.SetCell($"A{i}", "No. Cuenta/Auxiliar");
      _excelFile.SetCell($"B{i}", "Sector");
      _excelFile.SetCell($"C{i}", "Descripción/Concepto");
      _excelFile.SetCell($"D{i}", "Verificado");
      _excelFile.SetCell($"E{i}", "Área");
      _excelFile.SetCell($"F{i}", "Moneda");
      _excelFile.SetCell($"G{i}", "T. de cambio");
      _excelFile.SetCell($"H{i}", "Debe");
      _excelFile.SetCell($"I{i}", "Haber");
      _excelFile.SetRowStyleBold(i);
      _excelFile.SetRowFontColorStyle(i, System.Drawing.Color.Green);
      _excelFile.SetRowBackgroundStyle(i, DocumentFormat.OpenXml.Spreadsheet.PatternValues.LightGray);
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
          _excelFile.SetCell($"C{i}", $"{entry.AccountName} {withSubledger}");
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

      _excelFile.SetCell($"I3", $"{voucher.Number}");
      _excelFile.SetCell($"I4", $"id: {voucher.Id}");

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


    private void SetVouchersListInfo(FixedList<VoucherDto> vouchers) {

      int i = 2, inc = 0;
      foreach (var voucher in vouchers) {
        inc = i;
        SetExcelHeaderByVoucher(voucher, inc, out i);
        inc = i;
        SetDataTitlesByVoucher(i);
        inc = i;
        SetDataByVoucher(voucher, inc, out i);
        inc = i;
        SetHeaderMovementesByVoucher(inc, out i);
        inc = i;
        SetDataMovementesByVoucher(voucher, inc, out i);
        i += 4;
      }

    }


    #endregion Private methods


  }
}
