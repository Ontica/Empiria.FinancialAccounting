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
using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.Reporting {

  /// <summary>Fill out table info for a Microsoft Excel file with trial balance information.</summary>
  internal class BalancesFillOutExcelExporter {

    private TrialBalanceQuery _query;

    private readonly DateTime MIN_LAST_CHANGE_DATE_TO_REPORT = DateTime.Parse("01/01/1970");

    public BalancesFillOutExcelExporter(TrialBalanceQuery query) {
      _query = query;
    }


    #region Public methods

    public void FillOutAnaliticoDeCuentas(ExcelFile _excelFile, 
                                          IEnumerable<AnaliticoDeCuentasEntryDto> entries) {
      int i = 5;

      foreach (var entry in entries) {
        if (_query.ShowCascadeBalances) {
          _excelFile.SetCell($"A{i}", entry.LedgerNumber);
          _excelFile.SetCell($"B{i}", entry.LedgerName);
        } else {
          _excelFile.SetCell($"A{i}", "Consolidada");
        }

        _excelFile.SetCell($"C{i}", entry.AccountMark);
        _excelFile.SetCell($"D{i}", entry.AccountNumber);
        _excelFile.SetCell($"E{i}", entry.AccountName);
        _excelFile.SetCell($"F{i}", entry.SectorCode);
        _excelFile.SetCell($"G{i}", entry.DomesticBalance);
        _excelFile.SetCell($"H{i}", entry.ForeignBalance);
        _excelFile.SetCell($"I{i}", entry.TotalBalance);

        if (MustFillOutAverageBalance(entry.AverageBalance, entry.LastChangeDate)) {
          _excelFile.SetCell($"J{i}", entry.AverageBalance);
          _excelFile.SetCell($"K{i}", entry.LastChangeDate);
        }

        if (entry.ItemType != TrialBalanceItemType.Entry &&
            entry.ItemType != TrialBalanceItemType.Summary) {
          _excelFile.SetRowStyleBold(i);
        }
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


    public void FillOutBalanzaComparativa(ExcelFile _excelFile,
                                          IEnumerable<BalanzaComparativaEntryDto> entries) {
      int i = 5;

      foreach (var entry in entries) {
        if (_query.ShowCascadeBalances) {
          _excelFile.SetCell($"A{i}", entry.LedgerNumber);
          _excelFile.SetCell($"B{i}", entry.LedgerName);
        } else {
          _excelFile.SetCell($"A{i}", "Consolidada");
        }

        _excelFile.SetCell($"C{i}", entry.CurrencyCode);
        _excelFile.SetCell($"D{i}", GetLedgerLevelAccountNumber(entry.AccountNumber));
        _excelFile.SetCell($"E{i}", GetSubAccountNumberWithSector(entry.AccountNumber, entry.SectorCode));
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

        if (MustFillOutAverageBalance(entry.AverageBalance, entry.LastChangeDate)) {
          _excelFile.SetCell($"W{i}", entry.AverageBalance);
          _excelFile.SetCell($"X{i}", entry.LastChangeDate.ToString("dd/MMM/yyyy"));
        }

        i++;
      }

      if (!_query.WithAverageBalance) {
        _excelFile.RemoveColumn("X");
        _excelFile.RemoveColumn("W");
      }
      if (!_query.ShowCascadeBalances) {
        _excelFile.RemoveColumn("B");
      }
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
        if (entry.ItemType == TrialBalanceItemType.Summary) {
          _excelFile.SetRowStyleBold(i);
        }
        i++;
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
          _excelFile.SetRowStyleBold(i);
        }

        i++;
      }
    }


    public void FillOutBalanzaConContabilidadesEnCascada(ExcelFile _excelFile,
                                             IEnumerable<BalanzaContabilidadesCascadaEntryDto> entries) {
      int i = 5;

      foreach (var entry in entries) {
        var account = StandardAccount.Parse(entry.StandardAccountId);

        _excelFile.SetCell($"A{i}", entry.CurrencyCode);
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
        } else {
          _excelFile.SetCell($"C{i}", account.Name);
          _excelFile.SetCell($"D{i}", entry.SectorCode);
          _excelFile.SetCell($"E{i}", entry.LedgerNumber);
          _excelFile.SetCell($"F{i}", entry.AccountName);
        }

        _excelFile.SetCell($"G{i}", entry.InitialBalance);
        _excelFile.SetCell($"H{i}", entry.Debit);
        _excelFile.SetCell($"I{i}", entry.Credit);
        _excelFile.SetCell($"J{i}", (decimal) entry.CurrentBalance);

        if (MustFillOutAverageBalance((decimal) entry.AverageBalance, entry.LastChangeDate)) {
          _excelFile.SetCell($"K{i}", (decimal) entry.AverageBalance);
          _excelFile.SetCell($"L{i}", entry.LastChangeDate.ToString("dd/MMM/yyyy"));
        }

        if (entry.LedgerNumber.Length == 0) {
          _excelFile.SetRowStyleBold(i);
        }
        i++;
      }

      if (!_query.WithAverageBalance) {
        _excelFile.RemoveColumn("L");
        _excelFile.RemoveColumn("K");
      }
    }


    public void FillOutBalanza(ExcelFile _excelFile,
                               IEnumerable<BalanzaTradicionalEntryDto> entries) {
      int i = 5;

      foreach (var entry in entries) {
        if (_query.ShowCascadeBalances) {
          _excelFile.SetCell($"A{i}", entry.LedgerNumber);
          if (entry.ItemType == TrialBalanceItemType.Entry ||
              entry.ItemType == TrialBalanceItemType.Summary) {
            _excelFile.SetCell($"B{i}", entry.LedgerName);
          }
        } else {
          _excelFile.SetCell($"A{i}", "Consolidada");
        }

        _excelFile.SetCell($"C{i}", entry.CurrencyCode);
        if (entry.ItemType == TrialBalanceItemType.Entry) {
          if (!entry.IsParentPostingEntry) {
            _excelFile.SetCell($"D{i}", "*");
          } else {
            _excelFile.SetCell($"D{i}", "**");
          }
        }
        _excelFile.SetCell($"E{i}", entry.AccountNumber);
        _excelFile.SetCell($"F{i}", entry.AccountName);
        _excelFile.SetCell($"G{i}", entry.SectorCode);
        _excelFile.SetCell($"H{i}", entry.InitialBalance);
        _excelFile.SetCell($"I{i}", entry.Debit);
        _excelFile.SetCell($"J{i}", entry.Credit);
        _excelFile.SetCell($"K{i}", (decimal) entry.CurrentBalance);
        _excelFile.SetCell($"L{i}", Math.Round(entry.ExchangeRate, 6));

        if (MustFillOutAverageBalance((decimal) entry.AverageBalance, entry.LastChangeDate)) {
          _excelFile.SetCell($"M{i}", (decimal) entry.AverageBalance);
          _excelFile.SetCell($"N{i}", entry.LastChangeDate.ToString("dd/MMM/yyyy"));
        }

        if (entry.ItemType != TrialBalanceItemType.Entry &&
            entry.ItemType != TrialBalanceItemType.Summary) {
          _excelFile.SetRowStyleBold(i);
        }
        i++;
      }

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


    #endregion Public methods


    #region Utility methods


    // TODO: CLEAN THIS CODE. ISSUE USING NEW CHART OF ACCOUNTS
    private string GetLedgerLevelAccountNumber(string accountNumber) {
      var temp = string.Empty;

      if (accountNumber.Contains("-")) {
        temp = accountNumber.Substring(0, 4);
      } else if (accountNumber.Contains(".")) {
        temp = accountNumber.Substring(0, 1);
      }

      return temp;
    }


    // TODO: CLEAN THIS CODE. ISSUE USING NEW CHART OF ACCOUNTS
    private string GetSubAccountNumberWithSector(string accountNumber, string sectorCode) {
      var temp = string.Empty;

      if (accountNumber.Contains("-")) {

        temp = accountNumber.Substring(4);

        temp = temp.Replace("-", String.Empty);

        temp = temp.PadRight(12, '0');

      } else if (accountNumber.Contains(".")) {

        temp = accountNumber.Substring(2);

        temp = temp.Replace(".", String.Empty);

        temp = temp.PadRight(20, '0');
      }

      return temp + sectorCode;
    }


    internal bool MustFillOutAverageBalance(decimal averageBalance, DateTime lastChangeDate) {
      if (!_query.WithAverageBalance) {
        return false;
      }
      if (averageBalance != 0) {
        return true;
      }
      if (lastChangeDate < MIN_LAST_CHANGE_DATE_TO_REPORT) {
        return false;
      }
      if (lastChangeDate == ExecutionServer.DateMaxValue) {
        return false;
      }
      return true;
    }

    #endregion Utility methods

  } // class BalancesFillOutExcelExporter

} // namespace Empiria.FinancialAccounting.Reporting
