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
    public async Task BaseEntriesAreOk(AnaliticoCuentasTestCommandCase commandCase) {
      FixedList<AnaliticoDeCuentasEntryDto> expectedEntries = commandCase.GetExpectedEntries(TrialBalanceItemType.Entry);

      FixedList<AnaliticoDeCuentasEntryDto> sut = await GetFilteredAnaliticoCuentasEntries(commandCase,
                                                                                           TrialBalanceItemType.Entry);

      Assert.Equal(expectedEntries.Count, sut.Count);

      foreach (var sutEntry in sut) {
        var expected = expectedEntries.Find(x => x.Equals(sutEntry));

        Assert.True(expected != null, $"Expected entries do not have this base sut entry: {sutEntry.ToJson()}");
      }
    }


    [Theory]
    [InlineData(AnaliticoCuentasTestCommandCase.Default)]
    [InlineData(AnaliticoCuentasTestCommandCase.EnCascada)]
    public async Task SummaryEntriesAreOk(AnaliticoCuentasTestCommandCase commandCase) {

      FixedList<AnaliticoDeCuentasEntryDto> expectedEntries = commandCase.GetExpectedEntries(TrialBalanceItemType.Summary);

      FixedList<AnaliticoDeCuentasEntryDto> sut = await GetFilteredAnaliticoCuentasEntries(commandCase,
                                                                                           TrialBalanceItemType.Summary);

      Assert.Equal(expectedEntries.Count, sut.Count);

      foreach (var sutEntry in sut) {
        var expected = expectedEntries.Find(x => x.Equals(sutEntry));

        Assert.True(expected != null, $"Expected entries do not have this summary sut entry: {sutEntry.ToJson()}");
      }
    }


    [Theory]
    [InlineData(AnaliticoCuentasTestCommandCase.Default)]
    [InlineData(AnaliticoCuentasTestCommandCase.EnCascada)]
    public async Task BalanceTotalConsolidatedEntryIsOk(AnaliticoCuentasTestCommandCase commandCase) {

      FixedList<AnaliticoDeCuentasEntryDto> expectedEntries = commandCase.GetExpectedEntries(TrialBalanceItemType.BalanceTotalConsolidated);

      FixedList<AnaliticoDeCuentasEntryDto> sut = await GetFilteredAnaliticoCuentasEntries(commandCase,
                                                                                           TrialBalanceItemType.BalanceTotalConsolidated);

      Assert.Single(sut);

      var expected = expectedEntries.Find(x => x.Equals(sut[0]));

      Assert.True(expected != null, $"Expected entries do not have this sut balance total consolidated entry: {sut[0].ToJson()}");
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
