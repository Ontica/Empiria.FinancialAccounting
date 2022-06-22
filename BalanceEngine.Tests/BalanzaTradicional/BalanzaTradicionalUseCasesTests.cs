/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test cases                              *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Use cases tests                         *
*  Type     : BalanzaTradicionalUseCasesTests            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use case tests for 'Balanza tradicional' report.                                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Threading.Tasks;

using Xunit;

using Empiria.Tests;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.UseCases;

namespace Empiria.FinancialAccounting.Tests.BalanceEngine.Balanza {

  /// <summary>Use case tests for 'Balanza tradicional' report.</summary>
  public class BalanzaTradicionalUseCasesTests {

    #region Initialization

    public BalanzaTradicionalUseCasesTests() {
      TestsCommonMethods.Authenticate();
    }

    #endregion Initialization

    #region Theories

    [Theory]
    [InlineData(BalanzaTradicionalTestCase.Default)]
    [InlineData(BalanzaTradicionalTestCase.EnCascada)]
    public async Task QueriesMustBeEqual(BalanzaTradicionalTestCase testcase) {
      TrialBalanceQuery query = testcase.GetInvocationQuery();

      using (var usecase = TrialBalanceUseCases.UseCaseInteractor()) {
        BalanzaTradicionalDto sut = await usecase.BuildBalanzaTradicional(query);

        Assert.Equal(query.GetHashCode(), sut.Query.GetHashCode());
        Assert.Equal(query, sut.Query);
      }

    }


    #endregion Theories

  } // BalanzaTradicionalUseCasesTests

} // namespace Empiria.FinancialAccounting.Tests.BalanceEngine.Balanza
