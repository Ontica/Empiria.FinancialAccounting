/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                             Component : Interface adapters                  *
*  Assembly : FinancialAccounting.Reporting.dll              Pattern   : Data holder                         *
*  Type     : ListadoPolizasSqlClauses                       License   : Please read LICENSE.txt file        *
*                                                                                                            *
*  Summary  : Data holder with Sql clauses used to build 'Listado de polizas' report.                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Reporting.Data {

  /// <summary>Data holder with Sql clauses used to build 'Listado de polizas' report.</summary>
  internal class ListadoPolizasSqlClauses {

    internal AccountsChart AccountsChart {
      get; set;
    }


    internal DateTime FromDate {
      get; set;
    }


    internal DateTime ToDate {
      get; set;
    }


    internal string Ledgers {
      get; set;
    } = string.Empty;


    internal string Fields {
      get; set;
    } = string.Empty;


    internal string Filters {
      get; set;
    } = string.Empty;


    internal string Grouping {
      get; set;
    } = string.Empty;


  } // class ListadoPolizasSqlClauses

} // namespace Empiria.FinancialAccounting.Reporting.Data
