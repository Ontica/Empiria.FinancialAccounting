/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                            Component : Service Layer                        *
*  Assembly : FinancialAccounting.Reporting.dll             Pattern   : Service provider                     *
*  Type     : FinancialReportTextFileExporter               License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Provides services used to export financial reports to text-based files.                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.FinancialAccounting.FinancialReports;
using Empiria.FinancialAccounting.FinancialReports.Adapters;

using Empiria.FinancialAccounting.Reporting.Exporters.Text;

namespace Empiria.FinancialAccounting.Reporting.FinancialReports.Exporters {

  /// <summary>Provides services used to export financial reports to text-based files.</summary>
  internal class FinancialReportTextFileExporter : IFinancialReportBuilder {


    public FileReportDto Build(FinancialReportDto financialReportDto) {
      Assertion.Require(financialReportDto, "financialReportDto");

      return CreateTextFile(financialReportDto)
            .ToFileReportDto();
    }


    private TextFile CreateTextFile(FinancialReportDto financialReport) {
      var _textFile = new TextFile("siti.csv");

      FixedList<string> textLines = GetBodyLines(financialReport);

      _textFile.AppendLines(textLines);

      _textFile.Save();

      return _textFile;
    }


    private FixedList<string> GetBodyLines(FinancialReportDto financialReport) {
      var financialReportType = financialReport.Query.GetFinancialReportType();

      ExportTo exportTo = financialReportType.GetExportToConfig(financialReport.Query.ExportTo);

      var textLines = new List<string>(financialReport.Entries.Count * 2);

      foreach (var entry in financialReport.Entries) {
        var convertedEntry = (FinancialReportEntryDto) entry;

        string textLine = GetTextLine(exportTo, convertedEntry,
                                      FinancialReportTotalField.monedaNacional, "14");

        if (textLine.Length > 0) {
          textLines.Add(textLine);
        }
      }

      foreach (var entry in financialReport.Entries) {
        var convertedEntry = (FinancialReportEntryDto) entry;

        string textLine = GetTextLine(exportTo, convertedEntry,
                                      FinancialReportTotalField.monedaExtranjera, "4");

        if (textLine.Length > 0) {
          textLines.Add(textLine);
        }
      }

      return textLines.ToFixedList();
    }


    private string GetTextLine(ExportTo exportTo, FinancialReportEntryDto entry,
                               FinancialReportTotalField totalField, string currencyCode) {
      switch (exportTo.CsvBuilder) {
        case "ACM_SAIF_Banxico":
          return TextLine_ACM_SAIF_Banxico(entry, totalField, currencyCode);

        case "R01_SITI_CNBV":
          return TextLine_R01_SITI_CNBV(entry, totalField, currencyCode);

        case "R01_SAIF_Banxico":
          return TextLine_R01_SAIF_Banxico(entry, totalField, currencyCode);

        default:
          throw Assertion.EnsureNoReachThisCode($"Unrecognized CSV Builder {exportTo.CsvBuilder}.");
      }
    }


    private string TextLine_ACM_SAIF_Banxico(FinancialReportEntryDto entry,
                                             FinancialReportTotalField totalField,
                                             string currencyCode) {
      if (entry.GetTotalField(totalField) == 0) {
        return String.Empty;
      }

      return $"{entry.ConceptCode};;{currencyCode};" +
             $"{entry.GetTotalField(totalField).ToString("F0")}";
    }


    private string TextLine_R01_SAIF_Banxico(FinancialReportEntryDto entry,
                                             FinancialReportTotalField totalField,
                                             string currencyCode) {

      if (entry.GetTotalField(totalField) == 0) {
        return String.Empty;
      }

      return $"{entry.ConceptCode};111;{currencyCode};" +
             $"{entry.GetTotalField(totalField).ToString("F0")}";
    }


    private string TextLine_R01_SITI_CNBV(FinancialReportEntryDto entry,
                                          FinancialReportTotalField totalField,
                                          string currencyCode) {
      return $"111;{entry.ConceptCode};{currencyCode};" +
             $"{entry.GetTotalField(totalField).ToString("F2")}";
    }

  }  //class FinancialReportTextFileExporter

}  // namespace Empiria.FinancialAccounting.Reporting.FinancialReports.Exporters
