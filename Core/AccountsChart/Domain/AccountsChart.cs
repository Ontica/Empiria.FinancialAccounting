/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Aggregate Root                          *
*  Type     : AccountsChart                              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Aggregate root that holds an accounts chart.                                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Collections;
using Empiria.FinancialAccounting.Data;

namespace Empiria.FinancialAccounting {

  /// <summary>Aggregate root that holds an accounts chart.</summary>
  public class AccountsChart : GeneralObject {

    private Lazy<EmpiriaHashTable<Account>> _accounts;

    #region Constructors and parsers

    protected AccountsChart() {
      // Required by Empiria Framework.
    }


    static public AccountsChart Parse(int id) {
      return BaseObject.ParseId<AccountsChart>(id);
    }


    static public AccountsChart Parse(string uid) {
      return BaseObject.ParseKey<AccountsChart>(uid);
    }


    static public void Preload() {
      var list = GetList();

      foreach (var item in list) {
        var dummy = item.Accounts;
      }
    }


    static public FixedList<AccountsChart> GetList() {
      return BaseObject.GetList<AccountsChart>()
                       .FindAll(x => x.Status != StateEnums.EntityStatus.Deleted)
                       .ToFixedList();
    }


    static public AccountsChart Empty => BaseObject.ParseEmpty<AccountsChart>();


    protected override void OnLoad() {
      base.OnLoad();

      if (!this.IsEmptyInstance) {
        this.MasterData = new AccountsChartMasterData(this, this.ExtendedDataField);
        _accounts = new Lazy<EmpiriaHashTable<Account>>(() => AccountsChartData.GetAccounts(this));

      } else {
        this.MasterData = new AccountsChartMasterData(this);
        _accounts = new Lazy<EmpiriaHashTable<Account>>(() => new EmpiriaHashTable<Account>());
      }
    }

    #endregion Constructors and parsers


    #region Public properties

    public FixedList<Account> Accounts {
      get {
        return _accounts.Value.ToFixedList();
      }
    }


    public AccountsChartMasterData MasterData {
      get;
      private set;
    }


    #endregion Public properties

    #region Public methods

    public Account GetAccount(string accountNumber) {
      Account account;

      if (_accounts.Value.TryGetValue(accountNumber, out account)) {
        return account;
      }
      throw Assertion.AssertNoReachThisCode($"Account {accountNumber} was not found.");
    }


    public FixedList<Account> GetAccountHistory(string accountNumber) {
      return AccountsChartData.GetAccountHistory(this, accountNumber);
    }


    public Account GetAccountHistory(string accountNumber, DateTime date) {
      FixedList<Account> history = AccountsChartData.GetAccountHistory(this, accountNumber);

      Account account = history.Find(x => x.StartDate <= date && date <= x.EndDate);

      Assertion.AssertObject(account,
        $"There are not historic information for account {accountNumber} " +
        $"on date {date.ToString("dd/MMM/yyyy")}.");

      return account;
    }


    public FixedList<Account> GetAccountsInADate(DateTime date) {
      return _accounts.Value.ToFixedList()
                            .FindAll(x => x.StartDate <= date && date <= x.EndDate);
    }


    public FixedList<Account> GetChildren(Account account) {
      return _accounts.Value.ToFixedList()
                            .FindAll(x => x.Number.StartsWith(account.Number + this.MasterData.AccountNumberSeparator));
    }


    public bool HasChildren(Account account) {
      return this.GetChildren(account).Count > 0;
    }


    public FixedList<Account> Search(string filter) {
      return AccountsChartData.SearchAccounts(this, filter);
    }


    #endregion Public methods

  }  // class AccountsChart

}  // namespace Empiria.FinancialAccounting
