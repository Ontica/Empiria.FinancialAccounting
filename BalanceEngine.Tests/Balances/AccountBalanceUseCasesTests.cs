/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test cases                              *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Use cases tests                         *
*  Type     : AccountBalanceUseCasesTests                License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Test cases for an account balance.                                                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using Xunit;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.UseCases;

namespace Empiria.FinancialAccounting.Tests {


  /// <summary>Test cases for an account balance.</summary>
  public class AccountBalanceUseCasesTests {

    #region Fields

    private readonly AccountBalanceUseCases _usecases;

    #endregion Fields

    #region Initialization

    public AccountBalanceUseCasesTests() {
      CommonMethods.Authenticate();

      _usecases = AccountBalanceUseCases.UseCaseInteractor();
    }

    ~AccountBalanceUseCasesTests() {
      _usecases.Dispose();
    }

    #endregion Initialization

    #region Facts

    [Fact]
    public void Should_Get_An_Acccount_Balance() {
      AccountBalanceDto balance = _usecases.AccountBalance(TestingConstants.ACCOUNT_NUMBER);

      Assert.Equal(TestingConstants.ACCOUNT_NUMBER, balance.AccountNumber);
      Assert.Equal(TestingConstants.ACCOUNT_NAME, balance.AccountName);
      Assert.Equal(TestingConstants.ACCOUNT_BALANCE, balance.Total);
    }

    #endregion Facts

  }  // class AccountBalanceUseCasesTests

}  // namespace Empiria.FinancialAccounting.Tests
