/* Empiria Financial *****************************************************************************************
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

    //TODO Theories for Balanza comparativa entries

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
