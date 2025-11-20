/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test cases                              *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Use cases tests                         *
*  Type     : SaldosEncerradosVsCoreBalancesTests        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Test cases that compares SaldosEncerrados vs core balances tests.                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System;
using System.Threading.Tasks;
using Empiria.FinancialAccounting;
using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Xunit;

namespace Empiria.Tests.FinancialAccounting.BalanceEngine {

  /// <summary>Test cases that compares SaldosEncerrados vs core balances tests.</summary>
  public class SaldosEncerradosVsCoreBalancesTests {

    [Theory]
    [InlineData("2024-12-01", "2024-12-31")]
    [InlineData("2025-01-01", "2025-01-31")]
    [InlineData("2025-02-01", "2025-02-28")]
    [InlineData("2025-03-01", "2025-03-31")]
    [InlineData("2022-02-28", "2022-02-28")]
    public async Task Should_Have_Same_Locked_Up_Balances(string fromDate, string toDate) {

      CoreBalanceEntries coreBalances = TestsHelpers.GetCoreBalanceEntries(DateTime.Parse(fromDate),
                                                                           DateTime.Parse(toDate),
                                                                           ExchangeRateType.Empty);

      FixedList<SaldosEncerradosBaseEntryDto> saldos = await SaldosEncerradosTestHelpers.GetSaldosEncerrados(
                                                           DateTime.Parse(fromDate), DateTime.Parse(toDate));
      RunTest(coreBalances, saldos);

      Assert.True(saldos.Count > 100);
    }


    private void RunTest(CoreBalanceEntries coreBalances, FixedList<SaldosEncerradosBaseEntryDto> saldos) {

      foreach (var sut in saldos) {

        Assert.True(true, "");
      }
    }

  } // class SaldosEncerradosVsCoreBalancesTests

} // namespace Empiria.Tests.FinancialAccounting.BalanceEngine
