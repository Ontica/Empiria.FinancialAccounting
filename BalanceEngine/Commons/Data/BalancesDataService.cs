/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Data Layer                              *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Data Service                            *
*  Type     : BalancesDataService                        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides data read methods for balances.                                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Data;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.BalanceExplorer.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine.Data {

  /// <summary>Provides data read methods for balances.</summary>
  static internal class BalancesDataService {


    static internal FixedList<TrialBalanceEntry> GetTrialBalanceEntries(BalancesQuery query) {
      Assertion.Require(query, nameof(query));

      BalancesSqlClauses sqlClauses = BalancesSqlClauses.BuildFrom(query);

      return GetTrialBalanceEntries(sqlClauses);
    }


    static internal FixedList<TrialBalanceEntry> GetTrialBalanceEntries(TrialBalanceQuery query) {
      Assertion.Require(query, nameof(query));

      BalancesSqlClauses sqlClauses = BalancesSqlClauses.BuildFrom(query);

      return GetTrialBalanceEntries(sqlClauses);
    }


    static private FixedList<TrialBalanceEntry> GetTrialBalanceEntries(BalancesSqlClauses clauses) {
      var operation = DataOperation.Parse("@qryTrialBalance",
                            CommonMethods.FormatSqlDbDate(clauses.StoredInitialBalanceSet.BalancesDate),
                            CommonMethods.FormatSqlDbDate(clauses.FromDate),
                            CommonMethods.FormatSqlDbDate(clauses.ToDate),
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

      string random = EmpiriaString.BuildRandomString(8);

      DateTime startTime = DateTime.Now;

      EmpiriaLog.Debug($"{random}: {startTime}: {operation.AsText()}");

      var entries = DataReader.GetPlainObjectFixedList<TrialBalanceEntry>(operation);

      EmpiriaLog.Debug($"{random} call returns {entries.Count} parsed entities in " +
                       $"{DateTime.Now.Subtract(startTime).TotalSeconds} seconds.");

      return entries;
    }

  } // class BalancesDataService

} // namespace Empiria.FinancialAccounting.BalanceEngine.Data
