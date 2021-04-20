/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Service provider                        *
*  Type     : TrialBalanceEngine                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides services to retrieve the trial balance.                                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

using Empiria.FinancialAccounting.BalanceEngine.Data;

namespace Empiria.FinancialAccounting.BalanceEngine.Domain {

  /// <summary>Provides services to retrieve the trial balance.</summary>
  internal class TrialBalanceEngine {

    internal TrialBalanceEngine() {
      //no-op
    }


    internal FixedList<TrialBalance> TrialBalance(TrialBalanceFields fields) {
      Assertion.AssertObject(fields, "fields");

      return TrialBalanceDataService.GetTrialBalance(fields);
    }


  } // class TrialBalanceEngine

} // namespace Empiria.FinancialAccounting.BalanceEngine.Domain
