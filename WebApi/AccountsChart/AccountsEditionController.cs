/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart Edition                       Component : Web Api                               *
*  Assembly : Empiria.FinancialAccounting.WebApi.dll       Pattern   : Command controller                    *
*  Type     : AccountsEditionController                    License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Command Web Api used to create or update accounts in a chart of accounts.                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Web.Http;

using Empiria.Storage;
using Empiria.WebApi;

using Empiria.FinancialAccounting.Adapters;
using Empiria.FinancialAccounting.UseCases;

using Empiria.FinancialAccounting.AccountsChartEdition.Adapters;
using Empiria.FinancialAccounting.AccountsChartEdition.UseCases;

namespace Empiria.FinancialAccounting.WebApi {

  /// <summary>Command Web Api used to update accounts in a chart of accounts.</summary>
  public class AccountsEditionController : WebApiController {

    #region Web Apis


    [HttpGet]
    [Route("v2/financial-accounting/accounts-charts/{accountsChartUID:guid}/accounts/{accountUID:guid}")]
    public SingleObjectModel GetAccount([FromUri] string accountsChartUID,
                                    [FromUri] string accountUID) {

      using (var usecases = AccountsChartUseCases.UseCaseInteractor()) {
        AccountDto account = usecases.GetAccount(accountsChartUID, accountUID);

        return new SingleObjectModel(base.Request, account);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/accounts-charts/{accountsChartUID:guid}/accounts")]
    public SingleObjectModel CreateAccount([FromUri] string accountsChartUID,
                                           [FromBody] AccountEditionCommand command) {

      PrepareCommand(command, AccountEditionCommandType.CreateAccount, accountsChartUID);

      using (var usecases = AccountEditionUseCases.UseCaseInteractor()) {
        OperationSummary summary = usecases.ExecuteCommand(command);

        return new SingleObjectModel(base.Request, summary);
      }
    }


    [HttpPut, HttpPatch]
    [Route("v2/financial-accounting/accounts-charts/{accountsChartUID:guid}/accounts/{accountUID:guid}")]
    public SingleObjectModel UpdateAccount([FromUri] string accountsChartUID,
                                           [FromUri] string accountUID,
                                           [FromBody] AccountEditionCommand command) {

      PrepareCommand(command, AccountEditionCommandType.UpdateAccount, accountsChartUID, accountUID);

      using (var usecases = AccountEditionUseCases.UseCaseInteractor()) {
        OperationSummary summary = usecases.ExecuteCommand(command);

        return new SingleObjectModel(base.Request, summary);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/accounts-charts/update-from-excel-file")]
    public CollectionModel UpdateAccountsChartFromExcelFile() {

      InputFile excelFile = base.GetInputFileFromHttpRequest("ExcelFileWithAccountsChartEditionCommands");

      UpdateAccountsFromFileCommand command = base.GetFormDataFromHttpRequest<UpdateAccountsFromFileCommand>("command");

      using (var usecases = AccountEditionUseCases.UseCaseInteractor()) {
        FixedList<OperationSummary> summary = usecases.ExecuteCommandsFromExcelFile(command, excelFile);
        return new CollectionModel(base.Request, summary);
      }
    }


    #endregion Web Apis

    #region Helpers

    private void PrepareCommand(AccountEditionCommand command,
                                AccountEditionCommandType type,
                                string accountsChartUID,
                                string accountUID = "") {
      base.RequireBody(command);

      command.CommandType = type;
      command.AccountsChartUID = accountsChartUID;
      command.AccountUID = accountUID;
    }

    #endregion Helpers

  }  // class AccountsEditionController

}  // namespace Empiria.FinancialAccounting.WebApi

