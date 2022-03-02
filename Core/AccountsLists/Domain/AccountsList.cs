/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Lists                             Component : Domain Layer                            *
*  Assembly : Empiria.FinancialAccounting.dll            Pattern   : Empiria General Object                  *
*  Type     : AccountsList                               License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Describes a list of accounts.                                                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting {

  /// <summary>Describes a reconciliation type.</summary>
  internal class AccountsList : GeneralObject {

    #region Constructors and parsers

    protected AccountsList() {
      // Required by Empiria Framework.
    }

    static public AccountsList Parse(string uid) {
      return BaseObject.ParseKey<AccountsList>(uid);
    }


    static internal FixedList<AccountsList> GetList() {
      return BaseObject.GetList<AccountsList>(string.Empty, "ObjectName")
                       .ToFixedList();
    }

    #endregion Constructors and parsers

    #region Public methods

    public FixedList<AccountsListItem> GetItems() {
      return new FixedList<AccountsListItem>();
    }

    #endregion Public methods

  }  // class AccountsList

}  // namespace Empiria.FinancialAccounting
