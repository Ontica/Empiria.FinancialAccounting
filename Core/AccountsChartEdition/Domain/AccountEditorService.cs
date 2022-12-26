/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart Edition                     Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Service provider                        *
*  Type     : AccountEditorService                       License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides account edition services.                                                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;

using Empiria.FinancialAccounting.AccountsChartEdition.Adapters;

using Empiria.FinancialAccounting.AccountsChartEdition.Data;

namespace Empiria.FinancialAccounting.AccountsChartEdition {

  /// <summary>Provides account edition services.</summary>
  internal class AccountEditorService {

    #region Fields

    private readonly AccountEditionCommand _command;
    private readonly AccountEditionValidator _validator;
    private readonly AccountActionFactory _actionBuilder;
    private readonly List<AccountAction> _actionsList;

    #endregion Fields

    #region Constructor

    internal AccountEditorService(AccountEditionCommand command) {
      Assertion.Require(command, nameof(command));

      _command = command;

      _validator = new AccountEditionValidator(_command);
      _actionBuilder = new AccountActionFactory(_command);
      _actionsList = new List<AccountAction>(16);
    }

    #endregion Constructor

    #region Operations

    internal void AddCurrencies() {
      Account account = _command.GetAccountToEdit();

      if (!_validator.EnsureCanAddCurrenciesTo(account)) {
        return;
      }

      foreach (Currency currency in _command.GetCurrencies()) {
        AccountAction operation = _actionBuilder.BuildForAddCurrency(account, currency);

        _actionsList.Add(operation);

      }  // foreach
    }


    internal void AddSectors() {
      Account account = _command.GetAccountToEdit();

      if (!_validator.EnsureCanAddSectorsTo(account)) {
        return;
      }

      foreach (SectorInputRuleDto sectorRule in _command.GetSectorRules()) {
        AccountAction action = _actionBuilder.BuildForAddSector(account, sectorRule);

        _actionsList.Add(action);

      }  // foreach
    }


    internal void CreateAccount() {
      if (!_validator.EnsureCanCreateAccount()) {
        return;
      }

      AccountFieldsDto fields = _command.AccountFields;

      AccountAction action = _actionBuilder.BuildForCreateAccount(fields);

      _actionsList.Add(action);
    }


    internal void RemoveAccount() {
      throw new NotImplementedException();
    }


    internal void RemoveCurrencies() {
      throw new NotImplementedException();
    }


    internal void RemoveSectors() {
      throw new NotImplementedException();
    }


    internal void UpdateAccount() {
      throw new NotImplementedException();
    }

    #endregion Operations

    #region Commit and return result

    public bool Commited {
      get; private set;
    }


    internal AccountEditionResult GetResult() {
      FixedList<string> actions = _actionsList.Select(x => x.Description)
                                              .ToFixedList();

      Account account = Account.Empty;

      if (_command.CommandType != AccountEditionCommandType.CreateAccount) {
        account = _command.GetAccountToEdit();
      }

      return new AccountEditionResult(_command, account, actions, _validator.Issues);
    }


    internal void TryCommit() {
      if (_command.DryRun) {
        return;
      }

      Assertion.Require(_validator.Issues.Count == 0,
              "There are one or more issues. Please retry sending the same command with a dry run.");

      Assertion.Require(_actionsList.Count != 0,
              "Actions list is empty. There are not actions to execute.");

      Assertion.Require(!this.Commited,
              "Command operations were already commited.");


      ExecuteCommit();


      void ExecuteCommit() {

        foreach (AccountAction action in _actionsList) {
          AccountEditionDataService.Execute(action.DataOperations);
        }

        this.Commited = true;

      } // ExecuteCommit()

    }  // TryCommit()


    #endregion Commit and return result

  }  // AccountEditorService

}  // namespace Empiria.FinancialAccounting.AccountsChartEdition
