/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test cases                              *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Use cases tests                         *
*  Type     : BalanzaConsolidadaVsCoreBalancesTests      License   : Please read LICENSE.txt file            *
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
  public class BalanzaConsolidadaVsCoreBalancesTests {

    [Theory]
    [InlineData("2024-10-01", "2024-10-31", BalancesType.WithCurrentBalanceOrMovements)]
    [InlineData("2024-11-01", "2024-11-30", BalancesType.AllAccounts)]
    [InlineData("2024-12-01", "2024-12-31", BalancesType.WithCurrentBalanceOrMovements)]
    [InlineData("2025-01-01", "2025-01-31", BalancesType.AllAccounts)]
    [InlineData("2025-02-01", "2025-02-28", BalancesType.WithCurrentBalanceOrMovements)]
    [InlineData("2025-03-01", "2025-03-31", BalancesType.AllAccounts)]
    [InlineData("2025-04-01", "2025-04-30", BalancesType.WithCurrentBalanceOrMovements)]
    public void Should_Have_Same_Entries(string fromDate, string toDate, BalancesType balancesType) {

      CoreBalanceEntries coreBalances = TestsHelpers.GetCoreBalanceEntries(DateTime.Parse(fromDate),
                                                                           DateTime.Parse(toDate),
                                                                           ExchangeRateType.ValorizacionBanxico);

      FixedList<BalanzaTradicionalEntryDto> balanzaConsolidada =
                                BalanzaTradicionalTestHelpers.GetBalanzaConsolidada(DateTime.Parse(fromDate),
                                                                                      DateTime.Parse(toDate),
                                                                                      balancesType)
                                                     .FindAll(x => x.ItemType == TrialBalanceItemType.Entry);

      RunTest(coreBalances, balanzaConsolidada);

      Assert.True(balanzaConsolidada.Count > 500);
    }


    [Theory]
    [InlineData("2024-10-01", "2024-10-31", BalancesType.WithCurrentBalanceOrMovements)]
    [InlineData("2024-11-01", "2024-11-30", BalancesType.AllAccounts)]
    [InlineData("2024-12-01", "2024-12-31", BalancesType.WithCurrentBalanceOrMovements)]
    [InlineData("2025-01-01", "2025-01-31", BalancesType.AllAccounts)]
    [InlineData("2025-02-01", "2025-02-28", BalancesType.WithCurrentBalanceOrMovements)]
    [InlineData("2025-03-01", "2025-03-31", BalancesType.AllAccounts)]
    [InlineData("2025-04-01", "2025-04-30", BalancesType.WithCurrentBalanceOrMovements)]
    public void Should_Have_Same_Summaries(string fromDate, string toDate, BalancesType balancesType) {

      CoreBalanceEntries coreBalances = TestsHelpers.GetCoreBalanceEntries(DateTime.Parse(fromDate),
                                                                           DateTime.Parse(toDate),
                                                                           ExchangeRateType.ValorizacionBanxico);

      FixedList<BalanzaTradicionalEntryDto> balanzaConsolidada =
                                BalanzaTradicionalTestHelpers.GetBalanzaConsolidada(DateTime.Parse(fromDate),
                                                                                      DateTime.Parse(toDate),
                                                                                      balancesType)
                                                   .FindAll(x => x.ItemType == TrialBalanceItemType.Summary);

      RunTest(coreBalances, balanzaConsolidada);

      Assert.True(balanzaConsolidada.Count > 100);
    }


    private void RunTest(CoreBalanceEntries coreBalances, FixedList<BalanzaTradicionalEntryDto> balanza) {

      foreach (var sut in balanza) {

        var filtered = coreBalances.GetBalancesByAccountNumberAndSector(sut.AccountNumber, sut.SectorCode)
                                   .FindAll(x => x.Account.DebtorCreditor == sut.DebtorCreditor);

        var totalInitialBalance = filtered.Sum(x => x.InitialBalance);
        var totalDebit = filtered.Sum(x => x.Debit);
        var totalCredit = filtered.Sum(x => x.Credit);
        var totalCurrentBalance = filtered.Sum(x => x.CurrentBalance);

        Assert.True(Math.Abs(totalInitialBalance - sut.InitialBalance) <= 1,
                    TestsHelpers.BalanceDiffMsg("Saldo inicial", $"{sut.AccountNumber}, sector {sut.SectorCode} ({sut.DebtorCreditor})",
                                                totalInitialBalance, sut.InitialBalance));

        Assert.True(Math.Abs(totalDebit - sut.Debit) <= 1,
                    TestsHelpers.BalanceDiffMsg("Cargos", $"{sut.AccountNumber}, sector {sut.SectorCode} ({sut.DebtorCreditor})",
                                                totalDebit, sut.Debit));
        Assert.True(Math.Abs(totalCredit - sut.Credit) <= 1,
                    TestsHelpers.BalanceDiffMsg("Abonos", $"{sut.AccountNumber}, sector {sut.SectorCode} ({sut.DebtorCreditor})",
                                                totalCredit, sut.Credit));

        Assert.True(Math.Abs(totalCurrentBalance - sut.CurrentBalance.Value) <= 1,
                    TestsHelpers.BalanceDiffMsg("Saldo actual", $"{sut.AccountNumber}, sector {sut.SectorCode} ({sut.DebtorCreditor})",
                                                totalCurrentBalance, sut.CurrentBalance.Value));
      }
    }

  } // class BalanzaConsolidadaVsCoreBalancesTests

}  // namespace Empiria.Tests.FinancialAccounting.BalanceEngine
