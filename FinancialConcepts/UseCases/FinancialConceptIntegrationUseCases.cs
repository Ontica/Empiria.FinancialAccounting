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
      Assertion.Require(financialConceptUID, nameof(financialConceptUID));

      var financialConcept = FinancialConcept.Parse(financialConceptUID);

      return FinancialConceptMapper.Map(financialConcept.Integration);
    }


    public ExecutionResult InsertEntry(EditFinancialConceptEntryCommand command) {
      Assertion.Require(command, nameof(command));

      command.EnsureIsValid();

      var concept = FinancialConcept.Parse(command.FinancialConceptUID);

      ExecutionResult result = concept.InsertEntryFrom2(command);

      if (command.DryRun) {
        return result;
      }

      return CommitChanges(result);
    }


    public ExecutionResult RemoveEntry(EditFinancialConceptEntryCommand command) {
      Assertion.Require(command, nameof(command));

      command.EnsureIsValid();

      var concept = FinancialConcept.Parse(command.FinancialConceptUID);

      ExecutionResult result = concept.RemoveEntry(command);

      if (command.DryRun) {
        return result;
      }

      return CommitChanges(result);
    }


    public ExecutionResult UpdateEntry(EditFinancialConceptEntryCommand command) {
      Assertion.Require(command, nameof(command));

      command.EnsureIsValid();

      var concept = FinancialConcept.Parse(command.FinancialConceptUID);

      ExecutionResult result = concept.UpdateEntryFrom2(command);

      if (command.DryRun) {
        return result;
      }

      return CommitChanges(result);
    }


    public FinancialConceptEntryDto InsertFinancialConceptEntry(EditFinancialConceptEntryCommand command) {
      Assertion.Require(command, nameof(command));

      command.EnsureIsValid();

      var concept = FinancialConcept.Parse(command.FinancialConceptUID);

      FinancialConceptEntry entry = concept.InsertEntryFrom(command);

      entry.Save();

      return FinancialConceptMapper.Map(entry);
    }


    public void RemoveFinancialConceptEntry(string financialConceptUID, string financialConceptEntryUID) {
      Assertion.Require(financialConceptUID, nameof(financialConceptUID));
      Assertion.Require(financialConceptEntryUID, nameof(financialConceptEntryUID));

      FinancialConcept concept = FinancialConcept.Parse(financialConceptUID);

      FinancialConceptEntry entry = concept.GetEntry(financialConceptEntryUID);

      concept.RemoveEntry(entry);

      entry.Save();
    }


    public FinancialConceptEntryDto UpdateFinancialConceptEntry(EditFinancialConceptEntryCommand command) {
      Assertion.Require(command, nameof(command));

      command.EnsureIsValid();

      var concept = FinancialConcept.Parse(command.FinancialConceptUID);

      FinancialConceptEntry entry = concept.UpdateEntryFrom(command);

      entry.Save();

      return FinancialConceptMapper.Map(entry);
    }

    #endregion Use cases

    #region Helpers

    private ExecutionResult CommitChanges(ExecutionResult result) {
      result.EnsureCanBeCommited();

      FinancialConceptEntry entry = result.GetEntity<FinancialConceptEntry>();

      string msg;

      if (entry.IsNew) {
        msg = $"Se agregó la regla de integración al concepto " +
              $"{entry.FinancialConcept.Code} - {entry.FinancialConcept.Name}.";

      } else if (entry.Status == StateEnums.EntityStatus.Deleted) {
        msg = "La regla de integración fue eliminada con éxito.";

      } else {
        msg = "La regla de integración fue modificada con éxito.";
      }

      entry.Save();

      FinancialConceptEntryDto dto = FinancialConceptMapper.Map(entry);

      result.MarkAsCommited(dto, msg);

      return result;
    }

    #endregion Helpers

  }  // class FinancialConceptIntegrationUseCases

}  // namespace Empiria.FinancialAccounting.FinancialConcepts.UseCases
