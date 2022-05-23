/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Concepts                         Component : Use cases Layer                         *
*  Assembly : FinancialAccounting.FinancialConcepts.dll  Pattern   : Use case interactor class               *
*  Type     : FinancialConceptEditionUseCases            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases used to create and update financial concepts.                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.FinancialAccounting.FinancialConcepts.Adapters;

namespace Empiria.FinancialAccounting.FinancialConcepts.UseCases {

  /// <summary>Use cases used to create and update financial concepts.</summary>
  public class FinancialConceptEditionUseCases : UseCase {

    #region Constructors and parsers

    protected FinancialConceptEditionUseCases() {
      // no-op
    }

    static public FinancialConceptEditionUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<FinancialConceptEditionUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases


    public FixedList<FinancialConceptDescriptorDto> InsertFinancialConcept(FinancialConceptEditionCommand command) {
      Assertion.AssertObject(command, nameof(command));

      command.EnsureIsValid();

      var group = FinancialConceptGroup.Parse(command.GroupUID);

      FixedList<FinancialConcept> concepts = group.FinancialConcepts();

      return FinancialConceptMapper.Map(concepts);
    }


    public FixedList<FinancialConceptDescriptorDto> RemoveFinancialConcept(string financialConceptUID) {
      Assertion.AssertObject(financialConceptUID, nameof(financialConceptUID));

      FinancialConceptGroup group = FinancialConcept.Parse(financialConceptUID).Group;

      FixedList<FinancialConcept> concepts = group.FinancialConcepts();

      return FinancialConceptMapper.Map(concepts);
    }


    public FixedList<FinancialConceptDescriptorDto> UpdateFinancialConcept(FinancialConceptEditionCommand command) {
      Assertion.AssertObject(command, nameof(command));

      command.EnsureIsValid();

      var group = FinancialConceptGroup.Parse(command.GroupUID);

      FixedList<FinancialConcept> concepts = group.FinancialConcepts();

      return FinancialConceptMapper.Map(concepts);
    }

    #endregion Use cases

  }  // class FinancialConceptEditionUseCases

}  // namespace Empiria.FinancialAccounting.FinancialConcepts.UseCases
