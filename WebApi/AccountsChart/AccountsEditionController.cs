﻿/* Empiria Financial *****************************************************************************************
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

using Empiria.Commands;
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
                                        [FromUri] string accountUID,
                                        [FromUri] DateTime? date = null) {

      if (date == null || !date.HasValue) {
        date = DateTime.Today;
      }

      using (var usecases = AccountsChartUseCases.UseCaseInteractor()) {
        AccountDto account = usecases.GetAccount(accountsChartUID, accountUID, date.Value);

        base.SetOperation($"Se leyó la cuenta {account.Number} del catálogo de cuentas.");

        return new SingleObjectModel(base.Request, account);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/accounts-charts/{accountsChartUID:guid}/accounts")]
    public SingleObjectModel CreateAccount([FromUri] string accountsChartUID,
                                           [FromBody] AccountEditionCommand command) {

      PrepareCommand(command, accountsChartUID);

      Assertion.Require(command.CommandType == AccountEditionCommandType.CreateAccount,
                        $"Unrecognized command type '{command.CommandType}'.");

      base.SetOperation($"Se agregó la cuenta {command.AccountFields.AccountNumber} al catálogo de cuentas.");

      using (var usecases = AccountEditionUseCases.UseCaseInteractor()) {
        ExecutionResult<AccountDto> result = usecases.ExecuteCommand(command);

        return new SingleObjectModel(base.Request, result);
      }
    }


    [HttpDelete]
    [Route("v2/financial-accounting/accounts-charts/{accountsChartUID:guid}/accounts/{accountUID:guid}")]
    public SingleObjectModel DeleteAccount([FromUri] string accountsChartUID,
                                           [FromUri] string accountUID,
                                           [FromBody] AccountEditionCommand command) {

      PrepareCommand(command, accountsChartUID, accountUID);

      base.SetOperation($"Se eliminó la cuenta {command.AccountFields.AccountNumber} del catálogo de cuentas.");

      Assertion.Require(command.CommandType == AccountEditionCommandType.DeleteAccount,
                        $"Unrecognized command type '{command.CommandType}'.");

      using (var usecases = AccountEditionUseCases.UseCaseInteractor()) {
        ExecutionResult<AccountDto> result = usecases.ExecuteCommand(command);

        return new SingleObjectModel(base.Request, result);
      }
    }


    [HttpPut, HttpPatch]
    [Route("v2/financial-accounting/accounts-charts/{accountsChartUID:guid}/accounts/{accountUID:guid}")]
    public SingleObjectModel UpdateAccount([FromUri] string accountsChartUID,
                                           [FromUri] string accountUID,
                                           [FromBody] AccountEditionCommand command) {

      PrepareCommand(command, accountsChartUID, accountUID);

      Assertion.Require(command.CommandType == AccountEditionCommandType.UpdateAccount ||
                        command.CommandType == AccountEditionCommandType.FixAccountName,
                        $"Unrecognized command type '{command.CommandType}'.");

      base.SetOperation($"Se actualizó la cuenta {command.AccountFields.AccountNumber} del catálogo de cuentas.");

      using (var usecases = AccountEditionUseCases.UseCaseInteractor()) {
        ExecutionResult<AccountDto> result = usecases.ExecuteCommand(command);

        return new SingleObjectModel(base.Request, result);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/accounts-charts/update-from-excel-file")]
    public CollectionModel UpdateAccountsChartFromExcelFile() {

      InputFile excelFile = base.GetInputFileFromHttpRequest("ExcelFileWithAccountsChartEditionCommands");

      UpdateAccountsFromFileCommand command = base.GetFormDataFromHttpRequest<UpdateAccountsFromFileCommand>("command");

      base.SetOperation($"Se actualizaron varias cuentas en el catálogo de cuentas utilizando un archivo de entrada Excel.");

      using (var usecases = AccountEditionUseCases.UseCaseInteractor()) {
        FixedList<OperationSummary> summary = usecases.ExecuteCommandsFromExcelFile(command, excelFile);

        return new CollectionModel(base.Request, summary);
      }
    }


    #endregion Web Apis

    #region Helpers

    private void PrepareCommand(AccountEditionCommand command,
                                string accountsChartUID,
                                string accountUID = "") {
      base.RequireBody(command);

      command.AccountsChartUID = accountsChartUID;
      command.AccountUID = accountUID;
    }

    #endregion Helpers

  }  // class AccountsEditionController

}  // namespace Empiria.FinancialAccounting.WebApi

