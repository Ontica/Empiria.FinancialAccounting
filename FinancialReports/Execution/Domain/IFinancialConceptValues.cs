/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Domain Layer                            *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Interface                               *
*  Type     : IFinancialConceptValues                    License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Interface for types that hold and operate financial concepts values.                           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;

using Empiria.FinancialAccounting.ExternalData;

namespace Empiria.FinancialAccounting.FinancialReports {

  /// <summary>Interface for types that hold and operate financial concepts values.</summary>
  internal interface IFinancialConceptValues {

    #region Methods

    IFinancialConceptValues AbsoluteValue();

    void CopyTotalsTo(FinancialReportEntry copyTo);

    IFinancialConceptValues Round();

    IFinancialConceptValues Substract(IFinancialConceptValues total, string dataColumn);

    IFinancialConceptValues Substract(ITrialBalanceEntryDto balance, string dataColumn);

    IFinancialConceptValues Sum(IFinancialConceptValues total, string dataColumn);

    IFinancialConceptValues Sum(ITrialBalanceEntryDto balance, string dataColumn);

    IFinancialConceptValues Sum(ExternalValue value, string dataColumn);

    IFinancialConceptValues SumDebitsOrSubstractCredits(ITrialBalanceEntryDto balance, string dataColumn);

    #endregion Methods

  }  // class IFinancialConceptValues

}  // namespace Empiria.FinancialAccounting.FinancialReports
