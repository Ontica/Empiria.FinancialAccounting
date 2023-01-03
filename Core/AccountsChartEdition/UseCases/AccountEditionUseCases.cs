/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart Edition                     Component : Use cases Layer                         *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Use case interactor class               *
*  Type     : AccountEditionUseCases                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases used to create or update accounts in a chart of accounts.                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.IO;

using Empiria.Services;
using Empiria.Storage;

using Empiria.FinancialAccounting.AccountsChartEdition.Adapters;

namespace Empiria.FinancialAccounting.AccountsChartEdition.UseCases {

  /// <summary>Use cases used to create or update accounts in a chart of accounts.</summary>
  public class AccountEditionUseCases : UseCase {

    #region Constructors and parsers

    protected AccountEditionUseCases() {
      // no-op
    }

    static public AccountEditionUseCases UseCaseInteractor() {
      return CreateInstance<AccountEditionUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases


    public OperationSummary ExecuteCommand(AccountEditionCommand command) {
      Assertion.Require(command, nameof(command));

      command.Arrange();

      var processor = new AccountsChartEditionCommandsProcessor();

      return processor.Execute(command);
    }


    public FixedList<OperationSummary> ExecuteCommandsFromExcelFile(UpdateAccountsFromFileCommand command,
                                                                    InputFile excelFile) {
      Assertion.Require(command, nameof(command));
      Assertion.Require(excelFile, nameof(excelFile));

      AccountsChart chart = command.GetAccountsChart();
      DateTime applicationDate = command.ApplicationDate;

      Assertion.Require(chart.MasterData.StartDate <= applicationDate &&
                        applicationDate < chart.MasterData.EndDate,
                 $"La fecha de aplicación de cambios al catálogo {chart.Name} " +
                 $"debe estar los días {chart.MasterData.StartDate.ToString("dd/MMMM/yyyy")} " +
                 $"y {chart.MasterData.EndDate.ToShortDateString()}.");

      FileInfo excelFileInfo = FileUtilities.SaveFile(excelFile);

      var reader = new AccountsChartEditionFileReader(chart, applicationDate, excelFileInfo, command.DryRun);

      FixedList<AccountEditionCommand> commands = reader.GetCommands();

      var processor = new AccountsChartEditionCommandsProcessor();

      return processor.Execute(commands, command.DryRun);
    }

    #endregion Use cases

  }  // class AccountEditionUseCases

}  // namespace Empiria.FinancialAccounting.AccountsChartEdition.UseCases
