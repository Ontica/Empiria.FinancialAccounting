/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                                  Component : Test cases                         *
*  Assembly : FinancialAccounting.BalanceEngine.Tests         Pattern   : Use cases tests                    *
*  Type     : ExploradorSaldosPorAuxiliarVsCoreBalancesTests  License   : Please read LICENSE.txt file       *
*                                                                                                            *
*  Summary  : Test cases that compares Explorador de saldos por auxiliar vs core balances tests.             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Linq;
using Empiria.FinancialAccounting;
using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.BalanceExplorer.Adapters;
using Xunit;

namespace Empiria.Tests.FinancialAccounting.BalanceEngine {

  /// <summary>Test cases that compares Explorador de saldos por auxiliar vs core balances tests.</summary>
  public class ExploradorSaldosPorAuxiliarVsCoreBalancesTests {

    [Theory]
    [InlineData("2024-10-01", "2024-10-31", true)]
    [InlineData("2024-11-01", "2024-11-30", true)]
    [InlineData("2024-12-01", "2024-12-31", true)]
    [InlineData("2025-01-01", "2025-01-31", true)]
    [InlineData("2025-02-01", "2025-02-28", true)]
    [InlineData("2025-03-01", "2025-03-31", true)]
    [InlineData("2025-04-01", "2025-04-30", true)]
    [InlineData("2025-05-01", "2025-05-31", true)]
    public void Should_Have_Same_SubledgerAccount_Entries(string fromDate, string toDate,
                                                          bool accountsFilter) {

      CoreBalanceEntries coreBalances = TestsHelpers.GetCoreBalanceEntries(DateTime.Parse(fromDate),
                                                                           DateTime.Parse(toDate),
                                                                           ExchangeRateType.Empty);

      FixedList<BalanceExplorerEntryDto> saldosCuenta = TestsHelpers.GetExploradorSaldosPorAuxiliar(
                                                            DateTime.Parse(fromDate),
                                                            DateTime.Parse(toDate),
                                                            accountsFilter)
                                           .FindAll(x => x.ItemType == TrialBalanceItemType.Entry);

      RunTest(coreBalances, saldosCuenta);
    }


    private void RunTest(CoreBalanceEntries coreBalances,
                                            FixedList<BalanceExplorerEntryDto> saldosCuenta) {

      foreach (var sut in saldosCuenta) {

        var filtered = coreBalances.GetBalancesByAccountNumberAndSubledgerAccountNumberAndSector(
                        sut.AccountNumberForBalances, sut.SubledgerAccountNumber, sut.SectorCode)
                                   .FindAll(x => x.Account.DebtorCreditor.ToString() == sut.DebtorCreditor);

        var totalSubledgerAccount = filtered.FindAll(x => x.Currency.Code == sut.CurrencyCode &&
                                                          x.Ledger.Number == sut.LedgerNumber)
                                            .Sum(x => x.CurrentBalance);

        Assert.True(Math.Abs(totalSubledgerAccount - sut.CurrentBalance.Value) <= 1,
                    TestsHelpers.BalanceDiffMsg($"Saldo actual. Moneda {sut.CurrencyCode}.",
                                                $"Cuenta: {sut.AccountNumberForBalances}, " +
                                                $"Auxiliar: {sut.SubledgerAccountNumber}, " +
                                                $"sector {sut.SectorCode} ({sut.DebtorCreditor})",
                                                totalSubledgerAccount, sut.CurrentBalance.Value));
      }
    }

  } // class ExploradorSaldosPorAuxiliarVsCoreBalancesTests

} // namespace Empiria.Tests.FinancialAccounting.BalanceEngine
