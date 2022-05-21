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
    public async Task AccountEntriesAreOk(AnaliticoDeCuentasTestCase testcase) {
      FixedList<AnaliticoDeCuentasEntryDto> expectedEntries = testcase.GetExpectedEntries(TrialBalanceItemType.Entry);

      FixedList<AnaliticoDeCuentasEntryDto> sut = await GetAnaliticoDeCuentasEntries(testcase,
                                                                                     TrialBalanceItemType.Entry);

      Assert.Equal(expectedEntries.Count, sut.Count);

      foreach (var sutEntry in sut) {
        var expected = expectedEntries.Find(x => x.Equals(sutEntry));

        Assert.True(expected != null, $"Expected entries do not have this AccountEntry: {sutEntry.ToJson()}");
      }
    }


    [Theory]
    [InlineData(AnaliticoDeCuentasTestCase.CatalogoAnterior)]
    [InlineData(AnaliticoDeCuentasTestCase.ConAuxiliares)]
    [InlineData(AnaliticoDeCuentasTestCase.Default)]
    [InlineData(AnaliticoDeCuentasTestCase.EnCascada)]
    [InlineData(AnaliticoDeCuentasTestCase.EnCascadaConAuxiliares)]
    [InlineData(AnaliticoDeCuentasTestCase.Sectorizado)]
    public async Task AccountSummaryEntriesAreOk(AnaliticoDeCuentasTestCase testcase) {

      FixedList<AnaliticoDeCuentasEntryDto> expectedEntries = testcase.GetExpectedEntries(TrialBalanceItemType.Summary);

      FixedList<AnaliticoDeCuentasEntryDto> sut = await GetAnaliticoDeCuentasEntries(testcase,
                                                                                     TrialBalanceItemType.Summary);

      Assert.Equal(expectedEntries.Count, sut.Count);

      foreach (var sutEntry in sut) {
        var expected = expectedEntries.Find(x => x.Equals(sutEntry));

        Assert.True(expected != null, $"Expected entries do not have this AccountSummary entry: {sutEntry.ToJson()}");
      }
    }


    [Theory]
    [InlineData(AnaliticoDeCuentasTestCase.CatalogoAnterior)]
    [InlineData(AnaliticoDeCuentasTestCase.ConAuxiliares)]
    [InlineData(AnaliticoDeCuentasTestCase.Default)]
    [InlineData(AnaliticoDeCuentasTestCase.EnCascada)]
    [InlineData(AnaliticoDeCuentasTestCase.EnCascadaConAuxiliares)]
    [InlineData(AnaliticoDeCuentasTestCase.Sectorizado)]
    public async Task GroupTotalEntriesAreOk(AnaliticoDeCuentasTestCase testcase) {

      // ToDo: No distinguir entre grupos deudores y acreedores. Usar el dato debtorCreditor
      // Check Debtor account groups
      FixedList<AnaliticoDeCuentasEntryDto> expectedEntries = testcase.GetExpectedEntries(TrialBalanceItemType.BalanceTotalGroupDebtor);

      FixedList<AnaliticoDeCuentasEntryDto> sut = await GetAnaliticoDeCuentasEntries(testcase,
                                                                                     TrialBalanceItemType.BalanceTotalGroupDebtor);

      Assert.Equal(expectedEntries.Count, sut.Count);

      foreach (var sutEntry in sut) {
        var expected = expectedEntries.Find(x => x.Equals(sutEntry));

        Assert.True(expected != null, $"Expected entries do not have this GroupTotalEntry (debtor) entry: {sutEntry.ToJson()}");
      }


      // Check Creditor account groups
      expectedEntries = testcase.GetExpectedEntries(TrialBalanceItemType.BalanceTotalGroupCreditor);

      sut = await GetAnaliticoDeCuentasEntries(testcase, TrialBalanceItemType.BalanceTotalGroupCreditor);

      Assert.Equal(expectedEntries.Count, sut.Count);

      foreach (var sutEntry in sut) {
        var expected = expectedEntries.Find(x => x.Equals(sutEntry));

        Assert.True(expected != null, $"Expected entries do not have this GroupTotalEntry entry: (creditor) {sutEntry.ToJson()}");
      }

    }

    [Theory]
    [InlineData(AnaliticoDeCuentasTestCase.CatalogoAnterior)]
    [InlineData(AnaliticoDeCuentasTestCase.ConAuxiliares)]
    [InlineData(AnaliticoDeCuentasTestCase.Default)]
    [InlineData(AnaliticoDeCuentasTestCase.EnCascada)]
    [InlineData(AnaliticoDeCuentasTestCase.EnCascadaConAuxiliares)]
    [InlineData(AnaliticoDeCuentasTestCase.Sectorizado)]
    public async Task LedgerDebtorCreditorTotalEntriesAreOk(AnaliticoDeCuentasTestCase testcase) {

      // ToDo: No distinguir entre totales deudores y acreedores por ledger. Usar el dato debtorCreditor

      // Check Debtor totals
      FixedList<AnaliticoDeCuentasEntryDto> expectedEntries = testcase.GetExpectedEntries(TrialBalanceItemType.BalanceTotalDebtor);

      FixedList<AnaliticoDeCuentasEntryDto> sut = await GetAnaliticoDeCuentasEntries(testcase,
                                                                                     TrialBalanceItemType.BalanceTotalDebtor);

      Assert.Equal(expectedEntries.Count, sut.Count);

      foreach (var sutEntry in sut) {
        var expected = expectedEntries.Find(x => x.Equals(sutEntry));

        Assert.True(expected != null, $"Expected entries do not have this  LedgerDebtorCreditorTotal (debtor) entry: {sutEntry.ToJson()}");
      }


      // Check Creditor totals
      expectedEntries = testcase.GetExpectedEntries(TrialBalanceItemType.BalanceTotalCreditor);

      sut = await GetAnaliticoDeCuentasEntries(testcase, TrialBalanceItemType.BalanceTotalCreditor);

      Assert.Equal(expectedEntries.Count, sut.Count);

      foreach (var sutEntry in sut) {
        var expected = expectedEntries.Find(x => x.Equals(sutEntry));

        Assert.True(expected != null, $"Expected entries do not have this  LedgerDebtorCreditorTotal entry: (creditor) {sutEntry.ToJson()}");
      }

    }



    [Theory]
    [InlineData(AnaliticoDeCuentasTestCase.CatalogoAnterior)]
    [InlineData(AnaliticoDeCuentasTestCase.ConAuxiliares)]
    [InlineData(AnaliticoDeCuentasTestCase.Default)]
    [InlineData(AnaliticoDeCuentasTestCase.EnCascada)]
    [InlineData(AnaliticoDeCuentasTestCase.EnCascadaConAuxiliares)]
    [InlineData(AnaliticoDeCuentasTestCase.Sectorizado)]
    public async Task LedgerTotalEntriesAreOk(AnaliticoDeCuentasTestCase testcase) {

      FixedList<AnaliticoDeCuentasEntryDto> expectedEntries = testcase.GetExpectedEntries(TrialBalanceItemType.BalanceTotalConsolidated);

      FixedList<AnaliticoDeCuentasEntryDto> sut = await GetAnaliticoDeCuentasEntries(testcase,
                                                                                     TrialBalanceItemType.BalanceTotalConsolidated);

      Assert.Equal(expectedEntries.Count, sut.Count);

      foreach (var sutEntry in sut) {
        var expected = expectedEntries.Find(x => x.Equals(sutEntry));

        Assert.True(expected != null, $"Expected entries do not have this LedgerTotal entry: {sutEntry.ToJson()}");
      }
    }


    #endregion Theories

    #region Helpers

    private async Task<FixedList<AnaliticoDeCuentasEntryDto>> GetAnaliticoDeCuentasEntries(AnaliticoDeCuentasTestCase testcase,
                                                                                           TrialBalanceItemType entriesFilter) {
      TrialBalanceCommand command = testcase.GetInvocationCommand();

      AnaliticoDeCuentasDto dto = TryReadAnaliticoDeCuentasFromCache(command);

      if (dto != null) {
        return dto.Entries.FindAll(x => x.ItemType == entriesFilter);
      }

      dto = await BalanceEngineProxy.BuildAnaliticoDeCuentas(command)
                                                          .ConfigureAwait(false);

      StoreAnaliticoDeCuentasIntoCache(command, dto);

      return dto.Entries.FindAll(x => x.ItemType == entriesFilter);
    }


    private void StoreAnaliticoDeCuentasIntoCache(TrialBalanceCommand command,
                                                  AnaliticoDeCuentasDto dto) {
      string key = command.ToString();

      _cache.Insert(key, dto);
    }

    private AnaliticoDeCuentasDto TryReadAnaliticoDeCuentasFromCache(TrialBalanceCommand command) {
      string key = command.ToString();

      if (_cache.ContainsKey(key)) {
        return _cache[key];
      }

      return null;
    }

    #endregion Helpers

  } // class AnaliticoCuentasEntriesTests

} // namespace Empiria.FinancialAccounting.Tests.Balances.AnaliticoCuentas
