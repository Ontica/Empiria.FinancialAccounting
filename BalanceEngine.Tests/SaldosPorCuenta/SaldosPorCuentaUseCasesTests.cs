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

    #endregion Theories

  } // class SaldosPorCuentaUseCasesTests

} // namespace Empiria.FinancialAccounting.Tests.BalanceEngine.SaldosPorCuenta
