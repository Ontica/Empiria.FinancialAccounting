/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test cases                              *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Service provider                        *
*  Type     : BalanzaColumnasTestHelpers                 License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides services for BalanzaColumnasVsCoreBalancesTests tests.                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System;
using Empiria.FinancialAccounting;
using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.Tests;

namespace Empiria.Tests.FinancialAccounting.BalanceEngine {

  /// <summary>Provides services for BalanzaColumnasVsCoreBalancesTests tests</summary>
  static public class BalanzaColumnasTestHelpers {

    #region Methods

    static internal FixedList<BalanzaColumnasMonedaEntryDto> GetBalanzaColumnas(DateTime fromDate,
                                                                              DateTime toDate,
                                                                              BalancesType balancesType) {

      var query = new TrialBalanceQuery() {
        TrialBalanceType = TrialBalanceType.BalanzaEnColumnasPorMoneda,
        AccountsChartUID = TestingConstants.IFRS_ACCOUNTS_CHART.UID,
        BalancesType = balancesType,
        ShowCascadeBalances = false,
        InitialPeriod = new BalancesPeriod {
          FromDate = fromDate,
          ToDate = toDate
        }
      };

      return TestsHelpers.ExecuteTrialBalance<BalanzaColumnasMonedaEntryDto>(query);
    }


    static internal CoreBalanceEntries GetCoreBalanceEntries(DateTime fromDate, DateTime toDate,
                                                             ExchangeRateType exchangeRateType) {
      var query = new TrialBalanceQuery() {
        AccountsChartUID = TestingConstants.IFRS_ACCOUNTS_CHART.UID,
        TrialBalanceType = TrialBalanceType.Balanza,
        BalancesType = BalancesType.AllAccounts,
        ShowCascadeBalances = false,
        WithSubledgerAccount = false,
        UseDefaultValuation = false,
        InitialPeriod = new BalancesPeriod {
          FromDate = fromDate,
          ToDate = toDate,
        }
      };

      return new CoreBalanceEntries(query, exchangeRateType);
    }

    #endregion Methods

  } // class BalanzaColumnasTestHelpers

} // namespace Empiria.Tests.FinancialAccounting.BalanceEngine
