/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                             Component : Interface adapters                  *
*  Assembly : FinancialAccounting.Reporting.dll              Pattern   : Command payload                     *
*  Type     : PolizaCommandData                              License   : Please read LICENSE.txt file        *
*                                                                                                            *
*  Summary  : Command payload used to build vouchers report.                                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Reporting {

  /// <summary>Command payload used to build vouchers report.</summary>
  internal class PolizaCommandData {

    public AccountsChart AccountsChart {
      get; internal set;
    }

    public DateTime FromDate {
      get; internal set;
    }

    public DateTime ToDate {
      get; internal set;
    }

    public string Ledgers {
      get; internal set;
    } = string.Empty;

    public string Filters {
      get; internal set;
    } = string.Empty;

  } // class PolizaCommandData

} // namespace Empiria.FinancialAccounting.Reporting
