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

using Empiria.FinancialAccounting;

using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.UseCases;

using Empiria.FinancialAccounting.Tests;

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


    static internal FixedList<AnaliticoDeCuentasEntryDto> GetAnaliticoCuentas(DateTime fromDate,
                                                                              DateTime toDate,
                                                                              BalancesType balancesType) {

      var query = new TrialBalanceQuery() {
        TrialBalanceType = TrialBalanceType.AnaliticoDeCuentas,
        AccountsChartUID = TestingConstants.IFRS_ACCOUNTS_CHART.UID,
        BalancesType = balancesType,
        ShowCascadeBalances = false,
        UseDefaultValuation = true,
        InitialPeriod = new BalancesPeriod {
          FromDate = fromDate,
          ToDate = toDate
        }
      };

      return ExecuteTrialBalance<AnaliticoDeCuentasEntryDto>(query);
    }


    static internal FixedList<AnaliticoDeCuentasEntryDto> GetAnaliticoCuentasWithSubledgerAccounts(DateTime fromDate,
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

      return ExecuteTrialBalance<AnaliticoDeCuentasEntryDto>(query);
    }


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

      return ExecuteTrialBalance<BalanzaColumnasMonedaEntryDto>(query);
    }


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

      return ExecuteTrialBalance<BalanzaTradicionalEntryDto>(query);
    }


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

      return ExecuteTrialBalance<BalanzaContabilidadesCascadaEntryDto>(query);
    }


    static internal FixedList<BalanzaDolarizadaEntryDto> GetBalanzaDolarizada(DateTime fromDate,
                                                                              DateTime toDate,
                                                                              BalancesType balancesType) {

      var query = new TrialBalanceQuery() {
        TrialBalanceType = TrialBalanceType.BalanzaDolarizada,
        AccountsChartUID = TestingConstants.IFRS_ACCOUNTS_CHART.UID,
        BalancesType = balancesType,
        ShowCascadeBalances = false,
        UseDefaultValuation = true,
        InitialPeriod = new BalancesPeriod {
          FromDate = fromDate,
          ToDate = toDate
        }
      };

      return ExecuteTrialBalance<BalanzaDolarizadaEntryDto>(query);
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

      return ExecuteTrialBalance<BalanzaTradicionalEntryDto>(query);
    }


    static internal CoreBalanceEntries GetCoreBalanceEntries(DateTime fromDate, DateTime toDate,
                                                             ExchangeRateType exchangeRateType) {
      var query = new TrialBalanceQuery() {
        AccountsChartUID = TestingConstants.IFRS_ACCOUNTS_CHART.UID,
        TrialBalanceType = TrialBalanceType.Balanza,
        BalancesType = BalancesType.AllAccounts,
        ShowCascadeBalances = false,
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
        InitialPeriod = new BalancesPeriod {
          FromDate = fromDate,
          ToDate = toDate,
        }
      };

      return new CoreBalanceEntries(query, exchangeRateType);
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

      return ExecuteTrialBalance<SaldosPorAuxiliarEntryDto>(query);
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

      return ExecuteTrialBalance<SaldosPorCuentaEntryDto>(query);
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

      return ExecuteTrialBalance<SaldosPorCuentaEntryDto>(query);
    }


    static internal FixedList<T> ExecuteTrialBalance<T>(TrialBalanceQuery query) {

      using (var usecase = TrialBalanceUseCases.UseCaseInteractor()) {

        var entries = usecase.BuildTrialBalance(query).Entries;

        return entries.Select(x => (T) x).ToFixedList();
      }
    }


    #endregion Methods

  } // class TestsHelpers

}  // namespace Empiria.Tests.FinancialAccounting.BalanceEngine
