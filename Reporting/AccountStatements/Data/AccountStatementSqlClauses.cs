/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                         Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Reporting.dll          Pattern   : Data Holder                             *
*  Type     : AccountStatementSqlClauses                 License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Contains data used to seek accounts statements information through Sql commands.               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Reporting.AccountStatements {

  /// <summary>Contains data used to seek accounts statements information through Sql commands.</summary>
  internal class AccountStatementSqlClauses {

    internal int AccountsChartId {
      get; set;
    }


    internal DateTime FromDate {
      get; set;
    }


    internal DateTime ToDate {
      get; set;
    }


    internal string Fields {
      get; set;
    } = string.Empty;


    internal string Filters {
      get; set;
    } = string.Empty;


    internal string Grouping {
      get; set;
    } = string.Empty;


    internal string Ordering {
      get; set;
    } = string.Empty;


  } // class AccountStatementSqlClauses

} // namespace Empiria.FinancialAccounting.Reporting.Data.Adapters
