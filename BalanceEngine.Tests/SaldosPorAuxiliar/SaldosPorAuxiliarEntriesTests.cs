﻿/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test cases                              *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Unit tests                              *
*  Type     : SaldosPorAuxiliarEntriesTests              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Unit test cases for 'Saldos por auxiliar' report entries.                                      *
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

namespace Empiria.FinancialAccounting.Tests.BalanceEngine.SaldosPorAuxiliar {

  /// <summary>Unit test cases for 'Saldos por auxiliar' report entries.</summary>
  public class SaldosPorAuxiliarEntriesTests {

    private readonly EmpiriaHashTable<SaldosPorAuxiliarDto> _cache =
                                        new EmpiriaHashTable<SaldosPorAuxiliarDto>(16);

    #region Initialization

    public SaldosPorAuxiliarEntriesTests() {
      TestsCommonMethods.Authenticate();
    }

    #endregion Initialization

    #region Theories

    [Theory]
    [InlineData(SaldosPorAuxiliarTestCase.CatalogoAnterior)]
    [InlineData(SaldosPorAuxiliarTestCase.Default)]
    [InlineData(SaldosPorAuxiliarTestCase.Valorizados)]
    public async Task ContainsTheSameEntries_Than_TestData(SaldosPorAuxiliarTestCase testcase) {
      FixedList<SaldosPorAuxiliarEntryDto> expectedEntries = testcase.GetExpectedEntries();

      FixedList<SaldosPorAuxiliarEntryDto> sut = await GetSaldosPorAuxiliarEntries(testcase)
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

    private async Task<FixedList<SaldosPorAuxiliarEntryDto>> GetSaldosPorAuxiliarEntries(SaldosPorAuxiliarTestCase testcase) {
      TrialBalanceQuery query = testcase.GetInvocationQuery();

      SaldosPorAuxiliarDto dto = TryReadSaldosPorAuxiliarFromCache(query);

      if (dto != null) {
        return dto.Entries;
      }

      dto = await BalanceEngineProxy.BuildSaldosPorAuxiliar(query)
                                    .ConfigureAwait(false);

      StoreSaldosPorAuxiliarIntoCache(query, dto);

      return dto.Entries;
    }


    private void StoreSaldosPorAuxiliarIntoCache(TrialBalanceQuery query,
                                                  SaldosPorAuxiliarDto dto) {
      string key = query.ToString();

      _cache.Insert(key, dto);
    }


    private SaldosPorAuxiliarDto TryReadSaldosPorAuxiliarFromCache(TrialBalanceQuery query) {
      string key = query.ToString();

      if (_cache.ContainsKey(key)) {
        return _cache[key];
      }

      return null;
    }

    #endregion Helpers

  } // class SaldosPorAuxiliarEntriesTests

} // namespace Empiria.FinancialAccounting.Tests.BalanceEngine.SaldosPorAuxiliar
