/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Concepts                         Component : Use cases Layer                         *
*  Assembly : FinancialAccounting.FinancialConcepts.dll  Pattern   : Use case interactor class               *
*  Type     : FinancialConceptsUseCases                  License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases used to create and update financial concepts.                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.FinancialAccounting.FinancialConcepts.Adapters;

namespace Empiria.FinancialAccounting.FinancialConcepts.UseCases {

  /// <summary>Use cases used to create and update financial concepts.</summary>
  public class FinancialConceptsUseCases : UseCase {

    #region Constructors and parsers

    protected FinancialConceptsUseCases() {
      // no-op
    }

    static public FinancialConceptsUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<FinancialConceptsUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases

    public FinancialConceptDto GetFinancialConcept(string financialConceptUID) {
      Assertion.Require(financialConceptUID, nameof(financialConceptUID));

      var financialConcept = FinancialConcept.Parse(financialConceptUID);

      return FinancialConceptMapper.Map(financialConcept);
    }


    public FinancialConceptDto InsertFinancialConcept(EditFinancialConceptCommand command) {
      Assertion.Require(command, nameof(command));

      command.EnsureIsValid();

      var group = FinancialConceptGroup.Parse(command.GroupUID);

      base.EnsureUserHasDataAccessTo(group);

      FinancialConcept concept = group.InsertFrom(command);

      concept.Save();

      return FinancialConceptMapper.Map(concept);
    }


    public void RemoveFinancialConcept(string financialConceptUID) {
      Assertion.Require(financialConceptUID, nameof(financialConceptUID));

      FinancialConcept concept = FinancialConcept.Parse(financialConceptUID);

      FinancialConceptGroup group = concept.Group;

      group.Remove(concept);

      concept.Save();
    }


    public FinancialConceptDto UpdateFinancialConcept(EditFinancialConceptCommand command) {
      Assertion.Require(command, nameof(command));

      command.EnsureIsValid();

      var group = FinancialConceptGroup.Parse(command.GroupUID);

      FinancialConcept concept = group.UpdateFrom(command);

      concept.Save();

      return FinancialConceptMapper.Map(concept);
    }

    #endregion Use cases

  }  // class FinancialConceptsUseCases

}  // namespace Empiria.FinancialAccounting.FinancialConcepts.UseCases
