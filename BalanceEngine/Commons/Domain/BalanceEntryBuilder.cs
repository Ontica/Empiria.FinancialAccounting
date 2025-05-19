/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Information Holder                      *
*  Type     : BalanceEntryBuilder                        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Contains the entries of a balance.                                                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.Data;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Contains the entries of a balance.</summary>
  internal class BalanceEntryBuilder {

    internal FixedList<BalanceEntry> GetBalanceEntries(TrialBalanceQuery query) {

      return BalanceEntryDataService.GetBalanceEntries(query);
    }

  } // class BalanceEntryBuilder

} // namespace Empiria.FinancialAccounting.BalanceEngine
