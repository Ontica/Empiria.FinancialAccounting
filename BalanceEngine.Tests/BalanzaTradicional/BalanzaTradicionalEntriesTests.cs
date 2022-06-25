/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test cases                              *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Unit tests                              *
*  Type     : BalanzaTradicionalEntriesTests             License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Unit test cases for 'Balanza tradicional' report entries.                                      *
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

namespace Empiria.FinancialAccounting.Tests.BalanceEngine.BalanzaTradicional {

  /// <summary>Unit test cases for 'Balanza tradicional' report entries.</summary>
  public class BalanzaTradicionalEntriesTests {

    private readonly EmpiriaHashTable<BalanzaTradicionalDto> _cache =
                                        new EmpiriaHashTable<BalanzaTradicionalDto>(16);

    #region Initialization

    public BalanzaTradicionalEntriesTests() {
      TestsCommonMethods.Authenticate();
    }

    #endregion Initialization


    #region Theories

    [Theory]
    [InlineData(BalanzaTradicionalTestCase.CatalogoAnterior)]
    [InlineData(BalanzaTradicionalTestCase.ConAuxiliares)]
    [InlineData(BalanzaTradicionalTestCase.Default)]
    [InlineData(BalanzaTradicionalTestCase.EnCascada)]
    [InlineData(BalanzaTradicionalTestCase.EnCascadaConAuxiliares)]
    [InlineData(BalanzaTradicionalTestCase.Sectorizada)]
    public async Task ContainsTheSameEntries_Than_TestData(BalanzaTradicionalTestCase testcase) {
      FixedList<BalanzaTradicionalEntryDto> expectedEntries = testcase.GetExpectedEntries();

      FixedList<BalanzaTradicionalEntryDto> sut = await GetBalanzaTradicionalEntries(testcase)
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
    [InlineData(BalanzaTradicionalTestCase.CatalogoAnterior)]
    [InlineData(BalanzaTradicionalTestCase.ConAuxiliares)]
    [InlineData(BalanzaTradicionalTestCase.Default)]
    [InlineData(BalanzaTradicionalTestCase.EnCascada)]
    [InlineData(BalanzaTradicionalTestCase.EnCascadaConAuxiliares)]
    [InlineData(BalanzaTradicionalTestCase.Sectorizada)]
    public async Task DebtorTotalsByGroup_Must_Be_The_Sum_Of_Its_DebtorAccounts(BalanzaTradicionalTestCase testcase) {
      FixedList<BalanzaTradicionalEntryDto> sut = await GetBalanzaTradicionalEntries(testcase)
                                                       .ConfigureAwait(false);

      var totalsByDebtorGroup = sut.FindAll(x => x.ItemType == TrialBalanceItemType.BalanceTotalGroupDebtor)
                          .Distinct();

      foreach (var totalByDebtorGroup in totalsByDebtorGroup) {

        decimal expected = (decimal) sut.FindAll(x => x.ItemType == TrialBalanceItemType.Entry &&
                                                 !x.IsParentPostingEntry &&
                                                 x.CurrencyCode == totalByDebtorGroup.CurrencyCode &&
                                                 x.DebtorCreditor == totalByDebtorGroup.DebtorCreditor)
                                       .Sum(x => x.CurrentBalance);

        decimal actual = (decimal) totalByDebtorGroup.CurrentBalance;

        Assert.True(expected == actual,
                    $"The sum of all debtor accounts for Total debtor '{totalByDebtorGroup}' was {expected}, " +
                    $"but it is not equals to the debtor total entry value {actual}.");
      }
    }


    [Theory]
    [InlineData(BalanzaTradicionalTestCase.CatalogoAnterior)]
    [InlineData(BalanzaTradicionalTestCase.ConAuxiliares)]
    [InlineData(BalanzaTradicionalTestCase.Default)]
    [InlineData(BalanzaTradicionalTestCase.EnCascada)]
    [InlineData(BalanzaTradicionalTestCase.EnCascadaConAuxiliares)]
    [InlineData(BalanzaTradicionalTestCase.Sectorizada)]
    public async Task CreditorTotalsByGroup_Must_Be_The_Sum_Of_Its_CreditorAccounts(BalanzaTradicionalTestCase testcase) {
      FixedList<BalanzaTradicionalEntryDto> sut = await GetBalanzaTradicionalEntries(testcase)
                                                       .ConfigureAwait(false);

      var totalsByCreditorGroup = sut.FindAll(x => x.ItemType == TrialBalanceItemType.BalanceTotalGroupCreditor)
                          .Distinct();

      foreach (var totalByCreditorGroup in totalsByCreditorGroup) {

        decimal expected = (decimal) sut.FindAll(x => x.ItemType == TrialBalanceItemType.Entry &&
                                                 !x.IsParentPostingEntry &&
                                                 x.CurrencyCode == totalByCreditorGroup.CurrencyCode &&
                                                 x.DebtorCreditor == totalByCreditorGroup.DebtorCreditor)
                                       .Sum(x => x.CurrentBalance);

        decimal actual = (decimal) totalByCreditorGroup.CurrentBalance;

        Assert.True(expected == actual,
                    $"The sum of all creditor accounts for Total creditor '{totalByCreditorGroup}' was {expected}, " +
                    $"but it is not equals to the creditor total entry value {actual}.");
      }
    }


    [Theory]
    [InlineData(BalanzaTradicionalTestCase.CatalogoAnterior)]
    [InlineData(BalanzaTradicionalTestCase.ConAuxiliares)]
    [InlineData(BalanzaTradicionalTestCase.Default)]
    [InlineData(BalanzaTradicionalTestCase.EnCascada)]
    [InlineData(BalanzaTradicionalTestCase.EnCascadaConAuxiliares)]
    [InlineData(BalanzaTradicionalTestCase.Sectorizada)]
    public async Task DebtorTotals_Must_Be_The_Sum_Of_Its_TotalGroupDebtors(BalanzaTradicionalTestCase testcase) {
      FixedList<BalanzaTradicionalEntryDto> sut = await GetBalanzaTradicionalEntries(testcase)
                                                       .ConfigureAwait(false);

      var debtorTotals = sut.FindAll(x => x.ItemType == TrialBalanceItemType.BalanceTotalDebtor)
                          .Distinct();

      foreach (var debtorTotal in debtorTotals) {

        decimal expected = (decimal) sut.FindAll(x => x.ItemType == TrialBalanceItemType.BalanceTotalGroupDebtor &&
                                                 x.CurrencyCode == debtorTotal.CurrencyCode)
                                        .Sum(x => x.CurrentBalance);

        decimal actual = (decimal) debtorTotal.CurrentBalance;

        Assert.True(expected == actual,
                    $"The sum of all debtor groups for Total debtors '{debtorTotal}' was {expected}, " +
                    $"but it is not equals to the Total debtors entry value {actual}.");
      }
    }


    [Theory]
    [InlineData(BalanzaTradicionalTestCase.CatalogoAnterior)]
    [InlineData(BalanzaTradicionalTestCase.ConAuxiliares)]
    [InlineData(BalanzaTradicionalTestCase.Default)]
    [InlineData(BalanzaTradicionalTestCase.EnCascada)]
    [InlineData(BalanzaTradicionalTestCase.EnCascadaConAuxiliares)]
    [InlineData(BalanzaTradicionalTestCase.Sectorizada)]
    public async Task CreditorTotals_Must_Be_The_Sum_Of_Its_TotalGroupCreditors(BalanzaTradicionalTestCase testcase) {
      FixedList<BalanzaTradicionalEntryDto> sut = await GetBalanzaTradicionalEntries(testcase)
                                                       .ConfigureAwait(false);

      var debtorTotals = sut.FindAll(x => x.ItemType == TrialBalanceItemType.BalanceTotalCreditor)
                          .Distinct();

      foreach (var debtorTotal in debtorTotals) {

        decimal expected = (decimal) sut.FindAll(x => x.ItemType == TrialBalanceItemType.BalanceTotalGroupCreditor &&
                                                 x.CurrencyCode == debtorTotal.CurrencyCode)
                                        .Sum(x => x.CurrentBalance);

        decimal actual = (decimal) debtorTotal.CurrentBalance;

        Assert.True(expected == actual,
                    $"The sum of all creditor groups for Total creditors '{debtorTotal}' was {expected}, " +
                    $"but it is not equals to the Total creditors entry value {actual}.");
      }
    }


    [Theory]
    [InlineData(BalanzaTradicionalTestCase.CatalogoAnterior)]
    [InlineData(BalanzaTradicionalTestCase.ConAuxiliares)]
    [InlineData(BalanzaTradicionalTestCase.Default)]
    [InlineData(BalanzaTradicionalTestCase.EnCascada)]
    [InlineData(BalanzaTradicionalTestCase.EnCascadaConAuxiliares)]
    [InlineData(BalanzaTradicionalTestCase.Sectorizada)]
    public async Task TotalByCurrency_must_be_the_sum_of_totalDebtors_minus_totalCreditors(BalanzaTradicionalTestCase testcase) {
      FixedList<BalanzaTradicionalEntryDto> sut = await GetBalanzaTradicionalEntries(testcase)
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

    private async Task<FixedList<BalanzaTradicionalEntryDto>> GetBalanzaTradicionalEntries(BalanzaTradicionalTestCase testcase) {
      TrialBalanceQuery query = testcase.GetInvocationQuery();

      BalanzaTradicionalDto dto = TryReadBalanzaTradicionalFromCache(query);

      if (dto != null) {
        return dto.Entries;
      }

      dto = await BalanceEngineProxy.BuildBalanzaTradicional(query)
                                    .ConfigureAwait(false);

      StoreBalanzaTradicionalIntoCache(query, dto);

      return dto.Entries;
    }


    private void StoreBalanzaTradicionalIntoCache(TrialBalanceQuery query,
                                                  BalanzaTradicionalDto dto) {
      string key = query.ToString();

      _cache.Insert(key, dto);
    }


    private BalanzaTradicionalDto TryReadBalanzaTradicionalFromCache(TrialBalanceQuery query) {
      string key = query.ToString();

      if (_cache.ContainsKey(key)) {
        return _cache[key];
      }

      return null;
    }

    #endregion Helpers

  } // class BalanzaTradicionalEntriesTests

} // namespace Empiria.FinancialAccounting.Tests.BalanceEngine.BalanzaTradicional
