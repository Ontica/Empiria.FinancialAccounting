/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test cases                              *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Service provider                        *
*  Type     : BalanzaDolarizadaTestHelpers               License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides services for BalanzaDolarizadaVsCoreBalancesTests tests.                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System;

using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

using Empiria.FinancialAccounting.Tests;

namespace Empiria.Tests.FinancialAccounting.BalanceEngine {

  /// <summary>Provides services for BalanzaDolarizadaVsCoreBalancesTests tests</summary>
  internal class BalanzaDolarizadaTestHelpers {

    #region Methods

    static internal FixedList<BalanzaDolarizadaEntryDto> GetBalanzaDolarizada(DateTime fromDate,
                                                                              DateTime toDate,
                                                                              BalancesType balancesType) {
      var query = new TrialBalanceQuery() {
        TrialBalanceType = TrialBalanceType.BalanzaDolarizada,
        AccountsChartUID = TestingConstants.IFRS_ACCOUNTS_CHART.UID,
        BalancesType = balancesType,
        ShowCascadeBalances = false,
        UseDefaultValuation = false,
        InitialPeriod = new BalancesPeriod {
          FromDate = fromDate,
          ToDate = toDate
        }
      };

      return TestsHelpers.ExecuteTrialBalance<BalanzaDolarizadaEntryDto>(query);
    }

    #endregion Methods

  } // class BalanzaDolarizadaTestHelpers

} // namespace Empiria.Tests.FinancialAccounting.BalanceEngine
