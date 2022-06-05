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
using System.Linq;

using Empiria.Collections;
using Empiria.Json;
using Empiria.Security;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.BalanceExplorer.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.BalanceExplorer;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Provides a cache of trial balances.</summary>
  static public class TrialBalanceCache {

    static readonly EmpiriaHashTable<TrialBalance> _trialBalancesCache = new EmpiriaHashTable<TrialBalance>();
    static readonly EmpiriaHashTable<BalanceExplorerResult>     _balancesCache = new EmpiriaHashTable<BalanceExplorerResult>();


    static public void Invalidate(DateTime date) {
      InvalidateTrialBalanceCache(date);
      InvalidateBalancesCache(date);
    }


    #region Consulta de balanzas

    static internal string GenerateHash(TrialBalanceQuery query) {
      int hashCode = query.GetHashCode();

      DateTime invalidationDate = GetInvalidationDate(query);

      return $"{hashCode}.{ToInvalidationDateString(invalidationDate)}";
    }


    static private DateTime GetInvalidationDate(TrialBalanceQuery query) {
      if (query.TrialBalanceType == TrialBalanceType.BalanzaValorizadaComparativa) {
        return query.FinalPeriod.ToDate;
      }
      return query.InitialPeriod.ToDate;
    }


    static private void InvalidateTrialBalanceCache(DateTime date) {
      string dateKey = ToInvalidationDateString(date);

      var toInvalidateKeys = _trialBalancesCache.Keys
                                                .Where(key => key.EndsWith(dateKey))
                                                .ToArray();

      foreach (string key in toInvalidateKeys) {
        _trialBalancesCache.Remove(key);
      }
    }


    static internal void StoreTrialBalance(string cacheHashKey, TrialBalance trialBalance) {
      _trialBalancesCache.Insert(cacheHashKey, trialBalance);
    }


    static internal TrialBalance TryGetTrialBalance(string cacheHashKey) {
      TrialBalance trialBalance;

      if (_trialBalancesCache.TryGetValue(cacheHashKey, out trialBalance)) {
        return trialBalance;
      }

      return null;
    }

    #endregion Consulta de balanzas


    #region Consulta rápida de saldos


    static internal string GenerateBalanceHash(BalanceExplorerQuery query) {
      string json = JsonConverter.ToJson(query);

      var hashCode = Cryptographer.CreateHashCode(json);

      DateTime invalidationDate = query.InitialPeriod.ToDate;

      return $"{hashCode}.{ToInvalidationDateString(invalidationDate)}";
    }


    static private void InvalidateBalancesCache(DateTime date) {
      string dateKey = ToInvalidationDateString(date);

      var toInvalidateKeys = _balancesCache.Keys.Where(key => key.EndsWith(dateKey))
                                               .ToArray();

      foreach (string key in toInvalidateKeys) {
        _balancesCache.Remove(key);
      }
    }


    static internal void StoreBalances(string cacheHashKey, BalanceExplorerResult balances) {
      _balancesCache.Insert(cacheHashKey, balances);
    }


    static internal BalanceExplorerResult TryGetBalances(string cacheHashKey) {
      BalanceExplorerResult balances;

      if (_balancesCache.TryGetValue(cacheHashKey, out balances)) {
        return balances;
      }

      return null;
    }

    #endregion Consulta rápida de saldos

    static private string ToInvalidationDateString(DateTime invalidationDate) {
      return invalidationDate.ToString("yyyy-MM");
    }


  }  // class TrialBalanceCache

}  // namespace Empiria.FinancialAccounting.BalanceEngine
