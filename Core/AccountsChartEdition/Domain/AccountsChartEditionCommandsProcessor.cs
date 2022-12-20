/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart Edition                     Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Service provider                        *
*  Type     : AccountsChartEditionCommandsProcessor      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Processes a set of chart of accounts edition commands.                                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;

using Empiria.Data;

using Empiria.FinancialAccounting.AccountsChartEdition.Adapters;
using Empiria.FinancialAccounting.AccountsChartEdition.Data;

namespace Empiria.FinancialAccounting.AccountsChartEdition {

  /// <summary>Processes a set of chart of accounts edition commands.</summary>
  internal class AccountsChartEditionCommandsProcessor {

    private readonly bool _dryRun;

    public AccountsChartEditionCommandsProcessor(bool dryRun) {
      _dryRun = dryRun;
    }

    internal FixedList<OperationSummary> Execute(FixedList<AccountEditionCommand> commands) {

      foreach (var command in commands) {
        command.Arrange();

        if (command.CommandType == AccountEditionCommandType.CreateAccount) {
          command.EnsureCanCreateAccount(commands);
        }
      }

      var allActions = new List<AccountsChartEditionAction>(128);

      foreach (var command in commands) {
        var actionsCreator = new AccountsChartEditionActionsBuilder(command);

        FixedList<AccountsChartEditionAction> commandActions = actionsCreator.BuildActions();

        allActions.AddRange(commandActions);
      }

      if (!_dryRun) {
        ProcessActions(allActions.ToFixedList());
        commands.First().GetAccountsChart().Refresh();
      }

      return CreateOperationSummaryList(commands);
    }

    private void ProcessActions(FixedList<AccountsChartEditionAction> allActions) {
      var list = new DataOperationList("AccountsChartEdition");

      foreach (var action in allActions) {
        list.Add(action.DataOperations);
      }

      AccountEditionDataService.Execute(list);
    }


    private FixedList<OperationSummary> CreateOperationSummaryList(FixedList<AccountEditionCommand> commands) {
      var list = new List<OperationSummary>();

      foreach (var summaryGroup in commands.Select(x => x.CommandText)
                                           .Distinct()) {

        var summary = new OperationSummary();

        summary.Operation = summaryGroup;

        foreach (var command in commands.FindAll(x => x.CommandText == summaryGroup)) {
          summary.Count++;
          summary.AddItem("Cuenta " + command.AccountFields.AccountNumber);
          summary.AddErrors(command.Issues);
        }

        list.Add(summary);
      }

      return list.ToFixedList();
    }

  }  // class AccountsChartEditionCommandsProcessor

}  // namespace Empiria.FinancialAccounting.AccountsChartEdition
