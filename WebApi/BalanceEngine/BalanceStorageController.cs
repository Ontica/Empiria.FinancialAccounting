/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                               Component : Web Api                               *
*  Assembly : Empiria.FinancialAccounting.WebApi.dll       Pattern   : Controller                            *
*  Type     : BalanceStorageController                     License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Command web API used to store account and account aggrupation balances.                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System.Web.Http;

using Empiria.WebApi;

using Empiria.FinancialAccounting.BalanceEngine.UseCases;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.WebApi.BalanceEngine {

  /// <summary>Command web API used to store account and account aggrupation balances.</summary>
  public class BalanceStorageController : WebApiController {

    #region Web Apis

    [HttpGet]
    [Route("v2/financial-accounting/accounts-charts/{accountsChartUID:guid}/balance-store/{balanceSetUID:guid}")]
    public SingleObjectModel GetStoredBalanceSet([FromUri] string accountsChartUID,
                                                 [FromUri] string balanceSetUID) {
      using (var usecases = BalanceStorageUseCases.UseCaseInteractor()) {
        StoredBalanceSetDto balanceSet = usecases.GetBalanceSet(accountsChartUID, balanceSetUID);

        return new SingleObjectModel(this.Request, balanceSet);
      }
    }


    [HttpGet]
    [Route("v2/financial-accounting/accounts-charts/{accountsChartUID:guid}/balance-store")]
    public CollectionModel GetStoredBalanceSetsList([FromUri] string accountsChartUID) {
      using (var usecases = BalanceStorageUseCases.UseCaseInteractor()) {
        FixedList<StoredBalanceSetDto> list = usecases.BalanceSetsList(accountsChartUID);

        return new CollectionModel(this.Request, list);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/accounts-charts/{accountsChartUID:guid}/balance-store")]
    public SingleObjectModel CreateOrGetStoredBalanceSet([FromUri] string accountsChartUID,
                                                         [FromBody] BalanceStorageCommand command) {
      base.RequireBody(command);

      using (var usecases = BalanceStorageUseCases.UseCaseInteractor()) {
        StoredBalanceSetDto balanceSet = usecases.CreateOrGetBalanceSet(accountsChartUID, command);

        return new SingleObjectModel(this.Request, balanceSet);
      }
    }

    #endregion Web Apis

  } // class BalanceStorageController

} // namespace Empiria.FinancialAccounting.WebApi.BalanceEngine
