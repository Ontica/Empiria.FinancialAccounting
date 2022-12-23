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
    private Lazy<AccountsChartRules> _rules;

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
      var list = GetList();

      foreach (var item in list) {
        _ = item.Accounts;
      }
    }


    static public FixedList<AccountsChart> GetList() {
      return BaseObject.GetList<AccountsChart>(String.Empty, "ObjectId DESC")
                       .FindAll(x => x.Status != StateEnums.EntityStatus.Deleted)
                       .ToFixedList();
    }


    static public AccountsChart Empty => BaseObject.ParseEmpty<AccountsChart>();


    internal void Refresh() {

      if (!this.IsEmptyInstance) {
        _accounts = new Lazy<EmpiriaHashTable<Account>>(() => AccountsChartData.GetAccounts(this));
      } else {
        _accounts = new Lazy<EmpiriaHashTable<Account>>(() => new EmpiriaHashTable<Account>());
      }

      _rules = new Lazy<AccountsChartRules>(() => new AccountsChartRules(this));
    }


    protected override void OnLoad() {
      if (!this.IsEmptyInstance) {
        this.MasterData = new AccountsChartMasterData(this, this.ExtendedDataField);
      } else {
        this.MasterData = new AccountsChartMasterData(this);
      }

      Refresh();
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

    internal string BuildSearchAccountsFilter(string keywords) {
      keywords = EmpiriaString.TrimSpacesAndControl(keywords);

      string[] keywordsParts = keywords.Split(' ');

      string accountNumber = string.Empty;

      for (int i = 0; i < keywordsParts.Length; i++) {
        string part = keywordsParts[i];

        part = EmpiriaString.RemovePunctuation(part)
                            .Replace(" ", string.Empty);

        if (EmpiriaString.IsInteger(part)) {
          accountNumber = part;
          keywordsParts[i] = string.Empty;
          break;
        }
      }

      if (accountNumber.Length != 0 && keywordsParts.Length == 1) {
        accountNumber = this.FormatAccountNumber(accountNumber);

        return $"NUMERO_CUENTA_ESTANDAR LIKE '{accountNumber}%'";

      } else if (accountNumber.Length != 0 && keywordsParts.Length > 1) {
        accountNumber = this.FormatAccountNumber(accountNumber);

        return $"NUMERO_CUENTA_ESTANDAR LIKE '{accountNumber}%' AND " +
               SearchExpression.ParseAndLikeKeywords("keywords_cuenta_estandar_hist", String.Join(" ", keywordsParts));

      } else {
        return SearchExpression.ParseAndLikeKeywords("keywords_cuenta_estandar_hist", keywords);
      }
    }


    public Account GetAccount(string accountNumber) {
      Account account = TryGetAccount(accountNumber);

      if (account != null) {
        return account;
      }

      throw Assertion.EnsureNoReachThisCode($"Account {accountNumber} was not found.");
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


    public bool IsAccountNumberFormatValid(string accountNumber) {
      var formatted = FormatAccountNumber(accountNumber);

      if (this.Equals(AccountsChart.IFRS) && formatted.Contains("00")) {
        return false;
      }

      if (this.Equals(AccountsChart.IFRS) && !EmpiriaString.All(formatted, "0123456789.")) {
        return false;
      }

      if (this.Equals(AccountsChart.IFRS) && formatted.StartsWith("0")) {
        return false;
      }

      return (formatted == accountNumber);
    }


    public string FormatAccountNumber(string accountNumber) {
      string temp = EmpiriaString.TrimSpacesAndControl(accountNumber);

      Assertion.Require(temp, nameof(accountNumber));

      char separator = this.MasterData.AccountNumberSeparator;
      string pattern = this.MasterData.AccountsPattern;

      temp = temp.Replace(separator.ToString(), string.Empty);

      temp = temp.TrimEnd('0');

      if (temp.Length > EmpiriaString.CountOccurences(pattern, '0')) {
        Assertion.RequireFail($"Number of placeholders in pattern ({pattern}) is less than the " +
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


    internal string GetParentAccountNumber(string number) {
      var accountNumberSeparator = this.MasterData.AccountNumberSeparator;

      return number.Substring(0, number.LastIndexOf(accountNumberSeparator));
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
      var parentAccountNumber = GetParentAccountNumber(accountNumber);

      return TryGetAccount(parentAccountNumber);
    }


    #endregion Public methods

  }  // class AccountsChart

}  // namespace Empiria.FinancialAccounting
