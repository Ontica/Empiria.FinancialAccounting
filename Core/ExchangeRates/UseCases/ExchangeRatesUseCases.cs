/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Exchange Rates                             Component : Use cases Layer                         *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Use case interactor class               *
*  Type     : ExchangeRatesUseCases                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases for exchange rates management.                                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.FinancialAccounting.Adapters;

namespace Empiria.FinancialAccounting.UseCases {

  /// <summary>Use cases for accounts chart management.</summary>
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

    public FixedList<NamedEntityDto> GetExchangeRatesTypes() {
      var list = ExchangeRateType.GetList();

      return list.MapToNamedEntityList();
    }


    public FixedList<ExchangeRateDto> GetExchangeRatesOnADate(DateTime date) {
      FixedList<ExchangeRate> exchangeRates = ExchangeRate.GetList(date);

      return ExchangeRatesMapper.Map(exchangeRates);
    }


    #endregion Use cases

  }  // class ExchangeRatesUseCases

}  // namespace Empiria.FinancialAccounting.UseCases
