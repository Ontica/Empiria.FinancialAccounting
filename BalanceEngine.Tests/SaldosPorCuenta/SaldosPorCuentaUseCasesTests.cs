/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test cases                              *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Use cases tests                         *
*  Type     : SaldosPorCuentaUseCasesTests               License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use case tests for 'Saldos por cuenta' report.                                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Threading.Tasks;

using Xunit;

using Empiria.Tests;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.UseCases;

namespace Empiria.FinancialAccounting.Tests.BalanceEngine.SaldosPorCuenta {

  /// <summary>Use case tests for 'Saldos por cuenta' report.</summary>
  public class SaldosPorCuentaUseCasesTests {

    #region Initialization

    public SaldosPorCuentaUseCasesTests() {
      TestsCommonMethods.Authenticate();
    }

    #endregion Initialization

    #region Theories

    [Theory]
    [InlineData(SaldosPorCuentaTestCase.Default)]
    [InlineData(SaldosPorCuentaTestCase.CatalogoAnterior)]
    public async Task QueriesMustBeEqual(SaldosPorCuentaTestCase testcase) {
      TrialBalanceQuery query = testcase.GetInvocationQuery();

      using (var usecase = TrialBalanceUseCases.UseCaseInteractor()) {
        SaldosPorCuentaDto sut = await usecase.BuildSaldosPorCuenta(query);

        Assert.Equal(query.GetHashCode(), sut.Query.GetHashCode());
        Assert.Equal(query, sut.Query);
      }

    }


    [Fact]
    public async Task MustBeMultipleBalancesTest() {

      TrialBalanceQuery query = GetTrialBalanceQuery();

      using (var usecase = TrialBalanceUseCases.UseCaseInteractor()) {
        SaldosPorCuentaDto sut = await usecase.BuildSaldosPorCuenta(query);

        Assert.True(sut.Entries.Count>0);
      }

    }


    #endregion Theories


    #region Helpers

    private TrialBalanceQuery GetTrialBalanceQuery() {
      TrialBalanceQuery query = new TrialBalanceQuery {
        TrialBalanceType = FinancialAccounting.BalanceEngine.TrialBalanceType.SaldosPorCuenta,
        AccountsChartUID = "47ec2ec7-0f4f-482e-9799-c23107b60d8a",
        BalancesType = FinancialAccounting.BalanceEngine.BalancesType.WithCurrentBalance,
        Accounts = new string[] { "1.05.01.01.05.01", "5.01.05.01.01.05.02" },
        SubledgerAccounts = new string[] { "90000000000007096", "90000000009010970", "90000000009011515" },
        ShowCascadeBalances = false,
        WithSubledgerAccount = true,
        InitialPeriod = { FromDate= new DateTime(2023,06,01), ToDate = new DateTime(2023, 06, 30) }
      };
      return query;
    }

    #endregion Helpers

  } // class SaldosPorCuentaUseCasesTests

} // namespace Empiria.FinancialAccounting.Tests.BalanceEngine.SaldosPorCuenta
