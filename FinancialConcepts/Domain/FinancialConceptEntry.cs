/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Concepts                         Component : Domain Layer                            *
*  Assembly : FinancialAccounting.FinancialConcepts.dll  Pattern   : Empiria Data Object                     *
*  Type     : FinancialConceptIntegrationEntry           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Describes an integration entry for a financial concept. Each integration entry is referenced   *
*             to another financial concept, to a financial account or to an external financial value.        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Commands;
using Empiria.Contacts;
using Empiria.StateEnums;

using Empiria.DynamicData.ExternalData;

using Empiria.FinancialAccounting.FinancialConcepts.Data;

namespace Empiria.FinancialAccounting.FinancialConcepts {

  public enum FinancialConceptEntryType {

    Account,

    ExternalVariable,

    FinancialConceptReference,

  }


  public enum OperatorType {

    Add = '+',

    Substract = '-',

    Multiply = '*',

    AbsoluteValue = '@'

  }


  /// <summary>Describes an integration entry for a financial concept. Each integration entry is referenced
  /// to another financial concept, to a financial account or to an external financial value.</summary>
  public class FinancialConceptEntry : BaseObject, IPositionable {

    #region Constructors and parsers

    protected FinancialConceptEntry() {
      // Required by Empiria Framework.
    }


    private FinancialConceptEntry(FinancialConceptEntryFields fields) {
      Load(fields);
    }


    static public FinancialConceptEntry Parse(int id) {
      return BaseObject.ParseId<FinancialConceptEntry>(id);
    }


    static public FinancialConceptEntry Parse(string uid) {
      return BaseObject.ParseKey<FinancialConceptEntry>(uid);
    }


    static public FinancialConceptEntry Empty {
      get {
        return FinancialConceptEntry.ParseEmpty<FinancialConceptEntry>();
      }
    }


    static internal FinancialConceptEntry Create(FinancialConceptEntryFields fields) {
      Assertion.Require(fields, nameof(fields));

      return new FinancialConceptEntry(fields);
    }


    #endregion Constructors and parsers

    #region Properties

    public FinancialConceptEntryType Type {
      get {
        if (!this.ReferencedFinancialConcept.IsEmptyInstance) {
          return FinancialConceptEntryType.FinancialConceptReference;

        } else if (this.AccountNumber.Length != 0) {
          return FinancialConceptEntryType.Account;

        } else if (this.ExternalVariableCode.Length != 0) {
          return FinancialConceptEntryType.ExternalVariable;

        } else if (this.IsEmptyInstance) {
          return FinancialConceptEntryType.FinancialConceptReference;

        } else {
          return FinancialConceptEntryType.ExternalVariable;

        }
      }
    }

    [DataField("ID_TIPO_INTEGRACION")]
    public int IntegrationTypeId {
      get; private set;
    }


    [DataField("ID_CONCEPTO")]
    public FinancialConcept FinancialConcept {
      get; private set;
    }


    [DataField("REFERENCIA_ID_CONCEPTO")]
    public FinancialConcept ReferencedFinancialConcept {
      get; private set;
    } = FinancialConcept.Empty;


    [DataField("NUMERO_CUENTA_ESTANDAR")]
    public string AccountNumber {
      get; private set;
    } = string.Empty;


    [DataField("NUMERO_CUENTA_AUXILIAR")]
    public string SubledgerAccountNumber {
      get; private set;
    } = string.Empty;


    [DataField("CLAVE_SECTOR")]
    public string SectorCode {
      get; private set;
    } = string.Empty;


    [DataField("CLAVE_VARIABLE")]
    public string ExternalVariableCode {
      get; private set;
    } = string.Empty;


    [DataField("CLAVE_MONEDA")]
    public string CurrencyCode {
      get; private set;
    } = string.Empty;


    [DataField("ID_LISTA_CUENTAS")]
    public int AccountsListId {
      get; private set;
    } = -1;


    [DataField("OPERADOR", Default = OperatorType.Add)]
    public OperatorType Operator {
      get; private set;
    }


    [DataField("REGLA_CALCULO")]
    public string CalculationRule {
      get; private set;
    } = "Default";


    [DataField("COLUMNA")]
    public string DataColumn {
      get; private set;
    } = "default";


    public FinancialConceptGroup Group {
      get {
        return this.FinancialConcept.Group;
      }
    }


    [DataField("POSICION")]
    public int Position {
      get; private set;
    } = -1;


    [DataField("STATUS_INTEGRACION", Default = EntityStatus.Active)]
    public EntityStatus Status {
      get; private set;
    } = EntityStatus.Active;


    [DataField("ID_EDITADA_POR")]
    public Contact UpdatedBy {
      get; private set;
    }


    public string Name {
      get {
        if (this.Type == FinancialConceptEntryType.Account) {

          Account account = TryGetAccount();

          if (account != null) {
            return account.Name;
          } else {
            return "La cuenta NO existe en el catálogo de cuentas.";
          }

        } else if (this.Type == FinancialConceptEntryType.FinancialConceptReference) {
          return this.ReferencedFinancialConcept.Name;

        } else if (this.Type == FinancialConceptEntryType.ExternalVariable) {

          ExternalVariable externalVariable = TryGetExternalVariable();

          if (externalVariable != null) {
            return externalVariable.Name;
          } else {
            return "El valor externo no ha sido definido en el catálogo de variables.";
          }

        } else {
          return string.Empty;
        }
      }
    }


    public string Code {
      get {
        if (this.Type == FinancialConceptEntryType.Account) {
          return this.AccountNumber;

        } else if (this.Type == FinancialConceptEntryType.FinancialConceptReference) {
          return this.ReferencedFinancialConcept.Code;

        } else if (this.Type == FinancialConceptEntryType.ExternalVariable) {
          return this.ExternalVariableCode;

        } else {
          return string.Empty;

        }
      }
    }


    public bool HasCurrency {
      get {
        return (this.CurrencyCode.Length != 0);
      }
    }


    public bool HasSector {
      get {
        return (this.SectorCode.Length != 0 && this.SectorCode != "00");
      }
    }


    public bool HasSubledgerAccount {
      get {
        return (this.SubledgerAccountNumber.Length != 0);
      }
    }


    public string SubledgerAccountName {
      get {
        SubledgerAccount subledgerAccount = TryGetSubledgerAccount();

        if (subledgerAccount != null) {
          return subledgerAccount.Name;
        }

        return string.Empty;
      }
    }

    #endregion Properties

    #region Methods

    internal void Cleanup() {
      this.CalculationRule = EmpiriaString.Clean(this.CalculationRule);
      this.AccountNumber = EmpiriaString.Clean(this.AccountNumber);
      this.SubledgerAccountNumber = EmpiriaString.Clean(this.SubledgerAccountNumber);
      this.SectorCode = EmpiriaString.Clean(this.SectorCode);
      this.CurrencyCode = EmpiriaString.Clean(this.CurrencyCode);
      this.ExternalVariableCode = EmpiriaString.Clean(this.ExternalVariableCode);
      this.DataColumn = EmpiriaString.Clean(this.DataColumn);
    }


    internal void Delete() {
      this.Status = EntityStatus.Deleted;
    }

    protected override void OnSave() {
      FinancialConceptsData.Write(this);
    }


    internal void SetPosition(int position) {
      Assertion.Require(position > 0, "Position must be greater than zero.");

      this.Position = position;
    }


    private void SetIntegrationType(FinancialConceptEntryType entryType) {
      switch (entryType) {
        case FinancialConceptEntryType.Account:
          this.IntegrationTypeId = 3075;
          return;

        case FinancialConceptEntryType.ExternalVariable:
          this.IntegrationTypeId = 3076;
          return;

        case FinancialConceptEntryType.FinancialConceptReference:
          this.IntegrationTypeId = 3074;
          return;

        default:
          throw Assertion.EnsureNoReachThisCode();
      }
    }


    public Account TryGetAccount() {
      return FinancialConcept.Group.AccountsChart.TryGetAccount(this.AccountNumber);
    }


    public ExternalVariable TryGetExternalVariable() {
      return ExternalVariable.TryParseWithCode(this.ExternalVariableCode);
    }


    public SubledgerAccount TryGetSubledgerAccount() {
      if (!HasSubledgerAccount) {
        return null;
      }
      return SubledgerAccount.TryParse(FinancialConcept.Group.AccountsChart,
                                       SubledgerAccountNumber);
    }


    internal void Update(FinancialConceptEntryFields fields) {
      Assertion.Require(fields, nameof(fields));

      Load(fields);
    }


    private void Load(FinancialConceptEntryFields fields) {
      Base_Load(fields);

      if (fields is AccountEntryTypeFields accountFields) {
        this.Load(accountFields);

      } else if (fields is ExternalVariableEntryTypeFields variableFields) {
        this.Load(variableFields);

      } else if (fields is FinancialConceptReferenceEntryTypeFields referenceFields) {
        this.Load(referenceFields);

      } else {
        throw Assertion.EnsureNoReachThisCode();
      }
    }


    private void Load(AccountEntryTypeFields fields) {
      AccountNumber = fields.AccountNumber;
      SubledgerAccountNumber = fields.SubledgerAccountNumber;
      SectorCode = fields.SectorCode;
      CurrencyCode = fields.CurrencyCode;
    }


    private void Load(FinancialConceptReferenceEntryTypeFields fields) {
      this.ReferencedFinancialConcept = fields.ReferencedFinancialConcept;
    }


    private void Load(ExternalVariableEntryTypeFields fields) {
      this.ExternalVariableCode = fields.ExternalVariableCode;
    }


    private void Base_Load(FinancialConceptEntryFields fields) {
      SetIntegrationType(fields.EntryType);
      FinancialConcept  = fields.FinancialConcept;
      Operator          = fields.Operator;
      CalculationRule   = fields.CalculationRule;
      DataColumn        = fields.DataColumn;
      UpdatedBy         = ExecutionServer.CurrentContact;
    }

    #endregion Methods

  }  // class FinancialConceptEntry

}  // namespace Empiria.FinancialAccounting.FinancialConcepts
