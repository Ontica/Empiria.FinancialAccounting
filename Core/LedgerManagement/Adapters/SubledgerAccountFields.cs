/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Ledger Management                          Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Input Data Holder                       *
*  Type     : SubledgerAccountFields                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Input data object to create or update subledger accounts.                                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Adapters {

  /// <summary>Input data object to create or update subledger accounts.</summary>
  public class SubledgerAccountFields {

    public string LedgerUID {
      get; set;
    }


    public string TypeUID {
      get; set;
    }


    public string Number {
      get; set;
    }


    public string Name {
      get; set;
    }


    public string Description {
      get; set;
    } = string.Empty;


  }  // public class SubledgerAccountFields


  /// <summary>Separated static class with extension methods for SubledgerAccountFields.</summary>
  static internal class SubledgerAccountFieldsExtensions {


    static public void EnsureValid(this SubledgerAccountFields fields) {
      Assertion.AssertObject(fields.LedgerUID, "fields.LedgerUID");
      Assertion.AssertObject(fields.TypeUID, "fields.TypeUID");
      Assertion.AssertObject(fields.Number, "fields.Number");
      Assertion.AssertObject(fields.Name, "fields.Name");
    }


    static public SubledgerType SubledgerType(this SubledgerAccountFields fields) {
      Assertion.AssertObject(fields.TypeUID, "fields.TypeUID");

      return Empiria.FinancialAccounting.SubledgerType.Parse(fields.TypeUID);
    }

  }  // class SubledgerAccountFieldsExtensions

}  // namespace Empiria.FinancialAccounting.Adapters
