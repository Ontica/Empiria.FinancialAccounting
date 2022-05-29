/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Exchange Rates                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Query payload                           *
*  Type     : ExchangeRatesQuery                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Query payload used to search exchange rates.                                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Linq;

namespace Empiria.FinancialAccounting.Adapters {

  /// <summary>Query payload used to search exchange rates.</summary>
  public class ExchangeRatesQuery {

    public string[] ExchangeRateTypes {
      get; set;
    } = new string[0];


    public string[] Currencies {
      get; set;
    } = new string[0];


    public DateTime FromDate {
      get; set;
    } = DateTime.Today;


    public DateTime ToDate {
      get; set;
    } = DateTime.Today;


  }  // public class ExchangeRatesQuery


  public static class ExchangeRatesQueryExtensions {

    static internal string MapToFilterString(this ExchangeRatesQuery query) {
      string periodFilter             = BuildPeriodFilter(query);
      string exchangeRatesTypesFilter = BuildExchangeRatesTypesFilter(query);
      string currenciesFilter         = BuildCurrenciesFilter(query);


      var filter = new Filter(periodFilter);

      filter.AppendAnd(exchangeRatesTypesFilter);
      filter.AppendAnd(currenciesFilter);

      return filter.ToString();
    }


    static private string BuildCurrenciesFilter(ExchangeRatesQuery query) {
      if (query.Currencies.Length == 0) {
        return string.Empty;
      }

      int[] arrayOfIds = query.Currencies.Select(uid => Currency.Parse(uid).Id)
                                         .ToArray();

      return SearchExpression.ParseInSet("TO_CURRENCY_ID", arrayOfIds);
    }


    static private string BuildExchangeRatesTypesFilter(ExchangeRatesQuery query) {
      if (query.ExchangeRateTypes.Length == 0) {
        return string.Empty;
      }

      int[] arrayOfIds = query.ExchangeRateTypes.Select(uid => ExchangeRateType.Parse(uid).Id)
                                                .ToArray();

      return SearchExpression.ParseInSet("EXCHANGE_RATE_TYPE_ID", arrayOfIds);
    }


    static private string BuildPeriodFilter(ExchangeRatesQuery query) {
      return $"({CommonMethods.FormatSqlDbDate(query.FromDate)} <= FROM_DATE AND " +
             $"FROM_DATE <= {CommonMethods.FormatSqlDbDate(query.ToDate)})";
    }


  }  // class ExchangeRatesQueryExtensions

}  // namespace Empiria.FinancialAccounting.Adapters
