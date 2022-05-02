/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                           Component : Excel Exporters                       *
*  Assembly : FinancialAccounting.Reporting.dll            Pattern   : IExcelExporter                        *
*  Type     : ReconciliationExcelExporter                  License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Exports a reconciliation result to Microsoft Excel.                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.Reconciliation.Adapters;

namespace Empiria.FinancialAccounting.Reporting.Exporters.Excel {

  /// <summary>Exports a reconciliation result to Microsoft Excel.</summary>
  internal class ReconciliationExcelExporter {

    private readonly FileTemplateConfig _templateConfig;

    private ExcelFile excelFile;

    public ReconciliationExcelExporter(FileTemplateConfig templateConfig) {
      Assertion.AssertObject(templateConfig, nameof(templateConfig));

      _templateConfig = templateConfig;
    }


    internal ExcelFile CreateExcelFile(ReconciliationResultDto reconciliationResult) {
      Assertion.AssertObject(reconciliationResult, nameof(reconciliationResult));

      excelFile = new ExcelFile(_templateConfig);

      excelFile.Open();

      SetHeader(reconciliationResult);

      FillOut(reconciliationResult);

      excelFile.Save();

      excelFile.Close();

      return excelFile;
    }


    #region Private methods

    private void FillOut(ReconciliationResultDto reconciliationResult) {
      int i = 5;

      foreach (var entry in reconciliationResult.Entries) {
        excelFile.SetCell($"A{i}", entry.CurrencyCode);
        excelFile.SetCell($"B{i}", entry.AccountNumber);
        excelFile.SetCell($"C{i}", entry.SectorCode);
        excelFile.SetCell($"D{i}", entry.OperationalTotal);
        excelFile.SetCell($"E{i}", entry.AccountingTotal);
        excelFile.SetCell($"F{i}", entry.Difference);
        i++;
      }
    }


    private void SetHeader(ReconciliationResultDto reconciliationResult) {
      excelFile.SetCell($"A2", _templateConfig.Title);

      excelFile.SetCell($"F2", reconciliationResult.Command.Date.ToString("dd/MMM/yyyy"));
    }

    #endregion Private methods


  } // class ReconciliationExcelExporter

} // namespace Empiria.FinancialAccounting.Reporting.Exporters.Excel
