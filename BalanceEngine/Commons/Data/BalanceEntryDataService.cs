/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Data Layer                              *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Data Service                            *
*  Type     : BalanceEntryDataService                    License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides data read methods for balance entries.                                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System.Collections.Generic;
using Empiria.Data;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine.Data {

  /// <summary>Provides data read methods for balance entries.</summary>
  static internal class BalanceEntryDataService {

    #region Public methods

    static internal FixedList<BalanceEntry> GetBalanceEntries(TrialBalanceQuery query) {
      Assertion.Require(query, nameof(query));

      BalancesSqlClauses sqlClauses = BalancesSqlClauses.BuildFrom(query);

      return new FixedList<BalanceEntry>(GetBalanceEntries(sqlClauses));
    }

    #endregion Public methods

    #region Helpers

    static private List<BalanceEntry> GetBalanceEntries(BalancesSqlClauses clauses) {
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

      return DataReader.GetPlainObjectList<BalanceEntry>(operation);
    }

    #endregion Helpers

  } // class BalanceEntryDataService

} // namespace Empiria.FinancialAccounting.BalanceEngine.Data
