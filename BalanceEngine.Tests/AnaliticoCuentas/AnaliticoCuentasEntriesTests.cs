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

using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine;

namespace Empiria.FinancialAccounting.Tests.BalanceEngine.AnaliticoCuentas {

  /// <summary>Unit test cases for 'Analítico de cuentas' report entries.</summary>
  public class AnaliticoCuentasEntriesTests {

    #region Initialization

    public AnaliticoCuentasEntriesTests() {
      CommonMethods.Authenticate();
    }

    #endregion Initialization

    #region Theories


    [Theory]
    [InlineData(AnaliticoCuentasTestCommandCase.Default)]
    [InlineData(AnaliticoCuentasTestCommandCase.EnCascada)]
    public async Task AccountEntriesAreOk(AnaliticoCuentasTestCommandCase commandCase) {
      FixedList<AnaliticoDeCuentasEntryDto> expectedEntries = commandCase.GetExpectedEntries(TrialBalanceItemType.Entry);

      FixedList<AnaliticoDeCuentasEntryDto> sut = await GetFilteredAnaliticoCuentasEntries(commandCase,
                                                                                           TrialBalanceItemType.Entry);

      Assert.Equal(expectedEntries.Count, sut.Count);

      foreach (var sutEntry in sut) {
        var expected = expectedEntries.Find(x => x.Equals(sutEntry));

        Assert.True(expected != null, $"Expected entries do not have this AccountEntry: {sutEntry.ToJson()}");
      }
    }


    [Theory]
    [InlineData(AnaliticoCuentasTestCommandCase.Default)]
    [InlineData(AnaliticoCuentasTestCommandCase.EnCascada)]
    public async Task AccountSummaryEntriesAreOk(AnaliticoCuentasTestCommandCase commandCase) {

      FixedList<AnaliticoDeCuentasEntryDto> expectedEntries = commandCase.GetExpectedEntries(TrialBalanceItemType.Summary);

      FixedList<AnaliticoDeCuentasEntryDto> sut = await GetFilteredAnaliticoCuentasEntries(commandCase,
                                                                                           TrialBalanceItemType.Summary);

      Assert.Equal(expectedEntries.Count, sut.Count);

      foreach (var sutEntry in sut) {
        var expected = expectedEntries.Find(x => x.Equals(sutEntry));

        Assert.True(expected != null, $"Expected entries do not have this AccountSummary entry: {sutEntry.ToJson()}");
      }
    }


    [Theory]
    [InlineData(AnaliticoCuentasTestCommandCase.Default)]
    [InlineData(AnaliticoCuentasTestCommandCase.EnCascada)]
    public async Task GroupTotalEntriesAreOk(AnaliticoCuentasTestCommandCase commandCase) {

      // ToDo: No distinguir entre grupos deudores y acreedores. Usar el dato debtorCreditor
      // Check Debtor account groups
      FixedList<AnaliticoDeCuentasEntryDto> expectedEntries = commandCase.GetExpectedEntries(TrialBalanceItemType.BalanceTotalGroupDebtor);

      FixedList<AnaliticoDeCuentasEntryDto> sut = await GetFilteredAnaliticoCuentasEntries(commandCase,
                                                                                           TrialBalanceItemType.BalanceTotalGroupDebtor);

      Assert.Equal(expectedEntries.Count, sut.Count);

      foreach (var sutEntry in sut) {
        var expected = expectedEntries.Find(x => x.Equals(sutEntry));

        Assert.True(expected != null, $"Expected entries do not have this GroupTotalEntry (debtor) entry: {sutEntry.ToJson()}");
      }


      // Check Creditor account groups
      expectedEntries = commandCase.GetExpectedEntries(TrialBalanceItemType.BalanceTotalGroupCreditor);

      sut = await GetFilteredAnaliticoCuentasEntries(commandCase, TrialBalanceItemType.BalanceTotalGroupCreditor);

      Assert.Equal(expectedEntries.Count, sut.Count);

      foreach (var sutEntry in sut) {
        var expected = expectedEntries.Find(x => x.Equals(sutEntry));

        Assert.True(expected != null, $"Expected entries do not have this GroupTotalEntry entry: (creditor) {sutEntry.ToJson()}");
      }

    }

    [Theory]
    [InlineData(AnaliticoCuentasTestCommandCase.Default)]
    [InlineData(AnaliticoCuentasTestCommandCase.EnCascada)]
    public async Task LedgerDebtorCreditorTotalEntriesAreOk(AnaliticoCuentasTestCommandCase commandCase) {

      // ToDo: No distinguir entre totales deudores y acreedores por ledger. Usar el dato debtorCreditor

      // Check Debtor totals
      FixedList<AnaliticoDeCuentasEntryDto> expectedEntries = commandCase.GetExpectedEntries(TrialBalanceItemType.BalanceTotalDebtor);

      FixedList<AnaliticoDeCuentasEntryDto> sut = await GetFilteredAnaliticoCuentasEntries(commandCase,
                                                                                           TrialBalanceItemType.BalanceTotalDebtor);

      Assert.Equal(expectedEntries.Count, sut.Count);

      foreach (var sutEntry in sut) {
        var expected = expectedEntries.Find(x => x.Equals(sutEntry));

        Assert.True(expected != null, $"Expected entries do not have this  LedgerDebtorCreditorTotal (debtor) entry: {sutEntry.ToJson()}");
      }


      // Check Creditor totals
      expectedEntries = commandCase.GetExpectedEntries(TrialBalanceItemType.BalanceTotalCreditor);

      sut = await GetFilteredAnaliticoCuentasEntries(commandCase, TrialBalanceItemType.BalanceTotalCreditor);

      Assert.Equal(expectedEntries.Count, sut.Count);

      foreach (var sutEntry in sut) {
        var expected = expectedEntries.Find(x => x.Equals(sutEntry));

        Assert.True(expected != null, $"Expected entries do not have this  LedgerDebtorCreditorTotal entry: (creditor) {sutEntry.ToJson()}");
      }

    }



    [Theory]
    [InlineData(AnaliticoCuentasTestCommandCase.Default)]
    [InlineData(AnaliticoCuentasTestCommandCase.EnCascada)]
    public async Task LedgerTotalEntriesAreOk(AnaliticoCuentasTestCommandCase commandCase) {

      FixedList<AnaliticoDeCuentasEntryDto> expectedEntries = commandCase.GetExpectedEntries(TrialBalanceItemType.BalanceTotalConsolidated);

      FixedList<AnaliticoDeCuentasEntryDto> sut = await GetFilteredAnaliticoCuentasEntries(commandCase,
                                                                                           TrialBalanceItemType.BalanceTotalConsolidated);

      Assert.Equal(expectedEntries.Count, sut.Count);

      foreach (var sutEntry in sut) {
        var expected = expectedEntries.Find(x => x.Equals(sutEntry));

        Assert.True(expected != null, $"Expected entries do not have this LedgerTotal entry: {sutEntry.ToJson()}");
      }
    }


    #endregion Theories

    #region Helpers

    private async Task<FixedList<AnaliticoDeCuentasEntryDto>> GetFilteredAnaliticoCuentasEntries(AnaliticoCuentasTestCommandCase commandCase,
                                                                                                 TrialBalanceItemType entriesFilter) {
      TrialBalanceCommand command = commandCase.BuildCommand();

      AnaliticoDeCuentasDto dto = await BalanceEngineProxy.BuildAnaliticoDeCuentas(command)
                                                          .ConfigureAwait(false);


      return dto.Entries.FindAll(x => x.ItemType == entriesFilter);
    }

    #endregion Helpers

  } // class AnaliticoCuentasEntriesTests

} // namespace Empiria.FinancialAccounting.Tests.Balances.AnaliticoCuentas
