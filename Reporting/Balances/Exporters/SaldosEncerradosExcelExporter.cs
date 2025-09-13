/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                           Component : Excel Exporters                       *
*  Assembly : FinancialAccounting.Reporting.dll            Pattern   : IExcelExporter                        *
*  Type     : SaldosEncerradosExcelExporter                License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Creates a Microsoft Excel file with locked up balances information.                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using DocumentFormat.OpenXml.Spreadsheet;
using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.Office;
using Empiria.Storage;

namespace Empiria.FinancialAccounting.Reporting.Balances {

  /// <summary>Creates a Microsoft Excel file with locked up balances information.</summary>
  internal class SaldosEncerradosExcelExporter {

    private readonly FileTemplateConfig _templateConfig;

    private ExcelFile _excelFile;

    public SaldosEncerradosExcelExporter(FileTemplateConfig templateConfig) {
      Assertion.Require(templateConfig, "templateConfig");

      _templateConfig = templateConfig;
    }


    internal ExcelFile CreateExcelFile(SaldosEncerradosDto reportData) {
      Assertion.Require(reportData, "reportData");

      _excelFile = new ExcelFile(_templateConfig);

      _excelFile.Open();

      SetHeader();

      SetTable(reportData);

      _excelFile.Save();

      _excelFile.Close();

      return _excelFile;
    }

    #region Private methods


    private void SetHeader() {
      _excelFile.SetCell($"A2", _templateConfig.Title);
    }

    private void SetTable(SaldosEncerradosDto reportData) {
      int i = 5;

      var entries = reportData.Entries.Select(x => (SaldosEncerradosBaseEntryDto) x);
      foreach (var entry in entries) {
        _excelFile.SetCell($"A{i}", entry.LedgerNumber);
        _excelFile.SetCell($"B{i}", entry.LedgerName);
        _excelFile.SetCell($"C{i}", entry.CurrencyCode);
        _excelFile.SetCell($"D{i}", entry.AccountNumber);
        _excelFile.SetCell($"E{i}", entry.ItemName);
        _excelFile.SetCell($"F{i}", entry.SectorCode);
        _excelFile.SetCell($"G{i}", entry.SubledgerAccount);
        _excelFile.SetCell($"H{i}", entry.LockedBalance);
        _excelFile.SetCell($"I{i}", entry.RoleChangeDate);
        _excelFile.SetCell($"J{i}", entry.RoleChange);
        if (entry.ItemType == TrialBalanceItemType.Summary) {
          _excelFile.SetRowBold(i, 10);
        }
        i++;
      }
    }

    #endregion Private methods

  } // class SaldosEncerradosExcelExporter

} // namespace Empiria.FinancialAccounting.Reporting.Balances
