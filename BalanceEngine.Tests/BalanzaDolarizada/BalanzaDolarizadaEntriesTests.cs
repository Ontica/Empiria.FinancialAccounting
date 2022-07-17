/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test cases                              *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Unit tests                              *
*  Type     : BalanzaDolarizadaEntriesTests              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Unit test cases for 'Balanza dolarizada' report entries.                                       *
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

namespace Empiria.FinancialAccounting.Tests.BalanceEngine.BalanzaDolarizada {

  /// <summary>Unit test cases for 'Balanza dolarizada' report entries.</summary>
  public class BalanzaDolarizadaEntriesTests {

    private readonly EmpiriaHashTable<BalanzaDolarizadaDto> _cache =
                                        new EmpiriaHashTable<BalanzaDolarizadaDto>(16);

    #region Initialization

    public BalanzaDolarizadaEntriesTests() {
      TestsCommonMethods.Authenticate();
    }

    #endregion Initialization

    #region Theories

    [Theory]
    [InlineData(BalanzaDolarizadaTestCase.CatalogoAnterior)]
    [InlineData(BalanzaDolarizadaTestCase.Default)]
    [InlineData(BalanzaDolarizadaTestCase.OficinasCentrales)]
    public async Task ContainsTheSameEntries_Than_TestData(BalanzaDolarizadaTestCase testcase) {
      FixedList<BalanzaDolarizadaEntryDto> expectedEntries = testcase.GetExpectedEntries();

      FixedList<BalanzaDolarizadaEntryDto> sut = await GetBalanzaDolarizadaEntries(testcase)
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
    [InlineData(BalanzaDolarizadaTestCase.CatalogoAnterior)]
    [InlineData(BalanzaDolarizadaTestCase.Default)]
    [InlineData(BalanzaDolarizadaTestCase.OficinasCentrales)]
    public async Task TotalEquivalence_Must_Be_The_Multiplication_Of_TotalBalance_And_ExchangeRate(
                        BalanzaDolarizadaTestCase testcase) {

      FixedList<BalanzaDolarizadaEntryDto> sut = await GetBalanzaDolarizadaEntries(testcase)
                                                       .ConfigureAwait(false);

      var entriesEquivalence = sut.FindAll(x => x.ItemType == TrialBalanceItemType.Entry ||
                                                 x.ItemType == TrialBalanceItemType.Summary)
                                   .Distinct();

      foreach (var totalEquivalence in entriesEquivalence) {

        var sutEntry = sut.FirstOrDefault(x => x.ItemType == TrialBalanceItemType.Entry ||
                                                 x.ItemType == TrialBalanceItemType.Summary &&
                                                 x.CurrencyCode == totalEquivalence.CurrencyCode &&
                                                 x.AccountNumber == totalEquivalence.AccountNumber);

        decimal expected = (decimal)sutEntry.TotalBalance * sutEntry.ExchangeRate;

        decimal actual = (decimal) totalEquivalence.TotalEquivalence;

        Assert.True(expected == actual,
                    $"The total equivalence '{totalEquivalence}' was {expected}, " +
                    $"but it is not equals to the entry value {actual}.");
      }
    }


    [Theory]
    [InlineData(BalanzaDolarizadaTestCase.CatalogoAnterior)]
    [InlineData(BalanzaDolarizadaTestCase.Default)]
    [InlineData(BalanzaDolarizadaTestCase.OficinasCentrales)]
    public async Task TotalByAccount_Must_Be_The_Sum_Of_TotalEquivalence_By_Currency(
                        BalanzaDolarizadaTestCase testcase) {

      FixedList<BalanzaDolarizadaEntryDto> sut = await GetBalanzaDolarizadaEntries(testcase)
                                                       .ConfigureAwait(false);

      var totalByAccounts = sut.FindAll(x => x.ItemType == TrialBalanceItemType.BalanceTotalCurrency)
                                   .Distinct();

      foreach (var totalByAccount in totalByAccounts) {

        decimal expected = sut.FindAll(x => x.ItemType == TrialBalanceItemType.Entry ||
                                        x.ItemType == TrialBalanceItemType.Summary &&
                                        x.AccountNumber == totalByAccount.AccountNumber)
                                  .Sum(x => x.TotalEquivalence);

        

        decimal actual = totalByAccount.TotalEquivalence;

        Assert.True(expected == actual,
                    $"The total equivalence by account '{totalByAccount}' was {expected}, " +
                    $"but it is not equals to the expected entry value {actual}.");
      }
    }

    #endregion Theories

    #region Helpers

    private async Task<FixedList<BalanzaDolarizadaEntryDto>> GetBalanzaDolarizadaEntries(BalanzaDolarizadaTestCase testcase) {
      TrialBalanceQuery query = testcase.GetInvocationQuery();

      BalanzaDolarizadaDto dto = TryReadBalanzaDolarizadaFromCache(query);

      if (dto != null) {
        return dto.Entries;
      }

      dto = await BalanceEngineProxy.BuildBalanzaDolarizada(query)
                                    .ConfigureAwait(false);

      StoreBalanzaDolarizadaIntoCache(query, dto);

      return dto.Entries;
    }


    private void StoreBalanzaDolarizadaIntoCache(TrialBalanceQuery query,
                                                  BalanzaDolarizadaDto dto) {
      string key = query.ToString();

      _cache.Insert(key, dto);
    }


    private BalanzaDolarizadaDto TryReadBalanzaDolarizadaFromCache(TrialBalanceQuery query) {
      string key = query.ToString();

      if (_cache.ContainsKey(key)) {
        return _cache[key];
      }

      return null;
    }

    #endregion Helpers

  } // class BalanzaDolarizadaEntriesTests

} // namespace Empiria.FinancialAccounting.Tests.BalanceEngine.BalanzaDolarizada
