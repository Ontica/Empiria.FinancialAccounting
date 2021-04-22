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

    static internal FixedList<TrialBalanceEntry> GetTrialBalanceEntries(int balanceGroupId,
                                                                        int generalLedgerId,
                                                                        int stdAccountTypeId,
                                                                        string stdAccountNumber,
                                                                        string stdAccountName,
                                                                        DateTime initialDate,
                                                                        DateTime finalDate) {
      var operation = DataOperation.Parse("getTrialBalance",
                                          generalLedgerId,
                                          stdAccountTypeId,
                                          stdAccountNumber,
                                          stdAccountName,
                                          initialDate, finalDate,
                                          balanceGroupId);

      return DataReader.GetPlainObjectFixedList<TrialBalanceEntry>(operation);
    }

  } // class TrialBalanceDataService

} // namespace Empiria.FinancialAccounting.BalanceEngine.Data
