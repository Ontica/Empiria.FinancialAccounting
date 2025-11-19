/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test cases                              *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Service provider                        *
*  Type     : BalanzaComparativaTestHelpers              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides services for BalanzaComparativaTestHelpers tests.                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System;
using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.Tests;

namespace Empiria.Tests.FinancialAccounting.BalanceEngine {

  /// <summary>Provides services for BalanzaComparativaTestHelpers tests</summary>
  static public class BalanzaComparativaTestHelpers {

    #region Methods

    static internal FixedList<BalanzaComparativaEntryDto> GetBalanzaComparativa(DateTime fromDate,
                                                                              DateTime toDate,
                                                                              DateTime fromDate2,
                                                                              DateTime toDate2,
                                                                              BalancesType balancesType) {
      var query = new TrialBalanceQuery() {
        TrialBalanceType = TrialBalanceType.BalanzaValorizadaComparativa,
        AccountsChartUID = TestingConstants.IFRS_ACCOUNTS_CHART.UID,
        BalancesType = balancesType,
        ShowCascadeBalances = false,
        UseDefaultValuation = true,
        WithSubledgerAccount = true,
        InitialPeriod = new BalancesPeriod {
          FromDate = fromDate,
          ToDate = toDate
        },
        FinalPeriod = new BalancesPeriod {
          FromDate = fromDate2,
          ToDate = toDate2
        }
      };

      return TestsHelpers.ExecuteTrialBalance<BalanzaComparativaEntryDto>(query);
    }

    #endregion Methods

  } // class BalanzaComparativaTestHelpers

} // namespace Empiria.Tests.FinancialAccounting.BalanceEngine
