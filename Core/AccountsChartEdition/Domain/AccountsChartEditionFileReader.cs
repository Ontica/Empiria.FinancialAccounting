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
      using (Spreadsheet spreadsheet = OpenSpreadsheet()) {

        var commands = ReadCommands(spreadsheet);

        Assertion.Require(commands.Count > 0,
              "El archivo no contiene ninguna operación " +
              "de modificación al catálogo de cuentas contables.");

        return commands;
      }
    }


    #region Helpers

    private Spreadsheet OpenSpreadsheet() {
      return Spreadsheet.Open(_excelFile.FullName);
    }


    private FixedList<AccountEditionCommand> ReadCommands(Spreadsheet spreadsheet) {
      var commandsList = new List<AccountEditionCommand>(128);

      int rowIndex = 5;

      while (spreadsheet.HasValue($"A{rowIndex}")) {
        AccountEditionCommand command = TryReadCommand(spreadsheet, rowIndex);

        if (command != null) {
          commandsList.Add(command);
        }

        rowIndex++;
      }

      EmpiriaLog.Info(
        $"Se leyeron {commandsList.Count} operaciones del archivo de " +
        $"modificaciones al catálogo de cuenta contables {_excelFile.Name}."
      );

      return commandsList.ToFixedList();
    }


    private AccountEditionCommand TryReadCommand(Spreadsheet spreadsheet, int rowIndex) {
      var rowReader = new AccountsChartEditionRowReader(spreadsheet, rowIndex);

      var command = new AccountEditionCommand {
        CommandType = rowReader.GetCommandType(),
        AccountsChartUID = _chart.UID,
        ApplicationDate = _applicationDate,
        DryRun = _dryRun,
        AccountFields = rowReader.GetAccountFields(),
        Currencies = rowReader.GetCurrencies(),
        Sectors = rowReader.GetSectors(),
        DataToBeUpdated = rowReader.GetDataToBeUpdated(),
        CommandText = rowReader.GetCommandText()
      };

      if (command.CommandType == AccountEditionCommandType.UpdateAccount) {
        command.AccountUID = _chart.GetAccount(command.AccountFields.AccountNumber).UID;
      }

      return command;
    }

    #endregion Helpers

  }  // class AccountsChartEditionFileReader

}  // namespace Empiria.FinancialAccounting.AccountsChartEdition
