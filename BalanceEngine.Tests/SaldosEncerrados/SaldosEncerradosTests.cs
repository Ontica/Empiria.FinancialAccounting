/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test cases                              *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Use cases tests                         *
*  Type     : SaldosEncerradosTests                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use case tests for 'Saldos encerrados' report.                                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Threading.Tasks;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.UseCases;
using Xunit;

namespace Empiria.FinancialAccounting.Tests.BalanceEngine.SaldosEncerrados {

  /// <summary>Use case tests for 'Saldos encerrados' report.</summary>
  public class SaldosEncerradosTests {


    [Fact]
    public async Task ShouldBuildLockedUpBalances() {

      using (var service = TrialBalanceUseCases.UseCaseInteractor()) {

        SaldosEncerradosQuery query = new SaldosEncerradosQuery {
          AccountsChartUID = AccountsChart.IFRS.UID,
          FromDate = new DateTime(2022, 02, 01),
          ToDate = new DateTime(2022, 02, 28)
        };

        SaldosEncerradosDto sut = await service.BuildSaldosEncerrados(query);
        Assert.NotNull(sut);
        Assert.NotNull(sut.Entries);
      }
    }

  }

} // namespace Empiria.FinancialAccounting.Tests.BalanceEngine.SaldosEncerrados
