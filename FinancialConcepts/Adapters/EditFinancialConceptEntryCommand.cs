/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Concepts                         Component : Interface adapters                      *
*  Assembly : FinancialAccounting.FinancialConcepts.dll  Pattern   : Command payload                         *
*  Type     : FinancialConceptEntryEditionCommand        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : The command used to create or update financial concept's integration entries.                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.FinancialConcepts.Adapters {

  /// <summary>The command used to create or update financial concept's integration entries.</summary>
  public class EditFinancialConceptEntryCommand : Command {

    public PayloadType Payload {
      get; set;
    } = new PayloadType();


    internal EntitiesType Entities {
      get; private set;
    } = new EntitiesType();



    protected override void Clean() {
      Payload.Clean();
    }


    protected override void Require() {
      Assertion.Require(Type == "InsertFinancialConceptEntry" || Type == "UpdateFinancialConceptEntry",
                        $"Unrecognized command type '{Type}'.");

      Assertion.Require(Payload, "Payload");

      Payload.Require(Type);
    }


    protected override void SetEntities() {
      this.Entities.FinancialConcept = FinancialConcept.Parse(this.Payload.FinancialConceptUID);

      if (Payload.FinancialConceptEntryUID.Length != 0) {
        this.Entities.FinancialConceptEntry = FinancialConceptEntry.Parse(Payload.FinancialConceptEntryUID);
      }

      if (Payload.ReferencedFinancialConceptUID.Length != 0) {
        this.Entities.ReferencedFinancialConcept = FinancialConcept.Parse(Payload.ReferencedFinancialConceptUID);
      }
    }


    protected override void SetIssues() {
      Payload.SetIssues(this.ExecutionResult);
    }


    protected override void SetWarnings() {
      Payload.SetWarnings(this.ExecutionResult);
    }


    internal FinancialConceptEntryFields MapToFields(int position) {

      FinancialConceptEntryFields fields;

      switch (Payload.EntryType) {
        case FinancialConceptEntryType.Account:
          fields = new AccountEntryTypeFields {
            AccountNumber           = Payload.AccountNumber,
            SubledgerAccountNumber  = Payload.SubledgerAccountNumber,
            SectorCode              = Payload.SectorCode,
            CurrencyCode            = Payload.CurrencyCode
          };
          break;

        case FinancialConceptEntryType.ExternalVariable:
          fields = new ExternalVariableEntryTypeFields(Payload.ExternalVariableCode);
          break;

        case FinancialConceptEntryType.FinancialConceptReference:
          fields = new FinancialConceptReferenceEntryTypeFields(Entities.ReferencedFinancialConcept);
          break;

        default:
          throw Assertion.EnsureNoReachThisCode();

      }

      fields.FinancialConcept = FinancialConcept.Parse(Payload.FinancialConceptUID);
      fields.Operator         = Payload.Operator;
      fields.CalculationRule  = Payload.CalculationRule;
      fields.DataColumn       = Payload.DataColumn;
      fields.Position         = position;

      return fields;
    }


    public class PayloadType {

      public string FinancialConceptUID {
        get; set;
      } = string.Empty;


      public string FinancialConceptEntryUID {
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


      //public string AccountsListUID {
      //  get; set;
      //} = string.Empty;


      public OperatorType Operator {
        get; set;
      } = OperatorType.Add;


      public string CalculationRule {
        get; set;
      } = "Default";


      public string DataColumn {
        get; set;
      } = "Default";


      public ItemPositioning Positioning {
        get; set;
      } = new ItemPositioning();


      internal void Clean() {
        AccountNumber           = EmpiriaString.Clean(AccountNumber);
        SubledgerAccountNumber  = EmpiriaString.Clean(SubledgerAccountNumber);
        SectorCode              = EmpiriaString.Clean(SectorCode);
        ExternalVariableCode    = EmpiriaString.Clean(ExternalVariableCode);
        CurrencyCode            = EmpiriaString.Clean(CurrencyCode);
        CalculationRule         = EmpiriaString.Clean(CalculationRule);
        DataColumn              = EmpiriaString.Clean(DataColumn);
        Positioning.Clean();
      }


      internal void Require(string commandType) {
        Assertion.Require(FinancialConceptUID, "payload.FinancialConceptUID");

        Assertion.Require(CalculationRule,     "payload.CalculationRule");
        Assertion.Require(DataColumn,          "payload.DataColumn");

        if (commandType == "InsertFinancialConceptEntry") {
          Assertion.Require(Positioning.Rule != PositioningRule.Undefined,
                            "payload.Positioning.Rule is required for insertion.");

        } else if (commandType == "UpdateFinancialConceptEntry") {
          Assertion.Require(FinancialConceptEntryUID, "payload.FinancialConceptEntryUID");
        }

        if (EntryType == FinancialConceptEntryType.Account) {
          Assertion.Require(DataColumn, "payload.AccountNumber");

          Assertion.Require(SectorCode.Length == 0 || Sector.Exists(SectorCode),
                           $"Unrecognized payload.SectorCode value '{SectorCode}'.");

          Assertion.Require(CurrencyCode.Length == 0 || Currency.Exists(CurrencyCode),
                           $"Unrecognized payload.CurencyCode value '{CurrencyCode}'.");

        } else if (EntryType == FinancialConceptEntryType.ExternalVariable) {
          Assertion.Require(ExternalVariableCode, "payload.ExternalVariableCode");

          Assertion.Require(ExternalVariableCode.Length == 0 || ExternalVariable.ExistsCode(ExternalVariableCode),
                           $"Unrecognized payload.ExternalVariableCode value '{ExternalVariableCode}'.");

        } else if (EntryType == FinancialConceptEntryType.FinancialConceptReference) {
          Assertion.Require(ReferencedFinancialConceptUID, "payload.ReferencedFinancialConceptUID");
        }


        Positioning.Require();
      }


      internal void SetIssues(ExecutionResult executionResult) {
        Positioning.SetIssues(executionResult);
      }


      internal void SetWarnings(ExecutionResult executionResult) {
        Positioning.SetWarnings(executionResult);
      }

    }  // class PayloadType


    internal class EntitiesType {

      internal FinancialConcept FinancialConcept {
        get; set;
      } = FinancialConcept.Empty;


      internal FinancialConceptEntry FinancialConceptEntry {
        get; set;
      } = FinancialConceptEntry.Empty;


      public FinancialConcept ReferencedFinancialConcept {
        get; internal set;
      } = FinancialConcept.Empty;


    }  // class EntitiesType

  }  // class EditFinancialConceptEntryCommand

}  // namespace Empiria.FinancialAccounting.FinancialConcepts.Adapters
