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

      long stdAccountId = 0;
      long stdAccountHistoryId = 0;

      if (!_command.DryRun) {
        stdAccountId = AccountEditionDataService.GetNextStandardAccountId();
        stdAccountHistoryId = AccountEditionDataService.GetNextStandardAccountHistoryId();
      }

      list.Add(BuildCreateAccountAction(stdAccountId));
      list.Add(BuildCreateHistoricAccountAction(stdAccountId, stdAccountHistoryId));

      if (_command.Currencies.Length != 0) {
        list.Add(BuildAddCurrenciesAction(stdAccountId));
      }

      if (_command.Sectors.Length != 0) {
        list.Add(BuildAddSectorsAction(stdAccountId));
      }

      return list.ToFixedList();
    }


    private AccountsChartEditionAction BuildAddCurrenciesAction(long newAccountId) {
      var operations = new List<DataOperation>();

      foreach (var currency in _command.GetCurrencies()) {

        var op = DataOperation.Parse("write_cof_mapeo_moneda",
                    newAccountId, currency.Id,
                    _command.ApplicationDate, Account.MAX_END_DATE);

        operations.Add(op);
      }

      return new AccountsChartEditionAction(_command, operations.ToFixedList());
    }


    private AccountsChartEditionAction BuildAddSectorsAction(long stdAccountId) {
      var operations = new List<DataOperation>();

      foreach (var sector in _command.GetSectors()) {

        DataOperation op = AccountEditionDataService.AddAccountSectorOp(
                                  stdAccountId, sector,
                                  _command.SectorsRole,
                                  _command.ApplicationDate);

        operations.Add(op);
      }

      return new AccountsChartEditionAction(_command, operations.ToFixedList());
    }


    private AccountsChartEditionAction BuildCreateAccountAction(long stdAccountId) {
      DataOperation op = AccountEditionDataService.WriteStandardAccountOp(stdAccountId,
                                                                          _command);

      return new AccountsChartEditionAction(_command, op);
    }


    private AccountsChartEditionAction BuildCreateHistoricAccountAction(long stdAccountId,
                                                                        long stdAccountHistoryId) {
      FixedList<DataOperation> op =
                      AccountEditionDataService.WriteStandardAccountHistoryOp(stdAccountId,
                                                                              stdAccountHistoryId,
                                                                              _command);

      return new AccountsChartEditionAction(_command, op);
    }


    private FixedList<AccountsChartEditionAction> BuildUpdateAccountActions() {
      return new FixedList<AccountsChartEditionAction>();
    }

  }  // class AccountsChartEditionActionsBuilder

}  // namespace Empiria.FinancialAccounting.AccountsChartEdition
