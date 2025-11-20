/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test cases                              *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Service provider                        *
*  Type     : TestsHelpers                               License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides services for balances tests.                                                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System;
using System.Threading.Tasks;
using Empiria.FinancialAccounting;

using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.BalanceExplorer.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.BalanceExplorer.UseCases;
using Empiria.FinancialAccounting.BalanceEngine.UseCases;

using Empiria.FinancialAccounting.Tests;
using Empiria.FinancialAccounting.Tests.BalanceEngine;

namespace Empiria.Tests.FinancialAccounting.BalanceEngine {

  /// <summary>Provides services for balances tests.</summary>
  static public class TestsHelpers {

    #region Methods

    static internal string BalanceDiffMsg(string column, string accountText,
                                          decimal expected, decimal sutValue) {

      return $"Diferencia en {column}, cuenta {accountText}. " +
             $"Valor esperado = {expected.ToString("C2")}, " +
             $"Reporte = {sutValue.ToString("C2")}. " +
             $"Diferencia = {(expected - sutValue).ToString("C2")}";
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


    static internal async Task<FixedList<SaldosEncerradosBaseEntryDto>> GetSaldosEncerrados(
                                DateTime fromDate, DateTime toDate) {

      var query = new SaldosEncerradosQuery() {
        AccountsChartUID = TestingConstants.IFRS_ACCOUNTS_CHART.UID,
        FromDate = fromDate,
        ToDate = toDate
      };

      return await ExecuteLockedUpBalances(query);
    }


    static internal async Task<FixedList<SaldosEncerradosBaseEntryDto>> ExecuteLockedUpBalances(SaldosEncerradosQuery query) {

      using (var usecase = TrialBalanceUseCases.UseCaseInteractor()) {

        var dto = await usecase.BuildSaldosEncerrados(query);

        return dto.Entries;
      }
    }


    static internal FixedList<T> ExecuteTrialBalance<T>(TrialBalanceQuery query) {

      using (var usecase = TrialBalanceUseCases.UseCaseInteractor()) {

        var _entries = BalanceEngineProxy.BuildTrialBalance(query).Entries;

        return _entries.Select(x => (T) x).ToFixedList();
      }
    }

    #endregion Methods

  } // class TestsHelpers

}  // namespace Empiria.Tests.FinancialAccounting.BalanceEngine
