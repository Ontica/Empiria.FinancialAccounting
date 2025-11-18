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
    [InlineData("2024-12-01", "2024-12-31", BalancesType.AllAccounts)]
    [InlineData("2025-06-01", "2025-06-30", BalancesType.AllAccounts)]
    [InlineData("2025-07-01", "2025-07-31", BalancesType.AllAccounts)]
    [InlineData("2025-08-01", "2025-08-31", BalancesType.AllAccounts)]
    [InlineData("2025-09-01", "2025-09-30", BalancesType.AllAccounts)]
    public void Should_Have_Same_Entries_With_Or_Without_Subledger_Accounts(string fromDate,
                                                                            string toDate,
                                                                            BalancesType balancesType) {

      CoreBalanceEntries coreBalances = AnaliticoCuentasTestHelpers.GetCoreBalanceEntries(
                                                                      DateTime.Parse(fromDate),
                                                                      DateTime.Parse(toDate),
                                                                      ExchangeRateType.ValorizacionBanxico);

      FixedList<AnaliticoDeCuentasEntryDto> analitico = AnaliticoCuentasTestHelpers.GetAnaliticoCuentas(
                                                                                DateTime.Parse(fromDate),
                                                                                DateTime.Parse(toDate),
                                                                                balancesType)
                                                     .FindAll(x => x.ItemType == TrialBalanceItemType.Entry);
      RunTest(coreBalances, analitico);

      Assert.True(analitico.Count > 500);
    }


    [Theory]
    [InlineData("2024-12-01", "2024-12-31", BalancesType.AllAccounts)]
    [InlineData("2025-01-01", "2025-01-31", BalancesType.AllAccounts)]
    [InlineData("2025-02-01", "2025-02-28", BalancesType.AllAccounts)]
    [InlineData("2025-03-01", "2025-03-31", BalancesType.AllAccounts)]
    [InlineData("2025-04-01", "2025-04-30", BalancesType.AllAccounts)]
    public void Should_Have_Same_Summaries(string fromDate, string toDate, BalancesType balancesType) {

      CoreBalanceEntries coreBalances = AnaliticoCuentasTestHelpers.GetCoreBalanceEntries(
                                                                      DateTime.Parse(fromDate),
                                                                      DateTime.Parse(toDate),
                                                                      ExchangeRateType.ValorizacionBanxico);

      FixedList<AnaliticoDeCuentasEntryDto> analitico = AnaliticoCuentasTestHelpers.GetAnaliticoCuentas(
                                                                                DateTime.Parse(fromDate),
                                                                                DateTime.Parse(toDate),
                                                                                balancesType)
                                                   .FindAll(x => x.ItemType == TrialBalanceItemType.Summary);
      RunTest(coreBalances, analitico);

      Assert.True(analitico.Count > 100);
    }


    [Theory]
    [InlineData("2024-12-01", "2024-12-31", BalancesType.AllAccounts)]
    [InlineData("2025-01-01", "2025-01-31", BalancesType.AllAccounts)]
    [InlineData("2025-02-01", "2025-02-28", BalancesType.AllAccounts)]
    [InlineData("2025-03-01", "2025-03-31", BalancesType.AllAccounts)]
    [InlineData("2025-04-01", "2025-04-30", BalancesType.AllAccounts)]
    public void Should_Have_Same_Summaries_By_Subledger_Accounts(string fromDate, string toDate,
                                                                 BalancesType balancesType) {

      CoreBalanceEntries coreBalances = AnaliticoCuentasTestHelpers.GetCoreBalanceEntriesWithSubledgerAccounts(
                                                                      DateTime.Parse(fromDate),
                                                                      DateTime.Parse(toDate),
                                                                      ExchangeRateType.ValorizacionBanxico);

      FixedList<AnaliticoDeCuentasEntryDto> analitico =
                                      AnaliticoCuentasTestHelpers.GetAnaliticoCuentasWithSubledgerAccounts(
                                                                      DateTime.Parse(fromDate),
                                                                      DateTime.Parse(toDate),
                                                                      balancesType)
                                                    .FindAll(x => x.ItemType == TrialBalanceItemType.Summary);
      RunTest(coreBalances, analitico);

      Assert.True(analitico.Count > 100);
    }


    private void RunTest(CoreBalanceEntries coreBalances, FixedList<AnaliticoDeCuentasEntryDto> analitico) {

      foreach (var sut in analitico) {

        FixedList<CoreBalanceEntry> filtered = new FixedList<CoreBalanceEntry>();

        if (sut.SubledgerAccountId > 0) {
          
          filtered = coreBalances.GetBalancesByAccountIdAndSubledgerAccountIdAndSector(sut.StandardAccountId,
                                                                                       sut.SubledgerAccountId,
                                                                                       sut.SectorCode)
                                 .FindAll(x => x.Account.DebtorCreditor == sut.DebtorCreditor);
        } else {

          filtered = coreBalances.GetBalancesByAccountNumberAndSector(sut.AccountNumber, sut.SectorCode)
                                 .FindAll(x => x.Account.DebtorCreditor == sut.DebtorCreditor);
        }

        var totalMN = filtered.FindAll(x => x.Currency.Equals(Currency.MXN) ||
                                            x.Currency.Equals(Currency.UDI))
                              .Sum(x => x.CurrentBalance);

        var totalME = filtered.FindAll(x => x.Currency.Distinct(Currency.MXN) &&
                                            x.Currency.Distinct(Currency.UDI))
                              .Sum(x => x.CurrentBalance);

        Assert.True(Math.Abs(totalMN - sut.DomesticBalance) <= 1,
                    TestsHelpers.BalanceDiffMsg("Mon Nacional", $"{sut.AccountNumber}, " +
                                                $"Deleg:{sut.LedgerNumber}, sector {sut.SectorCode}",
                                                totalMN, sut.DomesticBalance));

        Assert.True(Math.Abs(totalME - sut.ForeignBalance) <= 1,
                    TestsHelpers.BalanceDiffMsg("Mon Extranjera", $"{sut.AccountNumber}, " +
                                                $"Deleg: {sut.LedgerNumber}, sector {sut.SectorCode}",
                                                totalME, sut.ForeignBalance));

        Assert.True(Math.Abs(totalMN + totalME - sut.TotalBalance) <= 1,
                    TestsHelpers.BalanceDiffMsg("Total", $"{sut.AccountNumber}, sector {sut.SectorCode}",
                                                totalMN + totalME, sut.TotalBalance));
      }
    }

  } // class AnaliticoCuentasVsCoreBalancesTests

}  // namespace Empiria.Tests.FinancialAccounting.BalanceEngine
