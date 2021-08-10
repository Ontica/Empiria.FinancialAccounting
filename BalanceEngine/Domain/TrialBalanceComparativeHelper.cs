/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Helper methods                          *
*  Type     : TrialBalanceComparativeHelper              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Helper methods to build comparative balances.                                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Linq;
using System.Collections.Generic;

using Empiria.Collections;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Helper methods to build comparative balances.</summary>
  internal class TrialBalanceComparativeHelper {

    private readonly TrialBalanceCommand _command;

    internal TrialBalanceComparativeHelper(TrialBalanceCommand command) {
      _command = command;
    }


    internal List<TrialBalanceEntry> MergePeriodsIntoComparativeBalance(
                                      FixedList<TrialBalanceEntry> trialBalanceFirstPeriod,
                                      FixedList<TrialBalanceEntry> trialBalanceSecondPeriod) {

      List<TrialBalanceComparativeEntry> comparativeEntries = new List<TrialBalanceComparativeEntry>();
      List<TrialBalanceComparativeEntry> firstPeriod = new List<TrialBalanceComparativeEntry>();
      List<TrialBalanceComparativeEntry> secondPeriod = new List<TrialBalanceComparativeEntry>();



      comparativeEntries = CombineFirstAndSecondPeriod(firstPeriod, secondPeriod);
  
      throw new NotImplementedException();
    }


    #region Private methods

    private List<TrialBalanceComparativeEntry> CombineFirstAndSecondPeriod(
                                      List<TrialBalanceComparativeEntry> trialBalanceFirstPeriod,
                                      List<TrialBalanceComparativeEntry> trialBalanceSecondPeriod) {

      throw new NotImplementedException();
    }

    #endregion

  } // class TrialBalanceComparativeHelper

} // namespace Empiria.FinancialAccounting.BalanceEngine

