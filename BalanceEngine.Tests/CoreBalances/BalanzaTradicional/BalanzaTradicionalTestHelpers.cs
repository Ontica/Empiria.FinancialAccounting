/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test cases                              *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Service provider                        *
*  Type     : BalanzaTradicionalTestHelpers              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides services for BalanzaComparativaTestHelpers tests.                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System;

using Empiria.FinancialAccounting;
using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.Tests;

namespace Empiria.Tests.FinancialAccounting.BalanceEngine {

  /// <summary></summary>
  static internal class BalanzaTradicionalTestHelpers {

    #region Methods

    static internal FixedList<BalanzaTradicionalEntryDto> GetBalanzaConsolidada(DateTime fromDate,
                                                                                DateTime toDate,
                                                                                BalancesType balancesType) {
      var query = new TrialBalanceQuery() {
        TrialBalanceType = TrialBalanceType.Balanza,
        AccountsChartUID = TestingConstants.IFRS_ACCOUNTS_CHART.UID,
        BalancesType = balancesType,
        ShowCascadeBalances = false,
        ConsolidateBalancesToTargetCurrency = true,
        InitialPeriod = new BalancesPeriod {
          FromDate = fromDate,
          ToDate = toDate,
          ExchangeRateDate = toDate,
          ValuateToCurrrencyUID = Currency.MXN.Code,
          ExchangeRateTypeUID = ExchangeRateType.ValorizacionBanxico.UID,
        }
      };

      return TestsHelpers.ExecuteTrialBalance<BalanzaTradicionalEntryDto>(query);
    }


    static internal FixedList<BalanzaTradicionalEntryDto> GetBalanzaTradicional(DateTime fromDate,
                                                                              DateTime toDate,
                                                                              BalancesType balancesType) {
      var query = new TrialBalanceQuery() {
        TrialBalanceType = TrialBalanceType.Balanza,
        AccountsChartUID = TestingConstants.IFRS_ACCOUNTS_CHART.UID,
        BalancesType = balancesType,
        ShowCascadeBalances = false,
        InitialPeriod = new BalancesPeriod {
          FromDate = fromDate,
          ToDate = toDate
        }
      };

      return TestsHelpers.ExecuteTrialBalance<BalanzaTradicionalEntryDto>(query);
    }

    #endregion Methods

  } // BalanzaTradicionalTestHelpers

} // namespace Empiria.Tests.FinancialAccounting.BalanceEngine
