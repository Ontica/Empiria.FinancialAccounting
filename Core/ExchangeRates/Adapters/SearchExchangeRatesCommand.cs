/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Exchange Rates                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Data Transfer Object                    *
*  Type     : SearchExchangeRatesCommand                 License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Input DTO used to search exchange rates.                                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Linq;

namespace Empiria.FinancialAccounting.Adapters {

  /// <summary>Input DTO used to search exchange rates.</summary>
  public class SearchExchangeRatesCommand {

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


  }  // public class SearchExchangeRatesCommand



  public static class SearchExchangeRatesCommandExtensions {

    static internal string MapToFilterString(this SearchExchangeRatesCommand command) {
      string periodFilter = BuildPeriodFilter(command);
      string exchangeRatesTypesFilter = BuildExchangeRatesTypesFilter(command);
      string currenciesFilter = BuildCurrenciesFilter(command);


      var filter = new Filter(periodFilter);

      filter.AppendAnd(exchangeRatesTypesFilter);
      filter.AppendAnd(currenciesFilter);

      return filter.ToString();
    }


    private static string BuildCurrenciesFilter(SearchExchangeRatesCommand command) {
      if (command.Currencies.Length == 0) {
        return string.Empty;
      }

      int[] arrayOfIds = command.Currencies.Select(uid => Currency.Parse(uid).Id)
                                           .ToArray();

      return SearchExpression.ParseInSet("TO_CURRENCY_ID", arrayOfIds);
    }


    private static string BuildExchangeRatesTypesFilter(SearchExchangeRatesCommand command) {
      if (command.ExchangeRateTypes.Length == 0) {
        return string.Empty;
      }

      int[] arrayOfIds = command.ExchangeRateTypes.Select(uid => ExchangeRateType.Parse(uid).Id)
                                                  .ToArray();

      return SearchExpression.ParseInSet("EXCHANGE_RATE_TYPE_ID", arrayOfIds);
    }


    private static string BuildPeriodFilter(SearchExchangeRatesCommand command) {
      return SearchExpression.ParseBetweenValues("FROM_DATE",
                                                 CommonMethods.FormatSqlDate(command.FromDate),
                                                 CommonMethods.FormatSqlDate(command.ToDate));
    }


  }  // class SearchExchangeRatesCommandExtensions


}  // namespace Empiria.FinancialAccounting.Adapters
