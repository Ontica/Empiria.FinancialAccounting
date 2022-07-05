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

    //TODO Theories for entries

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
