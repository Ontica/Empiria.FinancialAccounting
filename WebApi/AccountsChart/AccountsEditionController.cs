/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart                               Component : Web Api                               *
*  Assembly : Empiria.FinancialAccounting.WebApi.dll       Pattern   : Command controller                    *
*  Type     : AccountsEditionController                    License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Command Web Api used to create or update an account in a chart of accounts.                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Web.Http;

using Empiria.WebApi;

using Empiria.FinancialAccounting.Adapters;
using Empiria.FinancialAccounting.UseCases;


namespace Empiria.FinancialAccounting.WebApi {

  /// <summary>Command Web Api used to update an account in a chart of accounts.</summary>
  public class AccountsEditionController : WebApiController {

    #region Web Apis

    [HttpPost]
    [Route("v2/financial-accounting/accounts-charts/process-command")]
    public SingleObjectModel EditAccounts([FromBody] AccountEditionCommand command) {

      base.RequireBody(command);

      using (var usecases = AccountEditionUseCases.UseCaseInteractor()) {
        AccountEditionResult result;

        switch (command.CommandType) {

          case AccountEditionCommandType.AddCurrencies:
            result = usecases.AddCurrencies(command);
            break;

          case AccountEditionCommandType.AddSectors:
            result = usecases.AddSectors(command);
            break;

          case AccountEditionCommandType.CreateAccount:
            result = usecases.CreateAccount(command);
            break;

          case AccountEditionCommandType.RemoveAccount:
            result = usecases.RemoveAccount(command);
            break;

          case AccountEditionCommandType.RemoveCurrencies:
            result = usecases.RemoveCurrencies(command);
            break;

          case AccountEditionCommandType.RemoveSectors:
            result = usecases.RemoveSectors(command);
            break;

          case AccountEditionCommandType.UpdateAccount:
            result = usecases.UpdateAccount(command);
            break;

          default:
            throw Assertion.AssertNoReachThisCode($"Unhandled command type '{command.CommandType}'.");

        }

        return new SingleObjectModel(base.Request, result);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/accounts-charts/{accountsChartUID:guid}/accounts/{accountUID:guid}/add-currencies")]
    public SingleObjectModel AddCurrencies([FromUri] string accountsChartUID,
                                           [FromUri] string accountUID,
                                           [FromBody] AccountEditionCommand command) {

      PrepareCommand(command, AccountEditionCommandType.AddCurrencies, accountsChartUID, accountUID);

      using (var usecases = AccountEditionUseCases.UseCaseInteractor()) {
        AccountEditionResult result = usecases.AddCurrencies(command);

        return new SingleObjectModel(base.Request, result);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/accounts-charts/{accountsChartUID:guid}/accounts/{accountUID:guid}/add-sectors")]
    public SingleObjectModel AddSectors([FromUri] string accountsChartUID,
                                        [FromUri] string accountUID,
                                        [FromBody] AccountEditionCommand command) {

      PrepareCommand(command, AccountEditionCommandType.AddSectors, accountsChartUID, accountUID);

      using (var usecases = AccountEditionUseCases.UseCaseInteractor()) {
        AccountEditionResult result = usecases.AddSectors(command);

        return new SingleObjectModel(base.Request, result);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/accounts-charts/{accountsChartUID:guid}/accounts/{accountUID:guid}/create-account")]
    public SingleObjectModel CreateAccount([FromUri] string accountsChartUID,
                                           [FromUri] string accountUID,
                                           [FromBody] AccountEditionCommand command) {

      PrepareCommand(command, AccountEditionCommandType.CreateAccount, accountsChartUID, accountUID);

      using (var usecases = AccountEditionUseCases.UseCaseInteractor()) {
        AccountEditionResult result = usecases.CreateAccount(command);

        return new SingleObjectModel(base.Request, result);
      }
    }


    [HttpDelete]
    [Route("v2/financial-accounting/accounts-charts/{accountsChartUID:guid}/accounts/{accountUID:guid}/remove-account")]
    public SingleObjectModel RemoveAccount([FromUri] string accountsChartUID,
                                           [FromUri] string accountUID,
                                           [FromBody] AccountEditionCommand command) {

      PrepareCommand(command, AccountEditionCommandType.RemoveAccount, accountsChartUID, accountUID);

      using (var usecases = AccountEditionUseCases.UseCaseInteractor()) {
        AccountEditionResult result = usecases.RemoveAccount(command);

        return new SingleObjectModel(base.Request, result);
      }
    }


    [HttpDelete]
    [Route("v2/financial-accounting/accounts-charts/{accountsChartUID:guid}/accounts/{accountUID:guid}/remove-currencies")]
    public SingleObjectModel RemoveCurrencies([FromUri] string accountsChartUID,
                                              [FromUri] string accountUID,
                                              [FromBody] AccountEditionCommand command) {

      PrepareCommand(command, AccountEditionCommandType.RemoveCurrencies, accountsChartUID, accountUID);

      using (var usecases = AccountEditionUseCases.UseCaseInteractor()) {
        AccountEditionResult result = usecases.RemoveCurrencies(command);

        return new SingleObjectModel(base.Request, result);
      }
    }


    [HttpDelete]
    [Route("v2/financial-accounting/accounts-charts/{accountsChartUID:guid}/accounts/{accountUID:guid}/remove-sectors")]
    public SingleObjectModel RemoveSectors([FromUri] string accountsChartUID,
                                           [FromUri] string accountUID,
                                           [FromBody] AccountEditionCommand command) {

      PrepareCommand(command, AccountEditionCommandType.RemoveSectors, accountsChartUID, accountUID);

      using (var usecases = AccountEditionUseCases.UseCaseInteractor()) {
        AccountEditionResult result = usecases.RemoveSectors(command);

        return new SingleObjectModel(base.Request, result);
      }
    }


    [HttpPut, HttpPatch]
    [Route("v2/financial-accounting/accounts-charts/{accountsChartUID:guid}/accounts/{accountUID:guid}/update-account")]
    public SingleObjectModel UpdateAccount([FromUri] string accountsChartUID,
                                           [FromUri] string accountUID,
                                           [FromBody] AccountEditionCommand command) {

      PrepareCommand(command, AccountEditionCommandType.UpdateAccount, accountsChartUID, accountUID);

      using (var usecases = AccountEditionUseCases.UseCaseInteractor()) {
        AccountEditionResult result = usecases.UpdateAccount(command);

        return new SingleObjectModel(base.Request, result);
      }
    }


    #endregion Web Apis

    #region Helpers

    private void PrepareCommand(AccountEditionCommand command,
                                AccountEditionCommandType type,
                                string accountsChartUID,
                                string accountUID) {
      base.RequireBody(command);

      command.CommandType = type;
      command.AccountsChartUID = accountsChartUID;
      command.AccountUID = accountUID;
    }

    #endregion Helpers

  }  // class AccountsEditionController

}  // namespace Empiria.FinancialAccounting.WebApi
