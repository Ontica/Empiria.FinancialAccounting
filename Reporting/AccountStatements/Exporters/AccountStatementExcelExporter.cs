/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                           Component : Excel Exporters                       *
*  Assembly : FinancialAccounting.Reporting.dll            Pattern   : IExcelExporter                        *
*  Type     : AccountStatementExcelExporter                License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Creates a Microsoft Excel file with an account statement.                                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.Office;
using Empiria.Storage;

using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.BalanceExplorer.Adapters;
using Empiria.FinancialAccounting.Reporting.AccountStatements.Adapters;

namespace Empiria.FinancialAccounting.Reporting.AccountStatements.Exporters {

  /// <summary>Creates a Microsoft Excel file with an account statement.</summary>
  internal class AccountStatementExcelExporter {

    private BalanceExplorerQuery _query = new BalanceExplorerQuery();
    private readonly FileTemplateConfig _templateConfig;
    private ExcelFile _excelFile;


    public AccountStatementExcelExporter(FileTemplateConfig templateConfig) {
      Assertion.Require(templateConfig, nameof(templateConfig));

      _templateConfig = templateConfig;
    }


    internal ExcelFile CreateExcelFile(AccountStatementDto accountStatement) {
      Assertion.Require(accountStatement, nameof(accountStatement));

      _query = accountStatement.Query;

      _excelFile = new ExcelFile(_templateConfig);

      _excelFile.Open();

      SetHeader();

      SetTable(accountStatement);

      _excelFile.Save();

      _excelFile.Close();

      return _excelFile;
    }


    private void SetHeader() {
      _excelFile.SetCell($"A2", _templateConfig.Title);

      var subTitle = $"Del {_query.InitialPeriod.FromDate.ToString("dd/MMM/yyyy")} " +
                      $"al {_query.InitialPeriod.ToDate.ToString("dd/MMM/yyyy")}";

      _excelFile.SetCell($"A3", subTitle);
    }


    private void SetTable(AccountStatementDto accountStatement) {
      FillOut(accountStatement.Entries.Select(x => (VouchersByAccountEntryDto) x));
    }


    private void FillOut(IEnumerable<VouchersByAccountEntryDto> vouchers) {
      int i = 5;

      foreach (var voucher in vouchers) {
        
        if (voucher.ItemType == TrialBalanceItemType.Entry) {
          
          _excelFile.SetCell($"A{i}", $"({voucher.LedgerNumber}) {voucher.LedgerName}");
          _excelFile.SetCell($"J{i}", voucher.ExchangeRate);

          //if (_query.UseDefaultValuation || _query.InitialPeriod.ExchangeRateTypeUID != string.Empty) {

          //}
        }
        _excelFile.SetCell($"B{i}", voucher.CurrencyCode);
        _excelFile.SetCell($"C{i}", voucher.AccountNumber);
        _excelFile.SetCell($"D{i}", voucher.SectorCode);
        _excelFile.SetCell($"E{i}", voucher.SubledgerAccountNumber);
        _excelFile.SetCell($"F{i}", voucher.VoucherNumber);
        _excelFile.SetCell($"I{i}", voucher.CurrentBalance);

        _excelFile.SetCell($"M{i}", EmpiriaString.Clean(voucher.Concept));
        _excelFile.SetCell($"N{i}", voucher.ElaboratedBy);

        
        SetCellClauses(voucher,i);
        i++;
      }

      if (!_query.UseDefaultValuation && _query.InitialPeriod.ExchangeRateTypeUID == string.Empty) {
        _excelFile.RemoveColumn("J");
      }

    }


    private void SetCellClauses(VouchersByAccountEntryDto voucher, int i) {

      if (voucher.Debit.HasValue) {
        _excelFile.SetCell($"G{i}", voucher.Debit.Value);
      }

      if (voucher.Credit.HasValue) {
        _excelFile.SetCell($"H{i}", voucher.Credit.Value);
      }

      if (voucher.AccountingDate != ExecutionServer.DateMaxValue) {
        _excelFile.SetCell($"K{i}", voucher.AccountingDate);
      }

      if (voucher.RecordingDate != ExecutionServer.DateMaxValue) {
        _excelFile.SetCell($"L{i}", voucher.RecordingDate);
      }

      if (voucher.ItemType == TrialBalanceItemType.Total) {
        _excelFile.SetRowBold(i, 13);
      }

    }
  } // class AccountStatementExcelExporter

} // namespace Empiria.FinancialAccounting.Reporting.AccountStatements.Exporters
