/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart Edition                     Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Service provider                        *
*  Type     : AccountEditionCommandValidator             License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides services to validate account edition commands.                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.FinancialAccounting.AccountsChartEdition.Adapters;

namespace Empiria.FinancialAccounting.AccountsChartEdition {

  /// <summary>Provides services to validate account edition commands.</summary>
  internal class AccountEditionCommandValidator {

    private readonly AccountEditionCommand _command;
    private readonly List<string> _issues;

    internal AccountEditionCommandValidator(AccountEditionCommand command) {
      Assertion.Require(command, nameof(command));

      _command = command;
      _issues = new List<string>();
    }


    #region Public methods

    internal FixedList<string> GetIssues() {
      InitialRequire();

      _issues.Clear();

      switch (_command.CommandType) {
        case AccountEditionCommandType.CreateAccount:
          SetCreateAccountIssues();
          break;

        case AccountEditionCommandType.DeleteAccount:
          SetDeleteAccountIssues();
          break;

        case AccountEditionCommandType.FixAccountName:
          SetFixAccountNameIssues();
          break;

        case AccountEditionCommandType.UpdateAccount:
          SetUpdateAccountNameIssues();
          break;

        default:
          throw Assertion.EnsureNoReachThisCode($"Unhandled command type '{_command.CommandType}'.");
      }

      return _issues.ToFixedList();
    }


    internal void InitialRequire() {
      Assertion.Require(_command.CommandType != AccountEditionCommandType.Undefined, "CommandType");
      Assertion.Require(_command.AccountsChartUID, "AccountsChartUID");
      Assertion.Require(_command.ApplicationDate != ExecutionServer.DateMinValue, "ApplicationDate");

      Assertion.Require(_command.AccountFields, "AccountFields");
      Assertion.Require(_command.AccountFields.AccountNumber, "AccountFields.AccountNumber");
      Assertion.Require(_command.AccountFields.Name, "AccountFields.Name");
      Assertion.Require(_command.AccountFields.AccountTypeUID, "AccountFields.AccountTypeUID");
      Assertion.Require(_command.AccountFields.Role != AccountRole.Undefined,
                        "AccountFields.Role");
      Assertion.Require(_command.AccountFields.DebtorCreditor != DebtorCreditorType.Undefined,
                        "AccountFields.DebtorCreditor");

      switch (_command.CommandType) {
        case AccountEditionCommandType.CreateAccount:
          Assertion.Require(_command.AccountUID.Length == 0,
              "command.AccountUID was provided but it's not needed for a CreateAccount command.");
          return;

        case AccountEditionCommandType.DeleteAccount:
        case AccountEditionCommandType.FixAccountName:
          RequireAccountToEditIsValid();
          return;

        case AccountEditionCommandType.UpdateAccount:
          RequireAccountToEditIsValid();

          Assertion.Require(_command.DataToBeUpdated, "DataToBeUpdated");
          Assertion.Require(_command.DataToBeUpdated.Length != 0,
                            "DataToBeUpdated must contain one or more values.");
          return;

        default:
          throw Assertion.EnsureNoReachThisCode($"Unhandled command type '{_command.CommandType}'.");
      }

      void RequireAccountToEditIsValid() {
        Assertion.Require(_command.AccountUID, "AccountUID");

        AccountsChart chart = _command.Entities.AccountsChart;
        Account account = _command.Entities.Account;

        Assertion.Require(account.AccountsChart.Equals(chart),
            $"Account to be edited '{account.Number}' does not belong to " +
            $"the chart of accounts {chart.Name}.");

        Assertion.Require(account.EndDate == Account.MAX_END_DATE,
            "The given accountUID corresponds to an historic account, so it can not be edited.");

        Assertion.Require(account.Number.Equals(_command.AccountFields.AccountNumber),
            $"ApplicationDate ({_command.ApplicationDate.ToString("dd/MMM/yyyy")}) " +
            $"must be greater or equal than the given account's " +
            $"start date {account.StartDate.ToString("dd/MMM/yyyy")}.");
      }
    }

    #endregion Public methods

    #region Main validators

    private void SetCreateAccountIssues() {
      AccountsChart accountsChart = _command.Entities.AccountsChart;

      Require(accountsChart.IsValidAccountNumber(_command.AccountFields.AccountNumber),
              $"La cuenta '{_command.AccountFields.AccountNumber}' " +
              $"tiene un formato que no reconozco.");
    }


    private void SetDeleteAccountIssues() {
      Account account = _command.Entities.Account;

      Require(account.GetChildren().Count != 0,
             "La cuenta no se puede eliminar porque tiene cuentas hijas");

      Require(account.LedgerRules.Count == 0,
              "La cuenta no se puede eliminar porque ya fue asignada a una o más contabilidades");

    }


    private void SetFixAccountNameIssues() {
      Account account = _command.Entities.Account;

      Require(!account.Name.Equals(_command.AccountFields.Name),
              $"La descripción de la cuenta es igual a la que ya está registrada.");
    }


    private void SetUpdateAccountNameIssues() {
      Account account = _command.Entities.Account;

      Require(account.StartDate <= _command.ApplicationDate,
              $"El último cambio de la cuenta fue el día {account.StartDate.ToString("dd/MMM/yyyy")}, " +
              $"por lo que la fecha de aplicación de los cambios debe ser posterior a ese día.");
    }

    #endregion Main validators

    #region Helpers

    private void Require(bool condition, string issue) {
      if (!condition) {
        _issues.Add(issue);
      }
    }

    #endregion Helpers

  }  // class AccountEditionCommandValidator

}  // namespace Empiria.FinancialAccounting.AccountsChartEdition
