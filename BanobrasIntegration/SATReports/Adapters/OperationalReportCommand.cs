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

namespace Empiria.FinancialAccounting.BanobrasIntegration.SATReports.Adapters {

  /// <summary>Command payload used to build operational reports.</summary>
  public class OperationalReportCommand {

    public string AccountsChartUID {
      get; set;
    }


    public OperationalReportType ReportType {
      get; set;
    }


    public DateTime FromDate {
      get; set;
    }


    public DateTime ToDate {
      get; set;
    }



  } // class OperationalReportCommand

} // namespace Empiria.FinancialAccounting.BanobrasIntegration.SATReports.Adapters
