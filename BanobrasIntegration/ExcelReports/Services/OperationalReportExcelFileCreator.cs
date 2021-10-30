/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                Component : Excel Reports                         *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll  Pattern   : Service                               *
*  Type     : OperationalReportExcelFileCreator                 License   : Please read LICENSE.txt file     *
*                                                                                                            *
*  Summary  : Creates a Microsoft Excel file with operational report from balance information.               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.FinancialAccounting.BanobrasIntegration.OperationalReports;

namespace Empiria.FinancialAccounting.BanobrasIntegration.ExcelReports {

  /// <summary>Creates a Microsoft Excel file with operational report from balance information.</summary>
  internal class OperationalReportExcelFileCreator {

    private OperationalReportCommand _command = new OperationalReportCommand();
    private readonly ExcelTemplateConfig _templateConfig;

    private ExcelFile _excelFile;

    public OperationalReportExcelFileCreator(ExcelTemplateConfig templateConfig) {
      Assertion.AssertObject(templateConfig, "templateConfig");

      _templateConfig = templateConfig;
    }


    internal ExcelFile CreateExcelFile(OperationalReportDto operationalReport) {
      Assertion.AssertObject(operationalReport, "operationalReport");

      _command = operationalReport.Command;

      _excelFile = new ExcelFile(_templateConfig);

      _excelFile.Open();

      SetHeader();

      SetTable(operationalReport);

      _excelFile.Save();

      _excelFile.Close();

      return _excelFile;
    }


    #region Private methods

    private void SetTable(OperationalReportDto operationalReport) {
      switch (_command.ReportType) {
        case OperationalReportType.BalanzaSAT:
          FillOutBalanzaSat(operationalReport.Entries.Select(x => (OperationalReportEntryDto) x));
          return;

        case OperationalReportType.CatalogoSAT:
          FillOutCatalogoDeCuentaSat(operationalReport.Entries.Select(x => (OperationalReportEntryDto) x));
          return;

      }
    }


    private void SetHeader() {
      _excelFile.SetCell($"A2", _templateConfig.Title);

      var subTitle = _command.ReportType == OperationalReportType.BalanzaSAT ?
                      $"Del { new DateTime(_command.Date.Year, _command.Date.Month, 1).ToString("dd/MMM/yyyy")} "
                      : "" +
                     $"Al {_command.Date.ToString("dd/MMM/yyyy")}";

      _excelFile.SetCell($"A3", subTitle);
    }

    private void FillOutBalanzaSat(IEnumerable<OperationalReportEntryDto> entries) {
      int i = 5;

      foreach (var entry in entries) {
        _excelFile.SetCell($"A{i}", entry.AccountNumber);
        _excelFile.SetCell($"B{i}", entry.InitialBalance);
        _excelFile.SetCell($"C{i}", entry.Debit);
        _excelFile.SetCell($"D{i}", entry.Credit);
        _excelFile.SetCell($"E{i}", entry.CurrentBalance);

        i++;
      }
    }

    private void FillOutCatalogoDeCuentaSat(IEnumerable<OperationalReportEntryDto> entries) {
      int i = 5;

      foreach (var entry in entries) {
        _excelFile.SetCell($"A{i}", entry.GroupingCode);
        _excelFile.SetCell($"B{i}", entry.AccountNumber);
        _excelFile.SetCell($"C{i}", entry.AccountName);
        _excelFile.SetCell($"D{i}", entry.AccountParent);
        _excelFile.SetCell($"E{i}", entry.AccountLevel);
        _excelFile.SetCell($"E{i}", entry.Naturaleza);

        i++;
      }
    }

    #endregion

  } // class OperationalReportExcelFileCreator

} // Empiria.FinancialAccounting.BanobrasIntegration.ExcelReports
