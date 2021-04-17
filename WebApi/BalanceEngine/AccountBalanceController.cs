/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                               Component : Web Api                               *
*  Assembly : Empiria.FinancialAccounting.WebApi.dll       Pattern   : Controller                            *
*  Type     : AccountBalanceController                     License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Query web API used to retrive account balances.                                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System.Web.Http;

using Empiria.WebApi;

using Empiria.FinancialAccounting.BalanceEngine.UseCases;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine.WebApi {

  /// <summary>Query web API used to retrive account balances.</summary>
  public class AccountBalanceController : WebApiController {

    #region Web Apis

    [HttpGet]
    [Route("v2/financial-accounting/balances/{accountNumber}")]
    public SingleObjectModel GetAccountBalance([FromUri] string accountNumber) {

      using (var usecases = AccountBalanceUseCases.UseCaseInteractor()) {
        AccountBalanceDto accountBalance = usecases.AccountBalance(accountNumber);

        return new SingleObjectModel(this.Request, accountBalance);
      }
    }

    #endregion Web Apis

  }  // class AccountBalanceController

}  // namespace Empiria.FinancialAccounting.BalanceEngine.WebApi
