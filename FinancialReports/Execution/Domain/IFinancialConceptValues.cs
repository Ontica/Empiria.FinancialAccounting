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

    IFinancialConceptValues ChangeSign();

    IFinancialConceptValues ConsolidateTotalsInto(string consolidatedFieldName);

    void CopyTotalsTo(DynamicFields copyTo);

    IFinancialConceptValues Round(RoundTo roundTo);

    IFinancialConceptValues Substract(IFinancialConceptValues values);

    IFinancialConceptValues Substract(ITrialBalanceEntryDto balance);

    IFinancialConceptValues Sum(IFinancialConceptValues values);

    IFinancialConceptValues Sum(ITrialBalanceEntryDto balance);

    IFinancialConceptValues Sum(ExternalValue value);

    IFinancialConceptValues SumDebitsOrSubstractCredits(ITrialBalanceEntryDto balance);

    #endregion Methods

  }  // class IFinancialConceptValues

}  // namespace Empiria.FinancialAccounting.FinancialReports
