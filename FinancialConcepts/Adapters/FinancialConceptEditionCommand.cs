/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Concepts                         Component : Interface adapters                      *
*  Assembly : FinancialAccounting.FinancialConcepts.dll  Pattern   : Command                                 *
*  Type     : FinancialConceptEditionCommand             License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : The command used to create or update financial concepts.                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.FinancialConcepts.Adapters {

  /// <summary>The command used to create or update financial concepts.</summary>
  public class FinancialConceptEditionCommand {

    public string FinancialConceptUID {
      get; set;
    } = string.Empty;


    public string GroupUID {
      get; set;
    } = string.Empty;


    public string Code {
      get; set;
    } = string.Empty;


    public string Name {
      get; set;
    } = string.Empty;


    public PositioningRule PositioningRule {
      get; set;
    } = PositioningRule.Undefined;


    public string PositioningOffsetConceptUID {
      get; set;
    } = string.Empty;


    public int Position {
      get; set;
    } = -1;


    public DateTime StartDate {
      get; set;
    } = ExecutionServer.DateMinValue;


    public DateTime EndDate {
      get; set;
    } = Account.MAX_END_DATE;


  }  // class FinancialConceptEditionCommand


  /// <summary>Extension methods for FinancialConceptEditionCommand.</summary>
  static class FinancialConceptEditionCommandExtensions {

    #region Methods

    static internal void EnsureIsValid(this FinancialConceptEditionCommand command) {
      command.Clean();

      Assertion.AssertObject(command.GroupUID, "command.GroupUID");
      Assertion.AssertObject(command.Code, "command.Code");
      Assertion.AssertObject(command.Name, "command.Name");

      EnsurePositioningRuleIsValid(command);
      EnsureDatesAreValid(command);
    }


    static internal FinancialConceptFields MapToFinancialConceptFields(this FinancialConceptEditionCommand command,
                                                                       int position) {
      return new FinancialConceptFields {
        Group = FinancialConceptGroup.Parse(command.GroupUID),
        Code = command.Code,
        Name = command.Name,
        StartDate = command.StartDate,
        EndDate = command.EndDate,
        Position = position
      };
    }

    #endregion Methods


    #region Helpers

    static private void EnsureDatesAreValid(FinancialConceptEditionCommand command) {
      Assertion.Assert(command.StartDate != ExecutionServer.DateMinValue,
                       "command.StartDate can not be empty.");

      Assertion.Assert(command.EndDate <= Account.MAX_END_DATE,
                       $"command.EndDate max value must be {Account.MAX_END_DATE.ToString("yyyy/MM/dd")}.");

      Assertion.Assert(command.StartDate <= command.EndDate,
                       $"command.StartDate can not be greater than command.EndDate.");
    }


    static private void EnsurePositioningRuleIsValid(FinancialConceptEditionCommand command) {
      Assertion.Assert(command.PositioningRule != PositioningRule.Undefined,
                       "command.PositioningRule can not be 'Undefined'.");

      if (command.PositioningRule.UsesOffset() && command.PositioningOffsetConceptUID.Length == 0) {
        Assertion.AssertFail($"command.PositioningRule is '{command.PositioningRule}', " +
                             $"so command.PositioningOffsetConceptUID can not be empty.");
      }

      if (command.PositioningRule.UsesPosition() && command.Position == -1) {
        Assertion.AssertFail($"command.PositioningRule is '{command.PositioningRule}', " +
                              "so command.Position is required.");
      }
    }


    static private void Clean(this FinancialConceptEditionCommand command) {
      command.FinancialConceptUID = EmpiriaString.Clean(command.FinancialConceptUID);
      command.GroupUID = EmpiriaString.Clean(command.GroupUID);
      command.Code = EmpiriaString.Clean(command.Code);
      command.Name = EmpiriaString.Clean(command.Name);
      command.PositioningOffsetConceptUID = EmpiriaString.Clean(command.PositioningOffsetConceptUID);
    }

    #endregion Helpers

  }  // class FinancialConceptEditionCommandExtensions

}  // namespace Empiria.FinancialAccounting.FinancialConcepts.Adapters
