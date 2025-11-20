/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test cases                              *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Service provider                        *
*  Type     : BalanzaContCascadaTestHelpers              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides services for BalanzaComparativaVsCoreBalancesTests tests.                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System;
using Empiria.FinancialAccounting;
using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.Tests;

namespace Empiria.Tests.FinancialAccounting.BalanceEngine {

  /// <summary>Provides services for BalanzaComparativaVsCoreBalancesTests tests</summary>
  internal class BalanzaContCascadaTestHelpers {

    #region Methods

    static internal FixedList<BalanzaContabilidadesCascadaEntryDto> GetBalanzaContabilidadesCascada(
                                            DateTime fromDate, DateTime toDate, BalancesType balancesType) {
      var query = new TrialBalanceQuery() {
        TrialBalanceType = TrialBalanceType.BalanzaConContabilidadesEnCascada,
        AccountsChartUID = TestingConstants.IFRS_ACCOUNTS_CHART.UID,
        BalancesType = balancesType,
        InitialPeriod = new BalancesPeriod {
          FromDate = fromDate,
          ToDate = toDate
        }
      };

      return TestsHelpers.ExecuteTrialBalance<BalanzaContabilidadesCascadaEntryDto>(query);
    }


    static internal CoreBalanceEntries GetCoreBalanceEntriesInCascade(DateTime fromDate, DateTime toDate,
                                                            ExchangeRateType exchangeRateType) {
      var query = new TrialBalanceQuery() {
        AccountsChartUID = TestingConstants.IFRS_ACCOUNTS_CHART.UID,
        TrialBalanceType = TrialBalanceType.Balanza,
        BalancesType = BalancesType.AllAccounts,
        InitialPeriod = new BalancesPeriod {
          FromDate = fromDate,
          ToDate = toDate,
        }
      };

      return new CoreBalanceEntries(query, exchangeRateType);
    }

    #endregion Methods

  } // class BalanzaContCascadaTestHelpers

} // namespace Empiria.Tests.FinancialAccounting.BalanceEngine
