/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test cases                              *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Service provider                        *
*  Type     : AnaliticoCuentasTestHelpers                License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides services for AnaliticoCuentasTestHelpers tests.                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System;

using Empiria.FinancialAccounting;
using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.Tests;

namespace Empiria.Tests.FinancialAccounting.BalanceEngine {

  /// <summary></summary>
  static public class AnaliticoCuentasTestHelpers {

    #region Methods

    static internal FixedList<AnaliticoDeCuentasEntryDto> GetAnaliticoCuentas(DateTime fromDate,
                                                                              DateTime toDate,
                                                                              BalancesType balancesType) {

      var query = new TrialBalanceQuery() {
        TrialBalanceType = TrialBalanceType.AnaliticoDeCuentas,
        AccountsChartUID = TestingConstants.IFRS_ACCOUNTS_CHART.UID,
        BalancesType = balancesType,
        ShowCascadeBalances = false,
        UseDefaultValuation = true,
        WithSubledgerAccount = true,
        InitialPeriod = new BalancesPeriod {
          FromDate = fromDate,
          ToDate = toDate
        }
      };

      return TestsHelpers.ExecuteTrialBalance<AnaliticoDeCuentasEntryDto>(query);
    }


    static internal FixedList<AnaliticoDeCuentasEntryDto> GetAnaliticoCuentasWithSubledgerAccounts(
                                                          DateTime fromDate, DateTime toDate,
                                                          BalancesType balancesType) {

      var query = new TrialBalanceQuery() {
        TrialBalanceType = TrialBalanceType.AnaliticoDeCuentas,
        AccountsChartUID = TestingConstants.IFRS_ACCOUNTS_CHART.UID,
        BalancesType = balancesType,
        ShowCascadeBalances = false,
        UseDefaultValuation = true,
        WithSubledgerAccount = true,
        InitialPeriod = new BalancesPeriod {
          FromDate = fromDate,
          ToDate = toDate
        }
      };

      return TestsHelpers.ExecuteTrialBalance<AnaliticoDeCuentasEntryDto>(query);
    }


    static internal CoreBalanceEntries GetCoreBalanceEntries(DateTime fromDate, DateTime toDate,
                                                             ExchangeRateType exchangeRateType) {
      var query = new TrialBalanceQuery() {
        AccountsChartUID = TestingConstants.IFRS_ACCOUNTS_CHART.UID,
        TrialBalanceType = TrialBalanceType.Balanza,
        BalancesType = BalancesType.AllAccounts,
        ShowCascadeBalances = false,
        WithSubledgerAccount = false,
        UseDefaultValuation = true,
        InitialPeriod = new BalancesPeriod {
          FromDate = fromDate,
          ToDate = toDate,
        }
      };

      return new CoreBalanceEntries(query, exchangeRateType);
    }


    static internal CoreBalanceEntries GetCoreBalanceEntriesWithSubledgerAccounts(DateTime fromDate, DateTime toDate,
                                                             ExchangeRateType exchangeRateType) {
      var query = new TrialBalanceQuery() {
        AccountsChartUID = TestingConstants.IFRS_ACCOUNTS_CHART.UID,
        TrialBalanceType = TrialBalanceType.Balanza,
        BalancesType = BalancesType.AllAccounts,
        WithSubledgerAccount = true,
        UseDefaultValuation = true,
        InitialPeriod = new BalancesPeriod {
          FromDate = fromDate,
          ToDate = toDate,
        }
      };

      return new CoreBalanceEntries(query, exchangeRateType);
    }

    #endregion Methods

  } // class AnaliticoCuentasTestHelpers

} // namespace Empiria.Tests.FinancialAccounting.BalanceEngine
