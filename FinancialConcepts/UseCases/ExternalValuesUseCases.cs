/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Concepts                         Component : Use cases Layer                         *
*  Assembly : FinancialAccounting.FinancialConcepts.dll  Pattern   : Use case interactor class               *
*  Type     : ExternalValuesUseCases                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases used to update and retrieve financial external values.                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.FinancialAccounting.FinancialConcepts.Adapters;

namespace Empiria.FinancialAccounting.FinancialConcepts.UseCases {

  /// <summary>Use cases used to update and retrieve financial external values.</summary>
  public class ExternalValuesUseCases : UseCase {

    #region Constructors and parsers

    protected ExternalValuesUseCases() {
      // no-op
    }

    static public ExternalValuesUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<ExternalValuesUseCases>();
    }


    #endregion Constructors and parsers

    #region Use cases

    public ExternalValuesDto GetExternalValues(ExternalValuesQuery query) {
      Assertion.Require(query, nameof(query));

      query.EnsureValid();

      ExternalVariablesSet variablesSet = query.GetExternalVariablesSet();

      ExternalValuesDataSet dataSet = ExternalValuesDataSet.Parse(variablesSet, query.Date);

      return ExternalValuesDataSetMapper.Map(query, dataSet);
    }

    #endregion Use cases

  }  // class ExternalValuesUseCases

}  // namespace Empiria.FinancialAccounting.FinancialConcepts.UseCases
