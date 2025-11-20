/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test cases                              *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Service provider                        *
*  Type     : SaldosPorAuxiliarYPorCuentaTestHelpers     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides services for SaldosPorAuxiliar and SaldosPorCuenta tests.                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System;
using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.BalanceExplorer.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.BalanceExplorer.UseCases;
using Empiria.FinancialAccounting.Tests;

namespace Empiria.Tests.FinancialAccounting.BalanceEngine {

  /// <summary>Provides services for SaldosPorAuxiliar and SaldosPorCuenta tests</summary>
  static internal class SaldosPorAuxiliarYPorCuentaTestHelpers {

    #region Methods

    static internal FixedList<BalanceExplorerEntryDto> GetExploradorSaldosPorAuxiliar(
                                                        DateTime fromDate,
                                                        DateTime toDate,
                                                        bool accountsFilter) {
      var query = new BalanceExplorerQuery() {
        TrialBalanceType = TrialBalanceType.SaldosPorAuxiliarConsultaRapida,
        AccountsChartUID = TestingConstants.IFRS_ACCOUNTS_CHART.UID,
        WithAllAccounts = accountsFilter,
        WithSubledgerAccount = true,
        SubledgerAccounts = new string[] { "90000000000123100" },
        InitialPeriod = new BalancesPeriod {
          FromDate = fromDate,
          ToDate = toDate
        }
      };

      return ExecuteBalanceExplorer(query);
    }


    static internal FixedList<BalanceExplorerEntryDto> GetExploradorSaldosPorCuenta(DateTime fromDate,
                                                                              DateTime toDate,
                                                                              bool accountsFilter) {
      var query = new BalanceExplorerQuery() {
        TrialBalanceType = TrialBalanceType.SaldosPorCuentaConsultaRapida,
        AccountsChartUID = TestingConstants.IFRS_ACCOUNTS_CHART.UID,
        WithAllAccounts = accountsFilter,
        InitialPeriod = new BalancesPeriod {
          FromDate = fromDate,
          ToDate = toDate
        }
      };

      return ExecuteBalanceExplorer(query);
    }


    static internal FixedList<BalanceExplorerEntryDto> GetExploradorSaldosPorCuentaConAuxiliar(
                                                        DateTime fromDate,
                                                        DateTime toDate,
                                                        bool accountsFilter) {
      var query = new BalanceExplorerQuery() {
        TrialBalanceType = TrialBalanceType.SaldosPorCuentaConsultaRapida,
        AccountsChartUID = TestingConstants.IFRS_ACCOUNTS_CHART.UID,
        WithAllAccounts = accountsFilter,
        WithSubledgerAccount = true,
        InitialPeriod = new BalancesPeriod {
          FromDate = fromDate,
          ToDate = toDate
        }
      };

      return ExecuteBalanceExplorer(query);
    }


    static internal FixedList<SaldosPorAuxiliarEntryDto> GetSaldosPorAuxiliar(DateTime fromDate,
                                                                              DateTime toDate,
                                                                              BalancesType balancesType) {
      var query = new TrialBalanceQuery() {
        TrialBalanceType = TrialBalanceType.SaldosPorAuxiliar,
        AccountsChartUID = TestingConstants.IFRS_ACCOUNTS_CHART.UID,
        BalancesType = balancesType,
        WithSubledgerAccount = true,
        InitialPeriod = new BalancesPeriod {
          FromDate = fromDate,
          ToDate = toDate
        }
      };

      return TestsHelpers.ExecuteTrialBalance<SaldosPorAuxiliarEntryDto>(query);
    }


    static internal FixedList<SaldosPorCuentaEntryDto> GetSaldosPorCuenta(DateTime fromDate,
                                                                              DateTime toDate,
                                                                              BalancesType balancesType) {
      var query = new TrialBalanceQuery() {
        TrialBalanceType = TrialBalanceType.SaldosPorCuenta,
        AccountsChartUID = TestingConstants.IFRS_ACCOUNTS_CHART.UID,
        BalancesType = balancesType,
        ShowCascadeBalances = false,
        InitialPeriod = new BalancesPeriod {
          FromDate = fromDate,
          ToDate = toDate
        }
      };

      return TestsHelpers.ExecuteTrialBalance<SaldosPorCuentaEntryDto>(query);
    }


    static internal FixedList<SaldosPorCuentaEntryDto> GetSaldosPorCuentaConAuxiliares(DateTime fromDate,
                                                                              DateTime toDate,
                                                                              BalancesType balancesType) {
      var query = new TrialBalanceQuery() {
        TrialBalanceType = TrialBalanceType.SaldosPorCuenta,
        AccountsChartUID = TestingConstants.IFRS_ACCOUNTS_CHART.UID,
        BalancesType = balancesType,
        ShowCascadeBalances = false,
        WithSubledgerAccount = true,
        InitialPeriod = new BalancesPeriod {
          FromDate = fromDate,
          ToDate = toDate
        }
      };

      return TestsHelpers.ExecuteTrialBalance<SaldosPorCuentaEntryDto>(query);
    }

    #endregion Methods


    #region Private methods

    static private FixedList<BalanceExplorerEntryDto> ExecuteBalanceExplorer(BalanceExplorerQuery query) {

      using (var usecase = BalanceExplorerUseCases.UseCaseInteractor()) {

        return usecase.GetBalances(query).Entries;
      }
    }

    #endregion Private methods

  } // class SaldosPorAuxiliarYPorCuentaTestHelpers

} // namespace Empiria.Tests.FinancialAccounting.BalanceEngine
