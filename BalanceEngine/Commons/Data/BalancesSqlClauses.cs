/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Data Layer                              *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Information Holder                      *
*  Type     : BalancesSqlClauses                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Holds Sql clauses used in balances data service.                                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.BalanceExplorer.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine.Data {

  /// <summary>Holds Sql clauses used in balances data service.</summary>
  sealed internal partial class BalancesSqlClauses {

    #region Constructors and parsers

    private BalancesSqlClauses() {
      // no-op
    }


    static internal BalancesSqlClauses BuildFrom(TrialBalanceCommand command) {
      Assertion.Require(command, nameof(command));

      var builder = new BalancesSqlClausesBuilder(command);

      return builder.Build();
    }


    static internal BalancesSqlClauses BuildFrom(BalanceCommand command) {
      Assertion.Require(command, nameof(command));

      TrialBalanceCommand trialBalanceCommand = CopyToTrialBalanceCommand(command);

      return BalancesSqlClauses.BuildFrom(trialBalanceCommand);
    }

    #endregion Constructors and parsers

    #region Properties

    public AccountsChart AccountsChart {
      get;
      private set;
    }

    public StoredBalanceSet StoredInitialBalanceSet {
      get;
      private set;
    }

    public DateTime FromDate {
      get;
      private set;
    }


    public DateTime ToDate {
      get;
      private set;
    }

    public string AverageBalance {
      get;
      private set;
    } = string.Empty;


    public string Fields {
      get;
      private set;
    } = string.Empty;


    public string InitialFields {
      get;
      private set;
    } = string.Empty;


    public string Filters {
      get; private set;
    } = string.Empty;


    public string AccountFilters {
      get;
      private set;
    } = string.Empty;


    public string InitialGrouping {
      get;
      private set;
    } = string.Empty;


    public string Grouping {
      get;
      private set;
    } = string.Empty;


    public string Where {
      get;
      private set;
    } = string.Empty;


    public string Ordering {
      get;
      private set;
    } = string.Empty;

    #endregion Properties

    #region Helpers

    static private TrialBalanceCommand CopyToTrialBalanceCommand(BalanceCommand command) {
      var trialBalanceCommand = new TrialBalanceCommand();

      trialBalanceCommand.AccountsChartUID = command.AccountsChartUID;
      trialBalanceCommand.FromAccount = command.FromAccount;
      trialBalanceCommand.InitialPeriod.FromDate = command.InitialPeriod.FromDate;
      trialBalanceCommand.InitialPeriod.ToDate = command.InitialPeriod.ToDate;
      trialBalanceCommand.SubledgerAccount = command.SubledgerAccount;
      trialBalanceCommand.TrialBalanceType = command.TrialBalanceType;
      trialBalanceCommand.BalancesType = command.BalancesType;
      trialBalanceCommand.Ledgers = command.Ledgers;
      trialBalanceCommand.WithSubledgerAccount = command.WithSubledgerAccount;

      return trialBalanceCommand;
    }

    #endregion Helpers

  } // class BalancesSqlClauses

} // namespace Empiria.FinancialAccounting.BalanceEngine
