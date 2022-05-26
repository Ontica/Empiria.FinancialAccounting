/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Concepts                         Component : Use cases Layer                         *
*  Assembly : FinancialAccounting.FinancialConcepts.dll  Pattern   : Use case interactor class               *
*  Type     : FinancialConceptIntegrationUseCases        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases used to create and update financial concept's integration entries.                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.FinancialAccounting.FinancialConcepts.Adapters;

namespace Empiria.FinancialAccounting.FinancialConcepts.UseCases {

  /// <summary>Use cases used to create and update financial concepts.</summary>
  public class FinancialConceptIntegrationUseCases : UseCase {

    #region Constructors and parsers

    protected FinancialConceptIntegrationUseCases() {
      // no-op
    }

    static public FinancialConceptIntegrationUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<FinancialConceptIntegrationUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases


    public FixedList<FinancialConceptEntryDto> GetFinancialConceptEntries(string financialConceptUID) {
      Assertion.AssertObject(financialConceptUID, nameof(financialConceptUID));

      var financialConcept = FinancialConcept.Parse(financialConceptUID);

      return FinancialConceptMapper.Map(financialConcept.Integration);
    }


    public FinancialConceptEntryDto InsertFinancialConceptEntry(FinancialConceptEntryEditionCommand command) {
      Assertion.AssertObject(command, nameof(command));

      command.EnsureIsValid();

      var concept = FinancialConcept.Parse(command.FinancialConceptUID);

      FinancialConceptEntry entry = concept.InsertEntryFrom(command);

      entry.Save();

      return FinancialConceptMapper.Map(entry);
    }


    public void RemoveFinancialConceptEntry(string financialConceptUID, string financialConceptEntryUID) {
      Assertion.AssertObject(financialConceptUID, nameof(financialConceptUID));
      Assertion.AssertObject(financialConceptEntryUID, nameof(financialConceptEntryUID));

      FinancialConcept concept = FinancialConcept.Parse(financialConceptUID);

      FinancialConceptEntry entry = concept.GetEntry(financialConceptEntryUID);

      concept.RemoveEntry(entry);

      entry.Save();
    }


    public FinancialConceptEntryDto UpdateFinancialConceptEntry(FinancialConceptEntryEditionCommand command) {
      Assertion.AssertObject(command, nameof(command));

      command.EnsureIsValid();

      var concept = FinancialConcept.Parse(command.FinancialConceptUID);

      FinancialConceptEntry entry = concept.UpdateEntryFrom(command);

      entry.Save();

      return FinancialConceptMapper.Map(entry);
    }

    #endregion Use cases

  }  // class FinancialConceptIntegrationUseCases

}  // namespace Empiria.FinancialAccounting.FinancialConcepts.UseCases
