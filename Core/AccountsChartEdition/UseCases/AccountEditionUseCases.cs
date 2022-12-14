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


    public FixedList<OperationSummary> UpdateFromExcelFile(BaseAccountEditionCommand command,
                                                          InputFile excelFile,
                                                          bool dryRun) {
      Assertion.Require(command, nameof(command));
      Assertion.Require(excelFile, nameof(excelFile));

      AccountsChart chart = command.GetAccountsChart();
      DateTime applicationDate = command.ApplicationDate;

      Assertion.Require(DateTime.Today.AddDays(1) <= applicationDate &&
                        applicationDate <= DateTime.Today.AddDays(5) &&
                        applicationDate < chart.MasterData.EndDate,
                        "La fecha de aplicación de los cambios en el catálogo " +
                        "debe ser a partir de mañana y hasta 5 días después del día de hoy.");

      var reader = new AccountsChartEditionFileReader(chart, applicationDate, excelFile, dryRun);

      FixedList<AccountEditionCommand> commands = reader.GetCommands();

      if (!dryRun) {

        var processor = new AccountsChartEditionCommandsProcessor();

        processor.Execute(commands);
      }

      return new FixedList<OperationSummary>();
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
