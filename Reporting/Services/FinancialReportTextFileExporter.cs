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

namespace Empiria.FinancialAccounting.Reporting {

  /// <summary>Provides services used to export financial reports to text-based files.</summary>
  internal class FinancialReportTextFileExporter : IFinancialReportBuilder {


    public FileReportDto Build(FinancialReportDto financialReportDto) {
      Assertion.AssertObject(financialReportDto, "financialReportDto");

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
      string exportTo = financialReport.Command.ExportTo;

      var textLines = new List<string>(financialReport.Entries.Count * 2);

      foreach (var entry in financialReport.Entries) {
        var convertedEntry = (FinancialReportEntryDto) entry;

        string textLine = GetTextLineCNBV(convertedEntry, FinancialReportTotalField.DomesticCurrencyTotal, "14");

        if (textLine.Length > 0) {
          textLines.Add(textLine);
        }
      }

      foreach (var entry in financialReport.Entries) {
        var convertedEntry = (FinancialReportEntryDto) entry;

        string textLine = GetTextLineCNBV(convertedEntry, FinancialReportTotalField.ForeignCurrencyTotal, "4");

        if (textLine.Length > 0) {
          textLines.Add(textLine);
        }
      }

      return textLines.ToFixedList();
    }

    //private string GetTextLine(string exportTo, FinancialReportEntryDto convertedEntry, FinancialReportTotalField domesticCurrencyTotal, string currencyCode) {
    //  switch (exportTo) {
    //    case "SITI-CNBV":
    //      return
    //    case "SITI-Banxico":

    //    case "SITI-ACM":

    //  }
    //}

    private string GetTextLineBanxico(FinancialReportEntryDto convertedEntry,
                                      FinancialReportTotalField totalField,
                                      string currencyCode) {
      if (convertedEntry.GetTotalField(totalField) == 0) {
        return String.Empty;
      }

      return $"{convertedEntry.ConceptCode.Replace(" ", string.Empty)};;{currencyCode};" +
             $"{convertedEntry.GetTotalField(totalField).ToString("F0")}";
    }


    private string GetTextLineACMBanxico(FinancialReportEntryDto convertedEntry,
                                         FinancialReportTotalField totalField,
                                         string currencyCode) {

      if (convertedEntry.GetTotalField(totalField) == 0) {
        return String.Empty;
      }

      return $"{convertedEntry.ConceptCode.Replace(" ", string.Empty)};111;{currencyCode};" +
             $"{convertedEntry.GetTotalField(totalField).ToString("F0")}";
    }


    private string GetTextLineCNBV(FinancialReportEntryDto convertedEntry,
                                   FinancialReportTotalField totalField,
                                   string currencyCode) {
      return $"111;{convertedEntry.ConceptCode.Replace(" ", string.Empty)};{currencyCode};" +
             $"{convertedEntry.GetTotalField(totalField).ToString("F2")}";
    }

  }  //class FinancialReportTextFileExporter

}  // namespace Empiria.FinancialAccounting.Reporting
