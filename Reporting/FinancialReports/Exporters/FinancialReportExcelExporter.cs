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

namespace Empiria.FinancialAccounting.Reporting.FinancialReports.Exporters {

  /// <summary>Creates a Microsoft Excel file with a financial report.</summary>
  internal class FinancialReportExcelExporter : IFinancialReportBuilder {

    private readonly FileTemplateConfig _templateConfig;
    private ExcelFile _excelFile;

    public FinancialReportExcelExporter(FileTemplateConfig templateConfig) {
      Assertion.Require(templateConfig, "templateConfig");

      _templateConfig = templateConfig;
    }


    public FileReportDto Build(FinancialReportDto financialReportDto) {
      Assertion.Require(financialReportDto, "financialReportDto");

      return CreateExcelFile(financialReportDto)
            .ToFileReportDto();
    }


    #region Private methods

    private ExcelFile CreateExcelFile(FinancialReportDto financialReport) {
      _excelFile = new ExcelFile(_templateConfig);

      _excelFile.Open();

      SetHeader(financialReport.Query);

      SetTable(financialReport);

      _excelFile.Save();

      _excelFile.Close();

      return _excelFile;
    }


    private void SetHeader(FinancialReportQuery buildQuery) {
      FinancialReportType reportType = buildQuery.GetFinancialReportType();

      switch (reportType.DesignType) {
        case FinancialReportDesignType.FixedRows:

          if (_templateConfig.TitleCell.Length != 0) {
            _excelFile.SetCell(_templateConfig.TitleCell, buildQuery.GetFinancialReportType().Title);
          }
          _excelFile.SetCell(_templateConfig.CurrentTimeCell, DateTime.Now);
          _excelFile.SetCell(_templateConfig.ReportDateCell, buildQuery.ToDate);

          return;

        case FinancialReportDesignType.AccountsIntegration:

          _excelFile.SetCell($"A2", buildQuery.GetFinancialReportType().BaseReport.Name);
          _excelFile.SetCell($"I2", DateTime.Now);
          _excelFile.SetCell($"I3", buildQuery.ToDate);

          return;

        default:
          return;
      }
    }


    private void SetTable(FinancialReportDto financialReport) {

      FinancialReportType reportType = financialReport.Query.GetFinancialReportType();

      if (reportType.DesignType == FinancialReportDesignType.FixedRows &&
          reportType.ConsecutiveRows) {

        FillOutFinancialReportConsecutiveRows(financialReport);

      } else if (reportType.DesignType == FinancialReportDesignType.FixedRows &&
                !reportType.ConsecutiveRows) {

        FillOutFinancialReportSeparatedRows(financialReport);

      } else if (reportType.DesignType == FinancialReportDesignType.FixedCells) {

        FillOutFinancialReportCells(financialReport);

      } else if (reportType.DesignType == FinancialReportDesignType.AccountsIntegration) {

        FillOutConceptsIntegrationReport(financialReport);

      } else {

        throw Assertion.EnsureNoReachThisCode("Unhandled option in method SetTable().");

      }
    }


    private void FillOutFinancialReportCells(FinancialReportDto financialReport) {
      throw new NotImplementedException();
    }


    private void FillOutFinancialReportConsecutiveRows(FinancialReportDto financialReport) {

      var rowFiller = new FinancialReportRowFiller(financialReport.Columns, _excelFile);

      var rowEntries = financialReport.Entries.Select(x => (FinancialReportEntryDto) x)
                                              .ToFixedList()
                                              .FindAll(x => x.ReportEntryType == FinancialReportEntryType.Row);

      int rowIndex = _templateConfig.FirstRowIndex;

      foreach (var entry in rowEntries) {
        var row = FinancialReportRow.Parse(entry.UID);

        rowFiller.FillOutRow(row, entry, rowIndex);

        rowIndex++;
      }
    }


    private void FillOutFinancialReportSeparatedRows(FinancialReportDto financialReport) {

      var rowFiller = new FinancialReportRowFiller(financialReport.Columns, _excelFile);

      var rowEntries = financialReport.Entries.Select(x => (FinancialReportEntryDto) x)
                                              .ToFixedList()
                                              .FindAll(x => x.ReportEntryType == FinancialReportEntryType.Row);

      foreach (var entry in rowEntries) {
        var row = FinancialReportRow.Parse(entry.UID);

        rowFiller.FillOutRow(row, entry);
      }
    }


    private void FillOutConceptsIntegrationReport(FinancialReportDto financialReport) {
      int i = 5;

      var integrationEntries = financialReport.Entries.Select(x => (IntegrationReportEntryDto) x)
                                                      .ToFixedList();

      foreach (var entry in integrationEntries) {

        _excelFile.SetCell($"A{i}", entry.ConceptCode);
        _excelFile.SetCell($"B{i}", entry.Concept);
        _excelFile.SetCell($"C{i}", entry.ItemCode);
        _excelFile.SetCell($"D{i}", entry.ItemName);
        _excelFile.SetCell($"E{i}", entry.SectorCode);
        _excelFile.SetCell($"F{i}", entry.Operator);

        foreach (var totalColumn in financialReport.Columns.FindAll(x => x.Type == "decimal")) {

          decimal totalField = entry.GetTotalField(totalColumn.Field);

          _excelFile.SetCell($"{totalColumn.Column}{i}", totalField);
        }

        if (entry.ItemType == FinancialReportItemType.Summary) {
          _excelFile.SetRowStyleBold(i);
        }

        i++;
      }
    }

    #endregion Private methods

  }  // class FinancialReportExcelExporter

}  // namespace Empiria.FinancialAccounting.Reporting.FinancialReports.Exporters
