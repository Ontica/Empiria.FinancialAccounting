/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test cases                              *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Use cases tests                         *
*  Type     : BalanzaContCascadaVsCoreBalancesTests           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Test cases that compares balanzaContabilidadesCascada vs core balances tests.                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System;
using System.Linq;
using Empiria.FinancialAccounting;
using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Xunit;

namespace Empiria.Tests.FinancialAccounting.BalanceEngine {

  /// <summary>Test cases that compares balanzaContabilidadesCascada vs core balances tests.</summary>
  public class BalanzaContCascadaVsCoreBalancesTests {

    [Theory]
    [InlineData("2025-08-01", "2025-08-31", BalancesType.WithCurrentBalanceOrMovements)]
    [InlineData("2025-09-01", "2025-09-30", BalancesType.WithCurrentBalanceOrMovements)]
    public void Should_Have_Same_Entries(string fromDate, string toDate, BalancesType balancesType) {

      CoreBalanceEntries coreBalances = TestsHelpers.GetCoreBalanceEntriesInCascade(
                                                                            DateTime.Parse(fromDate),
                                                                            DateTime.Parse(toDate),
                                                                            ExchangeRateType.Empty);

      FixedList<BalanzaContabilidadesCascadaEntryDto> balanzaCascada = TestsHelpers.GetBalanzaContabilidadesCascada(
                                                            DateTime.Parse(fromDate),
                                                            DateTime.Parse(toDate),
                                                            balancesType)
                                                        .FindAll(x => x.ItemType == TrialBalanceItemType.Entry);
      RunTest(coreBalances, balanzaCascada);

      Assert.True(balanzaCascada.Count > 500);
    }


    private void RunTest(CoreBalanceEntries coreBalances, FixedList<BalanzaContabilidadesCascadaEntryDto> balanzaCascada) {

      foreach (var sut in balanzaCascada.FindAll(x => !x.IsParentPostingEntry)) {
        
        var filtered = coreBalances.GetBalancesByAccountNumberAndSector(sut.AccountNumber, sut.SectorCode)
                                   .FindAll(x => x.Account.DebtorCreditor.ToString() == sut.DebtorCreditor);

        var totalInitialBalance = filtered.FindAll(x => x.Currency.Code == sut.CurrencyCode &&
                                                   x.Ledger.Number == sut.LedgerNumber).Sum(x => x.InitialBalance);

        var totalDebit = filtered.FindAll(x => x.Currency.Code == sut.CurrencyCode &&
                                                   x.Ledger.Number == sut.LedgerNumber).Sum(x => x.Debit);

        var totalCredit = filtered.FindAll(x => x.Currency.Code == sut.CurrencyCode &&
                                                x.Ledger.Number == sut.LedgerNumber).Sum(x => x.Credit);

        var totalCurrentBalance = filtered.FindAll(x => x.Currency.Code == sut.CurrencyCode &&
                                                        x.Ledger.Number == sut.LedgerNumber).Sum(x => x.CurrentBalance);

        Assert.True(Math.Abs(totalInitialBalance - sut.InitialBalance) <= 1,
                    TestsHelpers.BalanceDiffMsg($"Saldo inicial {sut.AccountName}. Moneda {sut.CurrencyCode}",
                                                $"{sut.AccountNumber}, sector {sut.SectorCode} ({sut.DebtorCreditor})",
                                                totalInitialBalance, sut.InitialBalance));

        Assert.True(Math.Abs(totalDebit - sut.Debit) <= 1,
                    TestsHelpers.BalanceDiffMsg($"Cargos. Moneda {sut.CurrencyCode}",
                                                $"{sut.AccountNumber}, sector {sut.SectorCode} ({sut.DebtorCreditor})",
                                                totalDebit, sut.Debit));
        Assert.True(Math.Abs(totalCredit - sut.Credit) <= 1,
                    TestsHelpers.BalanceDiffMsg($"Abonos. Moneda {sut.CurrencyCode}",
                                                $"{sut.AccountNumber}, sector {sut.SectorCode} ({sut.DebtorCreditor})",
                                                totalCredit, sut.Credit));

        Assert.True(Math.Abs(totalCurrentBalance - sut.CurrentBalance.Value) <= 1,
                    TestsHelpers.BalanceDiffMsg($"Saldo actual. Moneda {sut.CurrencyCode}",
                                                $"{sut.AccountNumber}, sector {sut.SectorCode} ({sut.DebtorCreditor})",
                                                totalCurrentBalance, sut.CurrentBalance.Value));
      }
    }

  } // class BalanzaContCascadaVsCoreBalancesTests

} // namespace Empiria.Tests.FinancialAccounting.BalanceEngine
