/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Domain Layer                            *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Information Holder                      *
*  Type     : ReportRowFields                            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Fields holder used to update financial report rows.                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.FinancialAccounting.FinancialReports {

  /// <summary>Fields holder used to update financial report rows.</summary>
  internal class ReportRowFields {

    internal string Label {
      get; set;
    }

    internal int Row {
      get; set;
    }

  }  // class ReportRowFields

}  // namespace Empiria.FinancialAccounting.FinancialReports
