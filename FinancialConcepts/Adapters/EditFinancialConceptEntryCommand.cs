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

using Empiria.FinancialAccounting.ExternalData;

namespace Empiria.FinancialAccounting.FinancialConcepts.Adapters {


  /// <summary>The command used to create or update financial concept's integration entries.</summary>
  public class EditFinancialConceptEntryCommand : Command {


    public EditFinancialConceptEntryCommandType Type {
      get; set;
    } = EditFinancialConceptEntryCommandType.Undefined;


    public PayloadType Payload {
      get; set;
    } = new PayloadType();


    internal EntitiesType Entities {
      get; private set;
    } = new EntitiesType();



    protected override void Initialize() {
      Payload.Initialize(this.Type);
    }


    protected override string GetCommandTypeName() {
      return Type.ToString();
    }


    protected override void InitialRequire() {
      Assertion.Require(this.Type != EditFinancialConceptEntryCommandType.Undefined, "Type");

      Assertion.Require(this.Payload, "Payload");

      Payload.Require(this.Type);
    }


    protected override void SetEntities() {
      Entities.FinancialConcept = FinancialConcept.Parse(Payload.FinancialConceptUID);

      if (Payload.FinancialConceptEntryUID.Length != 0) {
        Entities.FinancialConceptEntry = Entities.FinancialConcept.GetEntry(Payload.FinancialConceptEntryUID);
      }

      if (Payload.ReferencedFinancialConceptUID.Length != 0) {
        Entities.ReferencedFinancialConcept = FinancialConcept.Parse(Payload.ReferencedFinancialConceptUID);
      }
      if (Payload.Positioning.Rule.UsesOffset()) {
        Payload.Positioning.SetOffsetObject(Entities.FinancialConcept.GetEntry(Payload.Positioning.OffsetUID));
      }
    }


    protected override void SetIssues() {
      Payload.SetIssues(this.ExecutionResult);

      if (Payload.Positioning.Rule.UsesPosition()) {
        int maxPosition = Entities.FinancialConcept.Integration.Count;

        ExecutionResult.AddIssueIf(Type.ForUpdate() && Payload.Positioning.Position > maxPosition,
                                   $"La posición no puede ser mayor a {maxPosition}.");

        ExecutionResult.AddIssueIf(Type.ForInsert() && Payload.Positioning.Position > maxPosition + 1,
                                  $"La posición no puede ser mayor a {maxPosition + 1}.");
      }
    }


    protected override void FinalRequire() {
      if (Payload.SubledgerAccountNumber.Length != 0) {
        AccountsChart chart = Entities.FinancialConcept.Group.AccountsChart;

        Assertion.Require(SubledgerAccount.TryParse(chart, Payload.SubledgerAccountNumber),
              $"No existe ningún auxiliar con número '{Payload.SubledgerAccountNumber}'.");
      }
    }

    internal FinancialConceptEntryFields MapToFields() {

      FinancialConceptEntryFields fields;

      if (Type.OverAccount()) {

        fields = new AccountEntryTypeFields(Payload.AccountNumber) {
          SubledgerAccountNumber = Payload.SubledgerAccountNumber,
          SectorCode = Payload.SectorCode,
          CurrencyCode = Payload.CurrencyCode
        };

      } else if (Type.OverExternalVariable()) {
        fields = new ExternalVariableEntryTypeFields(Payload.ExternalVariableCode);

      } else if (Type.OverConceptReference()) {
        fields = new FinancialConceptReferenceEntryTypeFields(Entities.ReferencedFinancialConcept);

      } else {
        throw Assertion.EnsureNoReachThisCode($"Unhandled command type '{Type}'.");
      }

      fields.FinancialConcept = FinancialConcept.Parse(Payload.FinancialConceptUID);
      fields.Operator         = Payload.Operator;
      fields.CalculationRule  = Payload.CalculationRule;
      fields.DataColumn       = Payload.DataColumn;

      return fields;
    }


    public class PayloadType {

      public string FinancialConceptUID {
        get; set;
      } = string.Empty;


      public string FinancialConceptEntryUID {
        get; set;
      } = string.Empty;


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


      public OperatorType Operator {
        get; set;
      } = OperatorType.Add;


      public string CalculationRule {
        get; set;
      } = "Default";


      public string DataColumn {
        get; set;
      } = "Default";


      public Positioning Positioning {
        get; set;
      } = new Positioning();


      internal void Initialize(EditFinancialConceptEntryCommandType commandType) {
        AccountNumber           = EmpiriaString.Clean(AccountNumber);
        SubledgerAccountNumber  = EmpiriaString.Clean(SubledgerAccountNumber);
        SectorCode              = EmpiriaString.Clean(SectorCode);
        ExternalVariableCode    = EmpiriaString.Clean(ExternalVariableCode);
        CurrencyCode            = EmpiriaString.Clean(CurrencyCode);
        CalculationRule         = EmpiriaString.Clean(CalculationRule);
        DataColumn              = EmpiriaString.Clean(DataColumn);

        if (commandType.ForInsert() && Positioning.Rule == PositioningRule.Undefined) {
          Positioning = new Positioning {
            Rule = PositioningRule.AtEnd
          };
        }
      }


      internal void Require(EditFinancialConceptEntryCommandType commandType) {
        Assertion.Require(FinancialConceptUID, "payload.FinancialConceptUID");

        Assertion.Require(CalculationRule,     "payload.CalculationRule");
        Assertion.Require(DataColumn,          "payload.DataColumn");

        if (commandType.ForInsert()) {
          Assertion.Require(Positioning.Rule != PositioningRule.Undefined,
                            "payload.Positioning.Rule is required for insertion.");

        } else if (commandType.ForUpdate()) {
          Assertion.Require(FinancialConceptEntryUID, "payload.FinancialConceptEntryUID");
        }

        if (commandType.OverAccount()) {
          Assertion.Require(AccountNumber, "payload.AccountNumber");

          Assertion.Require(SectorCode.Length == 0 || Sector.Exists(SectorCode),
                           $"Unrecognized payload.SectorCode value '{SectorCode}'.");

          Assertion.Require(CurrencyCode.Length == 0 || Currency.Exists(CurrencyCode),
                           $"Unrecognized payload.CurencyCode value '{CurrencyCode}'.");

        } else if (commandType.OverExternalVariable()) {
          Assertion.Require(ExternalVariableCode, "payload.ExternalVariableCode");

          Assertion.Require(ExternalVariable.ExistsCode(ExternalVariableCode),
                           $"Unrecognized payload.ExternalVariableCode value '{ExternalVariableCode}'.");

        } else if (commandType.OverConceptReference()) {
          Assertion.Require(ReferencedFinancialConceptUID, "payload.ReferencedFinancialConceptUID");

        } else {
          throw Assertion.EnsureNoReachThisCode($"Unhandled command type '{commandType}'.");
        }

        Positioning.Require();
      }


      internal void SetIssues(ExecutionResult executionResult) {
        Positioning.SetIssues(executionResult);
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
