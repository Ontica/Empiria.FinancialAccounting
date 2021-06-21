/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Office Integration                           Component : Excel Exporter                        *
*  Assembly : FinancialAccounting.OficeIntegration.dll     Pattern   : Service                               *
*  Type     : TrialBalanceExcelFileCreator                 License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Creates a Microsoft Excel file with trial balance information.                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using System.Collections.Generic;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.OfficeIntegration {

  /// <summary>Creates a Microsoft Excel file with trial balance information.</summary>
  internal class TrialBalanceExcelFileCreator {

    private readonly ExcelTemplateConfig _templateConfig;

    public TrialBalanceExcelFileCreator(ExcelTemplateConfig templateConfig) {
      Assertion.AssertObject(templateConfig, "templateConfig");

      _templateConfig = templateConfig;
    }


    internal ExcelFile CreateExcelFile(TrialBalanceDto trialBalance) {
      Assertion.AssertObject(trialBalance, "trialBalance");

      var excelFile = new ExcelFile(_templateConfig);

      excelFile.Open();

      excelFile.SetCell($"A2", "Balanza de comprobación tradicional");

      var entries = trialBalance.Entries.Select(x => (TrialBalanceEntryDto) x);

      FillOut(entries, excelFile);

      excelFile.Save();

      excelFile.Close();

      return excelFile;
    }


    #region Private methods

    private void FillOut(IEnumerable<TrialBalanceEntryDto> entries, ExcelFile excelFile) {
      int i = 5;

      foreach (var entry in entries) {
        excelFile.SetCell($"A{i}", entry.LedgerNumber);
        excelFile.SetCell($"B{i}", entry.CurrencyCode);
        if (entry.ItemType == BalanceEngine.TrialBalanceItemType.BalanceEntry) {
          excelFile.SetCell($"C{i}", "*");
        }
        excelFile.SetCell($"D{i}", entry.AccountNumber);
        excelFile.SetCell($"E{i}", entry.AccountName);
        excelFile.SetCell($"F{i}", entry.SectorCode);
        excelFile.SetCell($"G{i}", entry.InitialBalance);
        excelFile.SetCell($"H{i}", entry.Debit);
        excelFile.SetCell($"I{i}", entry.Credit);
        excelFile.SetCell($"J{i}", entry.CurrentBalance);

        if (entry.ItemType != BalanceEngine.TrialBalanceItemType.BalanceEntry &&
            entry.ItemType != BalanceEngine.TrialBalanceItemType.BalanceSummary) {
          excelFile.SetRowStyle(Office.Style.Bold, i);
        }
        i++;
      }
    }

    #endregion Private methods

  }  // class TrialBalanceExcelFileCreator

}  // namespace Empiria.FinancialAccounting.OfficeIntegration
