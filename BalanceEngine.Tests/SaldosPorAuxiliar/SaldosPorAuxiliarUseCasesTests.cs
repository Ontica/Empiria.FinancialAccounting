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

    #endregion Theories

  } // class SaldosPorAuxiliarUseCasesTests

} // namespace Empiria.FinancialAccounting.Tests.BalanceEngine.SaldosPorAuxiliar
