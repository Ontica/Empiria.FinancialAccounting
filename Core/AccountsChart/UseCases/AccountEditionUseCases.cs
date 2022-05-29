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

    #endregion Use cases

    #region Helpers

    private AccountEditorService GetAccountEditorService(AccountEditionCommand command) {
      Assertion.Require(command, nameof(command));

      command.EnsureValid();

      return new AccountEditorService(command);
    }

    #endregion Helpers

  }  // class AccountEditionUseCases

}  // namespace Empiria.FinancialAccounting.UseCases
