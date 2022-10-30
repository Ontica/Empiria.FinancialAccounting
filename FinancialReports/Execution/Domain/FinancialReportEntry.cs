/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Domain Layer                            *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Information Holder                      *
*  Type     : FinancialReportEntry                       License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Type used to handle all final report entries with dynamic totals fields.                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.FinancialReports {

  /// <summary>Type used to handle all final report entries with dynamic totals fields.</summary>
  public class FinancialReportEntry : DynamicFields {

    public decimal GetTotalField(FinancialReportTotalField field) {
      return GetTotalField(field.ToString());
    }

    public void SetTotalField(FinancialReportTotalField field, decimal value) {
      SetTotalField(field.ToString(), value);
    }

  }  // class FinancialReportEntry

}  // namespace Empiria.FinancialAccounting.FinancialReports
