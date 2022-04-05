/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                           Component : Excel Exporters                       *
*  Assembly : FinancialAccounting.Reporting.dll            Pattern   : IExcelExporter                        *
*  Type     : FinancialReportExcelExporter                 License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Creates a Microsoft Excel file with a financial report.                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.FinancialReports;
using Empiria.FinancialAccounting.FinancialReports.Adapters;

namespace Empiria.FinancialAccounting.Reporting.Exporters.Excel {

  /// <summary>Creates a Microsoft Excel file with a financial report.</summary>
  internal class FinancialReportExcelExporter : IFinancialReportBuilder {

    private readonly FileTemplateConfig _templateConfig;
    private ExcelFile _excelFile;

    public FinancialReportExcelExporter(FileTemplateConfig templateConfig) {
      Assertion.AssertObject(templateConfig, "templateConfig");

      _templateConfig = templateConfig;
    }


    public FileReportDto Build(FinancialReportDto financialReportDto) {
      Assertion.AssertObject(financialReportDto, "financialReportDto");

      return CreateExcelFile(financialReportDto)
            .ToFileReportDto();
    }


    #region Private methods

    private ExcelFile CreateExcelFile(FinancialReportDto financialReport) {
      _excelFile = new ExcelFile(_templateConfig);

      _excelFile.Open();

      SetHeader(financialReport.Command);

      SetTable(financialReport);

      _excelFile.Save();

      _excelFile.Close();

      return _excelFile;
    }


    private void SetHeader(FinancialReportCommand command) {
      FinancialReportType reportType = command.GetFinancialReportType();

      switch (reportType.DesignType) {
        case FinancialReportDesignType.FixedRows:
          _excelFile.SetCell($"A1",
                             $"Fecha {command.Date.ToString("dd/MMM/yyyy")} " +
                              "Regulatorio R01  Reporte B 0111  Subreporte 1");
          return;

        case FinancialReportDesignType.AccountsIntegration:

          _excelFile.SetCell($"A2", command.GetFinancialReportType().BaseReport.Name);
          _excelFile.SetCell($"I2", DateTime.Now);
          _excelFile.SetCell($"I3", command.Date);

          return;

        default:
          return;
      }
    }


    private void SetTable(FinancialReportDto financialReport) {
      FinancialReportType reportType = financialReport.Command.GetFinancialReportType();


      switch (reportType.DesignType) {
        case FinancialReportDesignType.FixedRows:

          var entries = new FixedList<FinancialReportEntryDto>(financialReport.Entries.Select(x => (FinancialReportEntryDto) x));

          FillOutFixedRowsReport(entries);
          return;

        case FinancialReportDesignType.AccountsIntegration:

          var entries2 = new FixedList<IntegrationReportEntryDto>(financialReport.Entries.Select(x => (IntegrationReportEntryDto) x));

          FillOutConceptsIntegrationReport(entries2);
          return;

        default:
          throw Assertion.AssertNoReachThisCode();
      }
    }


    private void FillOutFixedRowsReport(FixedList<FinancialReportEntryDto> entries) {
      int i = 8;

      foreach (var entry in entries) {
        _excelFile.SetCell($"A{i}", entry.ConceptCode.Replace(" ", string.Empty));
        _excelFile.SetCell($"B{i}", entry.Concept);
        _excelFile.SetCell($"C{i}", entry.GetTotalField(FinancialReportTotalField.DomesticCurrencyTotal));
        _excelFile.SetCell($"D{i}", entry.GetTotalField(FinancialReportTotalField.ForeignCurrencyTotal));
        _excelFile.SetCell($"E{i}", entry.GetTotalField(FinancialReportTotalField.Total));
        i++;
      }
    }


    private void FillOutConceptsIntegrationReport(FixedList<IntegrationReportEntryDto> entries) {
      int i = 5;

      foreach (var entry in entries) {
        _excelFile.SetCell($"A{i}", entry.ConceptCode.Replace(" ", string.Empty));
        _excelFile.SetCell($"B{i}", entry.Concept);
        _excelFile.SetCell($"C{i}", entry.ItemCode);
        _excelFile.SetCell($"D{i}", entry.ItemName);
        _excelFile.SetCell($"E{i}", entry.SectorCode);
        _excelFile.SetCell($"F{i}", entry.Operator);
        _excelFile.SetCell($"G{i}", entry.GetTotalField(FinancialReportTotalField.DomesticCurrencyTotal));
        _excelFile.SetCell($"H{i}", entry.GetTotalField(FinancialReportTotalField.ForeignCurrencyTotal));
        _excelFile.SetCell($"I{i}", entry.GetTotalField(FinancialReportTotalField.Total));
        i++;
      }
    }

    #endregion Private methods

  }  // class FinancialReportExcelExporter

}  // namespace Empiria.FinancialAccounting.Reporting.Exporters.Excel
