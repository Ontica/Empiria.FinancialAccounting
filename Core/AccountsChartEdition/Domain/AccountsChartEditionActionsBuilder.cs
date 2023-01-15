/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart Edition                     Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Service provider                        *
*  Type     : AccountsChartEditionActionsBuilder         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Transforms an account edition command in a set of AccountsChartEditionAction items.            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.Data;

using Empiria.FinancialAccounting.AccountsChartEdition.Adapters;
using Empiria.FinancialAccounting.AccountsChartEdition.Data;

namespace Empiria.FinancialAccounting.AccountsChartEdition {

  /// <summary>Transforms an account edition command in a set of AccountsChartEditionAction items.</summary>
  internal class AccountsChartEditionActionsBuilder {

    private readonly AccountEditionCommand _command;

    public AccountsChartEditionActionsBuilder(AccountEditionCommand command) {
      Assertion.Require(command, nameof(command));

      _command = command;

      // no-op
    }


    internal FixedList<AccountsChartEditionAction> BuildActions() {

      if (_command.CommandType == AccountEditionCommandType.CreateAccount) {
        return BuildCreateAccountActions();
      }

      if (_command.CommandType == AccountEditionCommandType.DeleteAccount) {
        return BuildDeleteAccountActions();
      }

      if (_command.CommandType == AccountEditionCommandType.FixAccountName) {
        return BuildFixAccountNameActions();
      }

      if (_command.CommandType == AccountEditionCommandType.UpdateAccount) {
        return BuildUpdateAccountActions();
      }

      throw Assertion.EnsureNoReachThisCode();
    }


    private FixedList<AccountsChartEditionAction> BuildCreateAccountActions() {
      var list = new List<AccountsChartEditionAction>();

      (int NewStdAccountId, AccountsChartEditionAction CreateAccountAction) tuple = BuildCreateAccountAction();

      int stdAccountId = tuple.NewStdAccountId;

      list.Add(tuple.CreateAccountAction);

      if (_command.Currencies.Length != 0) {
        list.Add(BuildAddCurrenciesAction(stdAccountId));
      }

      if (_command.SectorRules.Length != 0) {
        list.Add(BuildAddSectorRulesAction(stdAccountId));
      }

      return list.ToFixedList();
    }


    private FixedList<AccountsChartEditionAction> BuildDeleteAccountActions() {

      Account account = _command.Entities.Account;

      DataOperation op = AccountEditionDataService.DeleteAccountOp(account);

      var action = new AccountsChartEditionAction(_command, op);

      return new[] { action }.ToFixedList();
    }


    private AccountsChartEditionAction BuildAddCurrenciesAction(int stdAccountId) {
      var operations = new List<DataOperation>();

      foreach (var currency in _command.Entities.GetCurrencies()) {

        DataOperation op =
            AccountEditionDataService.AddAccountCurrencyOp(stdAccountId, currency,
                                                           _command.ApplicationDate);

        operations.Add(op);
      }

      return new AccountsChartEditionAction(_command, operations.ToFixedList());
    }


    private AccountsChartEditionAction BuildAddSectorRulesAction(int stdAccountId) {
      var operations = new List<DataOperation>();

      foreach (var sectorRule in _command.Entities.GetSectorRules()) {

        DataOperation op =
                  AccountEditionDataService.AddAccountSectorOp(stdAccountId,
                                                               sectorRule.Sector,
                                                               sectorRule.Role,
                                                               _command.ApplicationDate);
        operations.Add(op);
      }

      return new AccountsChartEditionAction(_command, operations.ToFixedList());
    }


    private (int, AccountsChartEditionAction) BuildCreateAccountAction() {
      (int NewStdAccountId, DataOperation DataOperation) tuple =
                                          AccountEditionDataService.CreateStandardAccountOp(_command);

      var action = new AccountsChartEditionAction(_command, tuple.DataOperation);

      return (tuple.NewStdAccountId, action);
    }


    private FixedList<AccountsChartEditionAction> BuildFixAccountNameActions() {
      Account account = _command.Entities.Account;

      DataOperation op = AccountEditionDataService.FixStandardAccountNameOp(account,
                                                                            _command.AccountFields.Name);

      var action = new AccountsChartEditionAction(_command, op);

      return new[] { action }.ToFixedList();
    }


    private FixedList<AccountsChartEditionAction> BuildUpdateAccountActions() {
      var list = new List<AccountsChartEditionAction>();

      var dataToBeUpdated = _command.DataToBeUpdated.ToFixedList();

      bool sectorsRoleChanged = false;

      if (dataToBeUpdated.Contains(AccountDataToBeUpdated.Name) ||
          dataToBeUpdated.Contains(AccountDataToBeUpdated.DebtorCreditor) ||
          dataToBeUpdated.Contains(AccountDataToBeUpdated.AccountType) ||
          dataToBeUpdated.Contains(AccountDataToBeUpdated.MainRole)) {

        list.Add(BuildUpdateAccountDataAction());

      } else if (dataToBeUpdated.Contains(AccountDataToBeUpdated.SubledgerRole) &&
                 !_command.Entities.Account.Role.Equals(_command.AccountFields.Role)) {
        list.Add(BuildUpdateAccountDataAction());

      } else if (dataToBeUpdated.Contains(AccountDataToBeUpdated.SubledgerRole) &&
                 _command.Entities.Account.Role == AccountRole.Sectorizada &&
                 _command.Entities.Account.SectorRules[0].SectorRole != _command.SectorRules[0].Role) {
        sectorsRoleChanged = true;
      }

      if (dataToBeUpdated.Contains(AccountDataToBeUpdated.Currencies)) {
        list.Add(BuildUpdateCurrenciesAction());
      }

      if (dataToBeUpdated.Contains(AccountDataToBeUpdated.Sectors) || sectorsRoleChanged) {
        list.Add(BuildUpdateSectorsAction());
      }

      return list.ToFixedList();
    }


    private AccountsChartEditionAction BuildUpdateAccountDataAction() {
      Account account = _command.Entities.Account;

      DataOperation op = AccountEditionDataService.UpdateStandardAccountOp(account, _command);

      return new AccountsChartEditionAction(_command, op);
    }


    private AccountsChartEditionAction BuildUpdateCurrenciesAction() {
      var operations = new List<DataOperation>();

      FixedList<Currency> newCurrencies = _command.Entities.GetCurrencies();
      Account account = _command.Entities.Account;

      foreach (var currentCurrencyRule in account.CurrencyRules) {

        if (!newCurrencies.Contains(x => x.Equals(currentCurrencyRule.Currency))) {
          DataOperation op = AccountEditionDataService.RemoveAccountCurrencyOp(currentCurrencyRule,
                                                                              _command.ApplicationDate);
          operations.Add(op);
        }
      }

      foreach (var currency in newCurrencies) {

        if (!account.CurrencyRules.Contains(x => x.Currency.Equals(currency))) {
          DataOperation op = AccountEditionDataService.AddAccountCurrencyOp(account.StandardAccountId, currency,
                                                                            _command.ApplicationDate);
          operations.Add(op);
        }
      }

      return new AccountsChartEditionAction(_command, operations.ToFixedList());
    }


    private AccountsChartEditionAction BuildUpdateSectorsAction() {
      var operations = new List<DataOperation>();

      FixedList<SectorInputRuleDto> newSectorRules = _command.Entities.GetSectorRules();
      Account account = _command.Entities.Account;

      foreach (var currentSectorRule in account.SectorRules) {

        if (!newSectorRules.Contains(x => x.Sector.Equals(currentSectorRule.Sector) &&
                                          x.Role.Equals(currentSectorRule.SectorRole))) {
          DataOperation op = AccountEditionDataService.RemoveAccountSectorOp(currentSectorRule,
                                                                             _command.ApplicationDate);
          operations.Add(op);
        }
      }

      foreach (var sectorRule in newSectorRules) {

        if (!account.SectorRules.Contains(x => x.Sector.Equals(sectorRule.Sector) &&
                                               x.SectorRole.Equals(sectorRule.Role))) {
          DataOperation op = AccountEditionDataService.AddAccountSectorOp(account.StandardAccountId,
                                                                          sectorRule.Sector,
                                                                          sectorRule.Role,
                                                                          _command.ApplicationDate);
          operations.Add(op);
        }
      }

      return new AccountsChartEditionAction(_command, operations.ToFixedList());
    }

  }  // class AccountsChartEditionActionsBuilder

}  // namespace Empiria.FinancialAccounting.AccountsChartEdition
