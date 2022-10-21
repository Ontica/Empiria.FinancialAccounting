/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                            Component : Service Layer                        *
*  Assembly : FinancialAccounting.Reporting.dll             Pattern   : Service provider                     *
*  Type     : TransactionSlipExporter                       License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Exports transaction slips (volantes) to Microsoft Excel.                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.BanobrasIntegration.TransactionSlips;
using Empiria.FinancialAccounting.BanobrasIntegration.TransactionSlips.Adapters;

namespace Empiria.FinancialAccounting.Reporting.TransactionSlip.Exporters {

  /// <summary>Exports transaction slips (volantes) to Microsoft Excel.</summary>
  internal class TransactionSlipExporter {

    private readonly FileTemplateConfig _template;

    public TransactionSlipExporter(FileTemplateConfig template) {
      Assertion.Require(template, "template");

      _template = template;
    }


    internal ExcelFile CreateExcelFile(FixedList<TransactionSlipDto> transactionSlips) {
      var excelFile = new ExcelFile(_template);

      excelFile.Open();

      SetHeader(excelFile);

      FillOutRows(excelFile, transactionSlips);

      excelFile.Save();

      excelFile.Close();

      return excelFile;
    }


    internal ExcelFile CreateIsuesExcelFile(FixedList<TransactionSlipDto> transactionSlips) {
      var excelFile = new ExcelFile(_template);

      excelFile.Open();

      SetHeader(excelFile);

      FillOutIssuesRows(excelFile, transactionSlips);

      excelFile.Save();

      excelFile.Close();

      return excelFile;
    }

    #region Private methods


    private void FillOutRows(ExcelFile excelFile, FixedList<TransactionSlipDto> slips) {
      int i = 5;

      foreach (var slip in slips) {
        excelFile.SetCell($"A{i}", slip.Header.AccountsChartId);
        excelFile.SetCell($"B{i}", slip.Header.AccountingDate);
        excelFile.SetCell($"C{i}", EmpiriaString.Clean(slip.Header.SlipNumber));
        excelFile.SetCell($"D{i}", EmpiriaString.Clean(slip.Header.Concept));
        excelFile.SetCell($"E{i}", slip.Header.ControlTotal);
        excelFile.SetCell($"F{i}", slip.Header.RecordingDate);
        excelFile.SetCell($"G{i}", EmpiriaString.Clean(slip.Header.ElaboratedBy));
        excelFile.SetCell($"H{i}", EmpiriaString.Clean(slip.Header.FunctionalArea));
        excelFile.SetCell($"I{i}", EmpiriaString.Clean(slip.Header.VerificationNumber));
        excelFile.SetCell($"J{i}", slip.Header.SystemId);

        if (slip.Voucher != null) {
          excelFile.SetCell($"K{i}", EmpiriaString.Clean(slip.Voucher.Name));
        }

        if (slip.Header.Status != TransactionSlipStatus.Pending) {
          excelFile.SetCell($"L{i}", slip.Header.ProcessingDate);
        }

        excelFile.SetCell($"M{i}", slip.Header.StatusName);

        if (slip.Issues.Count != 0) {
          excelFile.SetRowStyleBold(i);
        }

        i++;
      }
    }


    private void FillOutIssuesRows(ExcelFile excelFile, FixedList<TransactionSlipDto> slips) {
      int i = 5;

      FixedList<TransactionSlipDto> slipsWithIssues = slips.FindAll(x => x.Issues.Count > 0);

      foreach (var slip in slipsWithIssues) {

        foreach (var issue in slip.Issues) {
          excelFile.SetCell($"A{i}", slip.Header.AccountsChartId);
          excelFile.SetCell($"B{i}", slip.Header.AccountingDate);
          excelFile.SetCell($"C{i}", EmpiriaString.Clean(slip.Header.SlipNumber));
          excelFile.SetCell($"D{i}", EmpiriaString.Clean(slip.Header.Concept));
          excelFile.SetCell($"E{i}", slip.Header.ControlTotal);
          excelFile.SetCell($"F{i}", slip.Header.RecordingDate);
          excelFile.SetCell($"G{i}", EmpiriaString.Clean(slip.Header.ElaboratedBy));
          excelFile.SetCell($"H{i}", EmpiriaString.Clean(slip.Header.FunctionalArea));
          excelFile.SetCell($"I{i}", EmpiriaString.Clean(slip.Header.VerificationNumber));
          excelFile.SetCell($"J{i}", slip.Header.SystemId);

          if (slip.Header.Status != TransactionSlipStatus.Pending) {
            excelFile.SetCell($"K{i}", slip.Header.ProcessingDate);
          }

          excelFile.SetCell($"L{i}", EmpiriaString.Clean(issue.Description));

          i++;

        }  // foreach (issue)

      }  // foreach (slip)
    }


    private void SetHeader(ExcelFile excelFile) {
      excelFile.SetCell($"A2", _template.Title);
    }

    #endregion Private methods


  }  // class TransactionSlipExporter

}  // namespace Empiria.FinancialAccounting.Reporting.TransactionSlip.Exporters
