/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                               Component : Test cases                            *
*  Assembly : FinancialAccounting.BalanceEngine.Tests      Pattern   : Use cases tests                       *
*  Type     : BalanceStorageUseCasesTests                  License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Test cases for store account an account aggrupation balances.                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Xunit;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.UseCases;

namespace Empiria.FinancialAccounting.Tests.Balances {

  /// <summary>Test cases for store account an account aggrupation balances.</summary>
  public class BalanceStorageUseCasesTests {

    #region Fields

    private readonly BalanceStorageUseCases _usecases;

    #endregion Fields

    #region Initialization

    public BalanceStorageUseCasesTests() {
      CommonMethods.Authenticate();

      _usecases = BalanceStorageUseCases.UseCaseInteractor();
    }

    ~BalanceStorageUseCasesTests() {
      _usecases.Dispose();
    }

    #endregion Initialization

    #region Facts

    [Fact]
    public void Should_Generate_A_Balances_Set() {
      var accountsChart = AccountsChart.Parse(TestingConstants.AccountCatalogueId);

      var BALANCES_DATE = new DateTime(2000, 12, 30);

      var command = new BalanceStorageCommand() {
        BalancesDate = BALANCES_DATE
      };

      var storedBalanceSet = _usecases.CreateOrGetBalanceSet(accountsChart.UID, command);

      Assert.NotNull(storedBalanceSet);
      Assert.Equal(BALANCES_DATE, storedBalanceSet.BalancesDate);
    }


    [Fact]
    public void Should_Get_Balances_Sets() {
      var accountsChart = AccountsChart.Parse(TestingConstants.AccountCatalogueId);

      FixedList<StoredBalanceSetDto> storedBalanceSetsList = _usecases.BalanceSetsList(accountsChart.UID);

      Assert.NotEmpty(storedBalanceSetsList);
    }


    #endregion Facts

  } // class BalanceStorageUseCasesTests

} // namespace Empiria.FinancialAccounting.Tests.Balances
