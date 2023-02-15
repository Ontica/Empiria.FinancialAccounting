/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Interface adapters                      *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Build query payload                     *
*  Type     : FinancialReportQuery                       License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Query payload used to build financial reports.                                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.FinancialReports.Adapters {

  /// <summary>Query payload used to build financial reports.</summary>
  public class FinancialReportQuery {

    public string FinancialReportType {
      get; set;
    }


    public string AccountsChartUID {
      get; set;
    }


    public DateTime FromDate {
      get; set;
    }


    public DateTime ToDate {
      get; set;
    } = ExecutionServer.DateMinValue;


    public bool GetAccountsIntegration {
      get; set;
    }


    public string ExportTo {
      get; set;
    }


    internal FinancialReportQuery Clone() {
      return (FinancialReportQuery) this.MemberwiseClone();
    }

    public FinancialReportType GetFinancialReportType() {
      return FinancialReports.FinancialReportType.Parse(this.FinancialReportType);
    }

  } // class FinancialReportQuery

} // namespace Empiria.FinancialAccounting.FinancialReports.Adapters
