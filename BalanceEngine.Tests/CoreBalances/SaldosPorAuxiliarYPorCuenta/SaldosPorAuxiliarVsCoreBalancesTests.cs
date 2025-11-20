/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test cases                              *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Use cases tests                         *
*  Type     : SaldosPorAuxiliarVsCoreBalancesTests       License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Test cases that compares SaldosPorAuxiliar vs core balances tests.                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System;

using System.Linq;

using Empiria.FinancialAccounting;
using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Xunit;

namespace Empiria.Tests.FinancialAccounting.BalanceEngine {

  /// <summary>Test cases that compares SaldosPorAuxiliar vs core balances tests.</summary>
  public class SaldosPorAuxiliarVsCoreBalancesTests {

    [Theory]
    [InlineData("2025-04-01", "2025-04-30", BalancesType.AllAccounts)]
    public void Should_Have_Same_Entries(string fromDate, string toDate, BalancesType balancesType) {

      CoreBalanceEntries coreBalances = TestsHelpers.GetCoreBalanceEntriesWithSubledgerAccounts(
                                                                            DateTime.Parse(fromDate),
                                                                            DateTime.Parse(toDate),
                                                                            ExchangeRateType.Empty);

      FixedList<SaldosPorAuxiliarEntryDto> saldosAuxiliar =
                                                SaldosPorAuxiliarYPorCuentaTestHelpers.GetSaldosPorAuxiliar(
                                                                                    DateTime.Parse(fromDate),
                                                                                    DateTime.Parse(toDate),
                                                                                    balancesType)
                                                    .FindAll(x => x.ItemType == TrialBalanceItemType.Entry &&
                                                                 !x.IsParentPostingEntry);
      
      RunTest(coreBalances, saldosAuxiliar);

      Assert.True(saldosAuxiliar.Count > 500);
    }


    private void RunTest(CoreBalanceEntries coreBalances, FixedList<SaldosPorAuxiliarEntryDto> saldosAuxiliar) {

      foreach (var sut in saldosAuxiliar) {

        var filtered = coreBalances.GetBalancesByAccountNumberAndSubledgerAccountNumberAndSector(
                                      sut.AccountNumber, sut.SubledgerAccountNumber, sut.SectorCode)
                                   .FindAll(x => x.Account.DebtorCreditor.ToString() == sut.DebtorCreditor &&
                                                 x.Ledger.Number == sut.LedgerNumber &&
                                                 x.LastChangeDate == sut.LastChangeDate);
        
        var totalCurrentBalance = filtered.FindAll(x => x.Currency.Code == sut.CurrencyCode)
                                          .Sum(x => x.CurrentBalance);

        Assert.True(Math.Abs(totalCurrentBalance - sut.CurrentBalance.Value) <= 1,
                    TestsHelpers.BalanceDiffMsg($"Saldo actual. Moneda {sut.CurrencyCode}. " +
                                                $"Auxiliar {sut.SubledgerAccountNumber} ",
                                                $"{sut.AccountNumber}, sector {sut.SectorCode} ({sut.DebtorCreditor})",
                                                totalCurrentBalance, sut.CurrentBalance.Value));
      }
    }

  } // class SaldosPorAuxiliarVsCoreBalancesTests

} // namespace Empiria.Tests.FinancialAccounting.BalanceEngine
