/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Exchange Rates                               Component : Web Api                               *
*  Assembly : Empiria.FinancialAccounting.WebApi.dll       Pattern   : Query Controller                      *
*  Type     : ExchangeRatesController                      License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Query web API used to get information about exchange rates.                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Web.Http;

using Empiria.WebApi;

using Empiria.FinancialAccounting.UseCases;
using Empiria.FinancialAccounting.Adapters;

namespace Empiria.FinancialAccounting.WebApi {

  /// <summary>Query web API used to get information about exchange rates.</summary>
  public class ExchangeRatesController : WebApiController {

    #region Web Apis


    [HttpGet]
    [Route("v2/financial-accounting/exchange-rates/exchange-rates-types")]
    public CollectionModel GetExchangeRatesTypes() {

      using (var usecases = ExchangeRatesUseCases.UseCaseInteractor()) {
        FixedList<NamedEntityDto> exchangeRatesTypes = usecases.GetExchangeRatesTypes();

        return new CollectionModel(base.Request, exchangeRatesTypes);
      }
    }



    [HttpGet]
    [Route("v2/financial-accounting/exchange-rates")]
    public CollectionModel GetExchangeRatesOnADate([FromUri] DateTime date) {

      using (var usecases = ExchangeRatesUseCases.UseCaseInteractor()) {
        FixedList<ExchangeRateDto> exchangeRates = usecases.GetExchangeRatesOnADate(date);

        return new CollectionModel(base.Request, exchangeRates);
      }
    }

    #endregion Web Apis

  }  // class ExchangeRatesController

}  // namespace Empiria.FinancialAccounting.WebApi
