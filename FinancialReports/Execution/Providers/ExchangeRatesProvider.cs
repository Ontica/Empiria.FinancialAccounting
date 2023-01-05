/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Providers                               *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Service provider                        *
*  Type     : ExchangeRatesProvider                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides currency exchange rates.                                                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.FinancialReports.Providers {

  /// <summary>Provides currency exchange rates.</summary>
  internal class ExchangeRatesProvider {

    private readonly FixedList<ExchangeRate> _exchangeRates;

    public ExchangeRatesProvider(DateTime date) {
      _exchangeRates = ExchangeRate.GetList(date);
    }


    internal decimal Convert(ExchangeRateType exchangeRateType,
                             Currency toCurrency, decimal fromValue,
                             int decimals) {
      var exchangeRate = GetExchangeRate(exchangeRateType, toCurrency);

      return Math.Round(exchangeRate * fromValue, decimals);
    }


    internal decimal Convert_EUR_To_MXN(decimal euros, int decimals) {
      return Convert(ExchangeRateType.ValorizacionBanxico, Currency.EUR, euros, decimals);
    }


    internal decimal Convert_EUR_To_USD(decimal euros, int decimals) {
      return Convert(ExchangeRateType.Dolarizacion, Currency.EUR, euros, decimals);
    }


    internal decimal Convert_UDI_To_MXN(decimal udis, int decimals) {
      return Convert(ExchangeRateType.ValorizacionBanxico, Currency.UDI, udis, decimals);
    }


    internal decimal Convert_USD_To_MXN(decimal dollars, int decimals) {
      return Convert(ExchangeRateType.ValorizacionBanxico, Currency.USD, dollars, decimals);
    }


    internal decimal Convert_YEN_To_MXN(decimal yenes, int decimals) {
      return Convert(ExchangeRateType.ValorizacionBanxico, Currency.YEN, yenes, decimals);
    }


    internal decimal Convert_YEN_To_USD(decimal yenes, int decimals) {
      return Convert(ExchangeRateType.Dolarizacion, Currency.YEN, yenes, decimals);
    }


    internal decimal EUR_To_USD() {
      return GetExchangeRate(ExchangeRateType.Dolarizacion, Currency.EUR);
    }


    internal decimal YEN_To_USD() {
      return GetExchangeRate(ExchangeRateType.Dolarizacion, Currency.YEN);
    }


    internal decimal GetExchangeRate(ExchangeRateType exchangeRateType,
                                     Currency toCurrency) {

      ExchangeRate value = _exchangeRates.Find(x => x.ExchangeRateType.Equals(exchangeRateType) &&
                                                    x.ToCurrency.Equals(toCurrency));

      Assertion.Require(value, $"No se ha dado de alta el tipo de cambio {exchangeRateType.Name} " +
                               $"para la moneda {toCurrency.FullName}.");

      return value.Value;
    }

  }  // class ExchangeRatesProvider

}  // namespace Empiria.FinancialAccounting.FinancialReports.Providers
