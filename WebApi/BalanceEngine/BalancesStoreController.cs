/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                               Component : Web Api                               *
*  Assembly : Empiria.FinancialAccounting.WebApi.dll       Pattern   : Controller                            *
*  Type     : BalancesStoreController                      License   : Please read LICENSE.txt file          *
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
  public class BalancesStoreController : WebApiController {

    #region Web Apis

    [HttpGet]
    [Route("v2/financial-accounting/accounts-charts/{accountsChartUID:guid}/balances-store")]
    public CollectionModel GetStoredBalanceSet([FromUri] string accountsChartUID) {
      using (var usecases = BalancesStoreUseCases.UseCaseInteractor()) {
        FixedList<StoredBalancesSetDto> storedBalancesSets = usecases.StoredBalancesSets(accountsChartUID);

        return new CollectionModel(this.Request, storedBalancesSets);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/accounts-charts/{accountsChartUID:guid}/balances-store")]
    public SingleObjectModel CreateOrGetStoredBalanceSet([FromUri] string accountsChartUID,
                                                         [FromBody] StoreBalancesCommand command) {
      base.RequireBody(command);

      using (var usecases = BalancesStoreUseCases.UseCaseInteractor()) {
        StoredBalancesSetDto storedBalanceSet = usecases.CreateOrGetStoredBalanceSet(accountsChartUID, command);

        return new SingleObjectModel(this.Request, storedBalanceSet);
      }
    }

    #endregion Web Apis

  } // class BalancesStoreController

} // namespace Empiria.FinancialAccounting.WebApi.BalanceEngine
