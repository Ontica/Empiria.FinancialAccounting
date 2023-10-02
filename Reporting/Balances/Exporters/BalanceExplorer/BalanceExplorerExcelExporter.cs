/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                           Component : Excel Exporters                       *
*  Assembly : FinancialAccounting.Reporting.dll            Pattern   : IExcelExporter                        *
*  Type     : BalanceExplorerExcelExporter                 License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Creates a Microsoft Excel file with the results of a balance explorer query execution.         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.Office;
using Empiria.Storage;

using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.BalanceExplorer.Adapters;

namespace Empiria.FinancialAccounting.Reporting.Balances {

  /// <summary>Creates a Microsoft Excel file with the results of a balance explorer query execution.</summary>
  internal class BalanceExplorerExcelExporter {

    private BalanceExplorerQuery _query = new BalanceExplorerQuery();

    private readonly FileTemplateConfig _templateConfig;

    private ExcelFile _excelFile;


    public BalanceExplorerExcelExporter(FileTemplateConfig templateConfig) {
      Assertion.Require(templateConfig, nameof(templateConfig));

      _templateConfig = templateConfig;
    }


    internal ExcelFile CreateExcelFile(BalanceExplorerDto dto) {
      Assertion.Require(dto, nameof(dto));

      _query = dto.Query;

      _excelFile = new ExcelFile(_templateConfig);

      _excelFile.Open();

      SetHeader();

      SetTable(dto);

      _excelFile.Save();

      _excelFile.Close();

      return _excelFile;
    }


    private void SetHeader() {
      _excelFile.SetCell($"A2", _templateConfig.Title);
      _excelFile.SetCell($"E2", $"Fecha de consulta: {DateTime.Now.ToString("dd/MMM/yyyy")}");
      _excelFile.SetRowStyleBold(2);
      var subTitle = $"Del {_query.InitialPeriod.FromDate.ToString("dd/MMM/yyyy")} " +
                     $"al {_query.InitialPeriod.ToDate.ToString("dd/MMM/yyyy")}";

      _excelFile.SetCell($"A3", subTitle);
      _excelFile.SetRowStyleBold(3);
    }


    private void SetTable(BalanceExplorerDto dto) {
      switch (dto.Query.TrialBalanceType) {
        case TrialBalanceType.SaldosPorAuxiliarConsultaRapida:
          FillOutSaldosAuxiliar(dto.Entries);
          return;

        case TrialBalanceType.SaldosPorCuentaConsultaRapida:
          FillOutSaldosCuenta(dto.Entries);
          return;

        default:
          throw Assertion.EnsureNoReachThisCode();
      }
    }


    private void FillOutSaldosAuxiliar(IEnumerable<BalanceExplorerEntryDto> entries) {
      switch (_query.ExportTo) {
        case FileReportVersion.V1:
          FillOutSaldosAuxiliarConEncabezado(entries);
          break;
        case FileReportVersion.V2:
          FillOutSaldosAuxiliarPorColumna(entries);
          break;
        case FileReportVersion.V3:
          break;
        default:
          throw Assertion.EnsureNoReachThisCode();
      }
    }


    private void FillOutSaldosCuenta(IEnumerable<BalanceExplorerEntryDto> entries) {
      switch (_query.ExportTo) {
        case FileReportVersion.V1:
          FillOutSaldosCuentaConEncabezado(entries);
          break;
        case FileReportVersion.V2:
          FillOutSaldosCuentaPorColumna(entries);
          break;
        case FileReportVersion.V3:
          break;
        default:
          throw Assertion.EnsureNoReachThisCode();
      }
    }


    private void FillOutSaldosAuxiliarConEncabezado(IEnumerable<BalanceExplorerEntryDto> entries) {
      int i = 4;
      foreach (var entry in entries) {
        if (entry.ItemType == TrialBalanceItemType.Summary) {
          i++;
          _excelFile.SetCell($"D{i}", entry.AccountNumber);
          _excelFile.SetCell($"E{i}", $"{entry.AccountName}"/* , Naturaleza {entry.DebtorCreditor}"*/);
          _excelFile.SetCell($"G{i}", "");
          _excelFile.SetCell($"H{i}", "");
          _excelFile.SetRowStyleBold(i);

          i++;
          SetRowHeaderByAccount(i);

        } else {
          _excelFile.SetCell($"A{i}", entry.LedgerNumber);
          _excelFile.SetCell($"B{i}", entry.LedgerName);
          _excelFile.SetCell($"C{i}", $"({entry.CurrencyCode}) {entry.CurrencyName}");
          _excelFile.SetCell($"D{i}", entry.AccountNumber);
          _excelFile.SetCell($"E{i}", entry.AccountName);
          _excelFile.SetCell($"F{i}", entry.SectorCode);
          _excelFile.SetCell($"G{i}", (decimal) entry.CurrentBalance);
          _excelFile.SetCell($"H{i}", entry.LastChangeDate.ToString("dd/MMM/yyyy"));
        }

        i++;
      }
    }


    private void FillOutSaldosAuxiliarPorColumna(IEnumerable<BalanceExplorerEntryDto> entries) {
      int i = 4;
      foreach (var entry in entries) {
        if (entry.ItemType == TrialBalanceItemType.Entry) {
          _excelFile.SetCell($"A{i}", entry.LedgerNumber);
          _excelFile.SetCell($"B{i}", entry.LedgerName);
          _excelFile.SetCell($"C{i}", $"({entry.CurrencyCode}) {entry.CurrencyName}");
          _excelFile.SetCell($"D{i}", entry.AccountNumber);
          _excelFile.SetCell($"E{i}", entry.AccountName);
          _excelFile.SetCell($"F{i}", entry.SubledgerAccountNumber);
          _excelFile.SetCell($"G{i}", entry.SubledgerAccountName);
          _excelFile.SetCell($"H{i}", entry.SectorCode);
          _excelFile.SetCell($"I{i}", (decimal) entry.CurrentBalance);
          _excelFile.SetCell($"J{i}", entry.DebtorCreditor);
          _excelFile.SetCell($"K{i}", entry.LastChangeDate.ToString("dd/MMM/yyyy"));
        }
        i++;
      }
    }


    private void FillOutSaldosCuentaConEncabezado(IEnumerable<BalanceExplorerEntryDto> entries) {
      int i = 5;
      foreach (var entry in entries) {
        
        if (entry.ItemType == TrialBalanceItemType.Total) {
          _excelFile.SetCell($"D{i}", entry.AccountNumber);
          _excelFile.SetCell($"E{i}", $"{entry.AccountName}");
          _excelFile.SetCell($"G{i}", "");
          _excelFile.SetCell($"H{i}", "");
          _excelFile.SetRowStyleBold(i);

          i++;
          SetRowHeaderByAccount(i);

        } else if (entry.ItemType == TrialBalanceItemType.Group) {
          _excelFile.SetCell($"E{i}", entry.AccountName);
          _excelFile.SetCell($"G{i}", (decimal) entry.CurrentBalance);
          _excelFile.SetRowStyleBold(i);
          i += 3;

        } else {
          SetRowsForSaldosCuentaConEncabezado(entry, i);
        }

        i++;
      }
    }


    private void FillOutSaldosCuentaPorColumna(IEnumerable<BalanceExplorerEntryDto> entries) {
      int i = 5;
      foreach (var entry in entries) {

        if (entry.ItemType == TrialBalanceItemType.Entry) {
          _excelFile.SetCell($"A{i}", entry.LedgerNumber);
          _excelFile.SetCell($"B{i}", entry.LedgerName);
          _excelFile.SetCell($"C{i}", $"({entry.CurrencyCode}) {entry.CurrencyName}");
          _excelFile.SetCell($"D{i}", entry.AccountNumberForBalances);
          _excelFile.SetCell($"E{i}", entry.AccountNameToExport);
          _excelFile.SetCell($"F{i}", entry.SubledgerAccountNumber);
          _excelFile.SetCell($"G{i}", entry.SubledgerAccountName);
          _excelFile.SetCell($"H{i}", entry.SectorCode);
          _excelFile.SetCell($"I{i}", (decimal) entry.CurrentBalance);
          _excelFile.SetCell($"J{i}", entry.DebtorCreditor);
          _excelFile.SetCell($"K{i}", entry.LastChangeDate.ToString("dd/MMM/yyyy"));
          i++;
        } else if (entry.ItemType == TrialBalanceItemType.Group) {
          _excelFile.SetCell($"E{i}", entry.AccountName);
          _excelFile.SetCell($"I{i}", (decimal) entry.CurrentBalance);
          _excelFile.SetRowStyleBold(i);
          i++;
        }
      }
      if (!_query.WithSubledgerAccount) {
        _excelFile.RemoveColumn("G");
        _excelFile.RemoveColumn("F");
      }
    }


    private void SetRowsForSaldosCuentaConEncabezado(BalanceExplorerEntryDto entry, int i) {

      _excelFile.SetCell($"A{i}", entry.LedgerNumber);
      _excelFile.SetCell($"B{i}", entry.LedgerName);
      _excelFile.SetCell($"C{i}", $"({entry.CurrencyCode}) {entry.CurrencyName})");
      _excelFile.SetCell($"D{i}", entry.AccountNumber);
      if (!_query.WithSubledgerAccount) {
        _excelFile.SetCell($"E{i}", entry.AccountName);
      } else {
        _excelFile.SetCell($"E{i}", entry.SubledgerAccountName);
      }
      _excelFile.SetCell($"F{i}", entry.SectorCode);
      _excelFile.SetCell($"G{i}", (decimal) entry.CurrentBalance);
      _excelFile.SetCell($"H{i}", entry.LastChangeDate.ToString("dd/MMM/yyyy"));

    }


    private void SetRowHeaderByAccount(int i) {

      if (_query.TrialBalanceType == TrialBalanceType.SaldosPorAuxiliarConsultaRapida) {

        _excelFile.SetCell($"A{i}", "Deleg");
        _excelFile.SetCell($"B{i}", "Delegación");

        _excelFile.SetCell($"C{i}", "Moneda");

        _excelFile.SetCell($"D{i}", "Cuenta");
        _excelFile.SetCell($"E{i}", "Nombre de cuenta");

        _excelFile.SetCell($"F{i}", "Sector");
        _excelFile.SetCell($"G{i}", "Saldo actual");
        _excelFile.SetCell($"H{i}", "Último movimiento");

      }

      if (_query.TrialBalanceType == TrialBalanceType.SaldosPorCuentaConsultaRapida) {

        _excelFile.SetCell($"A{i}", "Deleg");
        _excelFile.SetCell($"B{i}", "Delegación");

        _excelFile.SetCell($"C{i}", "Moneda");

        _excelFile.SetCell($"D{i}", "Cuenta/Auxiliar");
        _excelFile.SetCell($"E{i}", "Nombre de cuenta/Auxiliar");

        _excelFile.SetCell($"F{i}", "Sector");
        _excelFile.SetCell($"G{i}", "Saldo actual");
        _excelFile.SetCell($"H{i}", "Último movimiento");

      }

      _excelFile.SetRowStyleBold(i);
    }

  } // class BalanceExplorerExcelExporter

} // namespace Empiria.FinancialAccounting.Reporting.Balances
