/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Command payload                         *
*  Type     : AccountEditionCommand                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Command payload used for accounts edition.                                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Adapters {

  /// <summary>Command payload used for accounts edition.</summary>
  public class AccountEditionCommand {

    public string[] Sectors {
      get; set;
    } = new string[0];


    public string[] Currencies {
      get; set;
    } = new string[0];

  }  // class AccountEditionCommand



  /// <summary>Extension methods for AccountEditionCommand interface adapter.</summary>
  static internal class AccountEditionCommandExtension {

    #region Public methods

    #endregion Public methods

    #region Private methods

    #endregion Private methods

  }  // AccountEditionCommandExtension

}  // namespace Empiria.FinancialAccounting.Adapters
