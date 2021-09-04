/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Office Integration                           Component : Excel Exporter                        *
*  Assembly : FinancialAccounting.OficeIntegration.dll     Pattern   : Service                               *
*  Type     : TrialBalanceExcelFileCreator                 License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Creates a Microsoft Excel file with trial balance information.                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.OfficeIntegration {

  /// <summary>Creates a Microsoft Excel file with trial balance information.</summary>
  internal class TrialBalanceExcelFileCreator {

    private readonly ExcelTemplateConfig _templateConfig;
    private ExcelFile _excelFile;

    public TrialBalanceExcelFileCreator(ExcelTemplateConfig templateConfig) {
      Assertion.AssertObject(templateConfig, "templateConfig");

      _templateConfig = templateConfig;
    }


    internal ExcelFile CreateExcelFile(TrialBalanceDto trialBalance) {
      Assertion.AssertObject(trialBalance, "trialBalance");

      _excelFile = new ExcelFile(_templateConfig);

      _excelFile.Open();

      SetHeader(trialBalance.Command);

      SetTable(trialBalance);

      _excelFile.Save();

      _excelFile.Close();

      return _excelFile;
    }


    #region Private methods

    private void SetHeader(TrialBalanceCommand command) {
      _excelFile.SetCell($"A2", _templateConfig.Title);

      var subTitle = $"Del {command.InitialPeriod.FromDate.ToString("dd/MMM/yyyy")} " +
                     $"al {command.InitialPeriod.ToDate.ToString("dd/MMM/yyyy")}";

      if (command.ValuateBalances) {
        subTitle += $". Saldos valorizados al {command.InitialPeriod.ExchangeRateDate.ToString("dd/MMM/yyyy")}.";
      }

      _excelFile.SetCell($"A3", subTitle);
    }


    private void SetTable(TrialBalanceDto trialBalance) {
      switch (trialBalance.Command.TrialBalanceType) {
        case TrialBalanceType.AnaliticoDeCuentas:
          FillOutAnaliticoDeCuentas(trialBalance.Entries.Select(x => (TwoColumnsTrialBalanceEntryDto) x));
          return;

        case TrialBalanceType.BalanzaValorizadaComparativa:
          SetBalanzaComparativaHeaders(trialBalance.Command);
          FillOutBalanzaComparativa(trialBalance.Entries.Select(x => (TrialBalanceComparativeDto) x));
          return;

        case TrialBalanceType.SaldosPorCuentaYMayor:
          FillOutSaldosPorCuentayMayor(trialBalance.Entries.Select(x => (TrialBalanceEntryDto) x));
          return;

        case TrialBalanceType.Balanza:
        case TrialBalanceType.Saldos:
          FillOutBalanza(trialBalance.Entries.Select(x => (TrialBalanceEntryDto) x));
          return;

        case TrialBalanceType.SaldosPorAuxiliar:
          FillOutSaldosAuxiliar(trialBalance.Entries.Select(x => (TrialBalanceEntryDto) x));
          return;

        case TrialBalanceType.SaldosPorCuenta:
          FillOutSaldosCuenta(trialBalance.Entries.Select(x => (TrialBalanceEntryDto) x),
                              trialBalance.Command.WithSubledgerAccount);
          return;

        default:
          throw Assertion.AssertNoReachThisCode();
      }
    }

    private void FillOutAnaliticoDeCuentas(IEnumerable<TwoColumnsTrialBalanceEntryDto> entries) {
      int i = 5;

      foreach (var entry in entries) {
        _excelFile.SetCell($"A{i}", entry.LedgerNumber);
        if (entry.ItemType == TrialBalanceItemType.BalanceEntry) {
          _excelFile.SetCell($"B{i}", "*");
        }
        _excelFile.SetCell($"C{i}", entry.AccountNumber);
        _excelFile.SetCell($"D{i}", entry.AccountName);
        _excelFile.SetCell($"E{i}", entry.SectorCode);
        _excelFile.SetCell($"F{i}", entry.DomesticBalance);
        _excelFile.SetCell($"G{i}", entry.ForeignBalance);

        if (entry.ItemType != TrialBalanceItemType.BalanceEntry &&
            entry.ItemType != TrialBalanceItemType.BalanceSummary) {
          _excelFile.SetRowStyleBold(i);
        }
        i++;
      }
    }


    private void FillOutBalanzaComparativa(IEnumerable<TrialBalanceComparativeDto> entries) {
      int i = 5;

      foreach (var entry in entries) {
        _excelFile.SetCell($"A{i}", entry.LedgerNumber);
        _excelFile.SetCell($"B{i}", entry.CurrencyCode);
        _excelFile.SetCell($"C{i}", GetLedgerLevelAccountNumber(entry.AccountNumber));
        _excelFile.SetCell($"D{i}", GetSubAccountNumberWithSector(entry.AccountNumber, entry.SectorCode));
        _excelFile.SetCell($"E{i}", entry.AccountNumber);
        _excelFile.SetCell($"F{i}", entry.SectorCode);
        _excelFile.SetCell($"G{i}", entry.SubledgerAccountNumber);
        _excelFile.SetCell($"H{i}", entry.SubledgerAccountName);
        _excelFile.SetCell($"I{i}", entry.FirstTotalBalance);
        _excelFile.SetCell($"J{i}", entry.FirstExchangeRate);
        _excelFile.SetCell($"K{i}", entry.FirstValorization);
        _excelFile.SetCell($"L{i}", entry.Debit);
        _excelFile.SetCell($"M{i}", entry.Credit);
        _excelFile.SetCell($"N{i}", entry.SecondTotalBalance);
        _excelFile.SetCell($"O{i}", entry.SecondExchangeRate);
        _excelFile.SetCell($"P{i}", entry.SecondValorization);
        _excelFile.SetCell($"Q{i}", entry.AccountName);
        _excelFile.SetCell($"R{i}", Convert.ToString((char) entry.DebtorCreditor));
        _excelFile.SetCell($"S{i}", entry.Variation);
        _excelFile.SetCell($"T{i}", entry.VariationByER);
        _excelFile.SetCell($"U{i}", entry.RealVariation);

        i++;
      }
    }


    private void SetBalanzaComparativaHeaders(TrialBalanceCommand command) {
      _excelFile.SetCell($"I4", $"{command.InitialPeriod.ToDate.ToString("MMM_yyyy")}");
      _excelFile.SetCell($"K4", $"{command.InitialPeriod.ToDate.ToString("MMM")}_VAL_A");
      _excelFile.SetCell($"N4", $"{command.FinalPeriod.ToDate.ToString("MMM_yyyy")}");
      _excelFile.SetCell($"P4", $"{command.FinalPeriod.ToDate.ToString("MMM")}_VAL_B");
    }


    private void FillOutSaldosPorCuentayMayor(IEnumerable<TrialBalanceEntryDto> entries) {
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
        _excelFile.SetCell($"J{i}", entry.CurrentBalance);

        if (entry.LedgerNumber.Length == 0) {
          _excelFile.SetRowStyleBold(i);
        }
        i++;
      }
    }


    private void FillOutBalanza(IEnumerable<TrialBalanceEntryDto> entries) {
      int i = 5;

      foreach (var entry in entries) {
        _excelFile.SetCell($"A{i}", entry.LedgerNumber);
        _excelFile.SetCell($"B{i}", entry.CurrencyCode);
        if (entry.ItemType == TrialBalanceItemType.BalanceEntry) {
          _excelFile.SetCell($"C{i}", "*");
        }
        _excelFile.SetCell($"D{i}", entry.AccountNumber);
        _excelFile.SetCell($"E{i}", entry.AccountName);
        _excelFile.SetCell($"F{i}", entry.SectorCode);
        _excelFile.SetCell($"G{i}", entry.InitialBalance);
        _excelFile.SetCell($"H{i}", entry.Debit);
        _excelFile.SetCell($"I{i}", entry.Credit);
        _excelFile.SetCell($"J{i}", entry.CurrentBalance);

        if (entry.ItemType != TrialBalanceItemType.BalanceEntry &&
            entry.ItemType != TrialBalanceItemType.BalanceSummary) {
          _excelFile.SetRowStyleBold(i);
        }
        i++;
      }
    }

    private void FillOutSaldosAuxiliar(IEnumerable<TrialBalanceEntryDto> entries) {
      int i = 5;

      foreach (var entry in entries) {
        var account = StandardAccount.Parse(entry.StandardAccountId);
        var subledgerAccount = SubsidiaryAccount.Parse(entry.SubledgerAccountId);

        _excelFile.SetCell($"A{i}", entry.LedgerNumber);
        _excelFile.SetCell($"B{i}", entry.CurrencyCode);
        if (entry.ItemType == TrialBalanceItemType.BalanceEntry) {
          _excelFile.SetCell($"C{i}", "*");
        }
        if (!subledgerAccount.IsEmptyInstance) {
          _excelFile.SetCell($"D{i}", subledgerAccount.Number);
          _excelFile.SetCell($"E{i}", subledgerAccount.Name);
        }

        if (!account.IsEmptyInstance) {
          _excelFile.SetCell($"F{i}", account.Number);
          _excelFile.SetCell($"G{i}", account.Name);
        } else {
          _excelFile.SetCell($"F{i}", entry.AccountNumber);
          _excelFile.SetCell($"G{i}", entry.AccountName);
        }
        _excelFile.SetCell($"H{i}", entry.SectorCode);

        _excelFile.SetCell($"I{i}", entry.CurrentBalance);
        _excelFile.SetCell($"J{i}", Convert.ToString((char) account.DebtorCreditor));
        _excelFile.SetCell($"K{i}", entry.LastChangeDate.ToString("dd/MMM/yyyy"));

        if (entry.ItemType != TrialBalanceItemType.BalanceEntry &&
            entry.ItemType != TrialBalanceItemType.BalanceSummary) {
          _excelFile.SetRowStyleBold(i);
        }
        i++;
      }
    }


    private void FillOutSaldosCuenta(IEnumerable<TrialBalanceEntryDto> entries,
                                     bool includeSubledgerAccounts) {
      int i = 5;

      foreach (var entry in entries) {
        var account = StandardAccount.Parse(entry.StandardAccountId);
        var subledgerAccount = SubsidiaryAccount.Parse(entry.SubledgerAccountId);

        _excelFile.SetCell($"A{i}", entry.LedgerNumber);
        _excelFile.SetCell($"B{i}", entry.CurrencyCode);
        if (entry.ItemType == TrialBalanceItemType.BalanceEntry) {
          _excelFile.SetCell($"C{i}", "*");
        }
        if (!account.IsEmptyInstance) {
          _excelFile.SetCell($"D{i}", account.Number);
          _excelFile.SetCell($"E{i}", account.Name);
        } else {
          _excelFile.SetCell($"D{i}", entry.AccountNumber);
          _excelFile.SetCell($"E{i}", entry.AccountName);
        }
        _excelFile.SetCell($"F{i}", entry.SectorCode);
        if (includeSubledgerAccounts && !subledgerAccount.IsEmptyInstance) {
          _excelFile.SetCell($"G{i}", subledgerAccount.Number);
          _excelFile.SetCell($"H{i}", subledgerAccount.Name);
        }
        _excelFile.SetCell($"I{i}", entry.CurrentBalance);
        _excelFile.SetCell($"J{i}", Convert.ToString((char) account.DebtorCreditor));
        _excelFile.SetCell($"K{i}", entry.LastChangeDate.ToString("dd/MMM/yyyy"));

        if (entry.ItemType != TrialBalanceItemType.BalanceEntry &&
            entry.ItemType != TrialBalanceItemType.BalanceSummary) {
          _excelFile.SetRowStyleBold(i);
        }
        i++;
      }

      if (!includeSubledgerAccounts) {
        _excelFile.RemoveColumn("G");
        _excelFile.RemoveColumn("H");
      }
    }

    #endregion Private methods

    #region Utility methods

    private string GetLedgerLevelAccountNumber(string accountNumber) {
      return accountNumber.Substring(0, 4);
    }

    private string GetSubAccountNumberWithSector(string accountNumber, string sectorCode) {
      var temp = accountNumber.Substring(4);

      temp = temp.Replace("-", String.Empty);

      temp = temp.PadRight(12, '0');

      return temp + sectorCode;
    }

    #endregion Utility methods

  }  // class TrialBalanceExcelFileCreator

}  // namespace Empiria.FinancialAccounting.OfficeIntegration
