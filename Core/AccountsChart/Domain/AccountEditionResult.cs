/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Immutable Type                          *
*  Type     : AccountEditionResult                       License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Holds information about the result of an account edition.                                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.FinancialAccounting.Adapters;

namespace Empiria.FinancialAccounting {

  /// <summary>Holds information about the result of an account edition.</summary>
  public class AccountEditionResult {

    #region Constructor

    internal AccountEditionResult(AccountEditionCommand command, Account account,
                                  FixedList<string> actions, FixedList<string> issues) {
      Assertion.Require(command, nameof(command));
      Assertion.Require(account, nameof(account));
      Assertion.Require(actions, nameof(actions));
      Assertion.Require(issues, nameof(issues));

      this.Command = command;
      this.Account = AccountsChartMapper.MapAccount(account);
      this.Actions = actions;
      this.Issues = issues;
    }

    #endregion Constructor

    #region Properties

    public AccountDto Account {
      get;
    }

    public FixedList<string> Actions {
      get;
    }

    public FixedList<string> Issues {
      get;
    }

    public AccountEditionCommand Command {
      get;
    }

    #endregion Properties

  }  // class AccountEditionResult

}  // namespace Empiria.FinancialAccounting
