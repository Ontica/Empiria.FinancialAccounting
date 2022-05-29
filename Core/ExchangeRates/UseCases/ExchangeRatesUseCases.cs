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
      Assertion.Require(exchangeRateTypeUID, nameof(exchangeRateTypeUID));

      var exchangeRateType = ExchangeRateType.Parse(exchangeRateTypeUID);

      FixedList<ExchangeRate> exchangeRates = ExchangeRate.GetList(exchangeRateType, date);

      return ExchangeRatesMapper.Map(exchangeRates);
    }


    public ExchangeRateValuesDto GetExchangeRatesForEdition(string exchangeRateTypeUID, DateTime date) {
      Assertion.Require(exchangeRateTypeUID, nameof(exchangeRateTypeUID));

      var exchangeRateType = ExchangeRateType.Parse(exchangeRateTypeUID);

      FixedList<ExchangeRate> exchangeRates = ExchangeRate.GetForEditionList(exchangeRateType, date);

      return ExchangeRatesMapper.MapForEdition(exchangeRateType, date, exchangeRates);
    }


    public FixedList<ExchangeRateTypeDto> GetExchangeRatesTypes() {
      var list = ExchangeRateType.GetList();

      return ExchangeRatesMapper.Map(list);
    }


    public FixedList<ExchangeRateDescriptorDto> SearchExchangeRates(ExchangeRatesQuery query) {
      Assertion.Require(query, nameof(query));

      string filter = query.MapToFilterString();

      FixedList<ExchangeRate> exchangeRates = ExchangeRatesData.SearchExchangeRates(filter);

      return ExchangeRatesMapper.MapToExchangeRateDescriptor(exchangeRates);
    }


    public ExchangeRateValuesDto UpdateAllExchangeRates(ExchangeRateValuesDto exchangeRates) {
      Assertion.Require(exchangeRates, nameof(exchangeRates));

      exchangeRates.EnsureValid();

      var exchangeRateType = ExchangeRateType.Parse(exchangeRates.ExchangeRateTypeUID);

      var toUpdate = new List<ExchangeRate>();

      foreach (var value in exchangeRates.Values) {
        var currency = Currency.Parse(value.ToCurrencyUID);

        var exchangeRate = new ExchangeRate(exchangeRateType, exchangeRates.Date,
                                            currency, value.Value);

        toUpdate.Add(exchangeRate);
      }

      ExchangeRatesData.DeleteAllExchangeRates(exchangeRateType, exchangeRates.Date);

      toUpdate.ForEach(x => x.Save());

      return GetExchangeRatesForEdition(exchangeRates.ExchangeRateTypeUID, exchangeRates.Date);
    }

    #endregion Use cases

  }  // class ExchangeRatesUseCases

}  // namespace Empiria.FinancialAccounting.UseCases
