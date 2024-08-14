/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart                             Component : Test cases                              *
*  Assembly : Empiria.FinancialAccounting.Tests.dll      Pattern   : Use cases tests                         *
*  Type     : AccountsChartUseCasesTests                 License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Test cases for retrieving accounts from the accounts chart.                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Xunit;

using Empiria.Tests;

using Empiria.FinancialAccounting.Adapters;
using Empiria.FinancialAccounting.UseCases;

namespace Empiria.FinancialAccounting.Tests {

  /// <summary>Test cases for retrieving accounts from the accounts chart.</summary>
  public class AccountsChartUseCasesTests {

    #region Use cases initialization

    private readonly AccountsChartUseCases _usecases;

    public AccountsChartUseCasesTests() {
      TestsCommonMethods.Authenticate();

      _usecases = AccountsChartUseCases.UseCaseInteractor();
    }

    ~AccountsChartUseCasesTests() {
      _usecases.Dispose();
    }

    #endregion Use cases initialization


    #region Facts

    [Fact]
    public void Should_Get_An_Account() {
      AccountDto account = _usecases.GetAccount(TestingConstants.ACCOUNTS_CHART_UID,
                                                TestingConstants.ACCOUNT_UID,
                                                DateTime.Today);

      Assert.Equal(TestingConstants.ACCOUNT_UID, account.UID);
      Assert.NotEmpty(account.AreaRules);
      Assert.NotEmpty(account.CurrencyRules);
      Assert.NotEmpty(account.LedgerRules);
      Assert.NotEmpty(account.SectorRules);
    }


    [Fact]
    public void Should_Get_Accounts() {
      AccountsChartDto accountsChart = _usecases.GetAccounts(TestingConstants.ACCOUNTS_CHART_UID);

      Assert.Equal(TestingConstants.ACCOUNTS_CHART_UID, accountsChart.UID);
    }


    [Fact]
    public void Should_Get_The_List_Of_Accounts_Chart() {
      FixedList<NamedEntityDto> accountsChartsList = _usecases.GetAccountsChartsList();

      Assert.Equal(TestingConstants.ACCOUNTS_CHART_COUNT, accountsChartsList.Count);
    }


    [Fact]
    public void Should_Get_The_List_Of_Accounts_Charts_Master_Data() {
      FixedList<AccountsChartMasterDataDto> masterDataList = _usecases.GetAccountsChartsMasterData();

      Assert.Equal(TestingConstants.ACCOUNTS_CHART_COUNT, masterDataList.Count);
    }


    [Fact]
    public void Should_Search_Accounts() {
      var query = new AccountsQuery {
        Date = new DateTime(2005, 10, 23),
        FromAccount = "1000",
        ToAccount = "5000",
        Keywords = "Circulante",
      };

      AccountsChartDto accountsChart = _usecases.SearchAccounts(TestingConstants.ACCOUNTS_CHART_UID,
                                                                query);

      Assert.Equal(TestingConstants.ACCOUNTS_CHART_UID, accountsChart.UID);
    }


    #endregion Facts

  }  // class AccountsChartUseCasesTests

}  // namespace Empiria.FinancialAccounting.Tests
