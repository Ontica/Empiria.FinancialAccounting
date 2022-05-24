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
      Assertion.AssertObject(financialConceptUID, nameof(financialConceptUID));

      var financialConcept = FinancialConcept.Parse(financialConceptUID);

      return FinancialConceptMapper.Map(financialConcept);
    }


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

  }  // class FinancialConceptsUseCases

}  // namespace Empiria.FinancialAccounting.FinancialConcepts.UseCases
