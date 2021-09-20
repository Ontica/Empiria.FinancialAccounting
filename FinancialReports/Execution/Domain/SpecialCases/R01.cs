/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Domain Layer                            *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Service provider                        *
*  Type     : R01                                        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Generador del reporte regulatorio R01.                                                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.UseCases;

using Empiria.FinancialAccounting.FinancialReports.Adapters;

namespace Empiria.FinancialAccounting.FinancialReports {

  /// <summary>Generador del reporte regulatorio R01.</summary>
  internal class R01 {

    private readonly FinancialReportCommand _command;

    internal R01(FinancialReportCommand command) {
      _command = command;
    }


    internal FinancialReport Generate() {
      FixedList<FinancialReportRow> rows = GetRows();

      FixedList<FinancialReportEntry> entries = GetEntries(rows);

      // var balances = GetBalances();

      // ProcessEntries(entries, balances.Entries);
      ProcessEntries(entries, null);

      return new FinancialReport(_command, entries);
    }


    private TrialBalanceDto GetBalances() {
      TrialBalanceCommand trialBalanceCommand = GetTrialBalanceCommand();

      using (var usecases = TrialBalanceUseCases.UseCaseInteractor()) {
        return usecases.BuildTrialBalance(trialBalanceCommand);
      }
    }

    private TrialBalanceCommand GetTrialBalanceCommand() {
      return new TrialBalanceCommand {
        AccountsChartUID = _command.AccountsChartUID,
        TrialBalanceType = BalanceEngine.TrialBalanceType.AnaliticoDeCuentas,
        UseDefaultValuation = true,
        ShowCascadeBalances = false,
        WithSubledgerAccount = false,
        InitialPeriod = new TrialBalanceCommandPeriod {
          FromDate = _command.Date,
          ToDate = _command.Date
        }
      };
    }

    private FixedList<FinancialReportEntry> GetEntries(FixedList<FinancialReportRow> rows) {
      var enumeration = rows.Select(x => new FinancialReportEntry { GroupingRule = x.GroupingRule });

      var entries = new FixedList<FinancialReportEntry>(enumeration);

      return entries;
    }


    private FixedList<FinancialReportRow> GetRows() {
      return _command.GetFinancialReportType().GetRows();
    }

    private void ProcessEntries(FixedList<FinancialReportEntry> entries,
                                FixedList<ITrialBalanceEntryDto> balances) {
      foreach (var entry in entries) {
        entry.DomesticCurrencyTotal = entry.GroupingRule.Items.Count * 100m;
        entry.ForeignCurrencyTotal = 5678;
        entry.Total = 12345678m;
      }
    }

  }  // class R01

}  // namespace Empiria.FinancialAccounting.FinancialReports
