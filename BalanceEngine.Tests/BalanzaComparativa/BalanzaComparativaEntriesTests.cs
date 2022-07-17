﻿/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test cases                              *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Unit tests                              *
*  Type     : BalanzaComparativaEntriesTests             License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Unit test cases for 'Balanza comparativa' report entries.                                      *
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

namespace Empiria.FinancialAccounting.Tests.BalanceEngine.BalanzaComparativa {

  /// <summary>Unit test cases for 'Balanza comparativa' report entries.</summary>
  public class BalanzaComparativaEntriesTests {

    private readonly EmpiriaHashTable<BalanzaComparativaDto> _cache =
                                        new EmpiriaHashTable<BalanzaComparativaDto>(16);

    #region Initialization

    public BalanzaComparativaEntriesTests() {
      TestsCommonMethods.Authenticate();
    }

    #endregion Initialization

    #region Theories

    [Theory]
    [InlineData(BalanzaComparativaTestCase.CatalogoAnterior)]
    [InlineData(BalanzaComparativaTestCase.Default)]
    [InlineData(BalanzaComparativaTestCase.EnCascada)]
    public async Task ContainsTheSameEntries_Than_TestData(BalanzaComparativaTestCase testcase) {
      FixedList<BalanzaComparativaEntryDto> expectedEntries = testcase.GetExpectedEntries();

      FixedList<BalanzaComparativaEntryDto> sut = await GetBalanzaComparativaEntries(testcase)
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


    #endregion Theories

    #region Helpers

    private async Task<FixedList<BalanzaComparativaEntryDto>> GetBalanzaComparativaEntries(BalanzaComparativaTestCase testcase) {
      TrialBalanceQuery query = testcase.GetInvocationQuery();

      BalanzaComparativaDto dto = TryReadBalanzaComparativaFromCache(query);

      if (dto != null) {
        return dto.Entries;
      }

      dto = await BalanceEngineProxy.BuildBalanzaComparativa(query)
                                    .ConfigureAwait(false);

      StoreBalanzaComparativaIntoCache(query, dto);

      return dto.Entries;
    }


    private void StoreBalanzaComparativaIntoCache(TrialBalanceQuery query,
                                                  BalanzaComparativaDto dto) {
      string key = query.ToString();

      _cache.Insert(key, dto);
    }


    private BalanzaComparativaDto TryReadBalanzaComparativaFromCache(TrialBalanceQuery query) {
      string key = query.ToString();

      if (_cache.ContainsKey(key)) {
        return _cache[key];
      }

      return null;
    }

    #endregion Helpers

  } // class BalanzaComparativaEntriesTests

} // namespace Empiria.FinancialAccounting.Tests.BalanceEngine.BalanzaComparativa
