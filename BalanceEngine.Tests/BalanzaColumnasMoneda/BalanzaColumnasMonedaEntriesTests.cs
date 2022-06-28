/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test cases                              *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Unit tests                              *
*  Type     : BalanzaColumnasMonedaEntriesTests          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Unit test cases for 'Balanza en columnas por moneda' report entries.                           *
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

namespace Empiria.FinancialAccounting.Tests.BalanceEngine.BalanzaColumnasMoneda {

  /// <summary>Unit test cases for 'Balanza en columnas por moneda' report entries.</summary>
  public class BalanzaColumnasMonedaEntriesTests {

    private readonly EmpiriaHashTable<BalanzaColumnasMonedaDto> _cache =
                                        new EmpiriaHashTable<BalanzaColumnasMonedaDto>(16);

    #region Initialization

    public BalanzaColumnasMonedaEntriesTests() {
      TestsCommonMethods.Authenticate();
    }

    #endregion Initialization

    #region Theories



    #endregion Theories

    #region Helpers

    private async Task<FixedList<BalanzaColumnasMonedaEntryDto>> GetBalanzaColumnasMonedaEntries(
                                                                 BalanzaColumnasMonedaTestCase testcase) {
      TrialBalanceQuery query = testcase.GetInvocationQuery();

      BalanzaColumnasMonedaDto dto = TryReadBalanzaColumnasMonedaFromCache(query);

      if (dto != null) {
        return dto.Entries;
      }

      dto = await BalanceEngineProxy.BuildBalanzaColumnasMoneda(query)
                                    .ConfigureAwait(false);

      StoreBalanzaTradicionalIntoCache(query, dto);

      return dto.Entries;
    }


    private void StoreBalanzaTradicionalIntoCache(TrialBalanceQuery query,
                                                  BalanzaColumnasMonedaDto dto) {
      string key = query.ToString();

      _cache.Insert(key, dto);
    }


    private BalanzaColumnasMonedaDto TryReadBalanzaColumnasMonedaFromCache(TrialBalanceQuery query) {
      string key = query.ToString();

      if (_cache.ContainsKey(key)) {
        return _cache[key];
      }

      return null;
    }

    #endregion Helpers

  } // class BalanzaColumnasMonedaEntriesTests

} // namespace Empiria.FinancialAccounting.Tests.BalanceEngine.BalanzaColumnasMoneda
