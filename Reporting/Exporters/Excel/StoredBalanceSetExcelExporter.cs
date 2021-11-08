/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                           Component : Excel Exporters                       *
*  Assembly : FinancialAccounting.Reporting.dll            Pattern   : IExcelExporter                        *
*  Type     : StoredBalanceSetExcelExporter                License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Creates a Microsoft Excel file for a set of stored balances.                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.Reporting.Exporters.Excel {

  /// <summary>Creates a Microsoft Excel file for a set of stored balances.</summary>
  internal class StoredBalanceSetExcelExporter {

    private readonly ExcelTemplateConfig _templateConfig;
    private ExcelFile _excelFile;

    public StoredBalanceSetExcelExporter(ExcelTemplateConfig templateConfig) {
      Assertion.AssertObject(templateConfig, "templateConfig");

      _templateConfig = templateConfig;
    }


    internal ExcelFile CreateExcelFile(StoredBalanceSetDto balanceSet) {
      Assertion.AssertObject(balanceSet, "balanceSet");

      _excelFile = new ExcelFile(_templateConfig);

      _excelFile.Open();

      SetHeader(balanceSet);

      SetTable(balanceSet);

      _excelFile.Save();

      _excelFile.Close();

      return _excelFile;
    }


    #region Private methods

    private void SetHeader(StoredBalanceSetDto balanceSet) {
      _excelFile.SetCell($"A2", _templateConfig.Title);

      var subTitle = $"Al {balanceSet.BalancesDate.ToString("dd/MMM/yyyy")} ";

      _excelFile.SetCell($"A3", subTitle);
    }


    private void SetTable(StoredBalanceSetDto balanceSet) {
      int i = 5;

      foreach (var balance in balanceSet.Balances) {
        var ledger = Ledger.Parse(balance.Ledger.UID);
        var account = StandardAccount.Parse(balance.StandardAccountId);

        _excelFile.SetCell($"A{i}", ledger.Number);
        _excelFile.SetCell($"B{i}", balance.Currency.UID);
        _excelFile.SetCell($"C{i}", balance.AccountNumber);
        _excelFile.SetCell($"D{i}", balance.AccountName);
        _excelFile.SetCell($"E{i}", balance.SectorCode);
        if (balance.SubledgerAccountId > 0) {
          _excelFile.SetCell($"F{i}", balance.SubledgerAccountNumber);
          _excelFile.SetCell($"G{i}", balance.SubledgerAccountName);
        }
        _excelFile.SetCell($"H{i}", account.DebtorCreditor.ToString());
        _excelFile.SetCell($"I{i}", balance.Balance);

        i++;
      }
    }

    #endregion Private methods

  }  // class StoredBalanceSetExcelExporter

}  // namespace Empiria.FinancialAccounting.Reporting.Exporters.Excel
