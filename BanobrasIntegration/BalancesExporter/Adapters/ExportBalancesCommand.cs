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

  /// <summary>Command payload used to export balances to other systems.</summary>
  public class ExportBalancesCommand {

    #region Fields

    public string BalanceType {
      get; set;
    }


    public int Empresa {
      get; set;
    }


    public DateTime FechaInicio {
      get; set;
    }

    public DateTime FechaFin {
      get; set;
    }


    public bool BreakdownLedgers {
      get; internal set;
    }


    public bool GuardarSaldos {
      get; set;
    }

    #endregion Fields

    #region Methods

    public TrialBalanceCommand MapToTrialBalanceCommandForBalancesByDay() {
      return new TrialBalanceCommand {
        TrialBalanceType = TrialBalanceType.Balanza,
        AccountsChartUID = AccountsChart.Parse(this.Empresa).UID,
        BalancesType = BalancesType.WithCurrentBalanceOrMovements,
        ShowCascadeBalances = BreakdownLedgers,
        WithAverageBalance = true,
        WithSubledgerAccount = true,
        InitialPeriod = new TrialBalanceCommandPeriod {
          FromDate = this.FechaFin,
          ToDate = this.FechaFin
        },
      };
    }


    public TrialBalanceCommand MapToTrialBalanceCommandForBalancesByMonth() {
      return new TrialBalanceCommand {
        TrialBalanceType = TrialBalanceType.Balanza,
        AccountsChartUID = AccountsChart.Parse(this.Empresa).UID,
        BalancesType = BalancesType.WithCurrentBalanceOrMovements,
        ShowCascadeBalances = BreakdownLedgers,
        WithAverageBalance = true,
        WithSubledgerAccount = true,
        InitialPeriod = new TrialBalanceCommandPeriod {
          FromDate = new DateTime(this.FechaFin.Year, this.FechaFin.Month, 1),
          ToDate = new DateTime(this.FechaFin.Year, this.FechaFin.Month,
                                DateTime.DaysInMonth(this.FechaFin.Year, this.FechaFin.Month))
        },
      };
    }

    public TrialBalanceCommand MapToTrialBalanceCommandForBalancesByPeriod() {
      return new TrialBalanceCommand {
        TrialBalanceType = TrialBalanceType.Balanza,
        AccountsChartUID = AccountsChart.Parse(this.Empresa).UID,
        BalancesType = BalancesType.WithCurrentBalanceOrMovements,
        ShowCascadeBalances = BreakdownLedgers,
        WithAverageBalance = true,
        WithSubledgerAccount = true,
        InitialPeriod = new TrialBalanceCommandPeriod {
          FromDate = this.FechaInicio,
          ToDate = this.FechaFin
        },
      };
    }

    #endregion Methods

  } // class ExportBalancesCommand

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.BalancesExporter.Adapters
