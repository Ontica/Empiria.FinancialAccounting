/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                 Component : Interface adapters                   *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll   Pattern   : Command payload                      *
*  Type     : OperationalReportCommand                      License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Command payload used to build operational reports.                                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.FinancialAccounting.Adapters;
using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.BanobrasIntegration.SATReports.Adapters {

  /// <summary>Command payload used to build operational reports.</summary>
  public class OperationalReportCommand {

    public string AccountsChartUID {
      get; set;
    } = "b2328e67-3f2e-45b9-b1f6-93ef6292204e";


    public OperationalReportType ReportType {
      get; set;
    }

    public FileType FileType {
      get; set;
    } = FileType.Excel;

    public DateTime Date {
      get; set;
    }


    public TrialBalanceCommand MapToTrialBalanceCommand() {
      return new TrialBalanceCommand {
        TrialBalanceType = TrialBalanceType.Balanza,
        AccountsChartUID = AccountsChart.Parse(this.AccountsChartUID).UID,
        BalancesType = BalancesType.WithCurrentBalanceOrMovements,
        UseDefaultValuation = true,
        ConsolidateBalancesToTargetCurrency = true,
        ShowCascadeBalances = false,
        InitialPeriod = new TrialBalanceCommandPeriod {
          FromDate = new DateTime(this.Date.Year, this.Date.Month, 1),
          ToDate = this.Date
        },
        IsOperationalReport = true,
      };
    }

    internal AccountsSearchCommand MapToAccountsSearchCommand() {
      return new AccountsSearchCommand{
        Date = this.Date
      };
    }
  } // class OperationalReportCommand

} // namespace Empiria.FinancialAccounting.BanobrasIntegration.SATReports.Adapters
