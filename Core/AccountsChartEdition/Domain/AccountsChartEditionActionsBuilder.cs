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

      if (_command.Sectors.Length != 0) {
        list.Add(BuildAddSectorsAction(stdAccountId));
      }

      return list.ToFixedList();
    }


    private AccountsChartEditionAction BuildAddCurrenciesAction(int stdAccountId) {
      var operations = new List<DataOperation>();

      foreach (var currency in _command.GetCurrencies()) {

        DataOperation op =
            AccountEditionDataService.AddAccountCurrencyOp(stdAccountId, currency,
                                                           _command.ApplicationDate);

        operations.Add(op);
      }

      return new AccountsChartEditionAction(_command, operations.ToFixedList());
    }


    private AccountsChartEditionAction BuildAddSectorsAction(int stdAccountId) {
      var operations = new List<DataOperation>();

      foreach (var sector in _command.GetSectors()) {

        DataOperation op =
                  AccountEditionDataService.AddAccountSectorOp(stdAccountId, sector,
                                                               _command.SectorsRole,
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
      Account account = _command.GetAccountToEdit();

      DataOperation op = AccountEditionDataService.FixStandardAccountNameOp(account,
                                                                            _command.AccountFields.Name);

      var action = new AccountsChartEditionAction(_command, op);

      return new[] { action }.ToFixedList();
    }


    private FixedList<AccountsChartEditionAction> BuildUpdateAccountActions() {
      var list = new List<AccountsChartEditionAction>();

      var dataToBeUpdated = _command.DataToBeUpdated.ToFixedList();

      if (dataToBeUpdated.Contains(AccountDataToBeUpdated.Name) ||
          dataToBeUpdated.Contains(AccountDataToBeUpdated.MainRole) ||
          dataToBeUpdated.Contains(AccountDataToBeUpdated.SubledgerRole) ||
          dataToBeUpdated.Contains(AccountDataToBeUpdated.DebtorCreditor)) {

        list.Add(BuildUpdateAccountDataAction());
      }


      if (dataToBeUpdated.Contains(AccountDataToBeUpdated.Currencies)) {
        list.Add(BuildUpdateCurrenciesAction());
      }

      if (dataToBeUpdated.Contains(AccountDataToBeUpdated.Sectors)) {
        list.Add(BuildUpdateSectorsAction());
      }

      return list.ToFixedList();
    }


    private AccountsChartEditionAction BuildUpdateAccountDataAction() {
      Account account = _command.GetAccountToEdit();

      DataOperation op = AccountEditionDataService.UpdateStandardAccountOp(account, _command);

      return new AccountsChartEditionAction(_command, op);
    }


    private AccountsChartEditionAction BuildUpdateCurrenciesAction() {
      var operations = new List<DataOperation>();

      FixedList<Currency> newCurrencies = _command.GetCurrencies();
      Account account = _command.GetAccountToEdit();

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

      FixedList<Sector> newSectors = _command.GetSectors();
      Account account = _command.GetAccountToEdit();

      foreach (var currentSectorRule in account.SectorRules) {

        if (!newSectors.Contains(x => x.Equals(currentSectorRule.Sector))) {
          DataOperation op = AccountEditionDataService.RemoveAccountSectorOp(currentSectorRule,
                                                                             _command.ApplicationDate);
          operations.Add(op);
        }
      }

      foreach (var sector in newSectors) {

        if (!account.SectorRules.Contains(x => x.Sector.Equals(sector))) {
          DataOperation op = AccountEditionDataService.AddAccountSectorOp(account.StandardAccountId, sector,
                                                                          _command.SectorsRole,
                                                                          _command.ApplicationDate);
          operations.Add(op);
        }
      }

      return new AccountsChartEditionAction(_command, operations.ToFixedList());
    }

  }  // class AccountsChartEditionActionsBuilder

}  // namespace Empiria.FinancialAccounting.AccountsChartEdition
