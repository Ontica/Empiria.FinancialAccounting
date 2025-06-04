/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test cases                              *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Use cases tests                         *
*  Type     : BalanzaColumnasVsCoreBalancesTests         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Test cases that compares BalanzaColumnasPorMonedaOrigen vs core balances tests.                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System;
using System.Linq;
using Empiria.FinancialAccounting;
using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Xunit;

namespace Empiria.Tests.FinancialAccounting.BalanceEngine {

  /// <summary>Test cases that compares BalanzaColumnasPorMonedaOrigen vs core balances tests.</summary>
  public class BalanzaColumnasVsCoreBalancesTests {

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
                                                                           ExchangeRateType.Empty);

      FixedList<BalanzaColumnasMonedaEntryDto> balanzaCol = TestsHelpers.GetBalanzaColumnas(DateTime.Parse(fromDate),
                                                                                         DateTime.Parse(toDate))
                                                                    .FindAll(x => x.ItemType == TrialBalanceItemType.Entry);
      RunTest(coreBalances, balanzaCol);

      Assert.True(balanzaCol.Count > 500);
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

      FixedList<BalanzaColumnasMonedaEntryDto> balanzaCol = TestsHelpers.GetBalanzaColumnas(DateTime.Parse(fromDate),
                                                                                         DateTime.Parse(toDate))
                                                                    .FindAll(x => x.ItemType == TrialBalanceItemType.Summary);
      RunTest(coreBalances, balanzaCol);

      Assert.True(balanzaCol.Count > 100);
    }


    private void RunTest(CoreBalanceEntries coreBalances, FixedList<BalanzaColumnasMonedaEntryDto> balanzaCol) {

      foreach (var sut in balanzaCol) {

        var filtered = coreBalances.GetBalancesByAccount(sut.AccountNumber);

        var totalMXN = filtered.FindAll(x => x.Currency.Equals(Currency.MXN)).Sum(x => x.CurrentBalance);
        var totalUSD = filtered.FindAll(x => x.Currency.Equals(Currency.USD)).Sum(x => x.CurrentBalance);
        var totalYEN = filtered.FindAll(x => x.Currency.Equals(Currency.YEN)).Sum(x => x.CurrentBalance);
        var totalEUR = filtered.FindAll(x => x.Currency.Equals(Currency.EUR)).Sum(x => x.CurrentBalance);
        var totalUDI = filtered.FindAll(x => x.Currency.Equals(Currency.UDI)).Sum(x => x.CurrentBalance);

        Assert.True(Math.Abs(totalMXN - sut.DomesticBalance) <= 1,
                    TestsHelpers.BalanceDiffMsg("Mon Nacional", $"{sut.AccountNumber}," +
                                                $"sector {sut.SectorCode}", totalMXN, sut.DomesticBalance));

        Assert.True(Math.Abs(totalUSD - sut.DollarBalance) <= 1,
                    TestsHelpers.BalanceDiffMsg("Mon USD", $"{sut.AccountNumber}, sector {sut.SectorCode}",
                                                totalUSD, sut.DollarBalance));

        Assert.True(Math.Abs(totalYEN - sut.YenBalance) <= 1,
                    TestsHelpers.BalanceDiffMsg("Mon YEN", $"{sut.AccountNumber}, sector {sut.SectorCode}",
                                                totalYEN, sut.YenBalance));

        Assert.True(Math.Abs(totalEUR - sut.EuroBalance) <= 1,
                    TestsHelpers.BalanceDiffMsg("Mon EUR", $"{sut.AccountNumber}, sector {sut.SectorCode}",
                                                totalEUR, sut.EuroBalance));

        Assert.True(Math.Abs(totalUDI - sut.UdisBalance) <= 1,
                    TestsHelpers.BalanceDiffMsg("Mon UDI", $"{sut.AccountNumber}, sector {sut.SectorCode}",
                                                totalUDI, sut.UdisBalance));
      }
    }

  } // class BalanzaColumnasVsCoreBalancesTests

} // namespace Empiria.Tests.FinancialAccounting.BalanceEngine
