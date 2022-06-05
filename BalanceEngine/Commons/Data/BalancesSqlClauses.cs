/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Data Layer                              *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Data Holder                             *
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


    // This builder uses the trial balances query to generate balances sql clauses
    static internal BalancesSqlClauses BuildFrom(TrialBalanceQuery query) {
      Assertion.Require(query, nameof(query));

      var builder = new BalancesSqlClausesBuilder(query);

      return builder.Build();
    }


    static internal BalancesSqlClauses BuildFrom(BalanceExplorerQuery query) {
      Assertion.Require(query, nameof(query));

      TrialBalanceQuery convertedToTrialBalanceQuery = ConvertToTrialBalanceQuery(query);

      return BalancesSqlClauses.BuildFrom(convertedToTrialBalanceQuery);
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

    static private TrialBalanceQuery ConvertToTrialBalanceQuery(BalanceExplorerQuery query) {
      return new TrialBalanceQuery {
        AccountsChartUID      = query.AccountsChartUID,
        FromAccount           = query.FromAccount,
        SubledgerAccount      = query.SubledgerAccount,
        TrialBalanceType      = query.TrialBalanceType,
        BalancesType          = query.BalancesType,
        Ledgers               = query.Ledgers,
        WithSubledgerAccount  = query.WithSubledgerAccount,
        InitialPeriod = new BalancesPeriod {
          FromDate  = query.InitialPeriod.FromDate,
          ToDate    = query.InitialPeriod.ToDate,
        }
      };
    }

    #endregion Helpers

  } // class BalancesSqlClauses

} // namespace Empiria.FinancialAccounting.BalanceEngine
