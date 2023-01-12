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

    #region Fields

    private Lazy<EmpiriaHashTable<Account>> _accounts;
    private Lazy<EmpiriaHashTable<StandardAccount>> _standardAccounts;
    private Lazy<AccountsChartRules> _rules;

    #endregion Fields

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


    static public AccountsChart Former {
      get {
        return AccountsChart.Parse(1);
      }
    }


    static public AccountsChart IFRS {
      get {
        return AccountsChart.Parse(152);
      }
    }


    static public void Preload() {
      FixedList<AccountsChart> list = GetList();

      foreach (var item in list) {
        _ = item._accounts.Value;
        _ = item._standardAccounts.Value;
      }
    }


    static public FixedList<AccountsChart> GetList() {
      return BaseObject.GetList<AccountsChart>(String.Empty, "ObjectId DESC")
                       .FindAll(x => x.Status != StateEnums.EntityStatus.Deleted)
                       .ToFixedList();
    }


    static public AccountsChart Empty => BaseObject.ParseEmpty<AccountsChart>();


    internal void Refresh(bool afterUpdated) {
      _accounts = new Lazy<EmpiriaHashTable<Account>>(() => AccountsChartData.GetAccounts(this, true));
      _standardAccounts = new Lazy<EmpiriaHashTable<StandardAccount>>(() => AccountsChartData.GetStandardAccounts(this, true));
      _rules = new Lazy<AccountsChartRules>(() => new AccountsChartRules(this));

      if (afterUpdated) {
        _ = _accounts.Value;
        _ = _standardAccounts.Value;
        _ = _rules.Value;
      }
    }


    protected override void OnLoad() {
      if (!this.IsEmptyInstance) {
        this.MasterData = new AccountsChartMasterData(this, this.ExtendedDataField);
      } else {
        this.MasterData = new AccountsChartMasterData(this);
      }

      Refresh(false);
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

    public AccountsChartRules Rules {
      get {
        return _rules.Value;
      }
    }


    #endregion Public properties

    #region Public methods

    public Account GetAccount(string accountNumber) {
      Account account = TryGetAccount(accountNumber);

      Assertion.Require(account, $"Account '{accountNumber}' was not found in the history log.");

      return account;
    }


    public StandardAccount GetStandardAccount(string accountNumber) {
      StandardAccount stdAccount;

      _standardAccounts.Value.TryGetValue(accountNumber, out stdAccount);

      Assertion.Require(stdAccount, $"Standard account '{accountNumber}' was not found.");

      return stdAccount;
    }


    public Account TryGetAccount(string accountNumber) {
      Account account;

      _accounts.Value.TryGetValue(accountNumber, out account);

      return account;
    }


    public StandardAccount TryGetStandardAccount(string accountNumber) {
      StandardAccount stdAccount;

      _standardAccounts.Value.TryGetValue(accountNumber, out stdAccount);

      return stdAccount;
    }


    public FixedList<Account> GetAccountHistory(string accountNumber) {
      return AccountsChartData.GetAccountHistory(this, accountNumber);
    }


    public Account GetAccountHistory(string accountNumber, DateTime date) {
      FixedList<Account> history = AccountsChartData.GetAccountHistory(this, accountNumber);

      Account account = history.Find(x => x.StartDate <= date && date <= x.EndDate);

      Assertion.Require(account,
                  $"La cuenta {accountNumber} " +
                  $"no existe o no está activa el día {date.ToString("dd/MMM/yyyy")}.");

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


    public Ledger GetLedger(string ledgerNumber) {
      var ledger = this.TryGetLedger(ledgerNumber);

      Assertion.Require(ledger, "ledger");

      return ledger;
    }


    public bool HasChildren(Account account) {
      return this.GetChildren(account).Count > 0;
    }


    internal bool IsAccountingDateOpened(DateTime accountingDate) {
      FixedList<DateTime> openedAccoutingDates = this.OpenedAccountingDates();

      return openedAccoutingDates.Contains(accountingDate);
    }


    public bool IsValidAccountNumber(string accountNumber) {
      var formatter = new AccountsFormatter(this);

      return formatter.IsAccountNumberFormatValid(accountNumber);
    }


    public string FormatAccountNumber(string accountNumber) {
      var formatter = new AccountsFormatter(this);

      return formatter.FormatAccountNumber(accountNumber);
    }


    internal string BuildParentAccountNumber(string accountNumber) {
      var formatter = new AccountsFormatter(this);

      return formatter.BuildParentAccountNumber(accountNumber);
    }


    public FixedList<DateTime> OpenedAccountingDates() {
      return this.MasterData.Calendar.OpenedAccountingDates();
    }


    public FixedList<Account> Search(string filter) {
      return AccountsChartData.SearchAccounts(this, filter);
    }


    public Ledger TryGetLedger(string ledgerNumber) {
      Assertion.Require(ledgerNumber, "ledgerNumber");

      return this.MasterData.Ledgers.Find(x => x.Number.Equals(ledgerNumber));
    }


    internal Account TryGetParentAccount(string accountNumber) {
      var parentAccountNumber = BuildParentAccountNumber(accountNumber);

      return TryGetAccount(parentAccountNumber);
    }


    internal FixedList<Account> TryGetAccountsWithChange(
              AccountsChart accountsChart, DateTime fromDate, DateTime toDate) {

      return AccountsChartData.GetAccountsWithChanges(accountsChart, fromDate, toDate);
    }


    #endregion Public methods

  }  // class AccountsChart

}  // namespace Empiria.FinancialAccounting
