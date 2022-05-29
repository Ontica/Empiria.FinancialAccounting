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

      FixedList<ExchangeRate> exchangeRates = ExchangeRate.GetList(exchangeRateType, date);

      return ExchangeRatesMapper.Map(exchangeRates);
    }


    public FixedList<ExchangeRateDescriptorDto> GetExchangeRates(SearchExchangeRatesCommand command) {
      Assertion.Require(command, "command");

      FixedList<ExchangeRate> exchangeRates = ExchangeRatesData.SearchExchangeRates(command);

      return ExchangeRatesMapper.MapToExchangeRateDescriptor(exchangeRates);
    }


    public ExchangeRateValuesDto GetExchangeRatesForEdition(ExchangeRateValuesDto fields) {
      Assertion.Require(fields, "fields");

      var exchangeRateType = ExchangeRateType.Parse(fields.ExchangeRateTypeUID);

      FixedList<ExchangeRate> exchangeRates = ExchangeRate.GetForEditionList(exchangeRateType, fields.Date);

      return ExchangeRatesMapper.MapForEdition(exchangeRateType, fields.Date, exchangeRates);
    }


    public FixedList<ExchangeRateTypeDto> GetExchangeRatesTypes() {
      var list = ExchangeRateType.GetList();

      return ExchangeRatesMapper.Map(list);
    }


    public ExchangeRateValuesDto UpdateAllExchangeRates(ExchangeRateValuesDto exchangeRates) {
      Assertion.Require(exchangeRates, "exchangeRates");

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

      return GetExchangeRatesForEdition(exchangeRates);
    }

    #endregion Use cases

  }  // class ExchangeRatesUseCases

}  // namespace Empiria.FinancialAccounting.UseCases
