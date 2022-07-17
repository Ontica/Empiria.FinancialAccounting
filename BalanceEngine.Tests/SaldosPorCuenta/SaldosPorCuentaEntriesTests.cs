/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test cases                              *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Unit tests                              *
*  Type     : SaldosPorCuentaEntriesTests                License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Unit test cases for 'Saldos por cuenta' report entries.                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Linq;
using System.Threading.Tasks;

using Xunit;

using Empiria.Collections;
using Empiria.Tests;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine;


namespace Empiria.FinancialAccounting.Tests.BalanceEngine.SaldosPorCuenta {

  /// <summary>Unit test cases for 'Saldos por cuenta' report entries.</summary>
  public class SaldosPorCuentaEntriesTests {

    private readonly EmpiriaHashTable<SaldosPorCuentaDto> _cache =
                                        new EmpiriaHashTable<SaldosPorCuentaDto>(16);

    #region Initialization

    public SaldosPorCuentaEntriesTests() {
      TestsCommonMethods.Authenticate();
    }

    #endregion Initialization

    #region Theories

    [Theory]
    [InlineData(SaldosPorCuentaTestCase.CatalogoAnterior)]
    [InlineData(SaldosPorCuentaTestCase.ConAuxiliares)]
    [InlineData(SaldosPorCuentaTestCase.ConAuxiliaresValorizados)]
    [InlineData(SaldosPorCuentaTestCase.Default)]
    [InlineData(SaldosPorCuentaTestCase.EnCascada)]
    [InlineData(SaldosPorCuentaTestCase.Valorizados)]
    public async Task ContainsTheSameEntries_Than_TestData(SaldosPorCuentaTestCase testcase) {

      FixedList<SaldosPorCuentaEntryDto> expectedEntries = testcase.GetExpectedEntries();

      FixedList<SaldosPorCuentaEntryDto> sut = await GetSaldosPorCuentaEntries(testcase)
                                                       .ConfigureAwait(false);

      foreach (var expectedEntry in expectedEntries) {
        var actualEntryFound = sut.Contains(x => x.Equals(expectedEntry));

        Assert.True(actualEntryFound,
          $"Actual entries do not have this '{expectedEntry.ItemType}'" +
          $"expected entry: {expectedEntry.ToJson()}");
      }


      foreach (var actualEntry in sut) {
        var expectedEntryFound = expectedEntries.Contains(x => x.Equals(actualEntry));

        Assert.True(expectedEntryFound,
          $"Expected entries do not have this '{actualEntry.ItemType}' " +
          $"actual entry: {actualEntry.ToJson()}");
      }
    }


    [Theory]
    [InlineData(SaldosPorCuentaTestCase.CatalogoAnterior)]
    [InlineData(SaldosPorCuentaTestCase.ConAuxiliares)]
    [InlineData(SaldosPorCuentaTestCase.ConAuxiliaresValorizados)]
    [InlineData(SaldosPorCuentaTestCase.Default)]
    [InlineData(SaldosPorCuentaTestCase.EnCascada)]
    [InlineData(SaldosPorCuentaTestCase.Valorizados)]
    public async Task DebtorTotals_Must_Be_The_Sum_Of_Its_DebtorEntries(SaldosPorCuentaTestCase testcase) {

      FixedList<SaldosPorCuentaEntryDto> sut = await GetSaldosPorCuentaEntries(testcase)
                                                       .ConfigureAwait(false);

      var debtorTotals = sut.FindAll(x => x.ItemType == TrialBalanceItemType.BalanceTotalDebtor)
                          .Distinct();

      foreach (var debtorTotal in debtorTotals) {

        decimal expected = (decimal) sut.FindAll(x => x.ItemType == TrialBalanceItemType.Entry &&
                                                 !x.IsParentPostingEntry &&
                                                 x.CurrencyCode == debtorTotal.CurrencyCode &&
                                                 x.DebtorCreditor == "Deudora")
                                        .Sum(x => x.CurrentBalance);

        decimal actual = (decimal) debtorTotal.CurrentBalance;

        Assert.True(expected == actual,
                    $"The sum of all debtor entries for Total debtors '{debtorTotal}' was {expected}, " +
                    $"but it is not equals to the Total debtors entry value {actual}.");
      }
    }


    [Theory]
    [InlineData(SaldosPorCuentaTestCase.CatalogoAnterior)]
    [InlineData(SaldosPorCuentaTestCase.ConAuxiliares)]
    [InlineData(SaldosPorCuentaTestCase.ConAuxiliaresValorizados)]
    [InlineData(SaldosPorCuentaTestCase.Default)]
    [InlineData(SaldosPorCuentaTestCase.EnCascada)]
    [InlineData(SaldosPorCuentaTestCase.Valorizados)]
    public async Task CreditorTotals_Must_Be_The_Sum_Of_Its_CreditorEntries(
                        SaldosPorCuentaTestCase testcase) {

      FixedList<SaldosPorCuentaEntryDto> sut = await GetSaldosPorCuentaEntries(testcase)
                                                       .ConfigureAwait(false);

      var creditorTotals = sut.FindAll(x => x.ItemType == TrialBalanceItemType.BalanceTotalCreditor)
                          .Distinct();

      foreach (var creditorTotal in creditorTotals) {

        decimal expected = (decimal) sut.FindAll(x => x.ItemType == TrialBalanceItemType.Entry &&
                                                 !x.IsParentPostingEntry &&
                                                 x.CurrencyCode == creditorTotal.CurrencyCode &&
                                                 x.DebtorCreditor == "Acreedora")
                                        .Sum(x => x.CurrentBalance);

        decimal actual = (decimal) creditorTotal.CurrentBalance;

        Assert.True(expected == actual,
                    $"The sum of all creditor entries for Total creditors '{creditorTotal}' was {expected}, " +
                    $"but it is not equals to the Total creditors entry value {actual}.");
      }
    }


    [Theory]
    [InlineData(SaldosPorCuentaTestCase.CatalogoAnterior)]
    [InlineData(SaldosPorCuentaTestCase.ConAuxiliares)]
    [InlineData(SaldosPorCuentaTestCase.ConAuxiliaresValorizados)]
    [InlineData(SaldosPorCuentaTestCase.Default)]
    [InlineData(SaldosPorCuentaTestCase.EnCascada)]
    [InlineData(SaldosPorCuentaTestCase.Valorizados)]
    public async Task TotalByCurrency_Must_Be_The_Sum_Of_TotalDebtors_Minus_TotalCreditors(
                        SaldosPorCuentaTestCase testcase) {

      FixedList<SaldosPorCuentaEntryDto> sut = await GetSaldosPorCuentaEntries(testcase)
                                                       .ConfigureAwait(false);

      var totalsByCurrency = sut.FindAll(x => x.ItemType == TrialBalanceItemType.BalanceTotalCurrency)
                          .Distinct();

      foreach (var totalByCurrency in totalsByCurrency) {

        decimal totalDebtor = (decimal) sut.FindAll(x => x.ItemType == TrialBalanceItemType.BalanceTotalDebtor &&
                                                 x.CurrencyCode == totalByCurrency.CurrencyCode)
                                        .Sum(x => x.CurrentBalance);

        decimal totalCreditor = (decimal) sut.FindAll(x => x.ItemType == TrialBalanceItemType.BalanceTotalCreditor &&
                                                 x.CurrencyCode == totalByCurrency.CurrencyCode)
                                        .Sum(x => x.CurrentBalance);

        decimal expected = totalDebtor - totalCreditor;

        decimal actual = (decimal) totalByCurrency.CurrentBalance;

        Assert.True(expected == actual,
                    $"The sum between total debtors minus total creditors for Total by currency '{totalByCurrency}' " +
                    $"was {expected}, but it is not equals to the Total by currency entry value {actual}.");
      }
    }


    #endregion Theories

    #region Helpers

    private async Task<FixedList<SaldosPorCuentaEntryDto>> GetSaldosPorCuentaEntries(SaldosPorCuentaTestCase testcase) {
      TrialBalanceQuery query = testcase.GetInvocationQuery();

      SaldosPorCuentaDto dto = TryReadSaldosPorCuentaFromCache(query);

      if (dto != null) {
        return dto.Entries;
      }

      dto = await BalanceEngineProxy.BuildSaldosPorCuenta(query)
                                    .ConfigureAwait(false);

      StoreSaldosPorCuentaIntoCache(query, dto);

      return dto.Entries;
    }


    private void StoreSaldosPorCuentaIntoCache(TrialBalanceQuery query,
                                                  SaldosPorCuentaDto dto) {
      string key = query.ToString();

      _cache.Insert(key, dto);
    }


    private SaldosPorCuentaDto TryReadSaldosPorCuentaFromCache(TrialBalanceQuery query) {
      string key = query.ToString();

      if (_cache.ContainsKey(key)) {
        return _cache[key];
      }

      return null;
    }

    #endregion Helpers

  } // class SaldosPorCuentaEntriesTests

} // namespace Empiria.FinancialAccounting.Tests.BalanceEngine.SaldosPorCuenta
