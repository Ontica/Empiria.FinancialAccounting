/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test cases                              *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Use cases tests                         *
*  Type     : AnaliticoCuentasUseCasesTests              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use case tests for 'Analitico de cuentas' report                                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Threading.Tasks;

using Xunit;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.UseCases;

namespace Empiria.FinancialAccounting.Tests.BalanceEngine.AnaliticoCuentas {

  /// <summary>Use case tests for 'Analitico de cuentas' report.</summary>
  public class AnaliticoCuentasUseCasesTests {

    #region Initialization

    public AnaliticoCuentasUseCasesTests() {
      CommonMethods.Authenticate();
    }

    #endregion Initialization

    #region Theories

    [Theory]
    [InlineData(AnaliticoDeCuentasTestCase.Default)]
    [InlineData(AnaliticoDeCuentasTestCase.EnCascada)]
    public async Task CommandsMustBeEqual(AnaliticoDeCuentasTestCase testcase) {
      TrialBalanceCommand command = testcase.GetInvocationCommand();

      using (var usecase = TrialBalanceUseCases.UseCaseInteractor()) {
        AnaliticoDeCuentasDto sut = await usecase.BuildAnaliticoDeCuentas(command);

        Assert.Equal(command.GetHashCode(), sut.Command.GetHashCode());
        Assert.Equal(command, sut.Command);
      }

    }

     #endregion Theories

  } // class AnaliticoCuentasUseCasesTests

} // namespace Empiria.FinancialAccounting.Tests.Balances
