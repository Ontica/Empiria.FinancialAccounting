﻿/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                Component : Excel Reports                         *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll  Pattern   : Service                               *
*  Type     : FinancialReportExcelFileCreator              License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Creates a Microsoft Excel file with a financial report.                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.FinancialReports;
using Empiria.FinancialAccounting.FinancialReports.Adapters;

namespace Empiria.FinancialAccounting.BanobrasIntegration.ExcelReports {

  /// <summary>Creates a Microsoft Excel file with a financial report.</summary>
  internal class FinancialReportExcelFileCreator {

    private readonly ExcelTemplateConfig _templateConfig;
    private ExcelFile _excelFile;

    public FinancialReportExcelFileCreator(ExcelTemplateConfig templateConfig) {
      Assertion.AssertObject(templateConfig, "templateConfig");

      _templateConfig = templateConfig;
    }


    internal ExcelFile CreateExcelFile(FinancialReportDto financialReport) {
      Assertion.AssertObject(financialReport, "financialReport");

      _excelFile = new ExcelFile(_templateConfig);

      _excelFile.Open();

      SetHeader(financialReport.Command);

      SetTable(financialReport);

      _excelFile.Save();

      _excelFile.Close();

      return _excelFile;
    }


    #region Private methods

    private void SetHeader(FinancialReportCommand command) {
      switch (command.FinancialReportType) {
        case FinancialReportType.R01:
          _excelFile.SetCell($"A1",
                $"Fecha {command.Date.ToString("dd/MMM/yyyy")} " +
                 "Regulatorio R01  Reporte B 0111  Subreporte 1");
          return;

        default:
          throw Assertion.AssertNoReachThisCode();
      }
    }


    private void SetTable(FinancialReportDto financialReport) {
      switch (financialReport.Command.FinancialReportType) {
        case FinancialReportType.R01:
          FillOutR01(financialReport.Entries);
          return;

        default:
          throw Assertion.AssertNoReachThisCode();
      }
    }

    private void FillOutR01(FixedList<FinancialReportEntryDto> entries) {
      int i = 8;

      foreach (var entry in entries) {
        _excelFile.SetCell($"A{i}", entry.ConceptCode);
        _excelFile.SetCell($"B{i}", entry.Concept);
        _excelFile.SetCell($"C{i}", entry.DomesticCurrencyTotal);
        _excelFile.SetCell($"D{i}", entry.ForeignCurrencyTotal);
        _excelFile.SetCell($"E{i}", entry.Total);
        i++;
      }
    }

    #endregion Private methods

  }  // class FinancialReportExcelFileCreator

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.ExcelReports
