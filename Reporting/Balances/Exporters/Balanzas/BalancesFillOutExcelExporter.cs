/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                           Component : Excel Exporters                       *
*  Assembly : FinancialAccounting.Reporting.dll            Pattern   : IExcelExporter                        *
*  Type     : BalancesFillOutExcelExporter                 License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Fill out table info for a Microsoft Excel file with trial balance information.                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.Office;

using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.Reporting.Balances {

  /// <summary>Fill out table info for a Microsoft Excel file with trial balance information.</summary>
  internal class BalancesFillOutExcelExporter {

    private TrialBalanceQuery _query;

    public BalancesFillOutExcelExporter(TrialBalanceQuery query) {
      _query = query;
    }


    #region Public methods

    public void FillOutAnaliticoDeCuentas(ExcelFile _excelFile,
                                          IEnumerable<AnaliticoDeCuentasEntryDto> entries) {

      var utility = new UtilityToFillOutExcelExporter(_query);

      int i = 5;
      foreach (var entry in entries) {

        _excelFile.SetCell($"A{i}", "Consolidada");

        SetRowClausesForAnalitico(_excelFile, entry, utility, i);

        _excelFile.SetCell($"C{i}", entry.AccountMark);
        _excelFile.SetCell($"D{i}", entry.AccountNumber);
        _excelFile.SetCell($"E{i}", entry.AccountName);
        _excelFile.SetCell($"F{i}", entry.SectorCode);
        _excelFile.SetCell($"G{i}", entry.DomesticBalance);
        _excelFile.SetCell($"H{i}", entry.ForeignBalance);
        _excelFile.SetCell($"I{i}", entry.TotalBalance);

        SetRowStyleBoldForAnaliticoAndBalanza(_excelFile, entry, i, 9);

        i++;
      }

      if (!_query.WithAverageBalance) {
        _excelFile.RemoveColumn("K");
        _excelFile.RemoveColumn("J");
      }
      if (!_query.ShowCascadeBalances) {
        _excelFile.RemoveColumn("B");
      }
    }


    public void FillOutBalanza(ExcelFile _excelFile,
                               IEnumerable<BalanzaTradicionalEntryDto> entries) {

      var utility = new UtilityToFillOutExcelExporter(_query);

      int i = 5;
      foreach (var entry in entries) {

        SetRowClausesForBalanza(_excelFile, entry, i);

        _excelFile.SetCell($"C{i}", entry.CurrencyCode);
        _excelFile.SetCell($"E{i}", entry.AccountNumber);
        _excelFile.SetCell($"F{i}", entry.AccountName);
        _excelFile.SetCell($"G{i}", entry.SectorCode);
        _excelFile.SetCell($"H{i}", entry.InitialBalance);
        _excelFile.SetCell($"I{i}", entry.Debit);
        _excelFile.SetCell($"J{i}", entry.Credit);
        _excelFile.SetCell($"K{i}", (decimal) entry.CurrentBalance);
        _excelFile.SetCell($"L{i}", Math.Round(entry.ExchangeRate, 6));

        if (utility.MustFillOutAverageBalance((decimal) entry.AverageBalance, entry.LastChangeDate)) {
          _excelFile.SetCell($"M{i}", (decimal) entry.AverageBalance);
          _excelFile.SetCell($"N{i}", entry.LastChangeDate.ToString("dd/MMM/yyyy"));
        }

        SetRowStyleBoldForAnaliticoAndBalanza(_excelFile, entry, i, 14);

        i++;
      }

      SetColumnClausesForBalanza(_excelFile);

    }


    public void FillOutBalanzaColumnasPorMoneda(ExcelFile _excelFile,
                                                IEnumerable<BalanzaColumnasMonedaEntryDto> entries) {
      int i = 5;

      foreach (var entry in entries) {
        _excelFile.SetCell($"A{i}", entry.AccountNumber);
        _excelFile.SetCell($"B{i}", entry.AccountName);
        _excelFile.SetCell($"C{i}", entry.DomesticBalance);
        _excelFile.SetCell($"D{i}", entry.DollarBalance);
        _excelFile.SetCell($"E{i}", entry.YenBalance);
        _excelFile.SetCell($"F{i}", entry.EuroBalance);
        _excelFile.SetCell($"G{i}", entry.UdisBalance);
        _excelFile.SetCell($"H{i}", entry.TotalValorized);
        if (entry.ItemType == TrialBalanceItemType.Summary) {
          _excelFile.SetRowBold(i, 8);
        }
        i++;
      }
    }


    public void FillOutBalanzaComparativa(ExcelFile _excelFile,
                                          IEnumerable<BalanzaComparativaEntryDto> entries) {

      var utility = new UtilityToFillOutExcelExporter(_query);

      int i = 5;
      foreach (var entry in entries) {

        _excelFile.SetCell($"A{i}", "Consolidada");

        SetRowClausesForBalanzaComparativa(_excelFile, entry, utility, i);

        _excelFile.SetCell($"C{i}", entry.CurrencyCode);
        _excelFile.SetCell($"D{i}", utility.GetLedgerLevelAccountNumber(entry.AccountNumber));
        _excelFile.SetCell($"E{i}", utility.GetSubAccountNumberWithSector(entry.AccountNumber, entry.SectorCode));
        _excelFile.SetCell($"F{i}", entry.AccountNumber);
        _excelFile.SetCell($"G{i}", entry.SectorCode);
        _excelFile.SetCell($"H{i}", entry.SubledgerAccountNumber);
        _excelFile.SetCell($"I{i}", entry.SubledgerAccountName);
        _excelFile.SetCell($"J{i}", entry.FirstTotalBalance);
        _excelFile.SetCell($"K{i}", entry.FirstExchangeRate);
        _excelFile.SetCell($"L{i}", entry.FirstValorization);
        _excelFile.SetCell($"M{i}", entry.Debit);
        _excelFile.SetCell($"N{i}", entry.Credit);
        _excelFile.SetCell($"O{i}", entry.SecondTotalBalance);
        _excelFile.SetCell($"P{i}", entry.SecondExchangeRate);
        _excelFile.SetCell($"Q{i}", entry.SecondValorization);
        _excelFile.SetCell($"R{i}", entry.AccountName);
        _excelFile.SetCell($"S{i}", Convert.ToString((char) entry.DebtorCreditor));
        _excelFile.SetCell($"T{i}", entry.Variation);
        _excelFile.SetCell($"U{i}", entry.VariationByER);
        _excelFile.SetCell($"V{i}", entry.RealVariation);

        i++;
      }
      SetColumnClausesForBalanzaComparativa(_excelFile);
    }


    public void FillOutBalanzaConContabilidadesEnCascada(ExcelFile _excelFile,
                                             IEnumerable<BalanzaContabilidadesCascadaEntryDto> entries) {

      var utility = new UtilityToFillOutExcelExporter(_query);

      int i = 5;
      foreach (var entry in entries) {

        _excelFile.SetCell($"A{i}", entry.CurrencyCode);

        SetRowClausesForBalanzaContabilidadesEnCascada(_excelFile, entry, i);

        _excelFile.SetCell($"G{i}", entry.InitialBalance);
        _excelFile.SetCell($"H{i}", entry.Debit);
        _excelFile.SetCell($"I{i}", entry.Credit);
        _excelFile.SetCell($"J{i}", (decimal) entry.CurrentBalance);

        if (utility.MustFillOutAverageBalance((decimal) entry.AverageBalance, entry.LastChangeDate)) {
          _excelFile.SetCell($"K{i}", (decimal) entry.AverageBalance);
          _excelFile.SetCell($"L{i}", entry.LastChangeDate.ToString("dd/MMM/yyyy"));
        }

        i++;
      }

      if (!_query.WithAverageBalance) {
        _excelFile.RemoveColumn("L");
        _excelFile.RemoveColumn("K");
      }
    }


    public void FillOutBalanzaDolarizada(ExcelFile _excelFile,
                                         IEnumerable<BalanzaDolarizadaEntryDto> entries) {
      int i = 5;

      foreach (var entry in entries) {
        _excelFile.SetCell($"A{i}", entry.AccountNumber);
        _excelFile.SetCell($"B{i}", entry.AccountName);
        _excelFile.SetCell($"C{i}", entry.CurrencyName);
        _excelFile.SetCell($"D{i}", entry.CurrencyCode);
        if (entry.ItemType != TrialBalanceItemType.BalanceTotalCurrency) {
          _excelFile.SetCell($"E{i}", (decimal) entry.TotalBalance);
          _excelFile.SetCell($"F{i}", (decimal) entry.ValuedExchangeRate);
        }
        _excelFile.SetCell($"G{i}", entry.TotalEquivalence);

        if (entry.ItemType != TrialBalanceItemType.Entry) {
          _excelFile.SetRowBold(i, 7);
        }

        i++;
      }
    }

    #endregion Public methods


    #region Private methods


    private void SetColumnClausesForBalanza(ExcelFile _excelFile) {

      if (!_query.WithAverageBalance) {
        _excelFile.RemoveColumn("N");
        _excelFile.RemoveColumn("M");
      }

      if (!_query.UseDefaultValuation &&
            (_query.InitialPeriod.ValuateToCurrrencyUID.Length == 0 &&
             _query.InitialPeriod.ExchangeRateTypeUID.Length == 0)) {
        _excelFile.RemoveColumn("L");
      }

      if (!_query.ShowCascadeBalances) {
        _excelFile.RemoveColumn("B");
      }

    }


    private void SetColumnClausesForBalanzaComparativa(ExcelFile _excelFile) {
      if (!_query.WithAverageBalance) {
        _excelFile.RemoveColumn("X");
        _excelFile.RemoveColumn("W");
      }
      if (!_query.ShowCascadeBalances) {
        _excelFile.RemoveColumn("B");
      }
    }


    private void SetRowClausesForAnalitico(ExcelFile _excelFile,
      AnaliticoDeCuentasEntryDto entry, UtilityToFillOutExcelExporter utility, int i) {

      if (_query.ShowCascadeBalances) {
        _excelFile.SetCell($"A{i}", entry.LedgerNumber);
        _excelFile.SetCell($"B{i}", entry.LedgerName);
      }

      if (utility.MustFillOutAverageBalance(entry.AverageBalance, entry.LastChangeDate)) {
        _excelFile.SetCell($"J{i}", entry.AverageBalance);
        _excelFile.SetCell($"K{i}", entry.LastChangeDate);
      }

    }


    private void SetRowClausesForBalanza(ExcelFile _excelFile, BalanzaTradicionalEntryDto entry, int i) {

      if (_query.ShowCascadeBalances) {
        _excelFile.SetCell($"A{i}", entry.LedgerNumber);
        if (entry.ItemType == TrialBalanceItemType.Entry ||
            entry.ItemType == TrialBalanceItemType.Summary) {
          _excelFile.SetCell($"B{i}", entry.LedgerName);
        }
      } else {
        _excelFile.SetCell($"A{i}", "Consolidada");
      }

      if (entry.ItemType == TrialBalanceItemType.Entry) {
        if (!entry.IsParentPostingEntry) {
          _excelFile.SetCell($"D{i}", "*");
        } else {
          _excelFile.SetCell($"D{i}", "**");
        }
      }

    }


    private void SetRowClausesForBalanzaComparativa(ExcelFile _excelFile,
      BalanzaComparativaEntryDto entry, UtilityToFillOutExcelExporter utility, int i) {

      if (_query.ShowCascadeBalances) {
        _excelFile.SetCell($"A{i}", entry.LedgerNumber);
        _excelFile.SetCell($"B{i}", entry.LedgerName);
      }

      if (utility.MustFillOutAverageBalance(entry.AverageBalance, entry.LastChangeDate)) {
        _excelFile.SetCell($"W{i}", entry.AverageBalance);
        _excelFile.SetCell($"X{i}", entry.LastChangeDate.ToString("dd/MMM/yyyy"));
      }
    }


    private void SetRowClausesForBalanzaContabilidadesEnCascada(
      ExcelFile _excelFile, BalanzaContabilidadesCascadaEntryDto entry, int i) {

      var account = StandardAccount.Parse(entry.StandardAccountId);

      if (account.IsEmptyInstance) {
        _excelFile.SetCell($"B{i}", entry.AccountNumber);
      } else {
        _excelFile.SetCell($"B{i}", account.Number);
      }

      if (entry.LedgerNumber.Length == 0) {
        _excelFile.SetCell($"C{i}", entry.AccountName);
        _excelFile.SetCell($"D{i}", entry.SectorCode);
        _excelFile.SetCell($"E{i}", "00");
        _excelFile.SetCell($"F{i}", "Todas");
        _excelFile.SetRowBold(i, 6);

      } else {
        _excelFile.SetCell($"C{i}", account.Name);
        _excelFile.SetCell($"D{i}", entry.SectorCode);
        _excelFile.SetCell($"E{i}", entry.LedgerNumber);
        _excelFile.SetCell($"F{i}", entry.AccountName);
      }

    }


    private void SetRowStyleBoldForAnaliticoAndBalanza(ExcelFile _excelFile,
                                                       ITrialBalanceEntryDto entry,
                                                       int rowIndex, int lastColumnIndex) {

      if (entry.ItemType != TrialBalanceItemType.Entry && entry.ItemType != TrialBalanceItemType.Summary) {
        _excelFile.SetRowBold(rowIndex, lastColumnIndex);
      }

    }


    #endregion Private methods

  } // class BalancesFillOutExcelExporter

} // namespace Empiria.FinancialAccounting.Reporting.Balances
