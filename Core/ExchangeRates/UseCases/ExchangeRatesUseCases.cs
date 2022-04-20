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

    public FixedList<ExchangeRateDto> AppendExchangeRate(ExchangeRateFields fields) {
      Assertion.AssertObject(fields, "fields");

      fields.EnsureValid();

      var exchangeRate = new ExchangeRate(fields);

      exchangeRate.Save();

      return GetExchangeRates(exchangeRate.ExchangeRateType.UID, exchangeRate.Date);
    }


    public FixedList<ExchangeRateDto> GetExchangeRates(string exchangeRateTypeUID, DateTime date) {
      var exchangeRateType = ExchangeRateType.Parse(exchangeRateTypeUID);

      FixedList<ExchangeRate> exchangeRates = ExchangeRate.GetList(exchangeRateType, date);

      return ExchangeRatesMapper.Map(exchangeRates);
    }


    public FixedList<NamedEntityDto> GetExchangeRatesTypes() {
      var list = ExchangeRateType.GetList();

      return list.MapToNamedEntityList();
    }


    public FixedList<ExchangeRateDto> RemoveExchangeRate(int exchangeRateId) {
      var exchangeRate = ExchangeRate.Parse(exchangeRateId);

      ExchangeRatesData.DeleteExchangeRate(exchangeRate);

      return GetExchangeRates(exchangeRate.ExchangeRateType.UID, exchangeRate.Date);
    }


    public FixedList<ExchangeRateDto> UpdateExchangeRate(int exchangeRateId, ExchangeRateFields fields) {
      Assertion.AssertObject(fields, "fields");

      var exchangeRate = ExchangeRate.Parse(exchangeRateId);

      exchangeRate.Update(fields);

      exchangeRate.Save();

      return GetExchangeRates(exchangeRate.ExchangeRateType.UID, exchangeRate.Date);
    }

    #endregion Use cases

  }  // class ExchangeRatesUseCases

}  // namespace Empiria.FinancialAccounting.UseCases
