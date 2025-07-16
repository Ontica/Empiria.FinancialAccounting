/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test cases                              *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Use cases tests                         *
*  Type     : BalanzaComparativaVsCoreBalancesTests      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Test cases that compares BalanzaComparativa vs core balances tests.                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Empiria.FinancialAccounting;
using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.Tests.FinancialAccounting.BalanceEngine;
using Xunit;

namespace Empiria.Tests.FinancialAccounting.BalanceEngine {

  /// <summary>Test cases that compares BalanzaComparativa vs core balances tests.</summary>
  public class BalanzaComparativaVsCoreBalancesTests {

    [Theory]
    [InlineData("2024-12-01", "2024-12-31", "2025-01-01", "2025-01-31", BalancesType.AllAccounts)]
    [InlineData("2025-01-01", "2025-01-31", "2025-02-01", "2025-02-28", BalancesType.AllAccounts)]
    [InlineData("2025-02-01", "2025-02-28", "2025-03-01", "2025-03-31", BalancesType.AllAccounts)]
    [InlineData("2025-03-01", "2025-03-31", "2025-04-01", "2025-04-30", BalancesType.AllAccounts)]
    [InlineData("2025-04-01", "2025-04-30", "2025-05-01", "2025-05-31", BalancesType.AllAccounts)]
    public void Should_Have_Same_Entries(string fromDate, string toDate,
                                                               string fromDate2, string toDate2, 
                                                               BalancesType balancesType) {

      CoreBalanceEntries coreBalances = TestsHelpers.GetCoreBalanceEntriesWithSubledgerAccounts(
                                                            DateTime.Parse(fromDate),
                                                            DateTime.Parse(toDate),
                                                            ExchangeRateType.ValorizacionBanxico);
      
      CoreBalanceEntries coreBalances2 = TestsHelpers.GetCoreBalanceEntriesWithSubledgerAccounts(
                                                            DateTime.Parse(fromDate2),
                                                            DateTime.Parse(toDate2),
                                                            ExchangeRateType.ValorizacionBanxico);

      FixedList<BalanzaComparativaEntryDto> comparativa = TestsHelpers.GetBalanzaComparativa(
                                                                    DateTime.Parse(fromDate),
                                                                    DateTime.Parse(toDate),
                                                                    DateTime.Parse(fromDate2),
                                                                    DateTime.Parse(toDate2),
                                                                    balancesType);

      RunTest(coreBalances, coreBalances2, comparativa.FindAll(x=>x.SubledgerAccountId == 0));

      RunTestForSubledgerAccount(coreBalances, coreBalances2, comparativa.FindAll(x => x.SubledgerAccountId > 0));

      Assert.True(comparativa.Count > 100);
    }


    private void RunTest(CoreBalanceEntries coreBalances, CoreBalanceEntries coreBalances2,
                         FixedList<BalanzaComparativaEntryDto> comparativa) {

      foreach (var sut in comparativa) {

        var filtered = coreBalances.GetBalancesByAccountNumberAndSector(sut.AccountNumber, sut.SectorCode)
                                   .FindAll(x => x.Account.DebtorCreditor == sut.DebtorCreditor &&
                                                 x.SubledgerAccount.Id <= 0);

        var filtered2 = coreBalances2.GetBalancesByAccountNumberAndSector(sut.AccountNumber, sut.SectorCode)
                                     .FindAll(x => x.Account.DebtorCreditor == sut.DebtorCreditor &&
                                                 x.SubledgerAccount.Id <= 0);

        var totalFirstPeriod = filtered.FindAll(x => x.Currency.Code.Equals(sut.CurrencyCode))
                                       .Sum(x => x.CurrentBalance);

        var totalSecondPeriod = filtered2.FindAll(x => x.Currency.Code.Equals(sut.CurrencyCode))
                                         .Sum(x => x.CurrentBalance);

        Assert.True(Math.Abs(totalFirstPeriod - sut.FirstValorization) <= 1,
                    TestsHelpers.BalanceDiffMsg($"Val_A, Moneda {sut.CurrencyCode}", $"{sut.AccountNumber}, " +
                                                $"{sut.SubledgerAccountNumber}, sector {sut.SectorCode}",
                                                totalFirstPeriod, sut.FirstValorization));

        Assert.True(Math.Abs(totalSecondPeriod - sut.SecondValorization) <= 1,
                    TestsHelpers.BalanceDiffMsg($"Val_B, Moneda {sut.CurrencyCode}", $"{sut.AccountNumber}, " +
                                                $"{sut.SubledgerAccountNumber}, sector {sut.SectorCode}",
                                                totalSecondPeriod, sut.SecondValorization));
      }
    }


    private void RunTestForSubledgerAccount(CoreBalanceEntries coreBalances, CoreBalanceEntries coreBalances2,
                         FixedList<BalanzaComparativaEntryDto> comparativa) {

      foreach (var sut in comparativa) {

        var filtered = coreBalances.GetBalancesByAccountNumberAndSubledgerAccountIdAndSector(
                                      sut.AccountNumber, sut.SubledgerAccountId, sut.SectorCode)
                                   .FindAll(x => x.Account.DebtorCreditor == sut.DebtorCreditor);

        var filtered2 = coreBalances2.GetBalancesByAccountNumberAndSubledgerAccountIdAndSector(
                                      sut.AccountNumber, sut.SubledgerAccountId, sut.SectorCode)
                                   .FindAll(x => x.Account.DebtorCreditor == sut.DebtorCreditor);

        var totalFirstPeriod = filtered.FindAll(x => x.Currency.Code.Equals(sut.CurrencyCode))
                                       .Sum(x => x.CurrentBalance);

        var totalSecondPeriod = filtered2.FindAll(x => x.Currency.Code.Equals(sut.CurrencyCode))
                                         .Sum(x => x.CurrentBalance);

        Assert.True(Math.Abs(totalFirstPeriod - sut.FirstValorization) <= 1,
                    TestsHelpers.BalanceDiffMsg($"AUX Val_A, Moneda {sut.CurrencyCode}", $"{sut.AccountNumber}, " +
                                                $"{sut.SubledgerAccountNumber}, sector {sut.SectorCode}",
                                                totalFirstPeriod, sut.FirstValorization));

        Assert.True(Math.Abs(totalSecondPeriod - sut.SecondValorization) <= 1,
                    TestsHelpers.BalanceDiffMsg($"AUX Val_B, Moneda {sut.CurrencyCode}", $"{sut.AccountNumber}, " +
                                                $"{sut.SubledgerAccountNumber}, sector {sut.SectorCode}",
                                                totalSecondPeriod, sut.SecondValorization));
      }
    }

  } // class BalanzaComparativaVsCoreBalancesTests

} // namespace Empiria.Tests.FinancialAccounting.BalanceEngine
