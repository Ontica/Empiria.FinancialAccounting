/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                           Component : Excel Exporters                       *
*  Assembly : FinancialAccounting.Reporting.dll            Pattern   : IExcelExporter                        *
*  Type     : ExchangeRateExcelExporter                    License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Main service used to export exchange rate to Microsoft Excel.                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Office;
using Empiria.Storage;

using Empiria.FinancialAccounting.Adapters;

namespace Empiria.FinancialAccounting.Reporting.Exporters.Excel {

  /// <summary>Main service used to export exchange rate to Microsoft Excel.</summary>
  internal class ExchangeRateExcelExporter {

    private readonly FileTemplateConfig _templateConfig;

    private ExcelFile excelFile;

    public ExchangeRateExcelExporter(FileTemplateConfig templateConfig) {
      Assertion.Require(templateConfig, "templateConfig");

      _templateConfig = templateConfig;
    }


    internal ExcelFile CreateExcelFile(FixedList<ExchangeRateDescriptorDto> exchangeRates) {
      Assertion.Require(exchangeRates, "exchangeRates");

      excelFile = new ExcelFile(_templateConfig);

      excelFile.Open();

      SetHeader();

      FillOut(exchangeRates, excelFile);

      excelFile.Save();

      excelFile.Close();

      return excelFile;
    }



    #region Private methods

    private void FillOut(FixedList<ExchangeRateDescriptorDto> exchangeRates, ExcelFile excelFile) {
      int i = 5;

      foreach (var exchangeRate in exchangeRates) {
        excelFile.SetCell($"A{i}", exchangeRate.Date);
        excelFile.SetCell($"B{i}", exchangeRate.ExchangeRateType);
        excelFile.SetCell($"C{i}", exchangeRate.Currency);
        excelFile.SetCell($"D{i}", exchangeRate.Value);
        i++;
      }

    }


    private void SetHeader() {
      excelFile.SetCell($"A2", _templateConfig.Title);

      var subTitle = $"Fecha de consulta {DateTime.Now:dd/MMM/yyyy} ";

      excelFile.SetCell($"A3", subTitle);
    }

    #endregion Private methods


  } // class ExchangeRateExcelExporter

} // namespace Empiria.FinancialAccounting.Reporting.Exporters.Excel
