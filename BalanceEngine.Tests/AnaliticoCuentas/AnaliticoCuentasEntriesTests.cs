/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test cases                              *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Unit tests                              *
*  Type     : AnaliticoCuentasEntriesTests               License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Unit test cases for 'Analítico de cuentas' report entries.                                     *
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

namespace Empiria.FinancialAccounting.Tests.BalanceEngine.AnaliticoCuentas {

  /// <summary>Unit test cases for 'Analítico de cuentas' report entries.</summary>
  public class AnaliticoCuentasEntriesTests {

    private readonly EmpiriaHashTable<AnaliticoDeCuentasDto> _cache =
                                        new EmpiriaHashTable<AnaliticoDeCuentasDto>(16);

    #region Initialization

    public AnaliticoCuentasEntriesTests() {
      TestsCommonMethods.Authenticate();
    }

    #endregion Initialization

    #region Theories


    [Theory]
    [InlineData(AnaliticoDeCuentasTestCase.CatalogoAnterior)]
    [InlineData(AnaliticoDeCuentasTestCase.ConAuxiliares)]
    [InlineData(AnaliticoDeCuentasTestCase.Default)]
    [InlineData(AnaliticoDeCuentasTestCase.EnCascada)]
    [InlineData(AnaliticoDeCuentasTestCase.EnCascadaConAuxiliares)]
    [InlineData(AnaliticoDeCuentasTestCase.Sectorizado)]
    public async Task EntriesCountPerEntryItemType_Must_Match_TestData(AnaliticoDeCuentasTestCase testcase) {
      FixedList<AnaliticoDeCuentasEntryDto> expectedEntries = testcase.GetExpectedEntries();

      var expectedEntryTypes = expectedEntries.GroupBy(x => x.ItemType)
                                              .Select(group => new {
                                                          EntryType = group.Key,
                                                          Count = group.Count()
                                                      })
                                              .ToList();

      FixedList<AnaliticoDeCuentasEntryDto> sut = await GetAnaliticoDeCuentasEntries(testcase)
                                                       .ConfigureAwait(false);


      var sutEntryTypes = sut.GroupBy(x => x.ItemType)
                             .Select(group => new {
                                          EntryType = group.Key,
                                          Count = group.Count()
                                      })
                             .ToList();

      foreach (var expectedEntryType in expectedEntryTypes) {
        var actualEntryType = sutEntryTypes.Find(x => x.EntryType == expectedEntryType.EntryType);

        Assert.True(actualEntryType != null,
              $"Entries count of type '{expectedEntryType.EntryType}': " +
              $"Expected: {expectedEntryType.Count}, Actual: none.");

        Assert.True(expectedEntryType.Count == actualEntryType.Count,
              $"Entries count of type '{expectedEntryType.EntryType}': " +
              $"Expected: {expectedEntryType.Count}, Actual: {actualEntryType.Count}.");

      }
    }


    [Theory]
    [InlineData(AnaliticoDeCuentasTestCase.CatalogoAnterior)]
    [InlineData(AnaliticoDeCuentasTestCase.ConAuxiliares)]
    [InlineData(AnaliticoDeCuentasTestCase.Default)]
    [InlineData(AnaliticoDeCuentasTestCase.EnCascada)]
    [InlineData(AnaliticoDeCuentasTestCase.EnCascadaConAuxiliares)]
    [InlineData(AnaliticoDeCuentasTestCase.Sectorizado)]
    public async Task ContainsTheSameEntries_Than_TestData(AnaliticoDeCuentasTestCase testcase) {
      FixedList<AnaliticoDeCuentasEntryDto> expectedEntries = testcase.GetExpectedEntries();

      FixedList<AnaliticoDeCuentasEntryDto> sut = await GetAnaliticoDeCuentasEntries(testcase)
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
    [InlineData(AnaliticoDeCuentasTestCase.CatalogoAnterior)]
    [InlineData(AnaliticoDeCuentasTestCase.ConAuxiliares)]
    [InlineData(AnaliticoDeCuentasTestCase.Default)]
    [InlineData(AnaliticoDeCuentasTestCase.EnCascada)]
    [InlineData(AnaliticoDeCuentasTestCase.EnCascadaConAuxiliares)]
    [InlineData(AnaliticoDeCuentasTestCase.Sectorizado)]
    public async Task ForAllEntries_TotalBalance_Must_Be_DomesticBalance_Plus_ForeignBalance(AnaliticoDeCuentasTestCase testcase) {
      FixedList<AnaliticoDeCuentasEntryDto> sut = await GetAnaliticoDeCuentasEntries(testcase)
                                                       .ConfigureAwait(false);

      foreach (var sutEntry in sut) {
        decimal expected = sutEntry.DomesticBalance + sutEntry.ForeignBalance;
        decimal actual = sutEntry.TotalBalance;

        Assert.True(expected == actual,
          $"Expected total balance was {expected} but have been {actual}, " +
          $"for '{sutEntry.ItemType}' entry: {sutEntry.ToJson()}");
      }
    }


    [Theory]
    [InlineData(AnaliticoDeCuentasTestCase.CatalogoAnterior)]
    [InlineData(AnaliticoDeCuentasTestCase.ConAuxiliares)]
    [InlineData(AnaliticoDeCuentasTestCase.Default)]
    [InlineData(AnaliticoDeCuentasTestCase.EnCascada)]
    [InlineData(AnaliticoDeCuentasTestCase.EnCascadaConAuxiliares)]
    [InlineData(AnaliticoDeCuentasTestCase.Sectorizado)]
    public async Task TotalByLedger_Must_Be_The_Sum_Of_DebtorMinusCreditorGroups(AnaliticoDeCuentasTestCase testcase) {
      FixedList<AnaliticoDeCuentasEntryDto> sut = await GetAnaliticoDeCuentasEntries(testcase)
                                                       .ConfigureAwait(false);

      var sutLedgers = sut.Select(x => x.LedgerUID)
                          .Distinct();

      foreach (var sutLedgerUID in sutLedgers) {

        decimal debtorGroups = sut.FindAll(x => x.ItemType == TrialBalanceItemType.BalanceTotalGroupDebtor &&
                                                x.DebtorCreditor == DebtorCreditorType.Deudora &&
                                                x.LedgerUID == sutLedgerUID)
                                  .Sum(x => x.TotalBalance);

        decimal creditorGroups = sut.FindAll(x => x.ItemType == TrialBalanceItemType.BalanceTotalGroupCreditor &&
                                                  x.DebtorCreditor == DebtorCreditorType.Acreedora &&
                                                  x.LedgerUID == sutLedgerUID)
                                    .Sum(x => x.TotalBalance);

        decimal expected = debtorGroups - creditorGroups;

        decimal actual = sut.FindAll(x => x.ItemType == TrialBalanceItemType.BalanceTotalConsolidated &&
                                          x.LedgerUID == sutLedgerUID)
                            .Sum(x => x.TotalBalance);

        Assert.True(expected == actual,
                    $"The sum of debtor minus creditor groups for ledger '{sutLedgerUID}' was {expected}, " +
                    $"but it is not equals to the total entry value {actual}.");
      }
    }


    [Theory]
    [InlineData(AnaliticoDeCuentasTestCase.CatalogoAnterior)]
    [InlineData(AnaliticoDeCuentasTestCase.ConAuxiliares)]
    [InlineData(AnaliticoDeCuentasTestCase.Default)]
    [InlineData(AnaliticoDeCuentasTestCase.EnCascada)]
    [InlineData(AnaliticoDeCuentasTestCase.EnCascadaConAuxiliares)]
    [InlineData(AnaliticoDeCuentasTestCase.Sectorizado)]
    public async Task DebtorTotalsByLedger_Must_Be_The_Sum_Of_Its_DebtorAccounts(AnaliticoDeCuentasTestCase testcase) {
      FixedList<AnaliticoDeCuentasEntryDto> sut = await GetAnaliticoDeCuentasEntries(testcase)
                                                       .ConfigureAwait(false);

      var sutLedgers = sut.Select(x => x.LedgerUID)
                          .Distinct();

      foreach (var sutLedgerUID in sutLedgers) {

        decimal expected = sut.FindAll(x => x.LedgerUID == sutLedgerUID &&
                                            x.ItemType == TrialBalanceItemType.Entry &&
                                            x.DebtorCreditor == DebtorCreditorType.Deudora)
                              .Sum(x => x.TotalBalance);

        decimal actual = sut.FindAll(x => x.LedgerUID == sutLedgerUID &&
                                          x.ItemType == TrialBalanceItemType.BalanceTotalDebtor &&
                                          x.DebtorCreditor == DebtorCreditorType.Deudora)
                            .Sum(x => x.TotalBalance);

        Assert.True(expected == actual,
                    $"The sum of all debtor accounts for ledger '{sutLedgerUID}' was {expected}, " +
                    $"but it is not equals to the debtor total entry value {actual}.");
      }
    }


    [Theory]
    [InlineData(AnaliticoDeCuentasTestCase.CatalogoAnterior)]
    [InlineData(AnaliticoDeCuentasTestCase.ConAuxiliares)]
    [InlineData(AnaliticoDeCuentasTestCase.Default)]
    [InlineData(AnaliticoDeCuentasTestCase.EnCascada)]
    [InlineData(AnaliticoDeCuentasTestCase.EnCascadaConAuxiliares)]
    [InlineData(AnaliticoDeCuentasTestCase.Sectorizado)]
    public async Task CreditorTotalsByLedger_Must_Be_The_Sum_Of_Its_CreditorAccounts(AnaliticoDeCuentasTestCase testcase) {
      FixedList<AnaliticoDeCuentasEntryDto> sut = await GetAnaliticoDeCuentasEntries(testcase)
                                                       .ConfigureAwait(false);

      var sutLedgers = sut.Select(x => x.LedgerUID)
                          .Distinct();

      foreach (var sutLedgerUID in sutLedgers) {

        decimal expected = sut.FindAll(x => x.LedgerUID == sutLedgerUID &&
                                            x.ItemType == TrialBalanceItemType.Entry &&
                                            x.DebtorCreditor == DebtorCreditorType.Acreedora)
                              .Sum(x => x.TotalBalance);

        decimal actual = sut.FindAll(x => x.LedgerUID == sutLedgerUID &&
                                          x.ItemType == TrialBalanceItemType.BalanceTotalCreditor &&
                                          x.DebtorCreditor == DebtorCreditorType.Acreedora)
                            .Sum(x => x.TotalBalance);

        Assert.True(expected == actual,
                    $"The sum of all creditor accounts for ledger '{sutLedgerUID}' was {expected}, " +
                    $"but it is not equals to the creditor total entry value {actual}.");
      }
    }


    #endregion Theories

    #region Helpers

    private async Task<FixedList<AnaliticoDeCuentasEntryDto>> GetAnaliticoDeCuentasEntries(AnaliticoDeCuentasTestCase testcase) {
      TrialBalanceQuery query = testcase.GetInvocationQuery();

      AnaliticoDeCuentasDto dto = TryReadAnaliticoDeCuentasFromCache(query);

      if (dto != null) {
        return dto.Entries;
      }

      dto = await BalanceEngineProxy.BuildAnaliticoDeCuentas(query)
                                    .ConfigureAwait(false);

      StoreAnaliticoDeCuentasIntoCache(query, dto);

      return dto.Entries;
    }


    private void StoreAnaliticoDeCuentasIntoCache(TrialBalanceQuery query,
                                                  AnaliticoDeCuentasDto dto) {
      string key = query.ToString();

      _cache.Insert(key, dto);
    }


    private AnaliticoDeCuentasDto TryReadAnaliticoDeCuentasFromCache(TrialBalanceQuery query) {
      string key = query.ToString();

      if (_cache.ContainsKey(key)) {
        return _cache[key];
      }

      return null;
    }

    #endregion Helpers

  } // class AnaliticoCuentasEntriesTests

} // namespace Empiria.FinancialAccounting.Tests.Balances.AnaliticoCuentas
