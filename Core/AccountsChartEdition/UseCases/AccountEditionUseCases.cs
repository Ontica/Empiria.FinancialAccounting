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

using Empiria.FinancialAccounting.Adapters;

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


    public AccountEditionResult AddCurrencies(AccountEditionCommand command) {
      AccountEditorService editor = GetAccountEditorService(command);

      editor.AddCurrencies();

      editor.TryCommit();

      return editor.GetResult();
    }


    public AccountEditionResult AddSectors(AccountEditionCommand command) {
      AccountEditorService editor = GetAccountEditorService(command);

      editor.AddSectors();

      editor.TryCommit();

      return editor.GetResult();
    }


    public AccountEditionResult CreateAccount(AccountEditionCommand command) {
      AccountEditorService editor = GetAccountEditorService(command);

      editor.CreateAccount();

      editor.TryCommit();

      return editor.GetResult();
    }


    public AccountEditionResult RemoveAccount(AccountEditionCommand command) {
      AccountEditorService editor = GetAccountEditorService(command);

      editor.RemoveAccount();

      editor.TryCommit();

      return editor.GetResult();
    }


    public AccountEditionResult RemoveCurrencies(AccountEditionCommand command) {
      AccountEditorService editor = GetAccountEditorService(command);

      editor.RemoveCurrencies();

      editor.TryCommit();

      return editor.GetResult();
    }


    public AccountEditionResult RemoveSectors(AccountEditionCommand command) {
      AccountEditorService editor = GetAccountEditorService(command);

      editor.RemoveSectors();

      editor.TryCommit();

      return editor.GetResult();
    }


    public AccountEditionResult UpdateAccount(AccountEditionCommand command) {
      AccountEditorService editor = GetAccountEditorService(command);

      editor.UpdateAccount();

      editor.TryCommit();

      return editor.GetResult();
    }


    public FixedList<OperationSummary> UpdateFromExcelFile(UpdateAccountsFromFileCommand command,
                                                           InputFile excelFile,
                                                           bool dryRun) {
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

      var reader = new AccountsChartEditionFileReader(chart, applicationDate, excelFileInfo, dryRun);

      FixedList<AccountEditionCommand> commands = reader.GetCommands();

      var processor = new AccountsChartEditionCommandsProcessor(dryRun);

      return processor.Execute(commands);
    }


    #endregion Use cases

    #region Helpers

    private AccountEditorService GetAccountEditorService(AccountEditionCommand command) {
      Assertion.Require(command, nameof(command));

      command.EnsureValid();

      return new AccountEditorService(command);
    }

    #endregion Helpers

  }  // class AccountEditionUseCases

}  // namespace Empiria.FinancialAccounting.AccountsChartEdition.UseCases
