/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                         Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Reporting.dll          Pattern   : Build query payload                     *
*  Type     : ListadoPolizasQuery                        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Query payload used to build voucher list report.                                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Reporting.VoucherRelatedReports.Adapters {

  /// <summary> Query payload used to build voucher list report.</summary>
  public class ListadoPolizasQuery {


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


  } // class ListadoPolizasQuery

} // namespace Empiria.FinancialAccounting.Reporting.VoucherRelatedReports.Adapters
