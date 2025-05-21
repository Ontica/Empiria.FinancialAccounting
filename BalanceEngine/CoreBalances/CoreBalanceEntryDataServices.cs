/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Data Layer                              *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Data Service                            *
*  Type     : CoreBalanceEntryDataServices               License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides data read methods for core balance entries.                                           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System.Collections.Generic;

using Empiria.Data;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine.Data {

  /// <summary>Provides data read methods for core balance entries.</summary>
  static internal class CoreBalanceEntryDataServices {

    #region Public methods

    static internal FixedList<CoreBalanceEntry> GetBalanceEntries(TrialBalanceQuery query) {
      Assertion.Require(query, nameof(query));

      BalancesSqlClauses sqlClauses = BalancesSqlClauses.BuildFrom(query);

      return new FixedList<CoreBalanceEntry>(GetBalanceEntries(sqlClauses));
    }

    #endregion Public methods

    #region Helpers

    static private List<CoreBalanceEntry> GetBalanceEntries(BalancesSqlClauses clauses) {
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

      return DataReader.GetPlainObjectList<CoreBalanceEntry>(operation);
    }

    #endregion Helpers

  } // class CoreBalanceEntryDataServices

} // namespace Empiria.FinancialAccounting.BalanceEngine.Data
