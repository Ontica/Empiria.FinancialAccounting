/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart                             Component : Use cases Layer                         *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Use case interactor class               *
*  Type     : AccountEditionUseCases                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases used to create or update an account in a chart of accounts.                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.FinancialAccounting.Adapters;
using Empiria.FinancialAccounting.Data;

namespace Empiria.FinancialAccounting.UseCases {

  /// <summary>Use cases used to create or update an account in a chart of accounts.</summary>
  public class AccountEditionUseCases : UseCase {

    #region Constructors and parsers

    protected AccountEditionUseCases() {
      // no-op
    }

    static public AccountEditionUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<AccountEditionUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases

    public void CleanupAccounts() {
      AccountsChartData.TestViews();
    }


    public AccountEditionResult AddCurrencies(string accountsChartUID, AccountEditionCommand command) {
      AccountEditorService editor = GetAccountEditorService(accountsChartUID, command);

      editor.AddCurrencies();

      editor.TryCommit();

      return editor.GetResult();
    }


    public AccountEditionResult AddSectors(string accountsChartUID, AccountEditionCommand command) {
      AccountEditorService editor = GetAccountEditorService(accountsChartUID, command);

      editor.AddSectors();

      editor.TryCommit();

      return editor.GetResult();
    }


    public AccountEditionResult CreateAccount(string accountsChartUID, AccountEditionCommand command) {
      AccountEditorService editor = GetAccountEditorService(accountsChartUID, command);

      editor.CreateAccount();

      editor.TryCommit();

      return editor.GetResult();
    }


    public AccountEditionResult RemoveAccount(string accountsChartUID, AccountEditionCommand command) {
      AccountEditorService editor = GetAccountEditorService(accountsChartUID, command);

      editor.RemoveAccount();

      editor.TryCommit();

      return editor.GetResult();
    }


    public AccountEditionResult RemoveCurrencies(string accountsChartUID, AccountEditionCommand command) {
      AccountEditorService editor = GetAccountEditorService(accountsChartUID, command);

      editor.RemoveCurrencies();

      editor.TryCommit();

      return editor.GetResult();
    }


    public AccountEditionResult RemoveSectors(string accountsChartUID, AccountEditionCommand command) {
      AccountEditorService editor = GetAccountEditorService(accountsChartUID, command);

      editor.RemoveSectors();

      editor.TryCommit();

      return editor.GetResult();
    }


    public AccountEditionResult UpdateAccount(string accountsChartUID, AccountEditionCommand command) {
      AccountEditorService editor = GetAccountEditorService(accountsChartUID, command);

      editor.UpdateAccount();

      editor.TryCommit();

      return editor.GetResult();
    }

    #endregion Use cases

    #region Helpers

    private AccountEditorService GetAccountEditorService(string accountsChartUID,
                                                         AccountEditionCommand command) {
      Assertion.AssertObject(accountsChartUID, nameof(accountsChartUID));
      Assertion.AssertObject(command, nameof(command));

      var accountsChart = AccountsChart.Parse(accountsChartUID);

      return new AccountEditorService(accountsChart, command);
    }

    #endregion Helpers

  }  // class AccountEditionUseCases

}  // namespace Empiria.FinancialAccounting.UseCases
