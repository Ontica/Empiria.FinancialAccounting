/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test cases                              *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Use cases tests                         *
*  Type     : BalanzaComparativaUseCasesTests            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use case tests for 'Balanza comparativa' report.                                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Threading.Tasks;

using Xunit;

using Empiria.Tests;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.UseCases;

namespace Empiria.FinancialAccounting.Tests.BalanceEngine.BalanzaComparativa {

  /// <summary>Use case tests for 'Balanza comparativa' report.</summary>
  public class BalanzaComparativaUseCasesTests {

    #region Initialization

    public BalanzaComparativaUseCasesTests() {
      TestsCommonMethods.Authenticate();
    }

    #endregion Initialization

    #region Theories

    [Theory]
    [InlineData(BalanzaComparativaTestCase.Default)]
    [InlineData(BalanzaComparativaTestCase.CatalogoAnterior)]
    public async Task QueriesMustBeEqual(BalanzaComparativaTestCase testcase) {
      TrialBalanceQuery query = testcase.GetInvocationQuery();

      using (var usecase = TrialBalanceUseCases.UseCaseInteractor()) {
        BalanzaComparativaDto sut = await usecase.BuildBalanzaComparativa(query);

        Assert.Equal(query.GetHashCode(), sut.Query.GetHashCode());
        Assert.Equal(query, sut.Query);
      }

    }

    #endregion Theories


  } // class BalanzaComparativaUseCasesTests

} // namespace Empiria.FinancialAccounting.Tests.BalanceEngine.BalanzaComparativa
