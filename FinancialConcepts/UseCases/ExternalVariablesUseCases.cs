/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Concepts                         Component : Use cases Layer                         *
*  Assembly : FinancialAccounting.FinancialConcepts.dll  Pattern   : Use case interactor class               *
*  Type     : ExternalVariablesUseCases                  License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases used to create and update financial external variables.                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.FinancialAccounting.FinancialConcepts.Adapters;

namespace Empiria.FinancialAccounting.FinancialConcepts.UseCases {

  /// <summary>Use cases used to create and update financial external variables.</summary>
  public class ExternalVariablesUseCases : UseCase {

    #region Constructors and parsers

    protected ExternalVariablesUseCases() {
      // no-op
    }

    static public ExternalVariablesUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<ExternalVariablesUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases


    public FixedList<ExternalVariableDto> GetExternalVariables(string setUID) {
      Assertion.Require(setUID, nameof(setUID));

      ExternalVariablesSet set = ExternalVariablesSet.Parse(setUID);

      return ExternalVariableMapper.Map(set.ExternalVariables);
    }


    public FixedList<ExternalVariablesSetDto> GetExternalVariablesSets() {
      FixedList<ExternalVariablesSet> sets = ExternalVariablesSet.GetList();

      return ExternalVariableMapper.Map(sets);
    }


    #endregion Use cases

  }  // class ExternalVariablesUseCases

}  // namespace Empiria.FinancialAccounting.FinancialConcepts.UseCases
