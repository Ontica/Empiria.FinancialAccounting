/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Exchange Rates                               Component : Web Api                               *
*  Assembly : Empiria.FinancialAccounting.WebApi.dll       Pattern   : Controller                            *
*  Type     : ExchangeRatesController                      License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Web apis used to get and update information about exchange rates.                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Web.Http;

using Empiria.WebApi;

using Empiria.FinancialAccounting.UseCases;
using Empiria.FinancialAccounting.Adapters;

namespace Empiria.FinancialAccounting.WebApi {

  /// <summary>Web apis used to get and update information about exchange rates.</summary>
  public class ExchangeRatesController : WebApiController {

    #region Web Api

    [HttpGet, AllowAnonymous]
    [Route("v2/financial-accounting/exchange-rates")]
    public CollectionModel GetAllExchangeRates([FromUri] DateTime date) {

      using (var usecases = ExchangeRatesUseCases.UseCaseInteractor()) {
        FixedList<ExchangeRateDto> exchangeRates = usecases.GetExchangeRates(date);

        return new CollectionModel(base.Request, exchangeRates);
      }
    }


    [HttpGet, AllowAnonymous]
    [Route("v2/financial-accounting/exchange-rates")]
    public CollectionModel GetExchangeRates([FromUri] string exchangeRateTypeUID,
                                            [FromUri] DateTime date) {

      using (var usecases = ExchangeRatesUseCases.UseCaseInteractor()) {
        FixedList<ExchangeRateDto> exchangeRates = usecases.GetExchangeRates(exchangeRateTypeUID, date);

        return new CollectionModel(base.Request, exchangeRates);
      }
    }


    [HttpGet]
    [Route("v2/financial-accounting/exchange-rates")]
    public SingleObjectModel GetExchangeRatesForEdition([FromUri] string exchangeRateTypeUID,
                                                        [FromUri] DateTime date,
                                                        [FromUri] bool forEdition) {

      Assertion.Assert(forEdition, "forEdition query string parameter must be true.");

      using (var usecases = ExchangeRatesUseCases.UseCaseInteractor()) {
        ExchangeRateFields exchangeRatesForEdition = usecases.GetExchangeRatesForEdition(exchangeRateTypeUID, date);

        return new SingleObjectModel(base.Request, exchangeRatesForEdition);
      }
    }


    [HttpGet]
    [Route("v2/financial-accounting/exchange-rates/exchange-rates-types")]
    public CollectionModel GetExchangeRatesTypes() {

      using (var usecases = ExchangeRatesUseCases.UseCaseInteractor()) {
        FixedList<ExchangeRateTypeDto> exchangeRatesTypes = usecases.GetExchangeRatesTypes();

        return new CollectionModel(base.Request, exchangeRatesTypes);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/exchange-rates")]
    public SingleObjectModel UpdateAllExchangeRates([FromBody] ExchangeRateFields fields) {

      RequireBody(fields);

      using (var usecases = ExchangeRatesUseCases.UseCaseInteractor()) {
        ExchangeRateFields exchangeRatesforEdition = usecases.UpdateAllExchangeRates(fields);

        return new SingleObjectModel(base.Request, exchangeRatesforEdition);
      }
    }

    #endregion Web Api

  }  // class ExchangeRatesController

}  // namespace Empiria.FinancialAccounting.WebApi
