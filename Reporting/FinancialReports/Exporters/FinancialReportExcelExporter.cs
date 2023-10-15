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
using System.Globalization;

using Empiria.Office;
using Empiria.Storage;

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
      if (_templateConfig.TitleCell.Length != 0) {
        _excelFile.SetCell(_templateConfig.TitleCell, _templateConfig.Title);
      }

      if (_templateConfig.SubTitleCell.Length != 0) {
        _excelFile.SetCell(_templateConfig.SubTitleCell, buildQuery.GetFinancialReportType().Name);
      }

      if (_templateConfig.CurrentTimeCell.Length != 0) {
        _excelFile.SetCell(_templateConfig.CurrentTimeCell, DateTime.Now);
      }

      if (_templateConfig.ReportDateCell.Length != 0) {
        _excelFile.SetCell(_templateConfig.ReportDateCell, buildQuery.ToDate);
      }

      if (_templateConfig.ReportDateFormat.Length != 0) {

        CultureInfo esUS = new CultureInfo("es-US");

        var formattedDate = _templateConfig.ReportDateFormat.Replace("{{LONG_DATE}}",
                                                                     buildQuery.ToDate.ToString("dd \\DE MMMM \\DE yyyy", esUS))
                                                            .ToUpper();
        _excelFile.SetCell(_templateConfig.ReportDateCell, formattedDate);
      }


      if (_templateConfig.ReportFooterCell.Length != 0) {

        CultureInfo esUS = new CultureInfo("es-US");

        var formattedDate = _templateConfig.ReportFooterFormat.Replace("{{LONG_DATE}}",
                                                                       buildQuery.ToDate.ToString("dd \\de MMMM \\de yyyy", esUS));

        _excelFile.SetCell(_templateConfig.ReportFooterCell, formattedDate);
      }

    }


    private void SetTable(FinancialReportDto financialReport) {

      FinancialReportType reportType = financialReport.Query.GetFinancialReportType();

      if (reportType.DesignType == FinancialReportDesignType.FixedRows &&
          reportType.ConsecutiveRows) {

        FillOutFinancialReportConsecutiveRows(financialReport);
        FillOutFinancialReportCells(financialReport);

      } else if (reportType.DesignType == FinancialReportDesignType.FixedRows &&
                !reportType.ConsecutiveRows) {

        FillOutFinancialReportSeparatedRows(financialReport);
        FillOutFinancialReportCells(financialReport);

      } else if (reportType.DesignType == FinancialReportDesignType.FixedCells) {

        FillOutFinancialReportCells(financialReport);

      } else if (reportType.DesignType == FinancialReportDesignType.AccountsIntegration) {

        FillOutConceptsIntegrationReport(financialReport);

      } else {

        throw Assertion.EnsureNoReachThisCode("Unhandled option in method SetTable().");

      }
    }


    private void FillOutFinancialReportCells(FinancialReportDto financialReport) {

      var cellFiller = new FinancialReportCellFiller(financialReport.Columns, _excelFile);

      var cellEntries = financialReport.Entries.Select(x => (FinancialReportEntryDto) x)
                                               .ToFixedList()
                                               .FindAll(x => x.ReportEntryType == FinancialReportEntryType.Cell);

      foreach (var entry in cellEntries) {
        var cell = FinancialReportCell.Parse(entry.UID);

        cellFiller.FillOutCell(cell, entry);
      }
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
