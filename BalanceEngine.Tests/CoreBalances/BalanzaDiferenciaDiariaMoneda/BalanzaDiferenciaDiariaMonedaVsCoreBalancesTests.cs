/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                                    Component : Test cases                       *
*  Assembly : FinancialAccounting.BalanceEngine.Tests           Pattern   : Use cases tests                  *
*  Type     : BalanzaDiferenciaDiariaMonedaVsCoreBalancesTests  License   : Please read LICENSE.txt file     *
*                                                                                                            *
*  Summary  : Test cases that compares BalanzaDiferenciaDiariaMoneda vs core balances tests.                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System;
using System.Linq;
using Empiria.FinancialAccounting;
using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Xunit;

namespace Empiria.Tests.FinancialAccounting.BalanceEngine {

  /// <summary>Test cases that compares BalanzaDiferenciaDiariaMoneda vs core balances tests.</summary>
  public class BalanzaDiferenciaDiariaMonedaVsCoreBalancesTests {

    [Theory]
    [InlineData("2024-09-01", "2024-09-30", BalancesType.WithCurrentBalanceOrMovements)]
    public void Should_Have_Same_Entries_In_Last_Date(string fromDate, string toDate,
                                                      BalancesType balancesType) {

      CoreBalanceEntries coreBalances = BalanzaDiferenciaDiariaMonedaTestHelpers.GetCoreBalanceEntries(
                                                                                  DateTime.Parse(fromDate),
                                                                                  DateTime.Parse(toDate),
                                                                                  ExchangeRateType.Empty);

      FixedList<BalanzaDiferenciaDiariaMonedaEntryDto> balanzaDiferencia =
        BalanzaDiferenciaDiariaMonedaTestHelpers.GetBalanzaDiferenciaDiaria(DateTime.Parse(fromDate),
                                                                            DateTime.Parse(toDate),
                                                                            balancesType)
                                                .FindAll(x => x.ItemType == TrialBalanceItemType.Entry);

      RunTest(coreBalances, balanzaDiferencia);

      Assert.True(balanzaDiferencia.Count > 500);
    }


    [Theory]
    [InlineData("2024-09-01", "2024-09-30", BalancesType.WithCurrentBalanceOrMovements)]
    public void Should_Have_Same_Entries_By_Working_Date(string fromDate, string toDate,
                                                         BalancesType balancesType) {

      CoreBalanceEntries coreBalances = BalanzaDiferenciaDiariaMonedaTestHelpers.GetCoreBalanceEntriesByDateRange(
                                                                                  DateTime.Parse(fromDate),
                                                                                  DateTime.Parse(toDate),
                                                                                  ExchangeRateType.Empty);

      FixedList<BalanzaDiferenciaDiariaMonedaEntryDto> balanzaDiferencia =
        BalanzaDiferenciaDiariaMonedaTestHelpers.GetBalanzaDiferenciaDiaria(DateTime.Parse(fromDate),
                                                                            DateTime.Parse(toDate),
                                                                            balancesType)
                                                .FindAll(x => x.ItemType == TrialBalanceItemType.Entry);

      RunTest(coreBalances, balanzaDiferencia);

      Assert.True(balanzaDiferencia.Count > 500);
    }


    private void RunTest(CoreBalanceEntries coreBalances, 
                         FixedList<BalanzaDiferenciaDiariaMonedaEntryDto> balanza) {

      foreach (var sut in balanza) {

        var filtered = coreBalances.GetBalancesByAccountNumberAndSector(sut.AccountNumber, sut.SectorCode)
                                   .FindAll(x => x.Account.DebtorCreditor == sut.DebtorCreditor);

        var mxnBalance = filtered.FindAll(x=>x.Currency.Code == Currency.MXN.Code).Sum(x => x.CurrentBalance);
        var usdBalance = filtered.FindAll(x => x.Currency.Code == Currency.USD.Code).Sum(x => x.CurrentBalance);
        var yenBalance = filtered.FindAll(x => x.Currency.Code == Currency.YEN.Code).Sum(x => x.CurrentBalance);
        var euroBalance = filtered.FindAll(x => x.Currency.Code == Currency.EUR.Code).Sum(x => x.CurrentBalance);
        var udiBalance = filtered.FindAll(x => x.Currency.Code == Currency.UDI.Code).Sum(x => x.CurrentBalance);

        Assert.True(Math.Abs(mxnBalance - sut.DomesticBalance) <= 1, TestsHelpers.BalanceDiffMsg(
                              $"Saldo actual. MXN,", $"{sut.AccountNumber}, sector {sut.SectorCode} " +
                              $"({sut.DebtorCreditor})", mxnBalance, sut.DomesticBalance));

        Assert.True(Math.Abs(usdBalance - sut.DollarBalance) <= 1, TestsHelpers.BalanceDiffMsg(
                              $"Saldo actual. USD,", $"{sut.AccountNumber}, sector {sut.SectorCode} " +
                              $"({sut.DebtorCreditor})", usdBalance, sut.DollarBalance));

        Assert.True(Math.Abs(yenBalance - sut.YenBalance) <= 1, TestsHelpers.BalanceDiffMsg(
                              $"Saldo actual. YEN,", $"{sut.AccountNumber}, sector {sut.SectorCode} " +
                              $"({sut.DebtorCreditor})", yenBalance, sut.YenBalance));

        Assert.True(Math.Abs(euroBalance - sut.EuroBalance) <= 1, TestsHelpers.BalanceDiffMsg(
                              $"Saldo actual. EUR,", $"{sut.AccountNumber}, sector {sut.SectorCode} " +
                              $"({sut.DebtorCreditor})", euroBalance, sut.EuroBalance));

        Assert.True(Math.Abs(udiBalance - sut.UdisBalance) <= 1, TestsHelpers.BalanceDiffMsg(
                              $"Saldo actual. UDI,", $"{sut.AccountNumber}, sector {sut.SectorCode} " +
                              $"({sut.DebtorCreditor})", udiBalance, sut.UdisBalance));
      }
    }

  } // class BalanzaDiferenciaDiariaMonedaVsCoreBalancesTests

} // namespace Empiria.Tests.FinancialAccounting.BalanceEngine
