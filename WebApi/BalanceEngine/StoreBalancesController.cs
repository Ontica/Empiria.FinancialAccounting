/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                               Component : Web Api                               *
*  Assembly : Empiria.FinancialAccounting.WebApi.dll       Pattern   : Controller                            *
*  Type     : StoreBalancesController                      License   : Please read LICENSE.txt file          *
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
  public class StoreBalancesController : WebApiController {

    #region Web Apis

    [HttpPost]
    [Route("v2/financial-accounting/trial-balance")]
    public NoDataModel StoreBalances([FromBody] StoreBalancesCommand command) {
      base.RequireBody(command);

      using (var usecases = StoreBalancesUseCases.UseCaseInteractor()) {
        usecases.StoreBalances(command);

        return new NoDataModel(this.Request);
      }
    }

    #endregion Web Apis

  } // class StoreBalancesController

} // namespace Empiria.FinancialAccounting.WebApi.BalanceEngine
