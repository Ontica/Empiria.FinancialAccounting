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
using System.Collections.Generic;
using Empiria.Data;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;

using Empiria.FinancialAccounting.BalanceEngine.Domain;

namespace Empiria.FinancialAccounting.BalanceEngine.Data {
  static internal class TrialBalanceDataService {

    static internal FixedList<TrialBalance> GetTrialBalance(TrialBalanceFields fields) {
      var operation = DataOperation.Parse("getTrialBalance", 
                                    fields.GeneralLedgerId, fields.InitialDate, 
                                    fields.FinalDate, fields.BalanceGroupId);

      return DataReader.GetPlainObjectFixedList<TrialBalance>(operation);
    }

  } // class TrialBalanceDataService

} // namespace Empiria.FinancialAccounting.BalanceEngine.Data
