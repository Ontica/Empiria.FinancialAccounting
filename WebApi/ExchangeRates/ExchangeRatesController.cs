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
using Empiria.FinancialAccounting.Reporting;

namespace Empiria.FinancialAccounting.WebApi {

  /// <summary>Web apis used to get and update information about exchange rates.</summary>
  public class ExchangeRatesController : WebApiController {

    #region Web Api

    [HttpGet, AllowAnonymous]
    [Route("v1/financial-accounting/exchange-rates")]
    public CollectionModel GetAllExchangeRates([FromUri] DateTime date) {

      using (var usecases = ExchangeRatesUseCases.UseCaseInteractor()) {
        FixedList<ExchangeRateDto> exchangeRates = usecases.GetExchangeRates(date);

        return new CollectionModel(base.Request, exchangeRates);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/exchange-rates")]
    public CollectionModel GetExchangeRates([FromBody] ExchangeRatesQuery query) {

      using (var usecases = ExchangeRatesUseCases.UseCaseInteractor()) {
        FixedList<ExchangeRateDescriptorDto> exchangeRates = usecases.SearchExchangeRates(query);

        return new CollectionModel(base.Request, exchangeRates);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/exchange-rates/for-edition")]
    public SingleObjectModel GetExchangeRatesForEdition([FromBody] ExchangeRateValuesDto query) {

      using (var usecases = ExchangeRatesUseCases.UseCaseInteractor()) {

        ExchangeRateValuesDto exchangeRatesForEdition =
                        usecases.GetExchangeRatesForEdition(query.ExchangeRateTypeUID, query.Date);

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
    [Route("v2/financial-accounting/exchange-rates/update-all")]
    public SingleObjectModel UpdateAllExchangeRates([FromBody] ExchangeRateValuesDto fields) {

      RequireBody(fields);

      using (var usecases = ExchangeRatesUseCases.UseCaseInteractor()) {
        ExchangeRateValuesDto exchangeRatesforEdition = usecases.UpdateAllExchangeRates(fields);

        return new SingleObjectModel(base.Request, exchangeRatesforEdition);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/exchange-rates/excel")]
    public SingleObjectModel GetExcelExchangeRates([FromBody] ExchangeRatesQuery query) {

      using (var usecases = ExchangeRatesUseCases.UseCaseInteractor()) {
        FixedList<ExchangeRateDescriptorDto> exchangeRates = usecases.SearchExchangeRates(query);

        var excelExporter = new ExcelExporterService();

        FileReportDto excelFileDto = excelExporter.Export(exchangeRates);

        return new SingleObjectModel(this.Request, excelFileDto);
      }
    }

    #endregion Web Api

  }  // class ExchangeRatesController

}  // namespace Empiria.FinancialAccounting.WebApi
