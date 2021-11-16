/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                         Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Reporting.dll          Pattern   : Command payload                         *
*  Type     : PolizaCommand                              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Command payload used to build policies report.                                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Reporting.Adapters {

  /// <summary>Command payload used to build policies report.</summary>
  public class PolizasCommand {


    public string AccountsChartUID {
      get; set;
    }


    public string[] Ledgers {
      get; set;
    } = new string[0];


    public DateTime FromDate {
      get; set;
    }


    public DateTime ToDate {
      get; set;
    }


  } // class PolizaCommand

} // namespace Empiria.FinancialAccounting.Reporting.Adapters
