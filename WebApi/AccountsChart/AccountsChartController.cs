/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart                               Component : Web Api                               *
*  Assembly : Empiria.FinancialAccounting.WebApi.dll       Pattern   : Controller                            *
*  Type     : AccountsChartController                      License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Query web API used to retrive accounts and accounts charts.                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Web.Http;

using Empiria.WebApi;

using Empiria.FinancialAccounting.UseCases;
using Empiria.FinancialAccounting.Adapters;

namespace Empiria.FinancialAccounting.WebApi {

  /// <summary>Query web API used to retrive accounts and accounts charts.</summary>
  public class AccountsChartController : WebApiController {

    #region Web Apis


    [HttpGet]
    [Route("v2/financial-accounting/accounts-chart/{accountsChartUID:guid}")]
    public SingleObjectModel GetAccounts([FromUri] string accountsChartUID) {

      using (var usecases = AccountsChartUseCases.UseCaseInteractor()) {
        AccountsChartDto accountsChart = usecases.GetAccounts(accountsChartUID);

        return new SingleObjectModel(base.Request, accountsChart);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/accounts-chart/{accountsChartUID:guid}")]
    public SingleObjectModel SearchAccounts([FromUri] string accountsChartUID,
                                            [FromBody] AccountsSearchCommand searchCommand) {
      base.RequireBody(searchCommand);

      using (var usecases = AccountsChartUseCases.UseCaseInteractor()) {
        AccountsChartDto accountsChart = usecases.SearchAccounts(accountsChartUID, searchCommand);

        return new SingleObjectModel(base.Request, accountsChart);
      }
    }

    #endregion Web Apis

  }  // class AccountsChartController

}  // namespace Empiria.FinancialAccounting.WebApi
