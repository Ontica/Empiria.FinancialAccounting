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

using Empiria.FinancialAccounting.Data;

namespace Empiria.FinancialAccounting.UseCases {

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

    public AccountDto GetAccount(string accountsChartUID, string accountUID) {
      Assertion.AssertObject(accountsChartUID, "accountsChartUID");
      Assertion.AssertObject(accountUID, "accountUID");

      var accountsChart = AccountsChart.Parse(accountsChartUID);

      Account account = Account.Parse(accountUID);

      Assertion.Assert(account.AccountsChart.Equals(accountsChart),
          $"The account with uid {account.UID} does not belong " +
          $"to accounts chart {accountsChart.Name}");

      return AccountsChartMapper.MapAccount(account);
    }


    public AccountsChartDto GetAccounts(string accountsChartUID) {
      Assertion.AssertObject(accountsChartUID, "accountsChartUID");

      var accountsChart = AccountsChart.Parse(accountsChartUID);

      return AccountsChartMapper.Map(accountsChart);
    }


    public AccountsChartDto GetAccountsInADate(string accountsChartUID, DateTime date) {
      Assertion.AssertObject(accountsChartUID, "accountsChartUID");

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
                                           AccountsSearchCommand command) {
      Assertion.AssertObject(accountsChartUID, "accountsChartUID");
      Assertion.AssertObject(command, "command");

      var accountsChart = AccountsChart.Parse(accountsChartUID);

      string filter = command.MapToFilterString(accountsChart);

      FixedList<Account> accounts = accountsChart.Search(filter);

      accounts = command.Restrict(accounts);

      if (command.WithSectors) {
        return AccountsChartMapper.MapWithSectors(accountsChart, accounts);
      } else {
        return AccountsChartMapper.Map(accountsChart, accounts);
      }
    }


    #endregion Use cases

  }  // class AccountsChartUseCases

}  // namespace Empiria.FinancialAccounting.UseCases
