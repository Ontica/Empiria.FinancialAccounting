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

namespace Empiria.FinancialAccounting.FinancialConcepts {

  public enum FinancialConceptEntryType {

    Account,

    ExternalVariable,

    FinancialConceptReference,

  }  // enum FinancialConceptEntryType


  public enum OperatorType {

    Add = '+',

    Substract = '-',

    AbsoluteValue = '@'

  }


  /// <summary>Describes an integration entry for a financial concept. Each integration entry is referenced
  /// to another financial concept, to a financial account or to an external financial value.</summary>
  public class FinancialConceptEntry : BaseObject {

    #region Constructors and parsers

    protected FinancialConceptEntry() {
      // Required by Empiria Framework.
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


    [DataField("ID_CONCEPTO")]
    public FinancialConcept FinancialConcept {
      get; private set;
    }


    [DataField("REGLA_CALCULO")]
    public string CalculationRule {
      get; private set;
    }


    [DataField("REF_ID_CONCEPTO")]
    public FinancialConcept ReferencedFinancialConcept {
      get; private set;
    }


    [DataField("NUMERO_CUENTA_ESTANDAR")]
    public string AccountNumber {
      get; private set;
    }


    [DataField("NUMERO_CUENTA_AUXILIAR")]
    public string SubledgerAccountNumber {
      get; private set;
    }


    [DataField("CLAVE_SECTOR")]
    public string SectorCode {
      get; private set;
    }


    [DataField("CLAVE_MONEDA")]
    public string CurrencyCode {
      get; private set;
    }


    [DataField("CLAVE_VARIABLE")]
    public string ExternalVariableCode {
      get; private set;
    }


    [DataField("ID_LISTA_CUENTAS")]
    public int AccountsListId {
      get; private set;
    }


    [DataField("ID_TIPO_INTEGRACION")]
    public int IntegrationTypeId {
      get; private set;
    }


    [DataField("OPERADOR", Default = OperatorType.Add)]
    public OperatorType Operator {
      get; private set;
    }


    [DataField("CALIFICACION")]
    public string Qualification {
      get; private set;
    }


    [DataField("ID_GRUPO")]
    public FinancialConceptGroup Group {
      get; private set;
    }


    [DataField("POSICION")]
    public int Position {
      get; private set;
    }


    public string Name {
      get {
        if (this.Type == FinancialConceptEntryType.Account) {

          var account = FinancialConcept.Group.AccountsChart.TryGetAccount(this.AccountNumber);

          if (account != null) {
            return account.Name;
          } else {
            return "La cuenta NO existe en el catálogo de cuentas.";
          }

        } else if (this.Type == FinancialConceptEntryType.FinancialConceptReference) {
          return this.ReferencedFinancialConcept.Name;

        } else if (this.Type == FinancialConceptEntryType.ExternalVariable) {

          var fixedValue = ExternalVariable.TryParseWithCode(this.ExternalVariableCode);

          if (fixedValue != null) {
            return fixedValue.Name;
          } else {
            return "El valor por defecto no existe en el catálogo de variables.";
          }

        } else {
          return "";
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


    public bool HasSector {
      get {
        return (this.SectorCode.Length != 0 && this.SectorCode != "00");
      }
    }


    public bool HasSubledgerAccount {
      get {
        return (this.SubledgerAccountNumber.Length > 4);
      }
    }


    public string SubledgerAccountName {
      get {
        if (!HasSubledgerAccount) {
          return string.Empty;
        }

        var subledgerAccount = SubledgerAccount.TryParse(this.SubledgerAccountNumber);

        if (subledgerAccount == null) {
          return "El auxiliar NO existe en el sistema.";
        }

        return subledgerAccount.Name;
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
      this.Qualification = EmpiriaString.Clean(this.Qualification);
    }

    #endregion Methods

  }  // class FinancialConceptEntry

}  // namespace Empiria.FinancialAccounting.FinancialConcepts
