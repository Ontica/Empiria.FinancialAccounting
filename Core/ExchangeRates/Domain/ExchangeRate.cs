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

using Empiria.FinancialAccounting.Data;

namespace Empiria.FinancialAccounting {

  /// <summary>Holds data about an exchange rate between currencies.</summary>
  public class ExchangeRate : BaseObject {

    #region Constructors and parsers

    protected ExchangeRate() {
      // Required by Empiria Framework.
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

    #region Public properties


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


    #endregion Public properties

  }  // class ExchangeRate

}  // namespace Empiria.FinancialAccounting
