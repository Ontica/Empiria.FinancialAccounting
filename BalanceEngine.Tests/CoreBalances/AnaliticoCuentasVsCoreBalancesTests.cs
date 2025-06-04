/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test cases                              *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Use cases tests                         *
*  Type     : AnaliticoCuentasVsCoreBalancesTests        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Test cases that compares AnaliticoCuentas vs core balances tests.                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System;
using System.Linq;

using Xunit;

using Empiria.FinancialAccounting;
using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.Tests.FinancialAccounting.BalanceEngine {

  /// <summary>Test cases that compares AnaliticoCuentas vs core balances tests.</summary>
  public class AnaliticoCuentasVsCoreBalancesTests {

    [Theory]
    [InlineData("2024-01-01", "2024-01-31")]
    [InlineData("2024-02-01", "2024-02-29")]
    [InlineData("2024-03-01", "2024-03-31")]
    [InlineData("2024-04-01", "2024-04-30")]
    [InlineData("2024-05-01", "2024-05-31")]
    [InlineData("2024-06-01", "2024-06-30")]
    [InlineData("2024-07-01", "2024-07-31")]
    [InlineData("2024-08-01", "2024-08-31")]
    [InlineData("2024-09-01", "2024-09-30")]
    [InlineData("2024-10-01", "2024-10-31")]
    [InlineData("2024-11-01", "2024-11-30")]
    [InlineData("2024-12-01", "2024-12-31")]
    [InlineData("2025-01-01", "2025-01-31")]
    [InlineData("2025-02-01", "2025-02-28")]
    [InlineData("2025-03-01", "2025-03-31")]
    [InlineData("2025-04-01", "2025-04-30")]
    public void Should_Have_Same_Entries(string fromDate, string toDate) {

      CoreBalanceEntries coreBalances = TestsHelpers.GetCoreBalanceEntries(DateTime.Parse(fromDate),
                                                                           DateTime.Parse(toDate),
                                                                           ExchangeRateType.ValorizacionBanxico);

      FixedList<AnaliticoDeCuentasEntryDto> analitico = TestsHelpers.GetAnaliticoCuentas(DateTime.Parse(fromDate),
                                                                                         DateTime.Parse(toDate))
                                                                    .FindAll(x => x.ItemType == TrialBalanceItemType.Entry);
      RunTest(coreBalances, analitico);

      Assert.True(analitico.Count > 500);
    }


    [Theory]
    [InlineData("2024-01-01", "2024-01-31")]
    [InlineData("2024-02-01", "2024-02-29")]
    [InlineData("2024-03-01", "2024-03-31")]
    [InlineData("2024-04-01", "2024-04-30")]
    [InlineData("2024-05-01", "2024-05-31")]
    [InlineData("2024-06-01", "2024-06-30")]
    [InlineData("2024-07-01", "2024-07-31")]
    [InlineData("2024-08-01", "2024-08-31")]
    [InlineData("2024-09-01", "2024-09-30")]
    [InlineData("2024-10-01", "2024-10-31")]
    [InlineData("2024-11-01", "2024-11-30")]
    [InlineData("2024-12-01", "2024-12-31")]
    [InlineData("2025-01-01", "2025-01-31")]
    [InlineData("2025-02-01", "2025-02-28")]
    [InlineData("2025-03-01", "2025-03-31")]
    [InlineData("2025-04-01", "2025-04-30")]
    public void Should_Have_Same_Summaries(string fromDate, string toDate) {

      CoreBalanceEntries coreBalances = TestsHelpers.GetCoreBalanceEntries(DateTime.Parse(fromDate),
                                                                           DateTime.Parse(toDate),
                                                                           ExchangeRateType.ValorizacionBanxico);

      FixedList<AnaliticoDeCuentasEntryDto> analitico = TestsHelpers.GetAnaliticoCuentas(DateTime.Parse(fromDate),
                                                                                         DateTime.Parse(toDate))
                                                                    .FindAll(x => x.ItemType == TrialBalanceItemType.Summary);
      RunTest(coreBalances, analitico);

      Assert.True(analitico.Count > 100);
    }


    private void RunTest(CoreBalanceEntries coreBalances, FixedList<AnaliticoDeCuentasEntryDto> analitico) {

      foreach (var sut in analitico) {

        var filtered = coreBalances.GetBalancesByAccountAndSector(sut.AccountNumber, sut.SectorCode)
                                   .FindAll(x => x.Account.DebtorCreditor == sut.DebtorCreditor);

        var totalMN = filtered.FindAll(x => x.Currency.Equals(Currency.MXN) ||
                                            x.Currency.Equals(Currency.UDI))
                              .Sum(x => x.CurrentBalance);

        var totalME = filtered.FindAll(x => x.Currency.Distinct(Currency.MXN) &&
                                            x.Currency.Distinct(Currency.UDI))
                              .Sum(x => x.CurrentBalance);

        Assert.True(Math.Abs(totalMN - sut.DomesticBalance) <= 1,
                    TestsHelpers.BalanceDiffMsg("Mon Nacional", $"{sut.AccountNumber}, sector {sut.SectorCode}",
                                                totalMN, sut.DomesticBalance));

        Assert.True(Math.Abs(totalME - sut.ForeignBalance) <= 1,
                    TestsHelpers.BalanceDiffMsg("Mon Extranjera", $"{sut.AccountNumber}, sector {sut.SectorCode}",
                                                totalME, sut.ForeignBalance));

        Assert.True(Math.Abs(totalMN + totalME - sut.TotalBalance) <= 1,
                    TestsHelpers.BalanceDiffMsg("Total", $"{sut.AccountNumber}, sector {sut.SectorCode}",
                                                totalMN + totalME, sut.TotalBalance));
      }
    }

  } // class AnaliticoCuentasVsCoreBalancesTests

}  // namespace Empiria.Tests.FinancialAccounting.BalanceEngine
