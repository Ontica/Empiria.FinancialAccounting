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

    public string SubledgerTypeUID {
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


  static internal class SubledgerAccountFieldsExtensionMethods {

    static public SubledgerType SubledgerType(this SubledgerAccountFields fields) {
      return Empiria.FinancialAccounting.SubledgerType.Pending;
    }

  }

}  // namespace Empiria.FinancialAccounting.Adapters
