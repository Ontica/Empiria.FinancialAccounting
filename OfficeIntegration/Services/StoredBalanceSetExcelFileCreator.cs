/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Office Integration                           Component : Excel Exporter                        *
*  Assembly : FinancialAccounting.OficeIntegration.dll     Pattern   : Service                               *
*  Type     : StoredBalanceSetExcelFileCreator             License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Creates a Microsoft Excel file for a set of stored balances.                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.OfficeIntegration {

  /// <summary>Creates a Microsoft Excel file for a set of stored balances.</summary>
  internal class StoredBalanceSetExcelFileCreator {

    private readonly ExcelTemplateConfig _templateConfig;
    private ExcelFile _excelFile;

    public StoredBalanceSetExcelFileCreator(ExcelTemplateConfig templateConfig) {
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
        if (balance.SubsidiaryAccountId > 0) {
          _excelFile.SetCell($"F{i}", balance.SubsidiaryAccountNumber);
          _excelFile.SetCell($"G{i}", balance.SubsidiaryAccountName);
        }
        _excelFile.SetCell($"H{i}", account.DebtorCreditor.ToString());
        _excelFile.SetCell($"I{i}", balance.Balance);

        i++;
      }
    }

    #endregion Private methods

  }  // class StoredBalanceSetExcelFileCreator

}  // namespace Empiria.FinancialAccounting.OfficeIntegration
