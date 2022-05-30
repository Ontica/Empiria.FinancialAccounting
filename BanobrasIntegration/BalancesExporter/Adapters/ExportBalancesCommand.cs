/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                Component : Balances Exporter                     *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll  Pattern   : Command payload                       *
*  Type     : ExportBalancesCommand                        License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Command payload used to export balances to other systems.                                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.BanobrasIntegration.BalancesExporter.Adapters {

  public enum StoreBalancesInto {

    None,

    Diario,

    MensualConsolidado,

    MensualPorContabilidad,

    SaldosRentabilidad,

    SaldosSIC

  }

  /// <summary>Command payload used to export balances to other systems.</summary>
  public class ExportBalancesCommand {

    #region Fields


    public int AccountsChartId {
      get; set;
    }


    public DateTime FromDate {
      get; set;
    } = ExecutionServer.DateMinValue;


    public DateTime ToDate {
      get; set;
    }


    public bool BreakdownLedgers {
      get; internal set;
    }


    public StoreBalancesInto StoreInto {
      get; set;
    } = StoreBalancesInto.None;


    #endregion Fields

    #region Methods

    private DateTime DetermineFromDate() {
      if (this.FromDate != ExecutionServer.DateMinValue) {
        return this.FromDate;
      }

      if (StoreInto == StoreBalancesInto.Diario) {
        return this.ToDate;
      }

      return new DateTime(this.ToDate.Year, this.ToDate.Month, 1);
    }


    public TrialBalanceQuery MapToTrialBalanceQuery() {
      return new TrialBalanceQuery {
        TrialBalanceType = TrialBalanceType.Balanza,
        AccountsChartUID = AccountsChart.Parse(this.AccountsChartId).UID,
        BalancesType = BalancesType.WithCurrentBalanceOrMovements,
        ShowCascadeBalances = BreakdownLedgers,
        WithAverageBalance = true,
        WithSubledgerAccount = true,
        InitialPeriod = new BalancesPeriod {
          FromDate = DetermineFromDate(),
          ToDate = this.ToDate
        },
      };
    }

    #endregion Methods

  } // class ExportBalancesCommand


  public class BanobrasExportBalancesCommand {

    public int Empresa {
      get; set;
    }


    public DateTime Fecha {
      get; set;
    }


    public bool Consolidado {
      get; set;
    } = true;


    public ExportBalancesCommand ConvertToExportBalancesCommandByDay() {
      return new ExportBalancesCommand {
        AccountsChartId = this.Empresa,
        BreakdownLedgers = !this.Consolidado,
        FromDate = this.Fecha,
        ToDate = this.Fecha
      };
    }


    public ExportBalancesCommand ConvertToExportBalancesCommandByMonth() {
      return new ExportBalancesCommand {
        AccountsChartId = this.Empresa,
        BreakdownLedgers = !this.Consolidado,
        FromDate = new DateTime(this.Fecha.Year, this.Fecha.Month, 1),
        ToDate = new DateTime(this.Fecha.Year, this.Fecha.Month,
                              DateTime.DaysInMonth(this.Fecha.Year, this.Fecha.Month)),
      };
    }

  }   // BanobrasExportBalancesCommand

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.BalancesExporter.Adapters
