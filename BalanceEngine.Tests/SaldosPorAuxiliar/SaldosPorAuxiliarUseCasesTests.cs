/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test cases                              *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Use cases tests                         *
*  Type     : SaldosPorAuxiliarUseCasesTests             License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use case tests for 'Saldos por auxiliar' report.                                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Threading.Tasks;

using Xunit;

using Empiria.Tests;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.UseCases;

namespace Empiria.FinancialAccounting.Tests.BalanceEngine.SaldosPorAuxiliar {

  /// <summary>Use case tests for 'Saldos por auxiliar' report.</summary>
  public class SaldosPorAuxiliarUseCasesTests {

    #region Initialization

    public SaldosPorAuxiliarUseCasesTests() {
      TestsCommonMethods.Authenticate();
    }

    #endregion Initialization

    #region Theories

    [Theory]
    [InlineData(SaldosPorAuxiliarTestCase.Default)]
    [InlineData(SaldosPorAuxiliarTestCase.CatalogoAnterior)]
    public async Task QueriesMustBeEqual(SaldosPorAuxiliarTestCase testcase) {
      TrialBalanceQuery query = testcase.GetInvocationQuery();

      using (var usecase = TrialBalanceUseCases.UseCaseInteractor()) {
        SaldosPorAuxiliarDto sut = await usecase.BuildSaldosPorAuxiliar(query);

        Assert.Equal(query.GetHashCode(), sut.Query.GetHashCode());
        Assert.Equal(query, sut.Query);
      }

    }


    [Fact]
    public async Task MustBeMultipleSubledgerAccountBalancesTest() {

      TrialBalanceQuery query = GetTrialBalanceQuery();

      using (var usecase = TrialBalanceUseCases.UseCaseInteractor()) {
        SaldosPorAuxiliarDto sut = await usecase.BuildSaldosPorAuxiliar(query);

        Assert.True(sut.Entries.Count > 0);
      }

    }

    #endregion Theories


    #region Helpers

    private TrialBalanceQuery GetTrialBalanceQuery() {
      TrialBalanceQuery query = new TrialBalanceQuery {
        TrialBalanceType = FinancialAccounting.BalanceEngine.TrialBalanceType.SaldosPorAuxiliar,
        AccountsChartUID = "47ec2ec7-0f4f-482e-9799-c23107b60d8a",
        BalancesType = FinancialAccounting.BalanceEngine.BalancesType.WithCurrentBalance,
        ShowCascadeBalances = true,
        WithSubledgerAccount = true,
        InitialPeriod = { FromDate = new DateTime(2023, 06, 01), ToDate = new DateTime(2023, 06, 30) },
        Accounts = new string[] { "1.05.01.01.05.01", "5.01.05.01.01.05.02" },
        SubledgerAccounts = new string[] { "90000000009011515", "90000000009010970" } //,"90000000009010970",
      };
      return query;
    }

    #endregion Helpers

  } // class SaldosPorAuxiliarUseCasesTests

} // namespace Empiria.FinancialAccounting.Tests.BalanceEngine.SaldosPorAuxiliar
