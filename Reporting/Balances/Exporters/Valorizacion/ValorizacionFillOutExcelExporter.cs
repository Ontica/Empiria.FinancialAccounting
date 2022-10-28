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
using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.Reporting.Balances {

  /// <summary>Fill out table info for a Microsoft Excel file with valorización information.</summary>
  internal class ValorizacionFillOutExcelExporter {

    private TrialBalanceQuery _query;

    public ValorizacionFillOutExcelExporter(TrialBalanceQuery query) {
      _query = query;
    }


    public void FillOutValorizacion(ExcelFile _excelFile, IEnumerable<ValorizacionEntryDto> entries) {

      var utility = new UtilityToFillOutExcelExporter(_query);

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
        _excelFile.SetCell($"K{i}", entry.CurrentUSD);
        _excelFile.SetCell($"L{i}", entry.CurrentYEN);
        _excelFile.SetCell($"M{i}", entry.CurrentEUR);
        _excelFile.SetCell($"N{i}", entry.CurrentUDI);
        _excelFile.SetCell($"O{i}", entry.ValuedEffectUSD);
        _excelFile.SetCell($"P{i}", entry.ValuedEffectYEN);
        _excelFile.SetCell($"Q{i}", entry.ValuedEffectEUR);
        _excelFile.SetCell($"R{i}", entry.ValuedEffectUDI);
        _excelFile.SetCell($"S{i}", entry.TotalValued);

        //DynamicColumns(_excelFile, entry, i);

        SetRowStyleBold(_excelFile, entry, i);
        i++;
      }

    }


    #region Private methods


    static private void DynamicColumns(ExcelFile _excelFile, ValorizacionEntryDto entry, int i) {

      List<string> members = new List<string>();

      members.AddRange(entry.GetDynamicMemberNames());
      
      foreach (var member in members) {
        //for (int index = 0; index <= ExcelColumns.Length; index++) {
        //  ExcelColumns[index]
        //}
        var b = entry.GetTotalField(member);
        _excelFile.SetCell($"{i}", entry.GetTotalField(member));
      }
    }


    static string[] ExcelColumns = {
      "T", "U", "V", "W", "X", "Y", "Z"
    };


    private void SetRowStyleBold(ExcelFile excelFile, ValorizacionEntryDto entry, int i) {

      if (entry.ItemType == TrialBalanceItemType.Summary) {
        excelFile.SetRowStyleBold(i);
      }
    }


    private void SetRowClauses(ExcelFile excelFile, ValorizacionEntryDto entry,
                                UtilityToFillOutExcelExporter utility, int i) {
      throw new NotImplementedException();
    }

    #endregion Private methods


  } // class ValorizacionFillOutExcelExporter

} // namespace Empiria.FinancialAccounting.Reporting.Balances
