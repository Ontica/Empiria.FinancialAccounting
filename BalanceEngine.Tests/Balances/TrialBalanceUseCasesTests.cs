/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test cases                              *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Use cases tests                         *
*  Type     : TrialBalanceUseCasesTests                  License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Test cases for trial balance reports.                                                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Linq;
using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.Tests;
using Xunit;

namespace Empiria.FinancialAccounting.Tests.BalanceEngine {

  /// <summary>Test cases for trial balance reports.</summary>
  public class TrialBalanceUseCasesTests {

    #region Initialization

    public TrialBalanceUseCasesTests() {
      TestsCommonMethods.Authenticate();
    }


    #endregion Initialization

    #region Facts

    [Fact]
    public void Should_Build_Analitico_De_Cuentas() {

      TrialBalanceQuery query = GetDefaultTrialBalanceQuery();
      query.TrialBalanceType = TrialBalanceType.AnaliticoDeCuentas;

      TrialBalanceDto analitico = BalanceEngineProxy.BuildTrialBalance(query);

      Assert.NotNull(analitico);
      Assert.Equal(query, analitico.Query);
      Assert.NotEmpty(analitico.Entries);
    }


    [Fact]
    public void Analitico_Totals_Must_Be_Same_As_BalanzaConsolidada() {

      TrialBalanceQuery query = GetDefaultTrialBalanceQuery();

      query.TrialBalanceType = TrialBalanceType.AnaliticoDeCuentas;
      TrialBalanceDto analitico = BalanceEngineProxy.BuildTrialBalance(query);

      Assert.NotNull(analitico);
      Assert.Equal(query, analitico.Query);
      Assert.NotEmpty(analitico.Entries);

      query.TrialBalanceType = TrialBalanceType.Balanza;
      query.ConsolidateBalancesToTargetCurrency = true;
      query.UseDefaultValuation = false;
      query.InitialPeriod = new BalancesPeriod {
        FromDate = TestingConstants.FROM_DATE,
        ToDate = TestingConstants.TO_DATE,
        ExchangeRateDate = new DateTime(2025, 01, 31),
        ExchangeRateTypeUID = ExchangeRateType.ValorizacionBanxico.UID,
        ValuateToCurrrencyUID = "01"
      };
      TrialBalanceDto balanza = BalanceEngineProxy.BuildTrialBalance(query);

      Assert.NotNull(balanza);
      Assert.Equal(query, balanza.Query);
      Assert.NotEmpty(balanza.Entries);

      var _balanza = balanza.Entries.Select(x => (BalanzaTradicionalEntryDto) x);
      var _analitico = analitico.Entries.Select(x => (AnaliticoDeCuentasEntryDto) x);

      foreach (var analiticEntry in _analitico.Where(x=>x.AccountNumber != string.Empty)) {

        var balanzaEntryBalance = _balanza.Where(x => x.AccountNumber == analiticEntry.AccountNumber &&
                                                 x.SectorCode == analiticEntry.SectorCode &&
                                                 x.DebtorCreditor == analiticEntry.DebtorCreditor
                                         ).FirstOrDefault();

        Assert.Equal(Math.Round(analiticEntry.TotalBalance), Math.Round((decimal) balanzaEntryBalance.CurrentBalance));

      }
    }


    [Fact]
    public void Should_Build_Balanza_Contabilidades_Cascada() {
      TrialBalanceQuery query = GetDefaultTrialBalanceQuery();

      query.TrialBalanceType = TrialBalanceType.BalanzaConContabilidadesEnCascada;
      query.BalancesType = BalancesType.WithCurrentBalanceOrMovements;

      TrialBalanceDto sut = BalanceEngineProxy.BuildTrialBalance(query);

      Assert.NotNull(sut);
      Assert.Equal(query, sut.Query);
      Assert.NotEmpty(sut.Entries);
    }


    [Fact]
    public void Should_Build_Comparacion_Entre_Periodos() {
      TrialBalanceQuery query = GetDefaultTrialBalanceQuery();

      query.TrialBalanceType = TrialBalanceType.BalanzaValorizadaComparativa;
      query.FromAccount = "1.05.01.01.06.02";
      query.ToAccount = "1.05.01.01.06.02";

      TrialBalanceDto sut = BalanceEngineProxy.BuildTrialBalance(query);

      Assert.NotNull(sut);
      Assert.Equal(query, sut.Query);
      Assert.NotEmpty(sut.Entries);
    }


    [Fact]
    public void Should_Build_A_Traditional_Trial_Balance() {

      TrialBalanceQuery query = GetDefaultTrialBalanceQuery();
      query.TrialBalanceType = TrialBalanceType.Balanza;
      query.FromAccount = "2.07.04.01.01.03.01";
      query.ToAccount = "2.07.04.01.01.03.01";
      TrialBalanceDto sut = BalanceEngineProxy.BuildTrialBalance(query);

      Assert.NotNull(sut);
      Assert.Equal(query, sut.Query);
      Assert.NotEmpty(sut.Entries);
    }


    [Fact]
    public void Should_Build_Saldos_Por_Auxiliar() {
      TrialBalanceQuery query = GetDefaultTrialBalanceQuery();
      query.TrialBalanceType = TrialBalanceType.SaldosPorAuxiliar;

      TrialBalanceDto sut = BalanceEngineProxy.BuildTrialBalance(query);

      Assert.NotNull(sut);
      Assert.Equal(query, sut.Query);
      Assert.NotEmpty(sut.Entries);
    }


    [Fact]
    public void Should_Build_Saldos_Por_Cuenta() {
      TrialBalanceQuery query = GetDefaultTrialBalanceQuery();
      query.TrialBalanceType = TrialBalanceType.SaldosPorCuenta;

      TrialBalanceDto sut = BalanceEngineProxy.BuildTrialBalance(query);

      Assert.NotNull(sut);
      Assert.Equal(query, sut.Query);
      Assert.NotEmpty(sut.Entries);
    }


    [Fact]
    public void Should_Build_A_Traditional_No_Consolidated_Trial_Balance() {
      TrialBalanceQuery query = GetDefaultTrialBalanceQuery();

      query.TrialBalanceType = TrialBalanceType.Balanza;

      TrialBalanceDto sut = BalanceEngineProxy.BuildTrialBalance(query);

      Assert.NotNull(sut);
      Assert.Equal(query, sut.Query);
      Assert.NotEmpty(sut.Entries);
    }


    [Fact]
    public void Should_Build_Balanza_Col_Monedas() {
      TrialBalanceQuery query = GetDefaultTrialBalanceQuery();
      query.TrialBalanceType = TrialBalanceType.BalanzaEnColumnasPorMoneda;
      query.BalancesType = BalancesType.WithCurrentBalanceOrMovements;

      TrialBalanceDto sut = BalanceEngineProxy.BuildTrialBalance(query);

      Assert.NotNull(sut);
      Assert.Equal(query, sut.Query);
      Assert.NotEmpty(sut.Entries);
    }


    [Fact]
    public void Should_Build_Balanza_Consolidada_Por_Moneda() {
      TrialBalanceQuery query = GetDefaultTrialBalanceQuery();

      TrialBalanceDto sut = BalanceEngineProxy.BuildTrialBalance(query);

      Assert.NotNull(sut);
      Assert.Equal(query, sut.Query);
      Assert.NotEmpty(sut.Entries);
    }


    [Fact]
    public void Should_Build_Balanza_Diferencia_Diaria() {

      TrialBalanceQuery query = GetDefaultTrialBalanceQuery();
      query.TrialBalanceType = TrialBalanceType.BalanzaDiferenciaDiariaPorMoneda;

      TrialBalanceDto sut = BalanceEngineProxy.BuildTrialBalance(query);

      Assert.NotNull(sut);
      Assert.Equal(query, sut.Query);
      Assert.NotEmpty(sut.Entries);
    }


    [Fact]
    public void Should_Build_Balanza_Dolarizada() {
      TrialBalanceQuery query = GetDefaultTrialBalanceQuery();
      query.TrialBalanceType = TrialBalanceType.BalanzaDolarizada;

      TrialBalanceDto sut = BalanceEngineProxy.BuildTrialBalance(query);

      Assert.NotNull(sut);
      Assert.Equal(query, sut.Query);
      Assert.NotEmpty(sut.Entries);
    }


    [Fact]
    public void Should_Build_Valorizacion() {
      TrialBalanceQuery query = GetDefaultTrialBalanceQuery();

      query.TrialBalanceType = TrialBalanceType.ValorizacionEstimacionPreventiva;
      query.BalancesType = BalancesType.WithCurrentBalanceOrMovements;

      TrialBalanceDto sut = BalanceEngineProxy.BuildTrialBalance(query);

      Assert.NotNull(sut);
      Assert.Equal(query, sut.Query);
      Assert.NotEmpty(sut.Entries);
    }


    #endregion Facts

    #region Helpers

    private TrialBalanceQuery GetDefaultTrialBalanceQuery() {
      return new TrialBalanceQuery() {
        AccountsChartUID = TestingConstants.IFRS_ACCOUNTS_CHART.UID,
        BalancesType = BalancesType.WithCurrentBalanceOrMovements,
        ShowCascadeBalances = false,
        UseDefaultValuation = false,
        WithSubledgerAccount = true,
        InitialPeriod = new BalancesPeriod {
          FromDate = TestingConstants.FROM_DATE,
          ToDate = TestingConstants.TO_DATE
        },
        FinalPeriod = new BalancesPeriod {
          FromDate = new DateTime(2025, 04, 01),
          ToDate = new DateTime(2025, 04, 30)
        }
      };
    }

    #endregion Helpers

  } // class TrialBalanceUseCasesTests

} // namespace Empiria.FinancialAccounting.Tests.Balances
