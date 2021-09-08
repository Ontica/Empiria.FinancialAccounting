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

    public string Number {
      get; internal set;
    }

    public string Name {
      get; internal set;
    }

    public string Description {
      get; internal set;
    }

  }  // public class SubledgerAccountFields

}  // namespace Empiria.FinancialAccounting.Adapters
