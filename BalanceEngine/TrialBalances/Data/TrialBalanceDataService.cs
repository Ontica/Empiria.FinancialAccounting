/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Data Layer                              *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Data Service                            *
*  Type     : AccountBalanceDataService                  License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides data read methods for trial balances.                                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Data;

namespace Empiria.FinancialAccounting.BalanceEngine.Data {

  static internal class TrialBalanceDataService {

    static internal FixedList<TrialBalanceEntry> GetTrialBalanceEntries(TrialBalanceCommandData commandData) {

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

  } // class TrialBalanceDataService

} // namespace Empiria.FinancialAccounting.BalanceEngine.Data
