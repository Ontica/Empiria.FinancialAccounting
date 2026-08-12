  /* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Lists                             Component : Domain Layer                            *
*  Assembly : Empiria.FinancialAccounting.dll            Pattern   : Common Storage Items                    *
*  Type     : AccountsList                               License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Describes a list of accounts.                                                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.DynamicData;

using Empiria.FinancialAccounting.AccountsLists.Data;

namespace Empiria.FinancialAccounting {

  /// <summary>Describes a list of accounts.</summary>
  public class AccountsList : CommonStorage {

    #region Constructors and parsers

    protected AccountsList() {
      // Required by Empiria Framework.
    }


    static public AccountsList Parse(int id) {
      return BaseObject.ParseId<AccountsList>(id);
    }


    static public AccountsList Parse(string uid) {
      return BaseObject.ParseKey<AccountsList>(uid);
    }


    static internal FixedList<AccountsList> GetList() {
      return BaseObject.GetList<AccountsList>(string.Empty, "Object_Name")
                       .ToFixedList();
    }


    static public AccountsList Empty => BaseObject.ParseEmpty<AccountsList>();

    #endregion Constructors and parsers

    #region Public methods

    public FixedList<DataTableColumn> DataTableColumns {
      get {
        return base.ExtData.GetFixedList<DataTableColumn>("columns", false);
      }
    }


    public bool IsEditable {
      get {
        return base.ExtData.Get("isEditable", false);
      }
    }


    public FixedList<T> GetItems<T>() where T : BaseObject, IAccountListItem {
      return AccountsListData.GetAccounts<T>(this);
    }


    public FixedList<T> GetItems<T>(string keywords) where T : BaseObject, IAccountListItem {
      return AccountsListData.GetAccounts<T>(this, keywords);
    }

    #endregion Public methods

  }  // class AccountsList

}  // namespace Empiria.FinancialAccounting
