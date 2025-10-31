/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test cases                              *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Service provider                        *
*  Type     : TestsHelpers                               License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides services for BalanzaDiferenciaDiariaMoneda tests.                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System;
using System.Collections.Generic;
using DocumentFormat.OpenXml.VariantTypes;
using Empiria.FinancialAccounting;
using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

using Empiria.FinancialAccounting.Tests;
using Empiria.Time;

namespace Empiria.Tests.FinancialAccounting.BalanceEngine {

  /// <summary>Provides services for BalanzaDiferenciaDiariaMoneda tests.</summary>
  static public class BalanzaDiferenciaDiariaMonedaTestHelpers {

    #region Methods

    static internal FixedList<BalanzaDiferenciaDiariaMonedaEntryDto> GetBalanzaDiferenciaDiaria(
                                                                      DateTime fromDate, DateTime toDate,
                                                                      BalancesType balancesType) {
      var query = new TrialBalanceQuery() {
        TrialBalanceType = TrialBalanceType.BalanzaDiferenciaDiariaPorMoneda,
        AccountsChartUID = TestingConstants.IFRS_ACCOUNTS_CHART.UID,
        BalancesType = balancesType,
        ShowCascadeBalances = false,
        InitialPeriod = new BalancesPeriod {
          FromDate = fromDate,
          ToDate = toDate
        }
      };

      return TestsHelpers.ExecuteTrialBalance<BalanzaDiferenciaDiariaMonedaEntryDto>(query);
    }


    static internal CoreBalanceEntries GetCoreBalanceEntries(DateTime fromDate, DateTime toDate,
                                                             ExchangeRateType exchangeRateType) {
      var query = GetTrialBalanceQuery(fromDate, toDate);

      return new CoreBalanceEntries(query, exchangeRateType);
    }


    static internal CoreBalanceEntries GetCoreBalanceEntriesByDateRange(DateTime fromDate, DateTime toDate,
                                                             ExchangeRateType exchangeRateType) {
      List<DateTime> datesRange = GetWorkingDatesRange(fromDate, toDate);

      TrialBalanceQuery _query = new TrialBalanceQuery();

      List<CoreBalanceEntry> coreEntriesList = new List<CoreBalanceEntry>();

      foreach (var date in datesRange) {
        
        _query = GetTrialBalanceQuery(date, date);

        coreEntriesList.AddRange(new CoreBalanceEntries(_query).ReloadEntries(exchangeRateType));
      }

      return new CoreBalanceEntries(_query, coreEntriesList.ToFixedList());
    }




    #endregion Methods


    #region Private methods

    static private List<DateTime> GetWorkingDatesRange(DateTime fromDate, DateTime toDate) {

      var calendar = EmpiriaCalendar.Default;

      var previousMonth = fromDate.AddMonths(-1);

      var lastWorkingDateFromPreviousMonth = calendar.LastWorkingDateWithinMonth(
                                                      previousMonth.Year, previousMonth.Month);

      List<DateTime> workingDays = new List<DateTime>();

      workingDays.Add(lastWorkingDateFromPreviousMonth);

      var _query = GetTrialBalanceQuery(fromDate, toDate);
      var balanceHelper = new TrialBalanceHelper(_query);

      workingDays.AddRange(balanceHelper.GetWorkingDaysRange());

      return workingDays;
    }


    static private TrialBalanceQuery GetTrialBalanceQuery(DateTime fromDate, DateTime toDate) {

      return new TrialBalanceQuery() {
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
    }

    #endregion Private methods

  } // class BalanzaDiferenciaDiariaMonedaTestHelpers

} // namespace Empiria.Tests.FinancialAccounting.BalanceEngine
