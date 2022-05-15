/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                            Component : Report Builders                      *
*  Assembly : FinancialAccounting.Reporting.dll             Pattern   : Report builder                       *
*  Type     : BalanzaSat                                    License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Balanza para la contabilidad electrónica del SAT.                                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.UseCases;

namespace Empiria.FinancialAccounting.Reporting.Builders {


  /// <summary>Balanza para la contabilidad electrónica del SAT.</summary>
  internal class BalanzaSat : IReportBuilder {

    #region Public methods

    public ReportDataDto Build(BuildReportCommand command) {
      Assertion.AssertObject(command, "command");

      TrialBalanceCommand trialBalanceCommand = this.GetTrialBalanceCommand(command);

      using (var usecases = TrialBalanceUseCases.UseCaseInteractor()) {
        TrialBalanceDto trialBalance = usecases.BuildTrialBalance(trialBalanceCommand);

        return MapToReportDataDto(command, trialBalance);
      }
    }


    #endregion Public methods


    #region Private methods


    static private FixedList<DataTableColumn> GetReportColumns(BuildReportCommand command) {
      List<DataTableColumn> columns = new List<DataTableColumn>();

      columns.Add(new DataTableColumn("cuenta", "Cuenta", "text-nowrap"));
      columns.Add(new DataTableColumn("saldoInicial", "Saldo Inicial", "decimal"));
      columns.Add(new DataTableColumn("debe", "Debe", "decimal"));
      columns.Add(new DataTableColumn("haber", "Haber", "decimal"));
      columns.Add(new DataTableColumn("saldoFinal", "Saldo Final", "decimal"));

      if (command.SendType == SendType.C) {
        columns.Add(new DataTableColumn("fechaModificacion", "Fecha Modificación", "date"));
      }

      return columns.ToFixedList();
    }



    private TrialBalanceCommand GetTrialBalanceCommand(BuildReportCommand command) {
      return new TrialBalanceCommand {
        TrialBalanceType = TrialBalanceType.Balanza,
        AccountsChartUID = AccountsChart.Parse(command.AccountsChartUID).UID,
        BalancesType = BalancesType.WithCurrentBalanceOrMovements,
        UseDefaultValuation = true,
        ConsolidateBalancesToTargetCurrency = true,
        ShowCascadeBalances = false,
        WithAverageBalance = true,
        InitialPeriod = new BalanceEngineCommandPeriod {
          FromDate = new DateTime(command.ToDate.Year, command.ToDate.Month, 1),
          ToDate = command.ToDate
        },
        IsOperationalReport = true,
      };
    }


    static private BalanzaSatEntry MapToBalanzaSATEntry(TrialBalanceEntryDto entry) {
      return new BalanzaSatEntry {
        Cuenta = entry.AccountNumber,
        SaldoInicial = entry.InitialBalance,
        Debe = entry.Debit,
        Haber = entry.Credit,
        SaldoFinal = (decimal) entry.CurrentBalance,
        FechaModificacion = entry.LastChangeDate
      };
    }


    static private ReportDataDto MapToReportDataDto(BuildReportCommand command,
                                                    TrialBalanceDto trialBalance) {
      return new ReportDataDto {
        Command = command,
        Columns = GetReportColumns(command),
        Entries = MapToReportDataEntries(trialBalance.Entries)
      };
    }


    static private FixedList<IReportEntryDto> MapToReportDataEntries(FixedList<ITrialBalanceEntryDto> list) {
      var mappedItems = list.Select((x) => MapToBalanzaSATEntry((TrialBalanceEntryDto) x));

      return new FixedList<IReportEntryDto>(mappedItems);
    }


    #endregion Private methods

  }  // class BalanzaSat


  public class BalanzaSatEntry : IReportEntryDto {

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


    public DateTime FechaModificacion {
      get;
      internal set;
    }

  }  // class BalanzaSatEntry

}  // namespace Empiria.FinancialAccounting.Reporting.Builders
