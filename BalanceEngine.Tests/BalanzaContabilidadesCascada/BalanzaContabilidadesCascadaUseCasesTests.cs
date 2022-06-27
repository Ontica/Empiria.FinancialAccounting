/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test cases                              *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Use cases tests                         *
*  Type     : BalanzaContabilidadesCascadaUseCasesTests  License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use case tests for 'Balanza con contabilidades en cascada' report.                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Threading.Tasks;

using Xunit;

using Empiria.Tests;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.UseCases;

namespace Empiria.FinancialAccounting.Tests.BalanceEngine.BalanzaContabilidadesCascada {

  /// <summary>Use case tests for 'Balanza con contabilidades en cascada' report.</summary>
  public class BalanzaContabilidadesCascadaUseCasesTests {

    #region Initialization

    public BalanzaContabilidadesCascadaUseCasesTests() {
      TestsCommonMethods.Authenticate();
    }

    #endregion Initialization

    #region Theories

    [Theory]
    [InlineData(BalanzaContabilidadesCascadaTestCase.Default)]
    [InlineData(BalanzaContabilidadesCascadaTestCase.CatalogoAnterior)]
    public async Task QueriesMustBeEqual(BalanzaContabilidadesCascadaTestCase testcase) {
      TrialBalanceQuery query = testcase.GetInvocationQuery();

      using (var usecase = TrialBalanceUseCases.UseCaseInteractor()) {
        BalanzaContabilidadesCascadaDto sut = await usecase.BuildBalanzaContabilidadesCascada(query);

        Assert.Equal(query.GetHashCode(), sut.Query.GetHashCode());
        Assert.Equal(query, sut.Query);
      }

    }

    #endregion Theories

  } // class BalanzaContabilidadesCascadaUseCasesTests

} // namespace Empiria.FinancialAccounting.Tests.BalanceEngine.BalanzaContabilidadesCascada
