/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Concepts                         Component : Interface adapters                      *
*  Assembly : FinancialAccounting.FinancialConcepts.dll  Pattern   : Command                                 *
*  Type     : FinancialConceptEntryEditionCommand        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : The command used to create or update financial concept's integration entries.                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.FinancialConcepts.Adapters {

  /// <summary>The command used to create or update financial concept's integration entries.</summary>
  public class FinancialConceptEntryEditionCommand {

    public bool DryRun {
      get; set;
    }


    public string FinancialConceptEntryUID {
      get; set;
    } = string.Empty;


    public string FinancialConceptUID {
      get; set;
    } = string.Empty;


    public FinancialConceptEntryType EntryType {
      get; set;
    }


    public string ReferencedFinancialConceptUID {
      get; set;
    } = string.Empty;


    public string AccountNumber {
      get; set;
    } = string.Empty;


    public string SubledgerAccountNumber {
      get; set;
    } = string.Empty;


    public string SectorCode {
      get; set;
    } = string.Empty;


    public string ExternalVariableCode {
      get; set;
    } = string.Empty;


    public string CurrencyCode {
      get; set;
    } = string.Empty;


    public string AccountsListUID {
      get; set;
    } = string.Empty;


    public OperatorType Operator {
      get; set;
    } = OperatorType.Add;


    public string CalculationRule {
      get; set;
    } = "Default";


    public string DataColumn {
      get; set;
    } = "Default";


    public PositioningRule PositioningRule {
      get; set;
    } = PositioningRule.Undefined;


    public string PositioningOffsetEntryUID {
      get; set;
    } = string.Empty;


    public int Position {
      get; set;
    } = -1;

  }  // class FinancialConceptEditionCommand


  /// <summary>Extension methods for FinancialConceptEntryEditionCommand.</summary>
  static class FinancialConceptEntryEditionCommandExtensions {

    #region Methods

    static internal void EnsureIsValid(this FinancialConceptEntryEditionCommand command) {
      command.Clean();

      Assertion.Require(command.FinancialConceptUID, "command.FinancialConceptUID");

      EnsurePositioningRuleIsValid(command);
    }


    static internal FinancialConceptEntryFields MapToFields(this FinancialConceptEntryEditionCommand command,
                                                            int position) {

      FinancialConceptEntryFields fields;

      switch (command.EntryType) {
        case FinancialConceptEntryType.Account:
          fields = new AccountEntryTypeFields {
            AccountNumber = command.AccountNumber,
            SubledgerAccountNumber = command.SubledgerAccountNumber,
            SectorCode = command.SectorCode,
            CurrencyCode = command.CurrencyCode
          };
          break;

        case FinancialConceptEntryType.ExternalVariable:
          fields = new ExternalVariableEntryTypeFields(command.ExternalVariableCode);
          break;

        case FinancialConceptEntryType.FinancialConceptReference:
          var reference = FinancialConcept.Parse(command.ReferencedFinancialConceptUID);

          fields = new FinancialConceptReferenceEntryTypeFields(reference);
          break;

        default:
          throw Assertion.EnsureNoReachThisCode();

      }

      fields.FinancialConcept = FinancialConcept.Parse(command.FinancialConceptUID);
      fields.Operator = command.Operator;
      fields.CalculationRule = command.CalculationRule;
      fields.DataColumn = command.DataColumn;
      fields.Position = position;

      return fields;
    }

    #endregion Methods

    #region Helpers

    static private void Clean(this FinancialConceptEntryEditionCommand command) {
      command.FinancialConceptEntryUID      = EmpiriaString.Clean(command.FinancialConceptEntryUID);
      command.FinancialConceptUID           = EmpiriaString.Clean(command.FinancialConceptUID);
      command.ReferencedFinancialConceptUID = EmpiriaString.Clean(command.ReferencedFinancialConceptUID);
      command.AccountNumber                 = EmpiriaString.Clean(command.AccountNumber);
      command.SubledgerAccountNumber        = EmpiriaString.Clean(command.SubledgerAccountNumber);
      command.SectorCode                    = EmpiriaString.Clean(command.SectorCode);
      command.ExternalVariableCode          = EmpiriaString.Clean(command.ExternalVariableCode);
      command.CurrencyCode                  = EmpiriaString.Clean(command.CurrencyCode);
      command.AccountsListUID               = EmpiriaString.Clean(command.AccountsListUID);
      command.CalculationRule               = EmpiriaString.Clean(command.CalculationRule);
      command.DataColumn                    = EmpiriaString.Clean(command.DataColumn);
      command.PositioningOffsetEntryUID     = EmpiriaString.Clean(command.PositioningOffsetEntryUID);
    }


    static private void EnsurePositioningRuleIsValid(FinancialConceptEntryEditionCommand command) {
      Assertion.Require(command.PositioningRule != PositioningRule.Undefined,
                       "command.PositioningRule can not be 'Undefined'.");

      if (command.PositioningRule.UsesOffset() && command.PositioningOffsetEntryUID.Length == 0) {
        Assertion.RequireFail($"command.PositioningRule is '{command.PositioningRule}', " +
                              $"so command.PositioningOffsetEntryUID can not be empty.");
      }

      if (command.PositioningRule.UsesPosition() && command.Position == -1) {
        Assertion.RequireFail($"command.PositioningRule is '{command.PositioningRule}', " +
                             "so command.Position is required.");
      }
    }

    #endregion Helpers

  }  // class FinancialConceptEntryEditionCommandExtensions

}  // namespace Empiria.FinancialAccounting.FinancialConcepts.Adapters
