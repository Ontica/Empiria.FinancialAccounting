/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart                             Component : Use cases Layer                         *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Use case interactor class               *
*  Type     : AccountsChartUseCases                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases for accounts chart searching and retriving.                                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.FinancialAccounting.Adapters;

namespace Empiria.FinancialAccounting.UseCases
{

    /// <summary>Use cases for accounts chart searching and retriving.</summary>
    public class AccountsChartUseCases : UseCase {

    #region Constructors and parsers

    protected AccountsChartUseCases() {
      // no-op
    }

    static public AccountsChartUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<AccountsChartUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases

    public AccountDto GetAccount(string accountsChartUID, string accountUID, DateTime date) {
      Assertion.Require(accountsChartUID, nameof(accountsChartUID));
      Assertion.Require(accountUID, nameof(accountUID));

      var accountsChart = AccountsChart.Parse(accountsChartUID);

      Account account = Account.Parse(accountUID);

      Assertion.Require(account.AccountsChart.Equals(accountsChart),
          $"The account with uid {account.UID} does not belong " +
          $"to accounts chart {accountsChart.Name}.");

      return AccountsChartMapper.MapAccount(account, date);
    }


    public AccountsChartDto GetAccounts(string accountsChartUID) {
      Assertion.Require(accountsChartUID, nameof(accountsChartUID));

      var accountsChart = AccountsChart.Parse(accountsChartUID);

      return AccountsChartMapper.Map(accountsChart);
    }


    static public AccountsChartDto GetAccounts(AccountsChartQueryDto query) {
      Assertion.Require(query, nameof(query));

      FixedList<Account> accounts = query.AccountsChart.GetAccountsInAPeriod(query.FromDate,
                                                                             query.ToDate);

      return AccountsChartMapper.Map(query.AccountsChart, accounts);
    }


    static public FixedList<AccountRange> GetAccountRangeToFilter(string[] accounts) {

      return AccountRangeClauses.GetAccountRangeToFilter(accounts);
    }


    public static FixedList<AccountRange> GetAccountRangeToFilter(string fromAccount, string toAccount) {

      return AccountRangeClauses.GetAccountRangeToFilter(fromAccount, toAccount);
    }


    public AccountsChartDto GetAccountsInADate(string accountsChartUID, DateTime date) {
      Assertion.Require(accountsChartUID, nameof(accountsChartUID));

      var accountsChart = AccountsChart.Parse(accountsChartUID);

      FixedList<Account> accounts = accountsChart.GetAccountsInADate(date);

      return AccountsChartMapper.Map(accountsChart, accounts);
    }

    
    public FixedList<NamedEntityDto> GetAccountsChartsList() {
      FixedList<AccountsChart> accountsChartsList = AccountsChart.GetList();

      return accountsChartsList.MapToNamedEntityList();
    }


    public FixedList<AccountsChartMasterDataDto> GetAccountsChartsMasterData() {
      FixedList<AccountsChart> accountsChartsList = AccountsChart.GetList();

      var masterDataList = new FixedList<AccountsChartMasterData>(accountsChartsList.Select(x => x.MasterData));

      return AccountsChartMasterDataMapper.Map(masterDataList);
    }


    public AccountsChartDto SearchAccounts(string accountsChartUID,
                                           AccountsQuery query) {
      Assertion.Require(accountsChartUID, nameof(accountsChartUID));
      Assertion.Require(query, nameof(query));

      var accountsChart = AccountsChart.Parse(accountsChartUID);

      string filter = query.MapToFilterString(accountsChart);

      FixedList<Account> accounts = accountsChart.Search(filter);

      accounts = query.ApplyTo(accounts);

      if (query.WithSectors) {
        return AccountsChartMapper.MapWithSectors(accountsChart, accounts);
      } else {
        return AccountsChartMapper.Map(accountsChart, accounts);
      }
    }


    #endregion Use cases

  }  // class AccountsChartUseCases

}  // namespace Empiria.FinancialAccounting.UseCases
