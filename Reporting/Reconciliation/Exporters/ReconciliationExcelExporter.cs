﻿/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                           Component : Excel Exporters                       *
*  Assembly : FinancialAccounting.Reporting.dll            Pattern   : IExcelExporter                        *
*  Type     : ReconciliationExcelExporter                  License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Exports a reconciliation result to Microsoft Excel.                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using Empiria.Office;
using Empiria.Storage;

using Empiria.FinancialAccounting.Reconciliation.Adapters;

namespace Empiria.FinancialAccounting.Reporting.Reconciliation.Exporters {

  /// <summary>Exports a reconciliation result to Microsoft Excel.</summary>
  internal class ReconciliationExcelExporter {

    private readonly FileTemplateConfig _templateConfig;

    private ExcelFile excelFile;

    public ReconciliationExcelExporter(FileTemplateConfig templateConfig) {
      Assertion.Require(templateConfig, nameof(templateConfig));

      _templateConfig = templateConfig;
    }


    internal ExcelFile CreateExcelFile(ReconciliationResultDto reconciliationResult) {
      Assertion.Require(reconciliationResult, nameof(reconciliationResult));

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

      SetSignatures(i + 2);
    }


    private void SetHeader(ReconciliationResultDto reconciliationResult) {
      excelFile.SetCell($"A3", _templateConfig.Title);

      excelFile.SetCell($"F3", reconciliationResult.Command.Date.ToString("dd/MMM/yyyy"));
    }


    private void SetSignatures(int startIndex) {
      int index = startIndex;

      excelFile.RemoveFormat($"A{index}", $"E{index + 10}");

      excelFile.SetCell($"D{index}", "Firmas");

      index++;

      excelFile.SetCell($"A{index}", "Elaboró:");
      excelFile.SetCell($"B{index}", "Especialista / Experto Técnico");

      index++;

      excelFile.SetCell($"A{index}", "Revisó:");
      excelFile.SetCell($"B{index}", "Subgerente de Información Contable");

      excelFile.SetCell($"D{index}", "Revisó:");
      excelFile.SetCell($"E{index}", "Subgerente de Registro e Información de Mercados Financieros");

      index++;

      excelFile.SetCell($"A{index}", "Autorizó:");
      excelFile.SetCell($"B{index}", "Gerente de Contabilidad");

      excelFile.SetCell($"D{index}", "Autorizó:");
      excelFile.SetCell($"E{index}", "Gerente de Operación Financiera");
    }

    #endregion Private methods

  } // class ReconciliationExcelExporter

} // namespace Empiria.FinancialAccounting.Reporting.Reconciliation.Exporters
