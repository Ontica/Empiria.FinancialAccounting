/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart                               Component : Web Api                               *
*  Assembly : Empiria.FinancialAccounting.WebApi.dll       Pattern   : Command controller                    *
*  Type     : AccountsEditionController                    License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Command Web Api used to create or update an account in a chart of accounts.                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System.Web.Http;

using Empiria.WebApi;

using Empiria.FinancialAccounting.UseCases;
using Empiria.FinancialAccounting.Adapters;

namespace Empiria.FinancialAccounting.WebApi {

  /// <summary>Command Web Api used to update an account in a chart of accounts.</summary>
  public class AccountsEditionController : WebApiController {

    #region Web Apis

    [HttpPost]
    [Route("v2/financial-accounting/accounts-charts/{accountsChartUID:guid}/add-currencies")]
    public SingleObjectModel AddCurrencies([FromUri] string accountsChartUID,
                                           [FromBody] AccountEditionCommand command) {

      base.RequireBody(command);

      using (var usecases = AccountEditionUseCases.UseCaseInteractor()) {
        AccountEditionResult result = usecases.AddCurrencies(accountsChartUID, command);

        return new SingleObjectModel(base.Request, result);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/accounts-charts/{accountsChartUID:guid}/add-sectors")]
    public SingleObjectModel AddSectors([FromUri] string accountsChartUID,
                                        [FromBody] AccountEditionCommand command) {

      base.RequireBody(command);

      using (var usecases = AccountEditionUseCases.UseCaseInteractor()) {
        AccountEditionResult result = usecases.AddSectors(accountsChartUID, command);

        return new SingleObjectModel(base.Request, result);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/accounts-charts/{accountsChartUID:guid}/create-account")]
    public SingleObjectModel CreateAccount([FromUri] string accountsChartUID,
                                           [FromBody] AccountEditionCommand command) {

      base.RequireBody(command);

      using (var usecases = AccountEditionUseCases.UseCaseInteractor()) {
        AccountEditionResult result = usecases.CreateAccount(accountsChartUID, command);

        return new SingleObjectModel(base.Request, result);
      }
    }


    [HttpDelete]
    [Route("v2/financial-accounting/accounts-charts/{accountsChartUID:guid}/remove-account")]
    public SingleObjectModel RemoveAccount([FromUri] string accountsChartUID,
                                           [FromBody] AccountEditionCommand command) {

      base.RequireBody(command);

      using (var usecases = AccountEditionUseCases.UseCaseInteractor()) {
        AccountEditionResult result = usecases.RemoveAccount(accountsChartUID, command);

        return new SingleObjectModel(base.Request, result);
      }
    }


    [HttpDelete]
    [Route("v2/financial-accounting/accounts-charts/{accountsChartUID:guid}/remove-currencies")]
    public SingleObjectModel RemoveCurrencies([FromUri] string accountsChartUID,
                                              [FromBody] AccountEditionCommand command) {

      base.RequireBody(command);

      using (var usecases = AccountEditionUseCases.UseCaseInteractor()) {
        AccountEditionResult result = usecases.RemoveCurrencies(accountsChartUID, command);

        return new SingleObjectModel(base.Request, result);
      }
    }


    [HttpDelete]
    [Route("v2/financial-accounting/accounts-charts/{accountsChartUID:guid}/remove-sectors")]
    public SingleObjectModel RemoveSectors([FromUri] string accountsChartUID,
                                           [FromBody] AccountEditionCommand command) {

      base.RequireBody(command);

      using (var usecases = AccountEditionUseCases.UseCaseInteractor()) {
        AccountEditionResult result = usecases.RemoveSectors(accountsChartUID, command);

        return new SingleObjectModel(base.Request, result);
      }
    }


    [HttpPut, HttpPatch]
    [Route("v2/financial-accounting/accounts-charts/{accountsChartUID:guid}/update-account")]
    public SingleObjectModel UpdateAccount([FromUri] string accountsChartUID,
                                           [FromBody] AccountEditionCommand command) {

      base.RequireBody(command);

      using (var usecases = AccountEditionUseCases.UseCaseInteractor()) {
        AccountEditionResult result = usecases.UpdateAccount(accountsChartUID, command);

        return new SingleObjectModel(base.Request, result);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/accounts-charts/cleanup")]
    public SingleObjectModel CleanupAccounts() {

      using (var usecases = AccountEditionUseCases.UseCaseInteractor()) {

        usecases.CleanupAccounts();

        return new SingleObjectModel(base.Request, "La operación de limpieza se ejecutó satisfactoriamente.");
      }
    }

    #endregion Web Apis

  }  // class AccountsEditionController

}  // namespace Empiria.FinancialAccounting.WebApi
