/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Lists                             Component : Domain Layer                            *
*  Assembly : Empiria.FinancialAccounting.dll            Pattern   : Information Holder                      *
*  Type     : AccountClassificatorList                   License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : List that holds accounts with classification data.                                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System;

using Empiria.FinancialAccounting.AccountsLists.Data;

namespace Empiria.FinancialAccounting.AccountsLists {

  public class AccountClassificatorList : AccountsList {

    #region Fields

    private Lazy<FixedList<AccountClassificator>> _items;

    #endregion Fields

    #region Constructors and parsers

    static public new AccountClassificatorList Parse(string listName) =>
                                                      ParseKey<AccountClassificatorList>(listName);

    protected override void OnLoad() {
      Reload();
    }

    #endregion Constructors and parsers

    #region Properties

    public FixedList<AccountClassificator> Items {
      get {
        return _items.Value;
      }
    }

    #endregion Properties

    #region Methods

    public AccountClassificator TryGetAccount(string accountNo) {
      Assertion.Require(accountNo, nameof(accountNo));

      return Items.Find(x => x.AccountNumber == accountNo);
    }


    public string TryGetAccountValue(string accountNo, string classKey) {
      Assertion.Require(accountNo, nameof(accountNo));
      Assertion.Require(classKey, nameof(classKey));

      AccountClassificator accountClassificator = TryGetAccount(accountNo);

      if (accountClassificator == null) {
        return null;
      }

      return accountClassificator.TryGetClassificatorValue(classKey);
    }

    #endregion Methods

    #region Helpers

    private void Reload() {
      _items = new Lazy<FixedList<AccountClassificator>>(() => AccountsListData.GetAccounts<AccountClassificator>(this));
    }

    #endregion Helpers

  }  // class AccountClassificatorList

}  // namespace Empiria.FinancialAccounting.AccountsLists
