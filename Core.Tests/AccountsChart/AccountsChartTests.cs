/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart                             Component : Test cases                              *
*  Assembly : Empiria.FinancialAccounting.Tests.dll      Pattern   : Domain tests                            *
*  Type     : AccountsChartTests                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Test cases for retrieving accounts from the accounts chart.                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using Xunit;

using Empiria.FinancialAccounting.Adapters;


namespace Empiria.FinancialAccounting.Tests {

  /// <summary>Test cases for retrieving accounts from the accounts chart.</summary>
  public class AccountsChartTests {

    #region Facts

    [Fact]
    public void Should_Parse_An_AccountsChart() {
      var chart = AccountsChart.Parse(TestingConstants.ACCOUNTS_CHART_UID);

      Assert.Equal(TestingConstants.ACCOUNTS_CHART_UID, chart.UID);

      Assert.NotEmpty(chart.Accounts);

      chart = AccountsChart.Parse(TestingConstants.ACCOUNTS_CHART_2021_UID);

      Assert.NotEmpty(chart.Accounts);
    }


    [Fact]
    public void Should_Parse_An_Account_By_Id() {
      var account = Account.Parse(TestingConstants.ACCOUNT_ID);

      Assert.Equal(TestingConstants.ACCOUNT_ID, account.Id);
      Assert.Equal(TestingConstants.ACCOUNT_NUMBER, account.Number);
      Assert.Equal(TestingConstants.ACCOUNT_NAME, account.Name);
    }


    [Fact]
    public void Should_Parse_The_Empty_Account() {
      var account = Account.Empty;

      Assert.Equal(-1, account.Id);
      Assert.Equal("Empty", account.UID);
    }


    [Fact]
    public void Should_Search_An_Account_In_An_AccountsChart() {
      var chart = AccountsChart.Parse(TestingConstants.ACCOUNTS_CHART_UID);

      Account account = chart.GetAccount(TestingConstants.ACCOUNT_NUMBER);

      Assert.Equal(TestingConstants.ACCOUNT_NUMBER, account.Number);
      Assert.Equal(TestingConstants.ACCOUNT_NAME, account.Name);
    }


    [Fact]
    public void Should_Search_An_Account_Range_In_An_AccountsChart() {
      var chart = AccountsChart.Parse(TestingConstants.ACCOUNTS_CHART_UID);

      var query = new AccountsQuery {
        FromAccount = "1101",
        ToAccount = "2699"
      };

      string filter = query.MapToFilterString(chart);

      FixedList<Account> accounts = chart.Search(filter);

      Assert.NotNull(accounts);

      Assert.DoesNotContain(accounts, x => query.FromAccount.CompareTo(x.Number) > 0);
    }


    [Fact]
    public void Should_Search_Many_Accounts_In_An_AccountsChart() {
      var chart = AccountsChart.Parse(TestingConstants.ACCOUNTS_CHART_UID);

      var accounts = chart.Accounts;

      foreach (var item in accounts) {
        var account = chart.GetAccount(item.Number);

        Assert.Equal(account.Number, item.Number);
      }
    }

    #endregion Facts

  }  // class AccountsChartTests

}  // namespace Empiria.FinancialAccounting.Tests
