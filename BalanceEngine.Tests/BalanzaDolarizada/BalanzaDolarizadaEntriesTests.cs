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

    //TODO Theories for entries

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
