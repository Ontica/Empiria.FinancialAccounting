/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test cases                              *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Use cases tests                         *
*  Type     : BalanzaColumnasMonedaUseCasesTests         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use case tests for 'Balanza en columnas por moneda' report.                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Threading.Tasks;

using Xunit;

using Empiria.Tests;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.UseCases;

namespace Empiria.FinancialAccounting.Tests.BalanceEngine.BalanzaColumnasMoneda {

  /// <summary>Use case tests for 'Balanza en columnas por moneda' report.</summary>
  public class BalanzaColumnasMonedaUseCasesTests {

    #region Initialization

    public BalanzaColumnasMonedaUseCasesTests() {
      TestsCommonMethods.Authenticate();
    }

    #endregion Initialization

    #region Theories

    [Theory]
    [InlineData(BalanzaColumnasMonedaTestCase.Default)]
    [InlineData(BalanzaColumnasMonedaTestCase.CatalogoAnterior)]
    public async Task QueriesMustBeEqual(BalanzaColumnasMonedaTestCase testcase) {
      TrialBalanceQuery query = testcase.GetInvocationQuery();

      using (var usecase = TrialBalanceUseCases.UseCaseInteractor()) {
        BalanzaColumnasMonedaDto sut = await usecase.BuildBalanzaColumnasMoneda(query);

        Assert.Equal(query.GetHashCode(), sut.Query.GetHashCode());
        Assert.Equal(query, sut.Query);
      }

    }

    #endregion Theories

  } // class BalanzaColumnasMonedaUseCasesTests

} // namespace Empiria.FinancialAccounting.Tests.BalanceEngine.BalanzaColumnasMoneda
