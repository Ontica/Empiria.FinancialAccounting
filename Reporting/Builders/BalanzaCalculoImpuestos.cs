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

      columns.Add(new DataTableColumn("moneda", "Moneda", "text"));
      columns.Add(new DataTableColumn("cuenta", "Cuenta", "text-nowrap"));
      columns.Add(new DataTableColumn("sector", "Sector", "text"));
      columns.Add(new DataTableColumn("descripcion", "Descripcion", "text"));
      
      columns.Add(new DataTableColumn("saldoInicial", "Saldo anterior", "decimal"));
      columns.Add(new DataTableColumn("debe", "Cargo", "decimal"));
      columns.Add(new DataTableColumn("haber", "Abono", "decimal"));
      columns.Add(new DataTableColumn("saldoFinal", "Saldo actual", "decimal"));

      columns.Add(new DataTableColumn("movimiento", "Movimiento", "text"));
      columns.Add(new DataTableColumn("contabilidad", "Contabilidad", "text"));
      columns.Add(new DataTableColumn("fechaConsulta", "Fecha consulta", "date"));

      columns.Add(new DataTableColumn("vBxcoEquivalencia", "VBxcoEquivalencia", "decimal", 6));
      columns.Add(new DataTableColumn("vBxcoSaldoInicial", "VBxco Saldo anterior", "decimal"));
      columns.Add(new DataTableColumn("vBxcoDebe", "VBxco Cargo", "decimal"));
      columns.Add(new DataTableColumn("vBxcoHaber", "VBxco Abono", "decimal"));
      columns.Add(new DataTableColumn("vBxcoSaldoFinal", "VBxco Saldo actual", "decimal"));

      return columns.ToFixedList();
    }


    static private TrialBalanceCommand GetTrialBalanceCommand(BuildReportCommand command) {
      return new TrialBalanceCommand {
        TrialBalanceType = TrialBalanceType.Balanza,
        AccountsChartUID = AccountsChart.Parse(command.AccountsChartUID).UID,
        BalancesType = BalancesType.WithCurrentBalanceOrMovements,
        UseDefaultValuation = true,
        ConsolidateBalancesToTargetCurrency = false,
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
        Entries = MapToReportDataEntries(trialBalance.Entries, command)
      };
    }


    static private FixedList<IReportEntryDto> MapToReportDataEntries(FixedList<ITrialBalanceEntryDto> list,
                                                                     BuildReportCommand command) {
      var mappedItems = list.Select((x) => MapToBalanzaCalculoImpuestosEntry((TrialBalanceEntryDto) x, command));

      return new FixedList<IReportEntryDto>(mappedItems);
    }


    static private BalanzaCalculoImpuestosEntry MapToBalanzaCalculoImpuestosEntry(TrialBalanceEntryDto entry,
                                                                                  BuildReportCommand command) {
      return new BalanzaCalculoImpuestosEntry {

        Moneda = entry.CurrencyCode,
        Cuenta = entry.AccountNumber,
        Sector = entry.SectorCode,
        Descripcion = entry.AccountName,

        SaldoInicial = entry.InitialBalance,
        Debe = entry.Debit,
        Haber = entry.Credit,
        SaldoFinal = (decimal) entry.CurrentBalance,
        Movimiento = entry.ItemType == TrialBalanceItemType.Entry &&
                     (entry.Debit != 0 || entry.Credit != 0) ?
                      "*" : "",

        Contabilidad = AccountsChart.Parse(command.AccountsChartUID).Name,
        FechaConsulta = DateTime.Now,

        VBxcoEquivalencia = entry.ExchangeRate,
        VBxcoSaldoInicial = entry.InitialBalance * entry.ExchangeRate,
        VBxcoDebe = entry.Debit * entry.ExchangeRate,
        VBxcoHaber = entry.Credit * entry.ExchangeRate,
        VBxcoSaldoFinal= (decimal) entry.CurrentBalance * entry.ExchangeRate
      };
    }

    #endregion Private methods

  }  // class BalanzaCalculoImpuestos


  public class BalanzaCalculoImpuestosEntry : IReportEntryDto {

    public string Moneda {
      get; internal set;
    }

    public string Cuenta {
      get; internal set;
    }

    public string Sector {
      get; internal set;
    }

    public string Descripcion {
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

    public string Movimiento {
      get; internal set;
    }

    public string Contabilidad {
      get; internal set;
    }

    public decimal VBxcoEquivalencia {
      get;
      internal set;
    }

    public decimal VBxcoSaldoInicial {
      get; internal set;
    }

    public decimal VBxcoDebe {
      get; internal set;
    }

    public decimal VBxcoHaber {
      get; internal set;
    }

    public decimal VBxcoSaldoFinal {
      get; internal set;
    }

    public DateTime FechaConsulta {
      get; internal set;
    } = DateTime.Now;

  }  // class BalanzaCalculoImpuestosEntry

}  // namespace Empiria.FinancialAccounting.Reporting.Builders
