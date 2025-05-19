/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Data Layer                              *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Data Service                            *
*  Type     : BalancesDataService                        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides data read methods for balances.                                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System.Collections.Generic;

using Empiria.Data;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.BalanceExplorer.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine.Data {

  /// <summary>Provides data read methods for balances.</summary>
  static internal class BalancesDataService {

    #region Public methods

    static internal FixedList<TrialBalanceEntry> GetTrialBalanceEntries(TrialBalanceQuery query) {
      Assertion.Require(query, nameof(query));

      BalancesSqlClauses sqlClauses = BalancesSqlClauses.BuildFrom(query);

      var trialBalanceList = GetTrialBalanceEntries(sqlClauses);

      FixedList<TrialBalanceEntry> balancesAndFlattenedAccounts =
        BalanceDataServiceClauses.GetBalancesWithFlattenedAccounts(query, trialBalanceList);

      return new FixedList<TrialBalanceEntry>(balancesAndFlattenedAccounts);
    }


    static internal FixedList<TrialBalanceEntry> GetTrialBalanceForBalancesExplorer(
                                                  BalanceExplorerQuery query) {
      Assertion.Require(query, nameof(query));

      BalancesSqlClauses sqlClauses = BalancesSqlClauses.BuildFrom(query);

      return GetTrialBalanceEntries(sqlClauses).ToFixedList();
    }


    #endregion Public methods

    #region Helpers

    
    static private List<TrialBalanceEntry> GetTrialBalanceEntries(BalancesSqlClauses clauses) {
      var operation = DataOperation.Parse("@qryTrialBalance",
                            DataCommonMethods.FormatSqlDbDate(clauses.StoredInitialBalanceSet.BalancesDate),
                            DataCommonMethods.FormatSqlDbDate(clauses.FromDate),
                            DataCommonMethods.FormatSqlDbDate(clauses.ToDate),
                            clauses.StoredInitialBalanceSet.Id,
                            clauses.Fields,
                            clauses.Filters,
                            clauses.AccountFilters,
                            clauses.Where,
                            clauses.Ordering,
                            clauses.InitialFields,
                            clauses.InitialGrouping,
                            clauses.AccountsChart.Id,
                            clauses.AverageBalance
                            );

      return DataReader.GetPlainObjectList<TrialBalanceEntry>(operation);
    }

    #endregion Helpers

  } // class BalancesDataService

} // namespace Empiria.FinancialAccounting.BalanceEngine.Data
