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

      //  long stdAccountId = 0;

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
      (long NewStdAccountId, FixedList<DataOperation> DataOperations) tuple =
                                          AccountEditionDataService.CreateStandardAccountOp(_command);

      var action = new AccountsChartEditionAction(_command, tuple.DataOperations);

      return (tuple.NewStdAccountId, action);
    }

  }  // class AccountsChartEditionActionsBuilder

}  // namespace Empiria.FinancialAccounting.AccountsChartEdition
