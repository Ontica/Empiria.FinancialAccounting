/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Exchange Rates                             Component : Use cases Layer                         *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Use case interactor class               *
*  Type     : ExchangeRatesUseCases                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases for exchange rates.                                                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.Services;

using Empiria.FinancialAccounting.Adapters;
using Empiria.FinancialAccounting.Data;

namespace Empiria.FinancialAccounting.UseCases {

  /// <summary>Use cases for exchange rates.</summary>
  public class ExchangeRatesUseCases : UseCase {

    #region Constructors and parsers

    protected ExchangeRatesUseCases() {
      // no-op
    }

    static public ExchangeRatesUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<ExchangeRatesUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases

    public FixedList<ExchangeRateDto> GetExchangeRates(DateTime date) {
      FixedList<ExchangeRate> exchangeRates = ExchangeRate.GetList(date);

      return ExchangeRatesMapper.Map(exchangeRates);
    }


    public FixedList<ExchangeRateDto> GetExchangeRates(string exchangeRateTypeUID, DateTime date) {
      var exchangeRateType = ExchangeRateType.Parse(exchangeRateTypeUID);

      return GetExchangeRates(exchangeRateType, date);
    }


    public FixedList<ExchangeRateDto> GetExchangeRates(ExchangeRateType exchangeRateType, DateTime date) {
      FixedList<ExchangeRate> exchangeRates = ExchangeRate.GetList(exchangeRateType, date);

      return ExchangeRatesMapper.Map(exchangeRates);
    }


    public FixedList<NamedEntityDto> GetExchangeRatesTypes() {
      var list = ExchangeRateType.GetList();

      return list.MapToNamedEntityList();
    }


    public FixedList<ExchangeRateDto> UpdateAllExchangeRates(ExchangeRateFields fields) {
      Assertion.AssertObject(fields, "fields");

      fields.EnsureValid();

      var exchangeRateType = ExchangeRateType.Parse(fields.ExchangeRateTypeUID);

      var toUpdate = new List<ExchangeRate>();

      foreach (var value in fields.Values) {
        var currency = Currency.Parse(value.ToCurrencyUID);

        var exchangeRate = new ExchangeRate(exchangeRateType, fields.Date,
                                            currency, value.Value);

        toUpdate.Add(exchangeRate);
      }

      ExchangeRatesData.DeleteAllExchangeRates(exchangeRateType, fields.Date);

      toUpdate.ForEach(x => x.Save());

      return GetExchangeRates(exchangeRateType, fields.Date);
    }

    #endregion Use cases

  }  // class ExchangeRatesUseCases

}  // namespace Empiria.FinancialAccounting.UseCases
