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
                            commandData.Having.Length > 0 ? commandData.Grouping : "",
                            commandData.Having,
                            commandData.Ordering,
                            commandData.InitialFields,
                            commandData.InitialGrouping
                            /* commandData.AccountsChart.Id */
                            );

      return DataReader.GetPlainObjectFixedList<TrialBalanceEntry>(operation);
    }


  } // class TrialBalanceDataService

} // namespace Empiria.FinancialAccounting.BalanceEngine.Data
