/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Data Layer                              *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Data Service                            *
*  Type     : BalanceDataService                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides data read methods for balances.                                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Data;

namespace Empiria.FinancialAccounting.BalanceEngine.BalanceExplorer.Data {

  /// <summary>Provides data read methods for balances.</summary>
  static internal class BalanceDataService {

    static internal FixedList<TrialBalanceEntry> GetBalanceEntries(TrialBalanceCommandData commandData) {

      var operation = DataOperation.Parse("@qryTrialBalance",
                            CommonMethods.FormatSqlDate(commandData.StoredInitialBalanceSet.BalancesDate),
                            CommonMethods.FormatSqlDate(commandData.FromDate),
                            CommonMethods.FormatSqlDate(commandData.ToDate),
                            commandData.StoredInitialBalanceSet.Id,
                            commandData.Fields,
                            commandData.Filters,
                            commandData.AccountFilters,
                            commandData.Where,
                            commandData.Ordering,
                            commandData.InitialFields,
                            commandData.InitialGrouping,
                            commandData.AccountsChart.Id,
                            commandData.AverageBalance
                            );

      string random = EmpiriaString.BuildRandomString(8);

      DateTime startTime = DateTime.Now;

      EmpiriaLog.Debug($"{random}: {startTime}: {operation.AsText()}");

      var entries = DataReader.GetPlainObjectFixedList<TrialBalanceEntry>(operation);

      EmpiriaLog.Debug($"{random} call returns {entries.Count} parsed entities in " +
                       $"{DateTime.Now.Subtract(startTime).TotalSeconds} seconds.");

      return entries;
    }

  } // class BalanceDataService

} // namespace Empiria.FinancialAccounting.BalanceEngine.BalanceExplorer.Data
