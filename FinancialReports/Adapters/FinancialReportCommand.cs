/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Interface adapters                      *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Command payload                         *
*  Type     : FinancialReportCommand                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Command payload used to build financial reports.                                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.FinancialReports.Adapters {

  /// <summary>Command payload used to build financial reports.</summary>
  public class FinancialReportCommand {

    public FinancialReportType FinancialReportType {
      get; set;
    }

    public string AccountsChartUID {
      get; set;
    }

    public DateTime Date {
      get; set;
    }

  } // class FinancialReportCommand


} // namespace Empiria.FinancialAccounting.FinancialReports.Adapters
