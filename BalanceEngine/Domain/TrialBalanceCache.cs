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
    static readonly EmpiriaHashTable<Balance> _cacheBalance = new EmpiriaHashTable<Balance>();

    #region Consulta de balanzas

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


    #endregion Consulta de balanzas


    #region Consulta rapida de saldos


    static internal string GenerateBalanceHash(BalanceCommand command) {
      string json = JsonConverter.ToJson(command);

      return Cryptographer.CreateHashCode(json);
    }


     static internal Balance TryGetBalance(string cacheHashKey) {
      if (_cacheBalance.ContainsKey(cacheHashKey)) {
        return _cacheBalance[cacheHashKey];
      }

      return null;
    }


    static internal void StoreBalance(string cacheHashKey, Balance balance) {
      _cacheBalance.Insert(cacheHashKey, balance);
    }


    #endregion Consulta rapida de saldos

  }  // class TrialBalanceCache

}  // namespace Empiria.FinancialAccounting.BalanceEngine
