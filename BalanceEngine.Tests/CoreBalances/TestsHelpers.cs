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

      return ExecuteTrialBalance<BalanzaComparativaEntryDto>(query);
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


    static internal FixedList<BalanzaDiferenciaDiariaMonedaEntryDto> GetBalanzaDiferenciaDiaria(DateTime fromDate,
                                                                              DateTime toDate,
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

      return ExecuteTrialBalance<BalanzaDiferenciaDiariaMonedaEntryDto>(query);
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


    static internal async Task<FixedList<SaldosEncerradosBaseEntryDto>> GetSaldosEncerrados(
                                DateTime fromDate, DateTime toDate) {

      var query = new SaldosEncerradosQuery() {
        AccountsChartUID = TestingConstants.IFRS_ACCOUNTS_CHART.UID,
        FromDate = fromDate,
        ToDate = toDate
      };

      return await ExecuteLockedUpBalances(query);
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


    static internal FixedList<BalanceExplorerEntryDto> ExecuteBalanceExplorer(BalanceExplorerQuery query) {

      using (var usecase = BalanceExplorerUseCases.UseCaseInteractor()) {

        return usecase.GetBalances(query).Entries;
      }
    }


    static internal async Task<FixedList<SaldosEncerradosBaseEntryDto>> ExecuteLockedUpBalances(SaldosEncerradosQuery query) {

      using (var usecase = TrialBalanceUseCases.UseCaseInteractor()) {

        var dto = await usecase.BuildSaldosEncerrados(query);

        return dto.Entries;
      }
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
