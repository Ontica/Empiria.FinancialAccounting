/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Ledger Management                            Component : Interface adapters                    *
*  Assembly : FinancialAccounting.Core.dll                 Pattern   : Command payload                       *
*  Type     : SearchSubledgerAccountCommand                License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Command payload used to search subledger accounts (cuentas auxiliares).                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Adapters {

  /// <summary>Command payload used to search subledger accounts (cuentas auxiliares).</summary>
  public class SearchSubledgerAccountCommand {

    public string Keywords {
      get; set;
    } = string.Empty;


    public string Type {
      get; set;
    } = string.Empty;

  }  // SearchSubledgerAccountCommand

}  // namespace Empiria.FinancialAccounting.Adapters
