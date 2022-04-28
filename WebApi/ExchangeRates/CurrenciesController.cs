/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Exchange Rates                               Component : Web Api                               *
*  Assembly : Empiria.FinancialAccounting.WebApi.dll       Pattern   : Controller                            *
*  Type     : CurrenciesController                         License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Web apis used to get information about currencies.                                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Web.Http;

using Empiria.WebApi;

using Empiria.FinancialAccounting.UseCases;

namespace Empiria.FinancialAccounting.WebApi {

  /// <summary>Web apis used to get information about currencies.</summary>
  public class CurrenciesController : WebApiController {

    #region Web Api

    [HttpGet]
    [Route("v2/financial-accounting/currencies")]
    public CollectionModel GetCurrencies() {

      using (var usecases = CurrenciesUseCases.UseCaseInteractor()) {
        FixedList<NamedEntityDto> currencies = usecases.GetCurrencies();

        return new CollectionModel(base.Request, currencies);
      }
    }

    #endregion Web Api

  }  // class CurrenciesController

}  // namespace Empiria.FinancialAccounting.WebApi
