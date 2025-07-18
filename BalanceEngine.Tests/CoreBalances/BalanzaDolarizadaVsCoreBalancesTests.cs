﻿/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test cases                              *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Use cases tests                         *
*  Type     : BalanzaDolarizadaVsCoreBalancesTests       License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Test cases that compares BalanzaDolarizada vs core balances tests.                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System;
using System.Linq;
using Empiria.FinancialAccounting;
using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Xunit;

namespace Empiria.Tests.FinancialAccounting.BalanceEngine {

  /// <summary>Test cases that compares BalanzaDolarizada vs core balances tests.</summary>
  public class BalanzaDolarizadaVsCoreBalancesTests {

    [Theory]
    [InlineData("2024-04-01", "2024-04-30", BalancesType.AllAccounts)]
    [InlineData("2024-05-01", "2024-05-31", BalancesType.AllAccounts)]
    [InlineData("2024-06-01", "2024-06-30", BalancesType.AllAccounts)]
    [InlineData("2024-07-01", "2024-07-31", BalancesType.AllAccounts)]
    [InlineData("2024-08-01", "2024-08-31", BalancesType.AllAccounts)]
    [InlineData("2024-09-01", "2024-09-30", BalancesType.AllAccounts)]
    [InlineData("2024-10-01", "2024-10-31", BalancesType.AllAccounts)]
    [InlineData("2024-11-01", "2024-11-30", BalancesType.AllAccounts)]
    [InlineData("2024-12-01", "2024-12-31", BalancesType.AllAccounts)]
    [InlineData("2025-01-01", "2025-01-31", BalancesType.AllAccounts)]
    [InlineData("2025-02-01", "2025-02-28", BalancesType.AllAccounts)]
    [InlineData("2025-03-01", "2025-03-31", BalancesType.AllAccounts)]
    [InlineData("2025-04-01", "2025-04-30", BalancesType.AllAccounts)]
    public void Should_Have_Same_Entries(string fromDate, string toDate, BalancesType balancesType) {

      CoreBalanceEntries coreBalances = TestsHelpers.GetCoreBalanceEntries(DateTime.Parse(fromDate),
                                                                           DateTime.Parse(toDate),
                                                                           ExchangeRateType.Empty);

      FixedList<BalanzaDolarizadaEntryDto> balanzaDol = TestsHelpers.GetBalanzaDolarizada(
                                                        DateTime.Parse(fromDate), DateTime.Parse(toDate),
                                                        balancesType)
                                                        .FindAll(x=>x.ItemType == TrialBalanceItemType.Summary ||
                                                                 x.ItemType == TrialBalanceItemType.Entry);

      RunTest(coreBalances, balanzaDol);

      Assert.True(balanzaDol.Count > 200);
    }


    private void RunTest(CoreBalanceEntries coreBalances, FixedList<BalanzaDolarizadaEntryDto> balanzaDol) {

      foreach (var sut in balanzaDol) {

        var filtered = coreBalances.GetBalancesByAccount(sut.AccountNumber);

        switch (sut.CurrencyCode) {
          case "02":
            var totalUSD = filtered.FindAll(x => x.Currency.Equals(Currency.USD)).Sum(x => x.CurrentBalance);

            Assert.True(Math.Abs(totalUSD - (decimal) sut.TotalBalance) <= 1,
                    TestsHelpers.BalanceDiffMsg("Mon USD", $"{sut.AccountNumber}, sector {sut.SectorCode}",
                                                totalUSD, (decimal) sut.TotalBalance));
            break;
          case "06":
            var totalYEN = filtered.FindAll(x => x.Currency.Equals(Currency.YEN)).Sum(x => x.CurrentBalance);

            Assert.True(Math.Abs(totalYEN - (decimal) sut.TotalBalance) <= 1,
                    TestsHelpers.BalanceDiffMsg("Mon YEN", $"{sut.AccountNumber}, sector {sut.SectorCode}",
                                                totalYEN, (decimal) sut.TotalBalance));
            break;
          case "27":
            var totalEUR = filtered.FindAll(x => x.Currency.Equals(Currency.EUR)).Sum(x => x.CurrentBalance);

            Assert.True(Math.Abs(totalEUR - (decimal) sut.TotalBalance) <= 1,
                    TestsHelpers.BalanceDiffMsg("Mon EUR", $"{sut.AccountNumber}, sector {sut.SectorCode}",
                                                totalEUR, (decimal) sut.TotalBalance));
            break;
          default:
            break;
        }
      }
    }

  } // class BalanzaDolarizadaVsCoreBalancesTests

} // namespace Empiria.Tests.FinancialAccounting.BalanceEngine
