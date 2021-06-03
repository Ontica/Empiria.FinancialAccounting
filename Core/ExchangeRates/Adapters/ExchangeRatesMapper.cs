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

namespace Empiria.FinancialAccounting.Adapters {

  /// <summary>Mapping methods for exchange rates.</summary>
  static internal class ExchangeRatesMapper {

    static internal FixedList<ExchangeRateDto> Map(FixedList<ExchangeRate> list) {
      return new FixedList<ExchangeRateDto>(list.Select((x) => Map(x)));
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

  }  // class ExchangeRatesMapper

}  // namespace Empiria.FinancialAccounting.Adapters
