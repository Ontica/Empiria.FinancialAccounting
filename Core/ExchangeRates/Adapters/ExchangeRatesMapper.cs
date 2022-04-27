/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Exchange Rates                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Mapper class                            *
*  Type     : ExchangeRatesMapper                        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Mapping methods for exchange rates.                                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

namespace Empiria.FinancialAccounting.Adapters {

  /// <summary>Mapping methods for exchange rates.</summary>
  static internal class ExchangeRatesMapper {

    static internal FixedList<ExchangeRateDto> Map(FixedList<ExchangeRate> list) {
      return new FixedList<ExchangeRateDto>(list.Select((x) => Map(x)));
    }


    static internal FixedList<ExchangeRateTypeDto> Map(FixedList<ExchangeRateType> list) {
      return new FixedList<ExchangeRateTypeDto>(list.Select((x) => Map(x)));
    }


    static internal ExchangeRateFields MapForEdition(ExchangeRateType exchangeRateType,
                                                      DateTime date,
                                                      FixedList<ExchangeRate> exchangeRates) {
      return new ExchangeRateFields {
        ExchangeRateTypeUID = exchangeRateType.UID,
        Date = date,
        Values = MapValuesForEdition(exchangeRates)
      };
    }


    static private ExchangeRateFieldValue[] MapValuesForEdition(FixedList<ExchangeRate> exchangeRates) {
      var list = new List<ExchangeRateFieldValue>(exchangeRates.Count);

      foreach (var exchangeRate in exchangeRates) {
        var value = new ExchangeRateFieldValue {
           ToCurrencyUID = exchangeRate.ToCurrency.UID,
           ToCurrency = exchangeRate.ToCurrency.FullName,
           Value = exchangeRate.Value
        };
        list.Add(value);
      }

      return list.ToArray();
    }


    static internal ExchangeRateDto Map(ExchangeRate exchangeRate) {
      return new ExchangeRateDto {
        Id = exchangeRate.Id,
        ExchangeRateType = exchangeRate.ExchangeRateType.MapToNamedEntity(),
        Date = exchangeRate.Date,
        FromCurrency = exchangeRate.FromCurrency.MapToNamedEntity(),
        ToCurrency = exchangeRate.ToCurrency.MapToNamedEntity(),
        Value = exchangeRate.Value
      };
    }


    static private ExchangeRateTypeDto Map(ExchangeRateType exchangeRateType) {
      return new ExchangeRateTypeDto {
        UID = exchangeRateType.UID,
        Name = exchangeRateType.Name,
        Currencies = exchangeRateType.Currencies.MapToNamedEntityList()
      };
    }


  }  // class ExchangeRatesMapper

}  // namespace Empiria.FinancialAccounting.Adapters
