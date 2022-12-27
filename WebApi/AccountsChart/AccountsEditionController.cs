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

using Empiria.FinancialAccounting.AccountsChartEdition.Adapters;
using Empiria.FinancialAccounting.AccountsChartEdition.UseCases;

namespace Empiria.FinancialAccounting.WebApi {

  /// <summary>Command Web Api used to update accounts in a chart of accounts.</summary>
  public class AccountsEditionController : WebApiController {

    #region Web Apis

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
    [Route("v2/financial-accounting/accounts-charts/update-from-excel-file/dry-run")]
    public CollectionModel UpdateAccountsChartFromExcelFile() {

      InputFile excelFile = base.GetInputFileFromHttpRequest("ExcelFileWithAccountsChartEditionCommands");

      UpdateAccountsFromFileCommand command = base.GetFormDataFromHttpRequest<UpdateAccountsFromFileCommand>("command");

      bool dryRun = RouteContainsDryRunFlag();

      using (var usecases = AccountEditionUseCases.UseCaseInteractor()) {
        FixedList<OperationSummary> summary = usecases.ExecuteCommandsFromExcelFile(command,
                                                                                    excelFile,
                                                                                    dryRun);
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


    private bool RouteContainsDryRunFlag() {
      return base.Request.RequestUri.PathAndQuery.EndsWith("/dry-run");
    }


    #endregion Helpers

  }  // class AccountsEditionController

}  // namespace Empiria.FinancialAccounting.WebApi

