/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                           Component : Excel Exporters                       *
*  Assembly : FinancialAccounting.Reporting.dll            Pattern   : IExcelExporter                        *
*  Type     : BalanceExcelExporter                         License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Creates a Microsoft Excel file with balance information.                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.Reporting.Exporters.Excel {

  /// <summary>Creates a Microsoft Excel file with balance information.</summary>
  internal class BalanceExcelExporter {

    private BalanceCommand _command = new BalanceCommand();
    private readonly ExcelTemplateConfig _templateConfig;
    private ExcelFile _excelFile;

    public BalanceExcelExporter(ExcelTemplateConfig templateConfig) {
      Assertion.AssertObject(templateConfig, "templateConfig");

      _templateConfig = templateConfig;
    }


    internal ExcelFile CreateExcelFile(BalanceDto balance) {
      Assertion.AssertObject(balance, "balance");

      _command = balance.Command;

      _excelFile = new ExcelFile(_templateConfig);

      _excelFile.Open();

      SetHeader();

      SetTable(balance);

      _excelFile.Save();

      _excelFile.Close();

      return _excelFile;
    }


    private void SetHeader() {
      _excelFile.SetCell($"A2", _templateConfig.Title);

      var subTitle = $"Del {_command.InitialPeriod.FromDate.ToString("dd/MMM/yyyy")} " +
                     $"al {_command.InitialPeriod.ToDate.ToString("dd/MMM/yyyy")}";

      _excelFile.SetCell($"A3", subTitle);
    }


    private void SetTable(BalanceDto balance) {
      switch (balance.Command.TrialBalanceType) {
        case TrialBalanceType.SaldosPorAuxiliarConsultaRapida:
          FillOutSaldosAuxiliar(balance.Entries.Select(x => (BalanceEntryDto) x));
          return;

        case TrialBalanceType.SaldosPorCuentaConsultaRapida:
          FillOutSaldosCuenta(balance.Entries.Select(x => (BalanceEntryDto) x));
          return;

        default:
          throw Assertion.AssertNoReachThisCode();
      }
    }

    private void FillOutSaldosCuenta(IEnumerable<BalanceEntryDto> entries) {
      int i = 5;
      foreach (var entry in entries) {

        if (entry.ItemType == TrialBalanceItemType.Total) {
          _excelFile.SetCell($"C{i}", entry.AccountNumber);
          _excelFile.SetCell($"D{i}", $"{entry.AccountName}");
          _excelFile.SetCell($"F{i}", entry.CurrentBalance.ToString());
          _excelFile.SetCell($"G{i}", "");
          _excelFile.SetRowStyleBold(i);

          i++;
          SetRowHeaderByAccount(i);

        } else if (entry.ItemType == TrialBalanceItemType.Group) {
          _excelFile.SetCell($"D{i}", entry.AccountName);
          _excelFile.SetCell($"F{i}", entry.CurrentBalance.ToString());
          _excelFile.SetRowStyleBold(i);
          i += 3;

        } else {
          _excelFile.SetCell($"A{i}", entry.LedgerNumber);
          _excelFile.SetCell($"B{i}", entry.LedgerName);
          _excelFile.SetCell($"C{i}", entry.AccountNumber);
          _excelFile.SetCell($"D{i}", entry.AccountName);
          _excelFile.SetCell($"E{i}", entry.SectorCode);
          _excelFile.SetCell($"F{i}", entry.CurrentBalance.ToString());
          _excelFile.SetCell($"G{i}", entry.LastChangeDate.ToString("dd/MMM/yyyy"));
        }

        i++;
      }
    }


    private void SetRowHeaderByAccount(int i) {
      _excelFile.SetCell($"A{i}", "Deleg");
      _excelFile.SetCell($"B{i}", "Delegación");
      _excelFile.SetCell($"C{i}", "Cuenta/Auxiliar");
      _excelFile.SetCell($"D{i}", "Nombre de cuenta/Auxiliar");
      _excelFile.SetCell($"E{i}", "Sector");
      _excelFile.SetCell($"F{i}", "Saldo actual");
      _excelFile.SetCell($"G{i}", "Último movimiento");
    }

    private void FillOutSaldosAuxiliar(IEnumerable<BalanceEntryDto> entries) {
      int i = 4;
      foreach (var entry in entries) {
        if (entry.ItemType == TrialBalanceItemType.Summary) {
          _excelFile.SetCell($"C{i}", entry.AccountNumber);
          _excelFile.SetCell($"D{i}", $"{entry.AccountName}, Naturaleza {entry.DebtorCreditor}");
          _excelFile.SetCell($"F{i}", entry.CurrentBalance.ToString());
          _excelFile.SetCell($"G{i}", "");
          _excelFile.SetRowStyleBold(i);

          i++;

        } else {
          _excelFile.SetCell($"A{i}", entry.LedgerNumber);
          _excelFile.SetCell($"B{i}", entry.LedgerName);
          _excelFile.SetCell($"C{i}", entry.AccountNumber);
          _excelFile.SetCell($"D{i}", entry.AccountName);
          _excelFile.SetCell($"E{i}", entry.SectorCode);
          _excelFile.SetCell($"F{i}", entry.CurrentBalance.ToString());
          _excelFile.SetCell($"G{i}", entry.LastChangeDate.ToString("dd/MMM/yyyy"));
        }

        i++;
      }
    }
  } // class BalanceExcelExporter

} // namespace Empiria.FinancialAccounting.Reporting.Exporters.Excel
