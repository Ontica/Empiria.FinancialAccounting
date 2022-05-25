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

    public AccountEntryTypeFields() : base(FinancialConceptEntryType.Account) {
      // no-op
    }


    internal string AccountNumber {
      get; set;
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

}  // namespace Empiria.FinancialAccounting.FinancialConcepts
