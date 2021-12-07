/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Exchange Rates                             Component : Data Access Layer                       *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Data Service with Cache                 *
*  Type     : ExchangeRatesData                          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Data access layer for exchange rates.                                                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Collections;
using Empiria.Data;

namespace Empiria.FinancialAccounting.Data {

  /// <summary>Data access layer for exchange rates.</summary>
  static internal class ExchangeRatesData {

    static readonly EmpiriaHashTable<FixedList<ExchangeRate>> _cache =
                                                  new EmpiriaHashTable<FixedList<ExchangeRate>>();


    static internal FixedList<ExchangeRate> GetExchangeRates(ExchangeRateType exchangeRateType,
                                                             DateTime date) {
      string hashKey = GetHashKey(exchangeRateType, date);

      if (_cache.ContainsKey(hashKey)) {
        return _cache[hashKey];
      }

      var sql = "SELECT * FROM AO_EXCHANGE_RATES " +
                $"WHERE EXCHANGE_RATE_TYPE_ID = {exchangeRateType.Id} " +
                $"AND FROM_DATE = '{CommonMethods.FormatSqlDate(date)}' " +
                $"ORDER BY TO_CURRENCY_ID";

      var dataOperation = DataOperation.Parse(sql);

      var list = DataReader.GetFixedList<ExchangeRate>(dataOperation);

      _cache.Insert(hashKey, list);

      return _cache[hashKey];
    }

    static internal FixedList<ExchangeRate> GetExchangeRates(DateTime date) {
      var sql = "SELECT * FROM AO_EXCHANGE_RATES " +
                $"WHERE FROM_DATE = '{CommonMethods.FormatSqlDate(date)}' " +
                $"ORDER BY EXCHANGE_RATE_TYPE_ID, TO_CURRENCY_ID";

      var dataOperation = DataOperation.Parse(sql);

      return DataReader.GetFixedList<ExchangeRate>(dataOperation);
    }


    static private string GetHashKey(ExchangeRateType exchangeRateType, DateTime date) {
      return $"{exchangeRateType.UID}||{date.ToString("dd/MM/yyyy")}";
    }

  }  // class ExchangeRatesData

}  // namespace Empiria.FinancialAccounting.Data
