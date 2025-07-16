/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test cases                              *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Use cases tests                         *
*  Type     : SaldosPorCuentaVsCoreBalancesTests         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Test cases that compares saldosPorCuenta vs core balances tests.                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System;
using System.Linq;
using Empiria.FinancialAccounting;
using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Xunit;

namespace Empiria.Tests.FinancialAccounting.BalanceEngine {

  /// <summary>Test cases that compares saldosPorCuenta vs core balances tests.</summary>
  public class SaldosPorCuentaVsCoreBalancesTests {

    [Theory]
    //[InlineData("2024-10-01", "2024-10-31", BalancesType.AllAccounts)]
    //[InlineData("2024-11-01", "2024-11-30", BalancesType.AllAccounts)]
    //[InlineData("2024-12-01", "2024-12-31", BalancesType.AllAccounts)]
    //[InlineData("2025-01-01", "2025-01-31", BalancesType.AllAccounts)]
    //[InlineData("2025-02-01", "2025-02-28", BalancesType.AllAccounts)]
    //[InlineData("2025-03-01", "2025-03-31", BalancesType.AllAccounts)]
    //[InlineData("2025-04-01", "2025-04-30", BalancesType.AllAccounts)]
    [InlineData("2025-05-01", "2025-05-31", BalancesType.AllAccounts)]
    public void Should_Have_Same_Entries(string fromDate, string toDate, BalancesType balancesType) {

      CoreBalanceEntries coreBalances = TestsHelpers.GetCoreBalanceEntries(DateTime.Parse(fromDate),
                                                                           DateTime.Parse(toDate),
                                                                           ExchangeRateType.Empty);

      FixedList<SaldosPorCuentaEntryDto> saldosCuenta = TestsHelpers.GetSaldosPorCuenta(
                                                            DateTime.Parse(fromDate),
                                                            DateTime.Parse(toDate),
                                                            balancesType)
                                           .FindAll(x => x.ItemType == TrialBalanceItemType.Entry);

      RunTest(coreBalances, saldosCuenta);

      Assert.True(saldosCuenta.Count > 500);
    }


    private void RunTest(CoreBalanceEntries coreBalances, FixedList<SaldosPorCuentaEntryDto> saldosCuenta) {

      foreach (var sut in saldosCuenta) {

        var filtered = coreBalances.GetBalancesByAccountNumberAndSector(sut.AccountNumber, sut.SectorCode)
                                   .FindAll(x => x.Account.DebtorCreditor.ToString() == sut.DebtorCreditor);

        var totalCurrentBalance = filtered.FindAll(x => x.Currency.Code == sut.CurrencyCode)
                                          .Sum(x => x.CurrentBalance);

        Assert.True(Math.Abs(totalCurrentBalance - sut.CurrentBalance.Value) <= 1,
                    TestsHelpers.BalanceDiffMsg($"Saldo actual. Moneda {sut.CurrencyCode}.",
                                                $"Cuenta: {sut.AccountNumber}, " +
                                                $"sector {sut.SectorCode} ({sut.DebtorCreditor})",
                                                totalCurrentBalance, sut.CurrentBalance.Value));
      }
    }


    [Theory]
    [InlineData("2024-10-01", "2024-10-31", BalancesType.AllAccounts)]
    [InlineData("2024-11-01", "2024-11-30", BalancesType.AllAccounts)]
    [InlineData("2024-12-01", "2024-12-31", BalancesType.AllAccounts)]
    [InlineData("2025-01-01", "2025-01-31", BalancesType.AllAccounts)]
    [InlineData("2025-02-01", "2025-02-28", BalancesType.AllAccounts)]
    [InlineData("2025-03-01", "2025-03-31", BalancesType.AllAccounts)]
    [InlineData("2025-04-01", "2025-04-30", BalancesType.AllAccounts)]
    [InlineData("2025-05-01", "2025-05-31", BalancesType.AllAccounts)]
    public void Should_Have_Same_SubledgerAccount_Entries(string fromDate, string toDate,
                                                          BalancesType balancesType) {

      CoreBalanceEntries coreBalances = TestsHelpers.GetCoreBalanceEntriesWithSubledgerAccounts(
                                                      DateTime.Parse(fromDate),
                                                      DateTime.Parse(toDate),
                                                      ExchangeRateType.Empty);

      FixedList<SaldosPorCuentaEntryDto> saldosCuenta = TestsHelpers.GetSaldosPorCuentaConAuxiliares(
                                                            DateTime.Parse(fromDate),
                                                            DateTime.Parse(toDate),
                                                            balancesType)
                                           .FindAll(x => x.ItemType == TrialBalanceItemType.Entry);

      RunTestForSubledgerAccount(coreBalances, saldosCuenta);

      Assert.True(saldosCuenta.Count > 500);
    }


    private void RunTestForSubledgerAccount(CoreBalanceEntries coreBalances,
                                            FixedList<SaldosPorCuentaEntryDto> saldosCuenta) {
      
      var _saldosCuenta = saldosCuenta.FindAll(x => !x.IsParentPostingEntry);

      foreach (var sut in _saldosCuenta) {

        var filtered = coreBalances.GetBalancesByAccountNumberAndSubledgerAccountNumberAndSector(sut.AccountNumberForBalances,
                                                                              sut.SubledgerAccountNumber,
                                                                              sut.SectorCode)
                                   .FindAll(x => x.Currency.Code == sut.CurrencyCode);

        var totalSubledgerAccount = filtered.FindAll(x => x.SubledgerAccount.Id == sut.SubledgerAccountId &&
                                                          x.LastChangeDate == sut.LastChangeDate)
                                            .Sum(x => x.CurrentBalance);

        if (sut.SubledgerAccountId == 0) {

          totalSubledgerAccount = filtered.Sum(x => x.CurrentBalance);
        }

        Assert.True(Math.Abs(totalSubledgerAccount - sut.CurrentBalance.Value) <= 1,
                    TestsHelpers.BalanceDiffMsg($"Saldo actual. Moneda {sut.CurrencyCode}.",
                                                $"Cuenta: {sut.AccountNumberForBalances}, " +
                                                $"Auxiliar: {sut.SubledgerAccountNumber}, " +
                                                $"sector {sut.SectorCode} ({sut.DebtorCreditor})",
                                                totalSubledgerAccount, sut.CurrentBalance.Value));
      }
    }

  } // class SaldosPorCuentaVsCoreBalancesTests

} // namespace Empiria.Tests.FinancialAccounting.BalanceEngine
