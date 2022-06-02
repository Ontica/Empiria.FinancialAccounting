/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Helper methods                          *
*  Type     : SaldosPorCuentaHelper                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Helper methods to build balances by account report.                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using Empiria.Collections;
using System.Linq;

using Empiria.FinancialAccounting.BalanceEngine.Data;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Helper methods to build balances by account report.</summary>
  internal class SaldosPorCuentaHelper {

    private readonly TrialBalanceQuery _query;

    internal SaldosPorCuentaHelper(TrialBalanceQuery query) {
      _query = query;
    }

    internal FixedList<TrialBalanceEntry> GetAccountEntries() {

      FixedList<TrialBalanceEntry> accountEntries = BalancesDataService.GetTrialBalanceEntries(_query);

      var trialBalanceHelper = new TrialBalanceHelper(_query);

      if (_query.ValuateBalances || _query.InitialPeriod.UseDefaultValuation) {
        trialBalanceHelper.ValuateAccountEntriesToExchangeRate(accountEntries);

        if (_query.ConsolidateBalancesToTargetCurrency) {

          accountEntries = trialBalanceHelper.ConsolidateToTargetCurrency(
                                              accountEntries, _query.InitialPeriod);
        }
      }
      trialBalanceHelper.RoundDecimals(accountEntries);
      return accountEntries;
    }

    
    #region Public methods



    #endregion Public methods


    #region Private methods



    #endregion Private methods

  } // class SaldosPorCuentaHelper

} // namespace Empiria.FinancialAccounting.BalanceEngine
