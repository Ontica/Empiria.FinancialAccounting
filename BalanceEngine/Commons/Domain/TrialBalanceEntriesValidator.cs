/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Helper methods                          *
*  Type     : TrialBalanceEntriesValidator               License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Helper methods to validate entries for trial balances and related accounting information.      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Linq;
using System.Collections.Generic;

using Empiria.Collections;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Helper methods to validate entries for trial balances and
  /// related accounting information.</summary>
  internal class TrialBalanceEntriesValidator {


    #region Public methods

    public bool ValidateToCountEntries(List<TrialBalanceEntry> entries) {

      if (entries.Count > 0) {

        for (int i = 0; i < entries.Count; i++) {
          CheckIfHaveNullEntry(entries, i);
        }

        return true;
      }

      return false;
    }


    public bool CheckIfIsNullEntry(TrialBalanceEntry entry) {

      if (entry is null) {
        Assertion.RequireFail($"The entry {entry.ItemType} is a null object!");
      }

      return true;
    }


    #endregion Public methods


    #region Private methods

    private void CheckIfHaveNullEntry(List<TrialBalanceEntry> entries, int i) {

      if (entries[i] == null) {
        Assertion.RequireFail($"The entry in position: {i} is a null object!");
      }

    }

    #endregion


  } // class TrialBalanceEntriesValidator

} // namespace Empiria.FinancialAccounting.BalanceEngine
