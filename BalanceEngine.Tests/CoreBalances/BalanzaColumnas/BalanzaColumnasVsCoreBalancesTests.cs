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
    [InlineData("2024-12-01", "2024-12-31", BalancesType.WithCurrentBalanceOrMovements)]
    [InlineData("2025-01-01", "2025-01-31", BalancesType.WithCurrentBalanceOrMovements)]
    [InlineData("2025-02-01", "2025-02-28", BalancesType.WithCurrentBalanceOrMovements)]
    [InlineData("2025-07-01", "2025-07-31", BalancesType.WithCurrentBalanceOrMovements)]
    [InlineData("2025-08-01", "2025-08-31", BalancesType.WithCurrentBalanceOrMovements)]
    [InlineData("2025-09-01", "2025-09-30", BalancesType.WithCurrentBalanceOrMovements)]
    public void Should_Have_Same_Entries(string fromDate, string toDate, BalancesType balancesType) {

      CoreBalanceEntries coreBalances = BalanzaColumnasTestHelpers.GetCoreBalanceEntries(
                                                                           DateTime.Parse(fromDate),
                                                                           DateTime.Parse(toDate),
                                                                           ExchangeRateType.Empty);

      FixedList<BalanzaColumnasMonedaEntryDto> balanzaCol = BalanzaColumnasTestHelpers.GetBalanzaColumnas(
                                                                                    DateTime.Parse(fromDate),
                                                                                    DateTime.Parse(toDate),
                                                                                    balancesType)
                                                    .FindAll(x => x.ItemType == TrialBalanceItemType.Entry);
      RunTest(coreBalances, balanzaCol);

      Assert.True(balanzaCol.Count > 500);
    }


    [Theory]
    [InlineData("2025-02-01", "2025-02-28", BalancesType.WithCurrentBalanceOrMovements)]
    [InlineData("2025-09-01", "2025-09-30", BalancesType.WithCurrentBalanceOrMovements)]
    public void Should_Have_Same_Summaries(string fromDate, string toDate, BalancesType balancesType) {

      CoreBalanceEntries coreBalances = BalanzaColumnasTestHelpers.GetCoreBalanceEntries(
                                                                           DateTime.Parse(fromDate),
                                                                           DateTime.Parse(toDate),
                                                                           ExchangeRateType.Empty);

      FixedList<BalanzaColumnasMonedaEntryDto> balanzaCol = BalanzaColumnasTestHelpers.GetBalanzaColumnas(
                                                                                    DateTime.Parse(fromDate),
                                                                                    DateTime.Parse(toDate),
                                                                                    balancesType)
                                                   .FindAll(x => x.ItemType == TrialBalanceItemType.Summary);
      RunTest(coreBalances, balanzaCol);

      Assert.True(balanzaCol.Count > 100);
    }


    private void RunTest(CoreBalanceEntries coreBalances, FixedList<BalanzaColumnasMonedaEntryDto> balanzaCol) {

      foreach (var sut in balanzaCol) {

        var filtered = coreBalances.GetBalancesByAccount(sut.AccountNumber);

        var totalDebtorMXN = filtered.FindAll(x => x.Currency.Equals(Currency.MXN) &&
                                              x.Account.DebtorCreditor == DebtorCreditorType.Deudora)
                                     .Sum(x => x.CurrentBalance);

        var totalDebtorUSD = filtered.FindAll(x => x.Currency.Equals(Currency.USD) &&
                                              x.Account.DebtorCreditor == DebtorCreditorType.Deudora)
                                     .Sum(x => x.CurrentBalance);

        var totalDebtorYEN = filtered.FindAll(x => x.Currency.Equals(Currency.YEN) &&
                                              x.Account.DebtorCreditor == DebtorCreditorType.Deudora)
                                     .Sum(x => x.CurrentBalance);

        var totalDebtorEUR = filtered.FindAll(x => x.Currency.Equals(Currency.EUR) &&
                                              x.Account.DebtorCreditor == DebtorCreditorType.Deudora)
                                     .Sum(x => x.CurrentBalance);

        var totalDebtorUDI = filtered.FindAll(x => x.Currency.Equals(Currency.UDI) &&
                                              x.Account.DebtorCreditor == DebtorCreditorType.Deudora)
                                     .Sum(x => x.CurrentBalance);

        var totalCreditorMXN = filtered.FindAll(x => x.Currency.Equals(Currency.MXN) &&
                                              x.Account.DebtorCreditor == DebtorCreditorType.Acreedora)
                                     .Sum(x => x.CurrentBalance);

        var totalCreditorUSD = filtered.FindAll(x => x.Currency.Equals(Currency.USD) &&
                                              x.Account.DebtorCreditor == DebtorCreditorType.Acreedora)
                                     .Sum(x => x.CurrentBalance);

        var totalCreditorYEN = filtered.FindAll(x => x.Currency.Equals(Currency.YEN) &&
                                              x.Account.DebtorCreditor == DebtorCreditorType.Acreedora)
                                     .Sum(x => x.CurrentBalance);

        var totalCreditorEUR = filtered.FindAll(x => x.Currency.Equals(Currency.EUR) &&
                                              x.Account.DebtorCreditor == DebtorCreditorType.Acreedora)
                                     .Sum(x => x.CurrentBalance);

        var totalCreditorUDI = filtered.FindAll(x => x.Currency.Equals(Currency.UDI) &&
                                              x.Account.DebtorCreditor == DebtorCreditorType.Acreedora)
                                     .Sum(x => x.CurrentBalance);

        var totalMXN = Math.Abs(totalDebtorMXN - totalCreditorMXN);
        var totalUSD = Math.Abs(totalDebtorUSD - totalCreditorUSD);
        var totalYEN = Math.Abs(totalDebtorYEN - totalCreditorYEN);
        var totalEUR = Math.Abs(totalDebtorEUR - totalCreditorEUR);
        var totalUDI = Math.Abs(totalDebtorUDI - totalCreditorUDI);

        Assert.True(Math.Abs(totalMXN - Math.Abs(sut.DomesticBalance)) <= 1,
                    TestsHelpers.BalanceDiffMsg("Mon Nacional", $"{sut.AccountNumber}," +
                                                $"sector {sut.SectorCode}", totalMXN, sut.DomesticBalance));

        Assert.True(Math.Abs(totalUSD - Math.Abs(sut.DollarBalance)) <= 1,
                    TestsHelpers.BalanceDiffMsg("Mon USD", $"{sut.AccountNumber}, sector {sut.SectorCode}",
                                                totalUSD, sut.DollarBalance));

        Assert.True(Math.Abs(totalYEN - Math.Abs(sut.YenBalance)) <= 1,
                    TestsHelpers.BalanceDiffMsg("Mon YEN", $"{sut.AccountNumber}, sector {sut.SectorCode}",
                                                totalYEN, sut.YenBalance));

        Assert.True(Math.Abs(totalEUR - Math.Abs(sut.EuroBalance)) <= 1,
                    TestsHelpers.BalanceDiffMsg("Mon EUR", $"{sut.AccountNumber}, sector {sut.SectorCode}",
                                                totalEUR, sut.EuroBalance));

        Assert.True(Math.Abs(totalUDI - Math.Abs(sut.UdisBalance)) <= 1,
                    TestsHelpers.BalanceDiffMsg("Mon UDI", $"{sut.AccountNumber}, sector {sut.SectorCode}",
                                                totalUDI, sut.UdisBalance));
      }
    }

  } // class BalanzaColumnasVsCoreBalancesTests

} // namespace Empiria.Tests.FinancialAccounting.BalanceEngine
