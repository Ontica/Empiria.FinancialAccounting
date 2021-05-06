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

using Empiria.FinancialAccounting.Adapters;
using Empiria.FinancialAccounting.UseCases;

namespace Empiria.FinancialAccounting.Tests {

  /// <summary>Test cases for retrieving accounts from the accounts chart.</summary>
  public class AccountsChartUseCasesTests {

    #region Use cases initialization

    private readonly AccountsChartUseCases _usecases;

    public AccountsChartUseCasesTests() {
      CommonMethods.Authenticate();

      _usecases = AccountsChartUseCases.UseCaseInteractor();
    }

    ~AccountsChartUseCasesTests() {
      _usecases.Dispose();
    }

    #endregion Use cases initialization


    #region Facts

    [Fact]
    public void Should_Get_Accounts() {
      AccountsChartDto accountsChart = _usecases.GetAccounts(TestingConstants.ACCOUNTS_CHART_UID);


      Assert.Equal(TestingConstants.ACCOUNTS_CHART_UID, accountsChart.UID);
    }


    [Fact]
    public void Should_Search_Accounts() {
      var searchCommand = new AccountsSearchCommand {
        Date = new DateTime(2005, 10, 23),
        FromAccount = "1000",
        ToAccount = "5000",
        Keywords = "Circulante",
      };

      AccountsChartDto accountsChart = _usecases.SearchAccounts(TestingConstants.ACCOUNTS_CHART_UID,
                                                                searchCommand);

      Assert.Equal(TestingConstants.ACCOUNTS_CHART_UID, accountsChart.UID);
    }


    #endregion Facts

  }  // class AccountsChartUseCasesTests

}  // namespace Empiria.FinancialAccounting.Tests
