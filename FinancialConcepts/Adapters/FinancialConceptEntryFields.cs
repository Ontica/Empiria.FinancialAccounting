/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Concepts                         Component : Domain Layer                            *
*  Assembly : FinancialAccounting.FinancialConcepts.dll  Pattern   : Fields Data transfer objects            *
*  Type     : FinancialConceptEntryFields                License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Fields sets used for financial concept entry edition.                                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.FinancialConcepts {

  /// <summary>Abstract class that contains the base fields for update a financial concept entry.</summary>
  abstract internal class FinancialConceptEntryFields {

    protected FinancialConceptEntryFields(FinancialConceptEntryType type) {
      this.EntryType = type;
    }

    internal FinancialConceptEntryType EntryType {
      get;
    }


    internal FinancialConcept FinancialConcept {
      get; set;
    }


    internal OperatorType Operator {
      get; set;
    }


    internal string CalculationRule {
      get; set;
    }


    internal string DataColumn {
      get; set;
    }


    internal int Position {
      get; set;
    }

  }  // class FinancialConceptEntryFields


  /// <summary>Edition fields for FinancialConceptEntry of type Account.</summary>
  internal class AccountEntryTypeFields : FinancialConceptEntryFields {

    public AccountEntryTypeFields(string accountNumber) : base(FinancialConceptEntryType.Account) {
      Assertion.Require(accountNumber, nameof(accountNumber));

      this.AccountNumber = accountNumber;
    }


    internal string AccountNumber {
      get;
    }


    internal string SubledgerAccountNumber {
      get; set;
    }


    internal string SectorCode {
      get; set;
    }


    internal string CurrencyCode {
      get; set;
    }

  }  // class AccountEntryTypeFields



  internal class ExternalVariableEntryTypeFields : FinancialConceptEntryFields {

    public ExternalVariableEntryTypeFields(string externalVariableCode) : base(FinancialConceptEntryType.ExternalVariable) {
      Assertion.Require(externalVariableCode, nameof(externalVariableCode));

      this.ExternalVariableCode = externalVariableCode;
    }


    public string ExternalVariableCode {
      get;
    }

  }  // class ExternalVariableEntryTypeFields



  internal class FinancialConceptReferenceEntryTypeFields : FinancialConceptEntryFields {

    public FinancialConceptReferenceEntryTypeFields(FinancialConcept referencedFinancialConcept)
                                            : base(FinancialConceptEntryType.FinancialConceptReference) {
      Assertion.Require(referencedFinancialConcept, nameof(referencedFinancialConcept));

      this.ReferencedFinancialConcept = referencedFinancialConcept;
    }


    public FinancialConcept ReferencedFinancialConcept {
      get;
    }

  }  //class FinancialConceptReferenceEntryTypeFields

}  // namespace Empiria.FinancialAccounting.FinancialConcepts
