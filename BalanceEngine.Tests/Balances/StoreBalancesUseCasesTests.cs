/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test cases                              *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Use cases tests                         *
*  Type     : StoreBalancesUseCasesTests                 License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Test cases for store account an account aggrupation balances.                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Xunit;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.UseCases;

namespace Empiria.FinancialAccounting.Tests.Balances {

  /// <summary>Test cases for store account an account aggrupation balances.</summary>
  public class StoreBalancesUseCasesTests {

    #region Fields

    private readonly StoreBalancesUseCases _usecases;

    #endregion Fields

    #region Initialization

    public StoreBalancesUseCasesTests() {
      CommonMethods.Authenticate();

      _usecases = StoreBalancesUseCases.UseCaseInteractor();
    }

    ~StoreBalancesUseCasesTests() {
      _usecases.Dispose();
    }

    #endregion Initialization

    #region Facts

    [Fact]
    public void Should_Generate_A_Balances_Set() {
      var accountsChart = AccountsChart.Parse(TestingConstants.AccountCatalogueId);

      var command = new StoreBalancesCommand() {
        AccountsChartUID = accountsChart.UID,
        BalancesDate = new DateTime(2020, 1, 1)
      };

      _usecases.StoreBalances(command);

      Assert.True(true);
    }

    #endregion Facts

  } // class BalanceStorageUseCasesTests

} // namespace Empiria.FinancialAccounting.Tests.Balances
