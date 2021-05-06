/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Command payload                         *
*  Type     : AccountsSearchCommand                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Command payload used for accounts searching.                                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Adapters {

  /// <summary>Command payload used for accounts searching.</summary>
  public class AccountsSearchCommand {

    public DateTime Date {
      get; set;
    } = DateTime.Today;


    public string FromAccount {
      get; set;
    } = string.Empty;


    public string ToAccount {
      get; set;
    } = string.Empty;


    public string Keywords {
      get; set;
    } = string.Empty;


  }  // class AccountsSearchCommand

}  // namespace Empiria.FinancialAccounting.Adapters
