/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                           Component : Excel Exporters                       *
*  Assembly : FinancialAccounting.Reporting.dll            Pattern   : IExcelExporter                        *
*  Type     : AccountsChartExcelExporter                   License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Main service used to export charts of accounts to Microsoft Excel.                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.Adapters;

namespace Empiria.FinancialAccounting.Reporting.Exporters.Excel {

  /// <summary>Main service used to export charts of accounts to Microsoft Excel.</summary>
  internal class AccountsChartExcelExporter {

    private readonly ExcelTemplateConfig _templateConfig;

    public AccountsChartExcelExporter(ExcelTemplateConfig templateConfig) {
      Assertion.AssertObject(templateConfig, "templateConfig");

      _templateConfig = templateConfig;
    }


    internal ExcelFile CreateExcelFile(AccountsChartDto accountsChart) {
      Assertion.AssertObject(accountsChart, "accountsChart");

      var excelFile = new ExcelFile(_templateConfig);

      excelFile.Open();

      excelFile.SetCell($"A2", _templateConfig.Title);

      FillOut(accountsChart, excelFile);

      excelFile.Save();

      excelFile.Close();

      return excelFile;
    }

    #region Private methods


    private void FillOut(AccountsChartDto accountsChart, ExcelFile excelFile) {
      int i = 5;
      bool hasEndDateData = false;

      foreach (var account in accountsChart.Accounts) {
        excelFile.SetCell($"C{i}", account.Number);
        excelFile.SetCell($"D{i}", account.Name);
        if (account.LastLevel) {
          excelFile.SetCell($"E{i}", "*");
        }
        excelFile.SetCell($"F{i}", account.Sector);
        excelFile.SetCell($"G{i}", account.Role.ToString());
        excelFile.SetCell($"H{i}", account.UsesSector ? "Sí": "No");
        excelFile.SetCell($"I{i}", account.UsesSubledger ? "Sí" : "No");
        excelFile.SetCell($"J{i}", account.Type);
        excelFile.SetCell($"K{i}", account.DebtorCreditor.ToString());
        excelFile.SetCell($"L{i}", account.StartDate);

        if (account.EndDate < Account.MAX_END_DATE) {
          excelFile.SetCell($"M{i}", account.EndDate);
          hasEndDateData = true;
        }

        if (account.SummaryWithNotChildren) {
          excelFile.SetCellStyleLineThrough($"D{i}");
          excelFile.SetCell($"N{i}", "Sumaria sin hijas");
        }

        i++;
      }

      if (!accountsChart.WithSectors) {
        excelFile.RemoveColumn("F");
      }
      if (!hasEndDateData) {
        excelFile.RemoveColumn("M");
      }
    }

    #endregion Private methods

  }  // class AccountsChartExcelExporter

}  // namespace namespace Empiria.FinancialAccounting.Reporting.Exporters.Excel
