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

namespace Empiria.FinancialAccounting.WebApi {

  /// <summary>Query web API used to retrive accounts and accounts charts.</summary>
  public class AccountsChartController : WebApiController {

    #region Web Apis

    [HttpGet]
    [Route("v2/financial-accounting/accounts-chart/{accountsChartUID:guid}")]
    public CollectionModel GetAccounts([FromUri] string accountsChartUID) {

      var chart = AccountsChart.Parse(accountsChartUID);

      return new CollectionModel(this.Request, chart.Accounts);
    }

    #endregion Web Apis

  }  // class AccountsChartController

}  // namespace Empiria.FinancialAccounting.WebApi
