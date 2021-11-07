/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                            Component : Report Builders                      *
*  Assembly : FinancialAccounting.Reporting.dll             Pattern   : Report builder                       *
*  Type     : BalanzaCalculoImpuestos                       License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Balanza para el cálculo de impuestos.                                                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.UseCases;

namespace Empiria.FinancialAccounting.Reporting.Builders {

  /// <summary>Balanza para el cálculo de impuestos.</summary>
  internal class BalanzaCalculoImpuestos : IReportBuilder {

    #region Public methods

    public ReportDataDto Build(BuildReportCommand command) {
      Assertion.AssertObject(command, "command");

      TrialBalanceCommand trialBalanceCommand = GetTrialBalanceCommand(command);

      using (var usecases = TrialBalanceUseCases.UseCaseInteractor()) {
        TrialBalanceDto trialBalance = usecases.BuildTrialBalance(trialBalanceCommand);

        return MapToReportDataDto(command, trialBalance);
      }

      throw new NotImplementedException();
    }

    #endregion Public methods

    #region Private methods

    static private FixedList<DataTableColumn> GetReportColumns() {
      var columns = new List<DataTableColumn>();

      columns.Add(new DataTableColumn("accountNumber", "Cuenta", "text"));
      columns.Add(new DataTableColumn("initialBalance", "Saldo Inicial", "decimal"));
      columns.Add(new DataTableColumn("debit", "Debe", "decimal"));
      columns.Add(new DataTableColumn("credit", "Haber", "decimal"));
      columns.Add(new DataTableColumn("currentBalance", "Saldo Final", "decimal"));

      return columns.ToFixedList();
    }


    static private TrialBalanceCommand GetTrialBalanceCommand(BuildReportCommand command) {
      return new TrialBalanceCommand {
        TrialBalanceType = TrialBalanceType.Balanza,
        AccountsChartUID = AccountsChart.Parse(command.AccountsChartUID).UID,
        BalancesType = BalancesType.WithCurrentBalanceOrMovements,
        UseDefaultValuation = true,
        ConsolidateBalancesToTargetCurrency = true,
        ShowCascadeBalances = false,
        InitialPeriod = new TrialBalanceCommandPeriod {
          FromDate = new DateTime(command.ToDate.Year, command.ToDate.Month, 1),
          ToDate = command.ToDate
        },
        IsOperationalReport = true,
      };
    }


    static private ReportDataDto MapToReportDataDto(BuildReportCommand command,
                                                    TrialBalanceDto trialBalance) {
      return new ReportDataDto {
        Command = command,
        Columns = GetReportColumns(),
        Entries = MapToReportDataEntries(trialBalance.Entries)
      };
    }


    static private FixedList<IReportEntryDto> MapToReportDataEntries(FixedList<ITrialBalanceEntryDto> list) {
      var mappedItems = list.Select((x) => MapToBalanzaCalculoImpuestosEntry((TrialBalanceEntryDto) x));

      return new FixedList<IReportEntryDto>(mappedItems);
    }


    static private BalanzaCalculoImpuestosEntry MapToBalanzaCalculoImpuestosEntry(TrialBalanceEntryDto entry) {
      return new BalanzaCalculoImpuestosEntry {
        Cuenta = entry.AccountNumber,
        SaldoInicial = entry.InitialBalance,
        Debe = entry.Debit,
        Haber = entry.Credit,
        SaldoFinal = entry.CurrentBalance
      };
    }

    #endregion Private methods

  }  // class BalanzaCalculoImpuestos


  public class BalanzaCalculoImpuestosEntry : IReportEntryDto {

    public string Cuenta {
      get; internal set;
    }


    public decimal SaldoInicial {
      get; internal set;
    }


    public decimal Debe {
      get; internal set;
    }


    public decimal Haber {
      get; internal set;
    }


    public decimal SaldoFinal {
      get; internal set;
    }

  }  // class BalanzaCalculoImpuestosEntry

}  // namespace Empiria.FinancialAccounting.Reporting.Builders
