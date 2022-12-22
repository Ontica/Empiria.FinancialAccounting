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

      (long NewStdAccountId, AccountsChartEditionAction CreateAccountAction) tuple = BuildCreateAccountAction();

      long stdAccountId = tuple.NewStdAccountId;

      list.Add(tuple.CreateAccountAction);

      if (_command.Currencies.Length != 0) {
        list.Add(BuildAddCurrenciesAction(stdAccountId));
      }

      if (_command.Sectors.Length != 0) {
        list.Add(BuildAddSectorsAction(stdAccountId));
      }

      return list.ToFixedList();
    }


    private AccountsChartEditionAction BuildAddCurrenciesAction(long stdAccountId) {
      var operations = new List<DataOperation>();

      foreach (var currency in _command.GetCurrencies()) {

        DataOperation op =
            AccountEditionDataService.AddAccountCurrencyOp(stdAccountId, currency,
                                                           _command.ApplicationDate);

        operations.Add(op);
      }

      return new AccountsChartEditionAction(_command, operations.ToFixedList());
    }


    private AccountsChartEditionAction BuildAddSectorsAction(long stdAccountId) {
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


    private (long, AccountsChartEditionAction) BuildCreateAccountAction() {
      (long NewStdAccountId, DataOperation DataOperation) tuple =
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

      Account account = _command.GetAccountToEdit();

      var dataToBeUpdated = _command.DataToBeUpdated.ToFixedList();

      if (dataToBeUpdated.Contains(AccountDataToBeUpdated.Currencies)) {
        list.Add(BuildUpdateCurrenciesAction(account));
      }

      if (dataToBeUpdated.Contains(AccountDataToBeUpdated.Sectors)) {
        list.Add(BuildUpdateSectorsAction(account));
      }

      return list.ToFixedList();
    }


    private AccountsChartEditionAction BuildUpdateCurrenciesAction(Account account) {
      var operations = new List<DataOperation>();

      var newCurrencies = _command.GetCurrencies();

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


    private AccountsChartEditionAction BuildUpdateSectorsAction(Account account) {
      var operations = new List<DataOperation>();

      var newSectors = _command.GetSectors();

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
