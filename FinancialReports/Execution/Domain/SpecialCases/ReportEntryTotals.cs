/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Domain Layer                            *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Information holder                      *
*  Type     : ReportEntryTotals                          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Holds information about a financial report entry total.                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.FinancialConcepts;

namespace Empiria.FinancialAccounting.FinancialReports {

  /// <summary>Holds information about a financial report entry total.</summary>
  abstract internal class ReportEntryTotals {

    #region Methods

    public abstract ReportEntryTotals AbsoluteValue();

    public abstract void CopyTotalsTo(FinancialReportEntry copyTo);

    public abstract ReportEntryTotals Round();

    public abstract ReportEntryTotals Substract(ReportEntryTotals total, string qualification);

    public abstract ReportEntryTotals Substract(ITrialBalanceEntryDto balance, string qualification);

    public abstract ReportEntryTotals Sum(ReportEntryTotals total, string qualification);

    public abstract ReportEntryTotals Sum(ITrialBalanceEntryDto balance, string qualification);

    public abstract ReportEntryTotals Sum(ExternalValue value, string qualification);

    public abstract ReportEntryTotals SumDebitsOrSubstractCredits(ITrialBalanceEntryDto balance, string qualification);

    #endregion Methods

  }  // class ReportEntryTotals

}  // namespace Empiria.FinancialAccounting.FinancialReports
