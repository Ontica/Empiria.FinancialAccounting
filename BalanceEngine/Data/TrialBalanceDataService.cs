/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Data Layer                        *
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

    static internal FixedList<TrialBalanceEntry> GetTrialBalanceEntries(TrialBalanceCommandData commandData, string[] fieldsGrouping, string filter) {
      
      var operation = DataOperation.Parse("@qryTrialBalance", 
                                          commandData.StartDate.ToString("dd/MM/yyyy"),
                                          commandData.EndDate.ToString("dd/MM/yyyy"), 
                                          commandData.BalanceGroupId, 
                                          fieldsGrouping[0],
                                          filter.Length != 0 ? filter : String.Empty, //commandData.Condition, 
                                          fieldsGrouping[1], 
                                          commandData.Having, commandData.Ordering
                                          );

      return DataReader.GetPlainObjectFixedList<TrialBalanceEntry>(operation);
    }

  } // class TrialBalanceDataService

} // namespace Empiria.FinancialAccounting.BalanceEngine.Data
