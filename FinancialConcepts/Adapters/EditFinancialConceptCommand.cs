/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Concepts                         Component : Interface adapters                      *
*  Assembly : FinancialAccounting.FinancialConcepts.dll  Pattern   : Command                                 *
*  Type     : EditFinancialConceptCommand                License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : The command used to create or update financial concepts.                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.FinancialConcepts.Adapters {

  /// <summary>The command used to create or update financial concepts.</summary>
  public class EditFinancialConceptCommand {

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


  }  // class EditFinancialConceptCommand


  /// <summary>Extension methods for EditFinancialConceptCommand.</summary>
  static class EditFinancialConceptCommandExtensions {

    #region Methods

    static internal void EnsureIsValid(this EditFinancialConceptCommand command) {
      command.Clean();

      Assertion.Require(command.GroupUID, "command.GroupUID");
      Assertion.Require(command.Code, "command.Code");
      Assertion.Require(command.Name, "command.Name");

      EnsurePositioningRuleIsValid(command);
      EnsureDatesAreValid(command);
    }


    static internal FinancialConceptFields MapToFinancialConceptFields(this EditFinancialConceptCommand command,
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

    static private void EnsureDatesAreValid(EditFinancialConceptCommand command) {
      Assertion.Require(command.StartDate != ExecutionServer.DateMinValue,
                       "command.StartDate can not be empty.");

      Assertion.Require(command.EndDate <= Account.MAX_END_DATE,
                       $"command.EndDate max value must be {Account.MAX_END_DATE.ToString("yyyy/MM/dd")}.");

      Assertion.Require(command.StartDate <= command.EndDate,
                       $"command.StartDate can not be greater than command.EndDate.");
    }


    static private void EnsurePositioningRuleIsValid(EditFinancialConceptCommand command) {
      Assertion.Require(command.PositioningRule != PositioningRule.Undefined,
                       "command.PositioningRule can not be 'Undefined'.");

      if (command.PositioningRule.UsesOffset() && command.PositioningOffsetConceptUID.Length == 0) {
        Assertion.RequireFail($"command.PositioningRule is '{command.PositioningRule}', " +
                              $"so command.PositioningOffsetConceptUID can not be empty.");
      }

      if (command.PositioningRule.UsesPosition() && command.Position == -1) {
        Assertion.RequireFail($"command.PositioningRule is '{command.PositioningRule}', " +
                              "so command.Position is required.");
      }
    }


    static private void Clean(this EditFinancialConceptCommand command) {
      command.FinancialConceptUID = EmpiriaString.Clean(command.FinancialConceptUID);
      command.GroupUID = EmpiriaString.Clean(command.GroupUID);
      command.Code = EmpiriaString.Clean(command.Code);
      command.Name = EmpiriaString.Clean(command.Name);
      command.PositioningOffsetConceptUID = EmpiriaString.Clean(command.PositioningOffsetConceptUID);
    }

    #endregion Helpers

  }  // class EditFinancialConceptCommandExtensions

}  // namespace Empiria.FinancialAccounting.FinancialConcepts.Adapters
