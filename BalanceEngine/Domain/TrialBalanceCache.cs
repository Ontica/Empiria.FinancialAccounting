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


namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Provides a cache of trial balances.</summary>
  static public class TrialBalanceCache {

    static readonly EmpiriaHashTable<TrialBalance> _cache = new EmpiriaHashTable<TrialBalance>();
    static readonly EmpiriaHashTable<Balance> _cacheBalance = new EmpiriaHashTable<Balance>();


    static public void Invalidate(DateTime date) {
      InvalidateTrialBalanceCache(date);
      InvalidateBalancesCache(date);
    }


    #region Consulta de balanzas

    static internal string GenerateHash(TrialBalanceCommand command) {
      string json = JsonConverter.ToJson(command);

      var hashCode = Cryptographer.CreateHashCode(json);

      DateTime invalidationDate = GetInvalidationDate(command);

      return $"{hashCode}.{ToInvalidationDateString(invalidationDate)}";
    }


    static private DateTime GetInvalidationDate(TrialBalanceCommand command) {
      if (command.TrialBalanceType != TrialBalanceType.BalanzaValorizadaComparativa) {
        return command.InitialPeriod.ToDate;
      } else {
        return command.FinalPeriod.ToDate;
      }
    }


    static private void InvalidateTrialBalanceCache(DateTime date) {
      string dateKey = ToInvalidationDateString(date);

      var toInvalidateKeys = _cache.Keys.Where(key => key.EndsWith(dateKey))
                                        .ToArray();

      foreach (string key in toInvalidateKeys) {
        _cache.Remove(key);
      }
    }


    static internal void Store(string cacheHashKey, TrialBalance trialBalance) {
      _cache.Insert(cacheHashKey, trialBalance);
    }


    static internal TrialBalance TryGet(string cacheHashKey) {
      TrialBalance trialBalance;

      if (_cache.TryGetValue(cacheHashKey, out trialBalance)) {
        return trialBalance;
      }

      return null;
    }

    #endregion Consulta de balanzas


    #region Consulta rápida de saldos


    static internal string GenerateBalanceHash(BalanceCommand command) {
      string json = JsonConverter.ToJson(command);

      var hashCode = Cryptographer.CreateHashCode(json);

      DateTime invalidationDate = command.InitialPeriod.ToDate;

      return $"{hashCode}.{ToInvalidationDateString(invalidationDate)}";
    }


    static private void InvalidateBalancesCache(DateTime date) {
      string dateKey = ToInvalidationDateString(date);

      var toInvalidateKeys = _cacheBalance.Keys.Where(key => key.EndsWith(dateKey))
                                               .ToArray();

      foreach (string key in toInvalidateKeys) {
        _cacheBalance.Remove(key);
      }
    }


    static internal void StoreBalance(string cacheHashKey, Balance balance) {
      _cacheBalance.Insert(cacheHashKey, balance);
    }


    static internal Balance TryGetBalance(string cacheHashKey) {
      Balance balance;

      if (_cacheBalance.TryGetValue(cacheHashKey, out balance)) {
        return balance;
      }

      return null;
    }

    #endregion Consulta rápida de saldos

    static private string ToInvalidationDateString(DateTime invalidationDate) {
      return invalidationDate.ToString("yyyy-MM");
    }


  }  // class TrialBalanceCache

}  // namespace Empiria.FinancialAccounting.BalanceEngine
