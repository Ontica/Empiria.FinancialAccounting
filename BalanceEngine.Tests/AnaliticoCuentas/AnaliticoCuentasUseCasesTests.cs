/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test cases                              *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Use cases tests                         *
*  Type     : AnaliticoCuentasUseCasesTests              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Test cases for Analitico de Cuentas report.                                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Threading.Tasks;

using Xunit;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.Tests.BalanceEngine {

  /// <summary>Test cases for trial balance reports.</summary>
  public class AnaliticoCuentasUseCasesTests {

    #region Initialization

    public AnaliticoCuentasUseCasesTests() {
      CommonMethods.Authenticate();
    }

    #endregion Initialization

    #region Theories

    [Theory]
    [InlineData(AnaliticoCuentasTestCommandCase.Default)]
    [InlineData(AnaliticoCuentasTestCommandCase.EnCascada)]
    public async Task CommandsMustBeEqual(AnaliticoCuentasTestCommandCase commandCase) {
      TrialBalanceCommand command = commandCase.BuildCommand();

      AnaliticoDeCuentasDto sut = await BalanceEngineUseCaseProxy.BuildAnaliticoDeCuentas(command);

      //EmpiriaLog.Trace(commandCase + "=> Expected => " + Json.JsonObject.Parse(command).ToString());
      //EmpiriaLog.Trace(commandCase + "=> Actual   => " + Json.JsonObject.Parse(sut.Command).ToString());

      Assert.Equal(command.GetHashCode(), sut.Command.GetHashCode());
      Assert.Equal(command, sut.Command);
    }


    [Theory]
    [InlineData(AnaliticoCuentasTestCommandCase.Default)]
    [InlineData(AnaliticoCuentasTestCommandCase.EnCascada)]
    public async Task MustHaveEntries(AnaliticoCuentasTestCommandCase commandCase) {
      TrialBalanceCommand command = commandCase.BuildCommand();

      AnaliticoDeCuentasDto sut = await BalanceEngineUseCaseProxy.BuildAnaliticoDeCuentas(command);

      Assert.NotNull(sut.Entries);
    }

    #endregion Theories

  } // class AnaliticoCuentasUseCasesTests

} // namespace Empiria.FinancialAccounting.Tests.Balances
