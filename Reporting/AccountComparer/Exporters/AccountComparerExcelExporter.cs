/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                           Component : Excel Exporters                       *
*  Assembly : FinancialAccounting.Reporting.dll            Pattern   : IExcelExporter                        *
*  Type     : AccountComparerExcelExporter                 License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Genera los datos del comparativo de cuentas en un archivo Excel.                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.Office;
using Empiria.Storage;

using Empiria.FinancialAccounting.Reporting.AccountComparer.Adapters;

namespace Empiria.FinancialAccounting.Reporting.AccountComparer.Exporters {

  /// <summary>Genera los datos del comparativo de cuentas en un archivo Excel.</summary>
  internal class AccountComparerExcelExporter : IExcelExporter {

    private readonly ReportDataDto _reportData;
    private readonly FileTemplateConfig _template;

    public AccountComparerExcelExporter(ReportDataDto reportData, FileTemplateConfig template) {
      Assertion.Require(reportData, "reportData");
      Assertion.Require(template, "template");

      _reportData = reportData;
      _template = template;
    }


    public FileReportDto CreateExcelFile() {
      var excelFile = new ExcelFile(_template);

      excelFile.Open();

      SetHeader(excelFile);

      FillOutRows(excelFile, _reportData.Entries.Select(x => (AccountComparerEntryDto) x));

      excelFile.Save();

      excelFile.Close();

      return excelFile.ToFileReportDto();
    }


    #region Private methods

    private void FillOutRows(ExcelFile excelFile, IEnumerable<AccountComparerEntryDto> comparers) {
      int i = 5;

      foreach (var comparer in comparers) {

        excelFile.SetCell($"A{i}", comparer.CurrencyCode ?? string.Empty);
        excelFile.SetCell($"B{i}", comparer.ActiveAccount ?? string.Empty);
        excelFile.SetCell($"C{i}", comparer.ActiveAccountName ?? string.Empty);
        excelFile.SetCell($"D{i}", comparer.ActiveBalance);
        excelFile.SetCell($"E{i}", comparer.PasiveAccount ?? string.Empty);
        excelFile.SetCell($"F{i}", comparer.PasiveAccountName ?? string.Empty);
        excelFile.SetCell($"G{i}", comparer.PasiveBalance);
        excelFile.SetCell($"H{i}", comparer.BalanceDifference);

        if (comparer.ItemType == BalanceEngine.TrialBalanceItemType.Total) {
          excelFile.SetRowStyleBold(i);
        }
        i++;

      }
    }


    private void SetHeader(ExcelFile excelFile) {
      excelFile.SetCell($"A2", _template.Title);

      var subTitle = $"Del {_reportData.Query.FromDate.ToString("dd/MMM/yyyy")} al " +
                     $"{_reportData.Query.ToDate.ToString("dd/MMM/yyyy")}";

      excelFile.SetCell($"A3", subTitle);
    }

    #endregion Private methods


  } // class AccountComparerExcelExporter

} // namespace Empiria.FinancialAccounting.Reporting.AccountComparer.Exporters
