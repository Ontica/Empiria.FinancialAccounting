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
      DetermineAccountTypeForNewAccounts(commands);
      RequireValidCurrencies(commands);
      RequireValidSectors(commands);
      RequireAllDataLoaded(commands);

      return commands;
    }


    #region Helpers

    private void CleanUpIrrelevantFields(AccountEditionCommand command) {
      if (command.CommandType == AccountEditionCommandType.CreateAccount) {
        return;
      }

      var dataToBeUpdated = command.DataToBeUpdated.ToFixedList();

      if (!dataToBeUpdated.Contains(AccountDataToBeUpdated.Currencies)) {
        command.Currencies = new string[0];
      }

      if (!dataToBeUpdated.Contains(AccountDataToBeUpdated.Sectors)) {
        command.Sectors = new string[0];
        command.SectorsRole = AccountRole.Undefined;
      }
    }


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

        string parentAccountNumber = _chart.GetParentAccountNumber(accountNumber);

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

      while (spreadsheet.HasValue($"A{rowIndex}")) {
        AccountEditionCommand command = ReadCommand(spreadsheet, rowIndex);

        commandsList.Add(command);

        rowIndex++;
      }

      EmpiriaLog.Info(
        $"Se leyeron {commandsList.Count} operaciones del archivo de " +
        $"modificaciones al catálogo de cuenta contables {_excelFile.Name}."
      );

      return commandsList.ToFixedList();
    }


    private AccountEditionCommand ReadCommand(Spreadsheet spreadsheet, int rowIndex) {
      var rowReader = new AccountsChartEditionRowReader(spreadsheet, rowIndex);

      var command = new AccountEditionCommand {
        CommandType = rowReader.GetCommandType(),
        AccountsChartUID = _chart.UID,
        ApplicationDate = _applicationDate,
        DryRun = _dryRun,
        AccountFields = rowReader.GetAccountFields(),
        SectorsRole = rowReader.GetSectorsRole(),
        Currencies = rowReader.GetCurrencies(),
        Sectors = rowReader.GetSectors(),
        DataToBeUpdated = rowReader.GetDataToBeUpdated(),
        CommandText = rowReader.GetCommandText(),
        DataSource = $"Fila {rowIndex}"
      };

      if (command.CommandType == AccountEditionCommandType.FixAccountName ||
          command.CommandType == AccountEditionCommandType.UpdateAccount) {
        Account account = _chart.GetAccount(command.AccountFields.AccountNumber);

        command.AccountUID = account.UID;
        command.AccountFields.AccountTypeUID = account.AccountType.UID;
        command.AccountFields.DebtorCreditor = account.DebtorCreditor;

        CleanUpIrrelevantFields(command);
      }

      return command;
    }


    private void RequireAccountNumbersAreValid(FixedList<AccountEditionCommand> commands) {
      foreach (var command in commands) {
        var accountNumber = command.AccountFields.AccountNumber;

        if (!_chart.IsAccountNumberFormatValid(accountNumber)) {
          Assertion.RequireFail(
                    $"La cuenta '{accountNumber}' tiene un formato que " +
                    $"no reconozco: {command.DataSource}");
        }

        if (commands.CountAll(x => x.AccountFields.AccountNumber == accountNumber) >  1) {
          Assertion.RequireFail(
                    $"La cuenta '{accountNumber}' viene más de una vez " +
                    $"en el archivo: {command.DataSource}");
        }

        Account account = _chart.TryGetAccount(accountNumber);

        if (command.CommandType == AccountEditionCommandType.CreateAccount) {

          Assertion.Require(account == null,
              $"Se está solicitando agregar la cuenta '{accountNumber}' " +
              $"pero ya existe en el catálogo de cuentas: {command.DataSource}");

        } else if (command.CommandType == AccountEditionCommandType.FixAccountName ||
                   command.CommandType == AccountEditionCommandType.UpdateAccount) {

          Assertion.Require(account != null,
              $"Se está solicitando modificar la cuenta '{accountNumber}' " +
              $"pero no está registrada en el catálogo de cuentas : {command.DataSource}.");
        }
      }
    }


    private void RequireAllDataLoaded(FixedList<AccountEditionCommand> commands) {
      foreach (var command in commands) {
        string accountNumber = command.AccountFields.AccountNumber;

        if (command.CommandType == AccountEditionCommandType.FixAccountName ||
            command.CommandType == AccountEditionCommandType.UpdateAccount) {
          Assertion.Require(command.AccountUID,
              $"No se registró el UID de la cuenta {accountNumber}: {command.DataSource}.");
        }

        Assertion.Require(command.AccountFields.Role != AccountRole.Undefined,
              $"No se indica el rol de la cuenta '{accountNumber}': {command.DataSource}.");

        Assertion.Require(command.AccountFields.AccountTypeUID,
              $"No se registró el tipo de la cuenta {accountNumber}: {command.DataSource}.");

        Assertion.Require(command.AccountFields.DebtorCreditor != DebtorCreditorType.Undefined,
              $"No se sabe si la naturaleza de la cuenta '{accountNumber}'" +
              $"es deudora o acreedora: {command.DataSource}");

        command.EnsureAccountFieldsAreValid();
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
          $"No reconozco la operación proveniente de: {command.DataSource}.");
      }
    }


    private void RequireApplicationDateIsValid(FixedList<AccountEditionCommand> commands) {
      if (GetUpdateAccountCommands(commands).Count != 0) {

        Assertion.Require(DateTime.Today.AddDays(1) <= _applicationDate &&
                          _applicationDate <= DateTime.Today.AddDays(8),
              "Debido a que el archivo contiene operaciones que modifican " +
              "cuentas ya existentes, la fecha de aplicación de los cambios " +
              "al catálogo debe ser a partir de mañana y hasta " +
             $"el {DateTime.Today.AddDays(8).ToShortDateString()}.");
      }
    }


    private void RequireValidCurrencies(FixedList<AccountEditionCommand> commands) {

      foreach (var command in commands) {
        string accountNumber = command.AccountFields.AccountNumber;

        if (command.CommandType == AccountEditionCommandType.CreateAccount ||
            command.DataToBeUpdated.ToFixedList().Contains(AccountDataToBeUpdated.Currencies)) {

          Assertion.Require(command.Currencies.Length > 0,
                $"No se proporcionó ninguna moneda para la cuenta: '{accountNumber}': {command.DataSource}");
        }

        foreach (var currencyCode in command.Currencies) {

          Assertion.Require(Currency.Exists(currencyCode),
                    $"La cuenta '{accountNumber}' tiene una moneda que " +
                    $"no reconozco: '{currencyCode}': {command.DataSource}.");

          Assertion.Require(command.Currencies.ToFixedList().CountAll(x => x == currencyCode) == 1,
                    $"La cuenta '{accountNumber}' contiene más de una vez " +
                    $"la moneda '{currencyCode}': {command.DataSource}.");

        }
      }
    }


    private void RequireValidSectors(FixedList<AccountEditionCommand> commands) {

      foreach (var command in commands) {
        string accountNumber = command.AccountFields.AccountNumber;

        if (command.CommandType == AccountEditionCommandType.CreateAccount ||
            command.DataToBeUpdated.ToFixedList().Contains(AccountDataToBeUpdated.Sectors)) {

          Assertion.Require(!(command.Sectors.Length == 0 &&
                              command.AccountFields.Role == AccountRole.Sectorizada),
              $"El rol de la cuenta '{accountNumber}' indica que es sectorizada" +
              $"pero la lista de sectores está vacía: {command.DataSource}");
        }

        foreach (var sectorCode in command.Sectors) {

          Assertion.Require(Sector.Exists(sectorCode),
                $"La cuenta '{accountNumber}' trae el sector '{sectorCode}', " +
                $"el cual no existe: {command.DataSource}'.");

          var sector = Sector.Parse(sectorCode);

          Assertion.Require(!sector.IsSummary,
              $"Se solicita registrar el sector '({sector.Code}) {sector.Name}' " +
              $"a la cuenta '{accountNumber}', pero dicho sector es sumarizador: {command.DataSource}.");

          Assertion.Require(command.Sectors.ToFixedList().CountAll(x => x == sector.Code) == 1,
              $"La cuenta '{accountNumber}' tiene más de una vez " +
              $"el sector '{sector.Code}': {command.DataSource}");
        }
      }
    }


    #endregion Helpers

  }  // class AccountsChartEditionFileReader

}  // namespace Empiria.FinancialAccounting.AccountsChartEdition
