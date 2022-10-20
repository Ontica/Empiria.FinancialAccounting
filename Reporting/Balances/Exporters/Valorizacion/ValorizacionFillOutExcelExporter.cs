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
        _excelFile.SetCell($"P{i}", entry.ValuedEffectEUR);
        _excelFile.SetCell($"P{i}", entry.ValuedEffectUDI);


        //SetRowClauses(_excelFile, entry, utility, i);
        SetRowStyleBold(_excelFile, entry, i);
        i++;
      }

      //_excelFile.RemoveColumn("");
    }


    #region Private methods

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
