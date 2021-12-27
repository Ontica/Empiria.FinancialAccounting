/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                           Component : Excel Exporters                       *
*  Assembly : FinancialAccounting.Reporting.dll            Pattern   : IExcelExporter                        *
*  Type     : VouchersByAccountExcelExporter               License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Creates a Microsoft Excel file with vouchers by account information.                           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.Reporting.Adapters;

namespace Empiria.FinancialAccounting.Reporting.Exporters.Excel {

  /// <summary>Creates a Microsoft Excel file with vouchers by account information.</summary>
  internal class VouchersByAccountExcelExporter {

    private BalanceCommand _command = new BalanceCommand();
    private readonly ExcelTemplateConfig _templateConfig;
    private ExcelFile _excelFile;


    public VouchersByAccountExcelExporter(ExcelTemplateConfig templateConfig) {
      Assertion.AssertObject(templateConfig, "templateConfig");

      _templateConfig = templateConfig;
    }


    internal ExcelFile CreateExcelFile(AccountStatementDto voucherDto) {
      Assertion.AssertObject(voucherDto, "voucherDto");

      _command = voucherDto.Command;

      _excelFile = new ExcelFile(_templateConfig);
      
      _excelFile.Open();

      SetHeader();

      SetTable(voucherDto);

      _excelFile.Save();

      _excelFile.Close();

      return _excelFile;
    }


    private void SetHeader() {
      _excelFile.SetCell($"A2", _templateConfig.Title);

      var subTitle = $"Del {_command.InitialPeriod.FromDate.ToString("dd/MMM/yyyy")} " +
                      $"al {_command.InitialPeriod.ToDate.ToString("dd/MMM/yyyy")}";

      _excelFile.SetCell($"A3", subTitle);
    }


    private void SetTable(AccountStatementDto voucherDto) {
      FillOutVouchersByAccount(voucherDto.Entries.Select(x => (VouchersByAccountEntryDto) x));
    } // class VouchersByAccountExcelExporter


    private void FillOutVouchersByAccount(IEnumerable<VouchersByAccountEntryDto> vouchers) {
      int i = 5;

      foreach (var voucher in vouchers) {
        var debit = voucher.Debit != null ? voucher.Debit.ToString() : "";
        var credit = voucher.Credit != null ? voucher.Credit.ToString() : "";
       
        _excelFile.SetCell($"A{i}", voucher.LedgerNumber);
        _excelFile.SetCell($"B{i}", voucher.CurrencyCode);
        _excelFile.SetCell($"C{i}", voucher.AccountNumber);
        _excelFile.SetCell($"D{i}", voucher.SectorCode);
        _excelFile.SetCell($"E{i}", voucher.SubledgerAccountNumber);
        _excelFile.SetCell($"F{i}", voucher.VoucherNumber);
        _excelFile.SetCell($"G{i}", debit);
        _excelFile.SetCell($"H{i}", credit);
        _excelFile.SetCell($"I{i}", voucher.CurrentBalance);
        _excelFile.SetCell($"J{i}", voucher.AccountingDate);
        _excelFile.SetCell($"K{i}", voucher.RecordingDate);
        _excelFile.SetCell($"L{i}", voucher.Concept);

        if (voucher.ItemType == TrialBalanceItemType.Total) {
          _excelFile.SetRowStyleBold(i);
        }
        i++;
      }
    }

  } // class VouchersByAccountExcelExporter

} // namespace Empiria.FinancialAccounting.Reporting.Exporters.Excel
