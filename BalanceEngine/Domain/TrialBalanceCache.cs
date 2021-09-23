/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Cache                                   *
*  Type     : TrialBalanceCache                          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides a cache of trial balances.                                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Collections;
using Empiria.Json;
using Empiria.Security;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Provides a cache of trial balances.</summary>
  static internal class TrialBalanceCache {

    static readonly EmpiriaHashTable<TrialBalance> _cache = new EmpiriaHashTable<TrialBalance>();

    static internal string GenerateHash(TrialBalanceCommand command) {
      string json = JsonConverter.ToJson(command);

      return Cryptographer.CreateHashCode(json);
    }


    static internal TrialBalance TryGet(string cacheHashKey) {
      if (_cache.ContainsKey(cacheHashKey)) {
        return _cache[cacheHashKey];
      }

      return null;
    }


    static internal void Store(string cacheHashKey, TrialBalance trialBalance) {
      _cache.Insert(cacheHashKey, trialBalance);
    }

  }  // class TrialBalanceCache

}  // namespace Empiria.FinancialAccounting.BalanceEngine
