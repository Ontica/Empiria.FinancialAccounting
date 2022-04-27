/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Exchange Rates                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Empiria Data Object                     *
*  Type     : ExchangeRate                               License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Holds data about an exchange rate between currencies.                                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.Adapters;
using Empiria.FinancialAccounting.Data;

namespace Empiria.FinancialAccounting {

  /// <summary>Holds data about an exchange rate between currencies.</summary>
  public class ExchangeRate : BaseObject {

    #region Constructors and parsers

    protected ExchangeRate() {
      // Required by Empiria Framework.
    }


    internal ExchangeRate(ExchangeRateType exchangeRateType, DateTime date,
                          Currency currency, decimal value) {
      Assertion.AssertObject(exchangeRateType, "exchangeRateType");
      Assertion.AssertObject(currency, "currency");
      Assertion.Assert(exchangeRateType.HasCurrency(currency),
          $"Currency {currency.FullName} is not defined for exchange rate type {exchangeRateType.Name}");
      Assertion.Assert(exchangeRateType.EditionValidOn(date),
          $"Date {date.ToShortDateString()} is not in exchange rate type's valid edition date period.");
      Assertion.Assert(value > 0, "Exchange rate value must be greater than zero.");

      this.ExchangeRateType = exchangeRateType;
      this.FromCurrency = Currency.MXN;
      this.ToCurrency = currency;
      this.Date = date;
      this.Value = Math.Round(value, 6);
    }


    static public ExchangeRate Parse(int id) {
      return BaseObject.ParseId<ExchangeRate>(id);
    }


    static public FixedList<ExchangeRate> GetList(DateTime date) {
      return ExchangeRatesData.GetExchangeRates(date);
    }


    static public FixedList<ExchangeRate> GetList(ExchangeRateType exchangeRateType, DateTime date) {
      return ExchangeRatesData.GetExchangeRates(exchangeRateType, date);
    }


    #endregion Constructors and parsers

    #region Properties


    [DataField("EXCHANGE_RATE_TYPE_ID", ConvertFrom = typeof(long))]
    public ExchangeRateType ExchangeRateType {
      get; private set;
    }


    [DataField("FROM_CURRENCY_ID", ConvertFrom = typeof(long))]
    public Currency FromCurrency {
      get; private set;
    }


    [DataField("TO_CURRENCY_ID", ConvertFrom = typeof(long))]
    public Currency ToCurrency {
      get; private set;
    }


    [DataField("FROM_DATE")]
    public DateTime Date {
      get; private set;
    }


    [DataField("EXCHANGE_RATE")]
    public decimal Value {
      get; private set;
    }

    #endregion Properties

    #region Methods


    protected override void OnBeforeSave() {
      Assertion.Assert(this.IsNew, "El método Save() sólo puede invocarse sobre tipos de cambio nuevos.");

      Assertion.Assert(!ExchangeRatesData.ExistsExchangeRate(this),
                 "Ya tengo registrado un tipo de cambio en esa fecha para las mismas monedas.");
    }


    protected override void OnSave() {
      ExchangeRatesData.AppendExchangeRate(this);
    }


    #endregion Methods

  }  // class ExchangeRate

}  // namespace Empiria.FinancialAccounting
