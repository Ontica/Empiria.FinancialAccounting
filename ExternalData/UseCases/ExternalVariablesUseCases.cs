﻿/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : External Data                              Component : Use cases Layer                         *
*  Assembly : FinancialAccounting.ExternalData.dll       Pattern   : Use case interactor class               *
*  Type     : ExternalVariablesUseCases                  License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases used to create and update financial external variables.                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.FinancialAccounting.ExternalData.Adapters;

namespace Empiria.FinancialAccounting.ExternalData.UseCases {

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


    public ExternalVariableDto AddVariable(string setUID, ExternalVariableFields fields) {
      Assertion.Require(setUID, nameof(setUID));
      Assertion.Require(fields, nameof(fields));

      ExternalVariablesSet set = ExternalVariablesSet.Parse(setUID);

      ExternalVariable variable = set.AddVariable(fields);

      variable.Save();

      return ExternalVariableMapper.Map(variable);
    }


    public FixedList<ExternalVariableDto> GetVariables(string setUID, DateTime date) {
      Assertion.Require(setUID, nameof(setUID));

      ExternalVariablesSet set = ExternalVariablesSet.Parse(setUID);

      return ExternalVariableMapper.Map(set.GetVariables(date));
    }


    public FixedList<ExternalVariablesSetDto> GetVariablesSets() {
      FixedList<ExternalVariablesSet> sets = ExternalVariablesSet.GetList();

      sets = base.RestrictUserDataAccessTo(sets);

      return ExternalVariableMapper.Map(sets);
    }


    public void RemoveVariable(string setUID, string variableUID) {
      Assertion.Require(setUID, nameof(setUID));
      Assertion.Require(variableUID, nameof(variableUID));

      ExternalVariablesSet set = ExternalVariablesSet.Parse(setUID);

      ExternalVariable variable = set.GetVariable(variableUID);

      set.DeleteVariable(variable);

      variable.Save();
    }


    public ExternalVariableDto UpdateVariable(string setUID,
                                              string variableUID,
                                              ExternalVariableFields fields) {
      Assertion.Require(setUID, nameof(setUID));
      Assertion.Require(variableUID, nameof(variableUID));
      Assertion.Require(fields, nameof(fields));

      ExternalVariablesSet set = ExternalVariablesSet.Parse(setUID);

      ExternalVariable variable = set.GetVariable(variableUID);

      set.UpdateVariable(variable, fields);

      variable.Save();

      return ExternalVariableMapper.Map(variable);
    }

    #endregion Use cases

  }  // class ExternalVariablesUseCases

}  // namespace Empiria.FinancialAccounting.ExternalData.UseCases
