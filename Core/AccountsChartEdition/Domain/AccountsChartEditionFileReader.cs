/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart Edition                     Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Service provider                        *
*  Type     : AccountsChartEditionFileReader             License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Reads account edition commands contained in an Excel file.                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.IO;

using Empiria.Office;

using Empiria.FinancialAccounting.AccountsChartEdition.Adapters;

namespace Empiria.FinancialAccounting.AccountsChartEdition {

  /// <summary>Reads account edition commands contained in an Excel file.</summary>
  internal class AccountsChartEditionFileReader {

    private readonly AccountsChart _chart;
    private readonly DateTime _applicationDate;
    private readonly FileInfo _excelFile;
    private readonly bool _dryRun;

    public AccountsChartEditionFileReader(AccountsChart chart,
                                          DateTime applicationDate,
                                          FileInfo excelFile,
                                          bool dryRun) {
      _chart = chart;
      _applicationDate = applicationDate;
      _excelFile = excelFile;
      _dryRun = dryRun;
    }


    internal FixedList<AccountEditionCommand> GetCommands() {
      FixedList<AccountEditionCommand> commands;

      using (Spreadsheet spreadsheet = OpenSpreadsheet()) {
        commands = ReadCommands(spreadsheet);
      }

      RequireCommandTypesAreValid(commands);
      RequireApplicationDateIsValid(commands);
      RequireAccountNumbersAreValid(commands);
      SetDataForExistingAccountsToBeUpdated(commands);
      DetermineAccountTypeForNewAccounts(commands);
      SetSkipParentValidationFlag(commands);
      EnsureAllDataIsLoaded(commands);

      return commands;
    }

    #region Helpers

    private void DetermineAccountTypeForNewAccounts(FixedList<AccountEditionCommand> commandsList) {
      var createAccountCommands = GetCreateAccountCommands(commandsList);

      // Sort list by account, because parent accounts must be processed first
      createAccountCommands.Sort((x, y) => x.AccountFields.AccountNumber.CompareTo(y.AccountFields.AccountNumber));

      foreach (var command in createAccountCommands) {

        string accountNumber = command.AccountFields.AccountNumber;

        Account parent = _chart.TryGetParentAccount(accountNumber);

        if (parent != null) {
          command.AccountFields.AccountTypeUID = parent.AccountType.UID;
          command.AccountFields.DebtorCreditor = parent.DebtorCreditor;

          continue;
        }

        string parentAccountNumber = _chart.BuildParentAccountNumber(accountNumber);

        AccountEditionCommand createParentAccountCommand =
                                    createAccountCommands.Find(x => x.AccountFields.AccountNumber == parentAccountNumber);

        if (createParentAccountCommand == null) {

          Assertion.RequireFail($"No encontré en el catálogo la cuenta sumaria de " +
                        $"'{accountNumber}', " +
                        $"y tampoco se está solicitando darla de alta en el " +
                        $"archivo proporcionado: {command.DataSource}");
        }

        Assertion.Require(createParentAccountCommand.AccountFields.Role == AccountRole.Sumaria,
                        $"La cuenta '{createParentAccountCommand.AccountFields.AccountNumber}' " +
                        $"deber tener el rol de 'Sumaria', ya que en el archivo " +
                        $"se indica que también se debe agregar su cuenta " +
                        $"hija '{accountNumber}': {command.DataSource}");

        Assertion.Ensure(createParentAccountCommand.AccountFields.AccountTypeUID,
                        $"Unregistered AccountTypeUID for account " +
                        $"'{createParentAccountCommand.AccountFields.AccountNumber}': {command.DataSource}");

        command.AccountFields.AccountTypeUID = createParentAccountCommand.AccountFields.AccountTypeUID;
        command.AccountFields.DebtorCreditor = createParentAccountCommand.AccountFields.DebtorCreditor;

      }  // for

    }


    private void SetSkipParentValidationFlag(FixedList<AccountEditionCommand> commandsList) {
      var changeRoleRelatedCommands = commandsList.FindAll(x => x.CommandType == AccountEditionCommandType.CreateAccount ||
                                                                x.DataToBeUpdated.ToFixedList().Contains(AccountDataToBeUpdated.MainRole));

      if (changeRoleRelatedCommands.Count == 1) {
        return;
      }

      foreach (AccountEditionCommand command in changeRoleRelatedCommands) {
        string accountNumber = command.AccountFields.AccountNumber;
        string parentNumber = _chart.BuildParentAccountNumber(accountNumber);

        if (changeRoleRelatedCommands.Contains(x => x.AccountFields.AccountNumber.Equals(parentNumber) &&
                                                    x.AccountFields.Role == AccountRole.Sumaria)) {
          command.SkipParentAccountValidation = true;
        }
      }
    }


    private FixedList<AccountEditionCommand> GetCreateAccountCommands(FixedList<AccountEditionCommand> commandsList) {
      return commandsList.FindAll(x => x.CommandType == AccountEditionCommandType.CreateAccount);
    }


    private FixedList<AccountEditionCommand> GetUpdateAccountCommands(FixedList<AccountEditionCommand> commandsList) {
      return commandsList.FindAll(x => x.CommandType == AccountEditionCommandType.UpdateAccount);
    }


    private Spreadsheet OpenSpreadsheet() {
      return Spreadsheet.Open(_excelFile.FullName);
    }


    private FixedList<AccountEditionCommand> ReadCommands(Spreadsheet spreadsheet) {
      var commandsList = new List<AccountEditionCommand>(32);

      int rowIndex = 5;
      const int MAX_SKIPPED_ROWS = 10;

      int skippedRows = 0;

      while (true) {
        if (spreadsheet.HasValue($"A{rowIndex}")) {

          AccountEditionCommand command = ReadCommand(spreadsheet, rowIndex);

          commandsList.Add(command);
          skippedRows = 0;

        } else if (skippedRows < MAX_SKIPPED_ROWS) {
          skippedRows++;

        } else {
          break;

        }
        rowIndex++;
      }

      EmpiriaLog.Info(
        $"Se leyeron {commandsList.Count} operaciones del archivo de " +
        $"cambios al catálogo de cuenta contables {_excelFile.Name}."
      );

      return commandsList.ToFixedList();
    }


    private AccountEditionCommand ReadCommand(Spreadsheet spreadsheet, int rowIndex) {
      var rowReader = new AccountsChartEditionRowReader(spreadsheet, rowIndex);

      AccountFieldsDto accountFields = rowReader.GetAccountFields();

      return new AccountEditionCommand {
        CommandType = rowReader.GetCommandType(),
        AccountsChartUID = _chart.UID,
        ApplicationDate = _applicationDate,
        DryRun = _dryRun,
        AccountFields = accountFields,
        Currencies = rowReader.GetCurrencies(),
        SectorRules = rowReader.GetSectorRules(),
        DataToBeUpdated = rowReader.GetDataToBeUpdated(),
        CommandText = rowReader.GetCommandText(),
        DataSource = $"Fila {rowIndex} (cuenta {accountFields.AccountNumber})"
      };
    }


    private void RequireAccountNumbersAreValid(FixedList<AccountEditionCommand> commands) {
      foreach (var command in commands) {
        var accountNumber = command.AccountFields.AccountNumber;

        if (!_chart.IsValidAccountNumber(accountNumber)) {
          Assertion.RequireFail(
                    $"La cuenta tiene un formato que " +
                    $"no reconozco: {command.DataSource}.");
        }

        if (commands.CountAll(x => x.AccountFields.AccountNumber == accountNumber) >  1) {
          Assertion.RequireFail(
                    $"La cuenta viene más de una vez " +
                    $"en el archivo: {command.DataSource}.");
        }

        Account account = _chart.TryGetAccount(accountNumber);

        if (command.CommandType == AccountEditionCommandType.FixAccountName ||
            command.CommandType == AccountEditionCommandType.UpdateAccount) {

          Assertion.Require(account != null,
              $"Se está solicitando modificar la cuenta " +
              $"pero no existe en el catálogo de cuentas : {command.DataSource}.");
        }
      }
    }


    private void EnsureAllDataIsLoaded(FixedList<AccountEditionCommand> commands) {
      foreach (var command in commands) {

        if (command.CommandType == AccountEditionCommandType.FixAccountName ||
            command.CommandType == AccountEditionCommandType.UpdateAccount) {
          Assertion.Require(command.AccountUID,
              $"No se registró el UID de la cuenta: {command.DataSource}.");
        }

        Assertion.Require(command.AccountFields.Role != AccountRole.Undefined,
              $"No se indicó el rol de la cuenta: {command.DataSource}.");

        Assertion.Require(command.AccountFields.AccountTypeUID,
              $"No se registró el tipo de la cuenta: {command.DataSource}.");

        Assertion.Require(command.AccountFields.DebtorCreditor != DebtorCreditorType.Undefined,
              $"No se sabe si la naturaleza de la cuenta " +
              $"es deudora o acreedora: {command.DataSource}");
      }
    }


    private void RequireCommandTypesAreValid(FixedList<AccountEditionCommand> commands) {
      Assertion.Require(commands.Count > 0,
                        "El archivo no contiene ninguna operación " +
                        "de modificación al catálogo de cuentas contables.");

      foreach (var command in commands) {
        Assertion.Require(command.CommandType == AccountEditionCommandType.CreateAccount ||
                          command.CommandType == AccountEditionCommandType.FixAccountName ||
                          command.CommandType == AccountEditionCommandType.UpdateAccount,
          $"No reconozco la operación: {command.DataSource}.");
      }
    }


    private void RequireApplicationDateIsValid(FixedList<AccountEditionCommand> commands) {
      if (GetCreateAccountCommands(commands).Count != 0) {
        var firstDate = new DateTime(DateTime.Today.AddMonths(-1).Year,
                                     DateTime.Today.AddMonths(-1).Month,
                                     1);

        Assertion.Require(firstDate <= _applicationDate && _applicationDate <= DateTime.Today.AddDays(8),
              "Debido a que el archivo contiene operaciones que agregan " +
              "cuentas ya existentes, la fecha de aplicación de los cambios " +
              $"al catálogo debe ser a partir del {firstDate.ToString("dd/MMM/yyyy")} " +
              $" y hasta el {DateTime.Today.AddDays(8).ToString("dd/MMM/yyyy")}.");
      }


      if (GetUpdateAccountCommands(commands).Count != 0) {

        Assertion.Require(DateTime.Today.AddDays(1) <= _applicationDate &&
                          _applicationDate <= DateTime.Today.AddDays(8),
              "Debido a que el archivo contiene operaciones que modifican " +
              "cuentas ya existentes, la fecha de aplicación de los cambios " +
              "al catálogo debe ser a partir de mañana y hasta " +
              $"el {DateTime.Today.AddDays(8).ToShortDateString()}.");
      }
    }


    private void SetDataForExistingAccountsToBeUpdated(FixedList<AccountEditionCommand> commands) {

      foreach (var command in commands) {
        if (command.CommandType == AccountEditionCommandType.CreateAccount) {
          continue;
        }

        Account account = _chart.GetAccount(command.AccountFields.AccountNumber)
                                .GetHistory(_applicationDate);

        command.AccountUID = account.UID;
        command.AccountFields.AccountTypeUID = account.AccountType.UID;
        command.AccountFields.DebtorCreditor = account.DebtorCreditor;

        if (!account.Role.Equals(command.AccountFields.Role)) {
          PatchDataToBeUpdatedToInclude(command, AccountDataToBeUpdated.MainRole);
        }
      }
    }


    private void PatchDataToBeUpdatedToInclude(AccountEditionCommand command,
                                               AccountDataToBeUpdated dataToAddIfNotIncluded) {
      var patchedList = new List<AccountDataToBeUpdated>(command.DataToBeUpdated);

      if (patchedList.Contains(dataToAddIfNotIncluded)) {
        return;
      }

      patchedList.Add(dataToAddIfNotIncluded);
      command.DataToBeUpdated = patchedList.ToArray();
    }

    #endregion Helpers

  }  // class AccountsChartEditionFileReader

}  // namespace Empiria.FinancialAccounting.AccountsChartEdition
