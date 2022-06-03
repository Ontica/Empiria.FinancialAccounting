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


    public FixedList<FinancialConceptEntryDescriptorDto> GetFinancialConceptEntries(string financialConceptUID) {
      Assertion.Require(financialConceptUID, nameof(financialConceptUID));

      var concept = FinancialConcept.Parse(financialConceptUID);

      return FinancialConceptMapper.Map(concept.Integration);
    }


    public FinancialConceptEntryDto GetFinancialConceptEntry(string financialConceptUID,
                                                              string financialConceptEntryUID) {
      Assertion.Require(financialConceptUID, nameof(financialConceptUID));
      Assertion.Require(financialConceptEntryUID, nameof(financialConceptEntryUID));

      var concept = FinancialConcept.Parse(financialConceptUID);

      FinancialConceptEntry entry = concept.GetEntry(financialConceptEntryUID);

      return FinancialConceptMapper.Map(entry);
    }


    public ExecutionResult<FinancialConceptEntryDto> InsertFinancialConceptEntry(EditFinancialConceptEntryCommand command) {
      Assertion.Require(command, nameof(command));

      command.Arrange();

      if (!command.IsValid || command.DryRun) {
        return command.MapToExecutionResult<FinancialConceptEntryDto>();
      }

      FinancialConcept concept = command.Entities.FinancialConcept;

      FinancialConceptEntry entry = concept.InsertEntry(command.MapToFields(0),
                                                        command.Payload.Positioning);

      entry.Save();

      FinancialConceptEntryDto outcome = FinancialConceptMapper.Map(entry);

      command.Done(outcome, $"Se agregó la regla de integración al concepto " +
                            $"{entry.FinancialConcept.Code} - {entry.FinancialConcept.Name}.");

      return command.MapToExecutionResult<FinancialConceptEntryDto>();
    }


    public void RemoveFinancialConceptEntry(string financialConceptUID, string financialConceptEntryUID) {
      Assertion.Require(financialConceptUID, nameof(financialConceptUID));
      Assertion.Require(financialConceptEntryUID, nameof(financialConceptEntryUID));

      FinancialConcept concept = FinancialConcept.Parse(financialConceptUID);

      FinancialConceptEntry entry = concept.GetEntry(financialConceptEntryUID);

      concept.RemoveEntry(entry);

      entry.Save();
    }


    public FinancialConceptEntryDescriptorDto UpdateFinancialConceptEntry(EditFinancialConceptEntryCommand command) {
      Assertion.Require(command, nameof(command));

      command.Arrange();

      var concept = FinancialConcept.Parse(command.Payload.FinancialConceptUID);

      FinancialConceptEntry entry = concept.UpdateEntryFrom(command);

      entry.Save();

      return FinancialConceptMapper.MapToDescriptor(entry);
    }

    #endregion Use cases

  }  // class FinancialConceptIntegrationUseCases

}  // namespace Empiria.FinancialAccounting.FinancialConcepts.UseCases
