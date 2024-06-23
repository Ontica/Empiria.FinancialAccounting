/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                           Component : Excel Exporters                       *
*  Assembly : FinancialAccounting.Reporting.dll            Pattern   : IExcelExporter                        *
*  Type     : BalancesFillOutExcelExporter                 License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Fill out table info for a Microsoft Excel file with valorización information.                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;

using Empiria.Office;
using Empiria.Storage;

using Empiria.FinancialAccounting.BalanceEngine;

using Empiria.FinancialAccounting.Reporting.ValorizacionEstimacionPreventiva.Adapters;

namespace Empiria.FinancialAccounting.Reporting.ValorizacionEstimacionPreventiva.Exporters {

  /// <summary>Fill out table info for a Microsoft Excel file with valorización information.</summary>
  internal class ValorizacionPreventivaExcelExporter : IExcelExporter {

    private const int LAST_COLUMN_INDEX = 48;

    private readonly ReportDataDto _reportData;
    private readonly FileTemplateConfig _template;

    public ValorizacionPreventivaExcelExporter(ReportDataDto reportData, FileTemplateConfig template) {
      Assertion.Require(reportData, "reportData");
      Assertion.Require(template, "template");

      _reportData = reportData;
      _template = template;
    }


    public FileDto CreateExcelFile() {
      var excelFile = new ExcelFile(_template);

      excelFile.Open();

      SetHeader(excelFile);

      FillOutRows(excelFile, _reportData.Entries.Select(x => (ValorizacionPreventivaEntryDto) x));

      excelFile.Save();

      excelFile.Close();

      return excelFile.ToFileDto();
    }


    public void FillOutRows(ExcelFile _excelFile, IEnumerable<ValorizacionPreventivaEntryDto> entries) {

      int i = 5;
      foreach (var entry in entries) {

        _excelFile.SetCell($"A{i}", entry.AccountNumber);
        _excelFile.SetCell($"B{i}", entry.AccountName);
        _excelFile.SetCell($"C{i}", entry.USD);
        _excelFile.SetCell($"D{i}", entry.YEN);
        _excelFile.SetCell($"E{i}", entry.EUR);
        _excelFile.SetCell($"F{i}", entry.UDI);
        _excelFile.SetCell($"G{i}", entry.LastUSD);
        _excelFile.SetCell($"H{i}", entry.LastYEN);
        _excelFile.SetCell($"I{i}", entry.LastEUR);
        _excelFile.SetCell($"J{i}", entry.LastUDI);
        _excelFile.SetCell($"K{i}", entry.ValuedUSD);
        _excelFile.SetCell($"L{i}", entry.ValuedYEN);
        _excelFile.SetCell($"M{i}", entry.ValuedEUR);
        _excelFile.SetCell($"N{i}", entry.ValuedUDI);
        _excelFile.SetCell($"O{i}", entry.ValuedEffectUSD);
        _excelFile.SetCell($"P{i}", entry.ValuedEffectYEN);
        _excelFile.SetCell($"Q{i}", entry.ValuedEffectEUR);
        _excelFile.SetCell($"R{i}", entry.ValuedEffectUDI);
        _excelFile.SetCell($"S{i}", entry.TotalValued);

        DynamicColumns(_excelFile, entry, i);
        SetRowStyleBold(_excelFile, entry, i);
        i++;
      }

    }


    #region Private methods


    static private void DynamicColumns(ExcelFile _excelFile,
                                       ValorizacionPreventivaEntryDto entry, int i) {

      List<string> members = new List<string>();

      members.AddRange(entry.GetDynamicMemberNames());

      int letterNumber = 18, secondLetterNumber = 0;
      string letter = string.Empty;
      string totalAccumulatedColumn = string.Empty;

      foreach (var member in members) {

        string[] memberName = member.Split('_');
        string memberMonth = memberName[0];

        var monthName = MonthName.Where(a => a.Contains(memberMonth)).FirstOrDefault();

        if (monthName != null) {

          if (letterNumber == 25) {

            letter = ExcelColumns[0] + ExcelColumns[secondLetterNumber];
            secondLetterNumber += 1;
            totalAccumulatedColumn = ExcelColumns[0] + ExcelColumns[secondLetterNumber];

          } else {

            letterNumber += 1;
            letter = ExcelColumns[letterNumber];

            if (letterNumber == 25) {
              totalAccumulatedColumn = "AA";

            } else {
              totalAccumulatedColumn = ExcelColumns[letterNumber + 1];
            }
          }

          _excelFile.SetCell($"{letter}4", member);
          _excelFile.SetCell($"{letter + i}", entry.GetTotalField(member));
        }
      }
      _excelFile.SetCell($"{totalAccumulatedColumn}4", "ACUMULADO");
      _excelFile.SetCell($"{totalAccumulatedColumn + i}", entry.TotalAccumulated);
      _excelFile.SetRowFontColorStyle(4, LAST_COLUMN_INDEX, System.Drawing.Color.White);
    }


    private void SetHeader(ExcelFile excelFile) {
      excelFile.SetCell($"A2", _template.Title);

      DateTime fromDate = new DateTime(_reportData.Query.ToDate.Year, 1, 1);

      var subTitle = $"Del {fromDate.ToString("dd/MMM/yyyy")} al " +
                     $"{_reportData.Query.ToDate.ToString("dd/MMM/yyyy")}";

      excelFile.SetCell($"A3", subTitle);
    }


    private void SetRowStyleBold(ExcelFile excelFile, ValorizacionPreventivaEntryDto entry, int i) {

      if (entry.ItemType == TrialBalanceItemType.Summary) {
        excelFile.SetRowBold(i, LAST_COLUMN_INDEX);
      }
    }


    #endregion Private methods


    #region Utility

    static string[] ExcelColumns = {
      "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M",
      "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"
    };


    static string[] MonthName = {
      "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio",
      "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"
    };

    #endregion Utility

  } // class ValorizacionFillOutExcelExporter

} // namespace Empiria.FinancialAccounting.Reporting.Balances
