/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                           Component : Excel Exporters                       *
*  Assembly : FinancialAccounting.Reporting.dll            Pattern   : IExcelExporter                        *
*  Type     : SaldosFillOutExcelExporter                   License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Fill out table info for a Microsoft Excel file with saldos information.                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.Reporting {

  /// <summary>Fill out table info for a Microsoft Excel file with saldos information.</summary>
  internal class SaldosFillOutExcelExporter {

    private TrialBalanceQuery _query;

    private readonly DateTime MIN_LAST_CHANGE_DATE_TO_REPORT = DateTime.Parse("01/01/1970");

    public SaldosFillOutExcelExporter(TrialBalanceQuery query) {
      _query = query;
    }


    #region Public methods

    public void FillOutSaldosPorAuxiliar(ExcelFile _excelFile,
                                      IEnumerable<SaldosPorAuxiliarEntryDto> entries) {

      int i = 5;
      foreach (var entry in entries) {
        var account = StandardAccount.Parse(entry.StandardAccountId);

        SetRowByItemType(_excelFile, entry, i);

        if (!account.IsEmptyInstance) {
          _excelFile.SetCell($"E{i}", account.Number);
          _excelFile.SetCell($"F{i}", account.Name);

        } else {
          _excelFile.SetCell($"E{i}", entry.AccountNumber);
          _excelFile.SetCell($"F{i}", entry.AccountName);

        }

        _excelFile.SetCell($"G{i}", entry.SectorCode);
        _excelFile.SetCell($"I{i}", entry.DebtorCreditor);

        if (entry.LastChangeDate != ExecutionServer.DateMaxValue &&
            entry.LastChangeDate >= MIN_LAST_CHANGE_DATE_TO_REPORT) {
          _excelFile.SetCell($"K{i}", entry.LastChangeDate.ToString("dd/MMM/yyyy"));
        }

        if (entry.ItemType == TrialBalanceItemType.Summary ||
            entry.ItemType == TrialBalanceItemType.Total) {
          _excelFile.SetRowStyleBold(i);
        }

        if (entry.ItemType == TrialBalanceItemType.Total) {
          i++;
        }
        i++;

      } // foreach

      if (!_query.WithAverageBalance) {
        _excelFile.RemoveColumn("J");
      }
      if (!_query.ShowCascadeBalances) {
        _excelFile.RemoveColumn("B");
      }
    }


    public void FillOutSaldosPorCuenta(ExcelFile _excelFile,
                                    IEnumerable<SaldosPorCuentaEntryDto> entries,
                                    bool includeSubledgerAccounts) {

      var utility = new UtilityToFillOutExcelExporter(_query);

      int i = 5;
      foreach (var entry in entries) {

        var account = StandardAccount.Parse(entry.StandardAccountId);
        var subledgerAccount = SubledgerAccount.Parse(entry.SubledgerAccountId);

        SetShowCascadeBalances(_excelFile, entry, i);

        _excelFile.SetCell($"C{i}", entry.CurrencyCode);

        SetByItemTypeAndRowStyle(_excelFile, entry, i);

        if (!account.IsEmptyInstance) {
          _excelFile.SetCell($"E{i}", account.Number);
          _excelFile.SetCell($"F{i}", account.Name);
        } else {
          _excelFile.SetCell($"E{i}", entry.AccountNumber);
          _excelFile.SetCell($"F{i}", entry.AccountName);
        }
        _excelFile.SetCell($"G{i}", entry.SectorCode);

        if (includeSubledgerAccounts && !subledgerAccount.IsEmptyInstance) {
          _excelFile.SetCell($"H{i}", subledgerAccount.Number);
          _excelFile.SetCell($"I{i}", subledgerAccount.Name);
        }
        _excelFile.SetCell($"J{i}", (decimal) entry.CurrentBalance);
        _excelFile.SetCell($"K{i}", entry.DebtorCreditor);

        if (utility.MustFillOutAverageBalance((decimal) entry.AverageBalance, entry.LastChangeDate)) {
          _excelFile.SetCell($"L{i}", (decimal) entry.AverageBalance);
        }

        if (entry.LastChangeDate != ExecutionServer.DateMaxValue &&
            entry.LastChangeDate >= MIN_LAST_CHANGE_DATE_TO_REPORT) {
          _excelFile.SetCell($"M{i}", entry.LastChangeDate.ToString("dd/MMM/yyyy"));
        }

        i++;
      }

      if (!_query.WithAverageBalance) {
        _excelFile.RemoveColumn("L");
      }
      if (!includeSubledgerAccounts) {
        _excelFile.RemoveColumn("I");
        _excelFile.RemoveColumn("H");
      }
      if (!_query.ShowCascadeBalances) {
        _excelFile.RemoveColumn("B");
      }
    }

    #endregion Public methods

    #region Private methods


    private void SetByItemTypeAndRowStyle(ExcelFile _excelFile, SaldosPorCuentaEntryDto entry, int i) {

      if (entry.ItemType == TrialBalanceItemType.Entry) {

        if (!entry.IsParentPostingEntry) {
          _excelFile.SetCell($"D{i}", "*");
        } else {
          _excelFile.SetCell($"D{i}", "**");
        }

      }

      if (entry.ItemType != TrialBalanceItemType.Entry &&
          entry.ItemType != TrialBalanceItemType.Summary) {
        _excelFile.SetRowStyleBold(i);
      }

    }


    private void SetRowByItemType(ExcelFile _excelFile, SaldosPorAuxiliarEntryDto entry, int i) {

      if (entry.ItemType != TrialBalanceItemType.Total) {
        _excelFile.SetCell($"A{i}", entry.LedgerNumber);
        _excelFile.SetCell($"B{i}", entry.LedgerName);

      } else {
        _excelFile.SetCell($"A{i}", "");
      }

      if (entry.ItemType == TrialBalanceItemType.Entry) {
        _excelFile.SetCell($"C{i}", $"({entry.CurrencyCode}) {entry.CurrencyName}");

        if (!entry.IsParentPostingEntry) {
          _excelFile.SetCell($"D{i}", "*");

        } else {
          _excelFile.SetCell($"D{i}", "**");

        }

        var utility = new UtilityToFillOutExcelExporter(_query);
        if (utility.MustFillOutAverageBalance((decimal) entry.AverageBalance, entry.LastChangeDate)) {
          _excelFile.SetCell($"J{i}", (decimal) entry.AverageBalance);
        }
      }

      if (entry.ItemType == TrialBalanceItemType.Entry ||
            entry.ItemType == TrialBalanceItemType.Total) {

        _excelFile.SetCell($"H{i}", (decimal) entry.CurrentBalance);
      }

    }


    private void SetShowCascadeBalances(ExcelFile _excelFile, SaldosPorCuentaEntryDto entry, int i) {

      if (_query.ShowCascadeBalances) {
        _excelFile.SetCell($"A{i}", entry.LedgerNumber);
        if (entry.ItemType == TrialBalanceItemType.Entry ||
          entry.ItemType == TrialBalanceItemType.Summary) {
          _excelFile.SetCell($"B{i}", entry.LedgerName);
        }
      } else {
        _excelFile.SetCell($"A{i}", "Consolidada");
      }

    }

    #endregion Private methods

  } // class SaldosFillOutExcelExporter

} // namespace Empiria.FinancialAccounting.Reporting
