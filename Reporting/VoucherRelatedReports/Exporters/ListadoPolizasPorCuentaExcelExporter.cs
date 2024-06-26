﻿/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                           Component : Excel Exporters                       *
*  Assembly : FinancialAccounting.Reporting.dll            Pattern   : IExcelExporter                        *
*  Type     : ListadoPolizasPorCuentaExcelExporter         License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Genera los datos de las polizas por cuenta en un archivo Excel.                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.Office;
using Empiria.Storage;

using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.Reporting.VoucherRelatedReports.Domain;

namespace Empiria.FinancialAccounting.Reporting {

  /// <summary>Genera los datos de las polizas por cuenta en un archivo Excel.</summary>
  internal class ListadoPolizasPorCuentaExcelExporter : IExcelExporter {

    private readonly ReportDataDto _reportData;
    private readonly FileTemplateConfig _template;

    public ListadoPolizasPorCuentaExcelExporter(ReportDataDto reportData, FileTemplateConfig template) {
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
        excelFile.SetCell($"A{i}", voucher.LedgerNumber);
        excelFile.SetCell($"B{i}", voucher.LedgerName);
        excelFile.SetCell($"C{i}", voucher.CurrencyCode);
        excelFile.SetCell($"D{i}", voucher.AccountNumber);
        excelFile.SetCell($"E{i}", voucher.SectorCode);
        excelFile.SetCell($"F{i}", voucher.SubledgerAccountNumber);
        excelFile.SetCell($"G{i}", voucher.VoucherNumber);
        excelFile.SetCell($"H{i}", (decimal) voucher.Debit);
        excelFile.SetCell($"I{i}", (decimal) voucher.Credit);

        SetCellByItemTypeAndRowStyle(excelFile, voucher, i);

        excelFile.SetCell($"L{i}", EmpiriaString.Clean(voucher.Concept));
        excelFile.SetCell($"M{i}", voucher.AuthorizedBy);
        excelFile.SetCell($"N{i}", voucher.ElaboratedBy);

        i++;
      }
      if (!_reportData.Query.WithSubledgerAccount) {
        excelFile.RemoveColumn("F");
      }
    }

    private void SetCellByItemTypeAndRowStyle(ExcelFile excelFile, VoucherByAccountEntry voucher, int i) {

      if (voucher.ItemType == TrialBalanceItemType.Entry) {
        excelFile.SetCell($"J{i}", voucher.AccountingDate);
        excelFile.SetCell($"K{i}", voucher.RecordingDate);
      }

      if (voucher.ItemType != TrialBalanceItemType.Entry) {
        excelFile.SetRowBold(i, 14);
      }

    }


    #endregion

  } // class ListadoPolizasPorCuentaExcelExporter

} // namespace Empiria.FinancialAccounting.Reporting.Exporters.Excel
