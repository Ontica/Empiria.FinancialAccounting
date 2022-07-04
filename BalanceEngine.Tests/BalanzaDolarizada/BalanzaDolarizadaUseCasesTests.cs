/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test cases                              *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Use cases tests                         *
*  Type     : BalanzaDolarizadaUseCasesTests             License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use case tests for 'Balanza dolarizada' report.                                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Threading.Tasks;

using Xunit;

using Empiria.Tests;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.UseCases;

namespace Empiria.FinancialAccounting.Tests.BalanceEngine.BalanzaDolarizada {

  /// <summary>Use case tests for 'Balanza dolarizada' report.</summary>
  public class BalanzaDolarizadaUseCasesTests {

    #region Initialization

    public BalanzaDolarizadaUseCasesTests() {
      TestsCommonMethods.Authenticate();
    }

    #endregion Initialization

    #region Theories

    [Theory]
    [InlineData(BalanzaDolarizadaTestCase.Default)]
    [InlineData(BalanzaDolarizadaTestCase.CatalogoAnterior)]
    public async Task QueriesMustBeEqual(BalanzaDolarizadaTestCase testcase) {
      TrialBalanceQuery query = testcase.GetInvocationQuery();

      using (var usecase = TrialBalanceUseCases.UseCaseInteractor()) {
        BalanzaDolarizadaDto sut = await usecase.BuildBalanzaDolarizada(query);

        Assert.Equal(query.GetHashCode(), sut.Query.GetHashCode());
        Assert.Equal(query, sut.Query);
      }

    }

    #endregion Theories

  } // class BalanzaDolarizadaUseCasesTests

} // namespace Empiria.FinancialAccounting.Tests.BalanceEngine.BalanzaDolarizada 
