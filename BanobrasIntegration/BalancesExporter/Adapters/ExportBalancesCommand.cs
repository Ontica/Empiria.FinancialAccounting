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

    MensualConsolidado,

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
    }

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


    public TrialBalanceCommand MapToTrialBalanceCommand() {
      return new TrialBalanceCommand {
        TrialBalanceType = TrialBalanceType.Balanza,
        AccountsChartUID = AccountsChart.Parse(this.AccountsChartId).UID,
        BalancesType = BalancesType.WithCurrentBalanceOrMovements,
        ShowCascadeBalances = BreakdownLedgers,
        WithAverageBalance = true,
        WithSubledgerAccount = true,
        InitialPeriod = new TrialBalanceCommandPeriod {
          FromDate = this.FromDate,
          ToDate = this.ToDate
        },
      };
    }

    #endregion Methods

  } // class ExportBalancesCommand


  public class ExportBalancesCommandBanobras {

    public int Empresa {
      get; set;
    }


    public DateTime Fecha {
      get; set;
    }


    public ExportBalancesCommand ConvertToExportBalancesCommandByDay() {
      return new ExportBalancesCommand {
        AccountsChartId = this.Empresa,
        BreakdownLedgers = false,
        FromDate = this.Fecha,
        ToDate = this.Fecha
      };
    }


    public ExportBalancesCommand ConvertToExportBalancesCommandByMonth() {
      return new ExportBalancesCommand {
        AccountsChartId = this.Empresa,
        BreakdownLedgers = false,
        FromDate = new DateTime(this.Fecha.Year, this.Fecha.Month, 1),
        ToDate = new DateTime(this.Fecha.Year, this.Fecha.Month,
                                DateTime.DaysInMonth(this.Fecha.Year, this.Fecha.Month)),
      };
    }

  }   // ExportBalancesCommandBanobras

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.BalancesExporter.Adapters
