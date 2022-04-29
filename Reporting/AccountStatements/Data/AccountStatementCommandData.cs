/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                         Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Reporting.dll          Pattern   : Command payload                         *
*  Type     : AccountStatementCommandData                License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Command payload used to generate account's statements.                                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Reporting.Data {

  /// <summary>Command payload used to generate account's statements.</summary>
  internal class AccountStatementCommandData {

    public int AccountsChartId {
      get;
      internal set;
    }


    public DateTime FromDate {
      get; internal set;
    }


    public DateTime ToDate {
      get; internal set;
    }


    public string Fields {
      get; internal set;
    } = string.Empty;


    public string Filters {
      get; set;
    } = string.Empty;


    public string Grouping {
      get; internal set;
    } = string.Empty;


    public string Ordering {
      get; internal set;
    } = string.Empty;


  } // class AccountStatementCommandData

} // namespace Empiria.FinancialAccounting.Reporting.Data.Adapters
