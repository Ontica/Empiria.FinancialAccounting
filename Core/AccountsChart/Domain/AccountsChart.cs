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
using System.Linq;
using System.Text.RegularExpressions;

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


    static public AccountsChart ParseForDate(DateTime accountingDate) {
      if (accountingDate >= new DateTime(2022, 1, 1)) {
        return AccountsChart.IFRS;
      } else {
        return AccountsChart.Former;
      }
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
      Account account = TryGetAccount(accountNumber);

      if (account != null) {
        return account;
      }

      throw Assertion.AssertNoReachThisCode($"Account {accountNumber} was not found.");
    }


    public StandardAccount GetStandardAccount(string accountNumber) {
      Account account = this.GetAccount(accountNumber);

      return StandardAccount.Parse(account.StandardAccountId);
    }


    public Account TryGetAccount(string accountNumber) {
      Account account;

      if (_accounts.Value.TryGetValue(accountNumber, out account)) {
        return account;
      } else {
        return null;
      }
    }


    public StandardAccount TryGetStandardAccount(string accountNumber) {
      Account account;

      if (_accounts.Value.TryGetValue(accountNumber, out account)) {
        return StandardAccount.Parse(account.StandardAccountId);
      } else {
        return null;
      }
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


    internal bool IsAccountingDateOpened(DateTime accountingDate) {
      FixedList<DateTime> openedAccoutingDates = this.OpenedAccountingDates();

      return openedAccoutingDates.Contains(accountingDate);
    }


    public string FormatAccountNumber(string accountNumber) {
      Assertion.AssertObject(accountNumber, "accountNumber");

      char separator = this.MasterData.AccountNumberSeparator;
      string pattern = this.MasterData.AccountsPattern;

      string temp = accountNumber.Replace(separator.ToString(), string.Empty);
      temp = EmpiriaString.TrimAll(temp);

      temp = temp.TrimEnd('0');

      if (temp.Length > EmpiriaString.CountOccurences(pattern, '0')) {
        Assertion.AssertFail($"Number of placeholders in pattern ({pattern}) is less than " +
                             $"number of characters in the input string ({accountNumber}).");
      } else {
        temp = temp.PadRight(EmpiriaString.CountOccurences(pattern, '0'), '0');
      }

      for (int i = 0; i < pattern.Length; i++) {
        if (pattern[i] == separator) {
          temp = temp.Insert(i, separator.ToString());
        }
      }

      while (true) {
        if (temp.EndsWith($"{separator}0000")) {
          temp = temp.Remove(temp.Length - 5);

        } else if (temp.EndsWith($"{separator}000")) {
          temp = temp.Remove(temp.Length - 4);

        } else if (temp.EndsWith($"{separator}00")) {
          temp = temp.Remove(temp.Length - 3);

        } else if (temp.EndsWith($"{separator}0")) {
          temp = temp.Remove(temp.Length - 2);

        } else {
          break;

        }
      }

      return temp;
    }


    public FixedList<DateTime> OpenedAccountingDates() {
      return this.MasterData.Calendar.OpenedAccountingDates();
    }


    public FixedList<Account> Search(string filter) {
      return AccountsChartData.SearchAccounts(this, filter);
    }


    public Ledger TryGetLedger(string ledgerNumber) {
      Assertion.AssertObject(ledgerNumber, "ledgerNumber");

      return this.MasterData.Ledgers.Find(x => x.Number.Equals(ledgerNumber));
    }


    #endregion Public methods

  }  // class AccountsChart

}  // namespace Empiria.FinancialAccounting
