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

using Xunit;

using Empiria.Tests;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine;

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
    public void Should_Build_Balanza_Contabilidades_Cascada() {
      TrialBalanceQuery query = GetDefaultTrialBalanceQuery();

      query.TrialBalanceType = TrialBalanceType.BalanzaConContabilidadesEnCascada;
      query.BalancesType = BalancesType.WithCurrentBalanceOrMovements;
      query.UseDefaultValuation = false;
      query.WithSubledgerAccount = false;
      query.WithAverageBalance = false;
      query.ShowCascadeBalances = true;

      TrialBalanceDto sut = BalanceEngineProxy.BuildTrialBalance(query);

      Assert.NotNull(sut);
      Assert.Equal(query, sut.Query);
      Assert.NotEmpty(sut.Entries);
    }


    [Fact]
    public void Should_Build_Comparacion_Entre_Periodos() {
      TrialBalanceQuery query = GetDefaultTrialBalanceQuery();

      query.TrialBalanceType = TrialBalanceType.BalanzaValorizadaComparativa;
      query.BalancesType = BalancesType.WithCurrentBalanceOrMovements;
      query.UseDefaultValuation = true;
      query.WithSubledgerAccount = true;
      query.WithAverageBalance = false;
      query.ShowCascadeBalances = false;

      TrialBalanceDto sut = BalanceEngineProxy.BuildTrialBalance(query);

      Assert.NotNull(sut);
      Assert.Equal(query, sut.Query);
      Assert.NotEmpty(sut.Entries);
    }


    [Fact]
    public void Should_Build_A_Traditional_Trial_Balance() {
      TrialBalanceQuery query = GetDefaultTrialBalanceQuery();

      query.TrialBalanceType = TrialBalanceType.Balanza;
      query.BalancesType = BalancesType.WithCurrentBalanceOrMovements;
      query.ShowCascadeBalances = false;
      query.WithSubledgerAccount = false;
      query.UseDefaultValuation = false;
      query.WithAverageBalance = false;
      query.FromAccount = "1.05.01.03.02.03.01";
      query.ToAccount = "1.05.01.03.02.03.02";
      query.Ledgers = new string[] { "2cd4a2d3-4951-04f6-9d9a-dca96837580c", "144f536c-457a-3520-6384-c9d45d3d9482", "ae98b697-674a-519f-1dba-e1545fe81af7" };
      TrialBalanceDto sut = BalanceEngineProxy.BuildTrialBalance(query);
      
      Assert.NotNull(sut);
      Assert.Equal(query, sut.Query);
      Assert.NotEmpty(sut.Entries);
    }


    [Fact]
    public void Should_Build_Saldos_Por_Cuenta() {
      TrialBalanceQuery query = GetDefaultTrialBalanceQuery();

      query.TrialBalanceType = TrialBalanceType.SaldosPorCuenta;
      query.BalancesType = BalancesType.WithCurrentBalance;
      query.ShowCascadeBalances = false;

      TrialBalanceDto sut = BalanceEngineProxy.BuildTrialBalance(query);

      Assert.NotNull(sut);
      Assert.Equal(query, sut.Query);
      Assert.NotEmpty(sut.Entries);
    }


    [Fact]
    public void Should_Build_A_Traditional_No_Consolidated_Trial_Balance() {
      TrialBalanceQuery query = GetDefaultTrialBalanceQuery();

      query.TrialBalanceType = TrialBalanceType.Balanza;
      query.ShowCascadeBalances = true;

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
    public void Should_Build_Balanza_Dolarizada() {
      TrialBalanceQuery query = GetDefaultTrialBalanceQuery();
      query.TrialBalanceType = TrialBalanceType.BalanzaDolarizada;
      query.BalancesType = BalancesType.WithCurrentBalanceOrMovements;
      query.UseDefaultValuation = true;
      query.ShowCascadeBalances = false;
      query.FromAccount = "1";
      query.ToAccount = "1";

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
      query.ShowCascadeBalances = false;
      query.UseDefaultValuation = true;
      query.WithAverageBalance = false;
      query.FromAccount = "3.02.01";
      query.ToAccount = "3.02.01";

      TrialBalanceDto sut = BalanceEngineProxy.BuildTrialBalance(query);

      Assert.NotNull(sut);
      Assert.Equal(query, sut.Query);
      Assert.NotEmpty(sut.Entries);
    }


    #endregion Facts

    #region Helpers

    private TrialBalanceQuery GetDefaultTrialBalanceQuery() {
      return new TrialBalanceQuery() {
        AccountsChartUID = TestingConstants.ACCOUNTS_CHART_UID,
        BalancesType = BalancesType.WithCurrentBalanceOrMovements,
        TrialBalanceType = TrialBalanceType.BalanzaEnColumnasPorMoneda,
        ShowCascadeBalances = false,
        Ledgers = TestingConstants.BALANCE_LEDGERS_ARRAY,
        FromAccount = "1.05",
        ToAccount = "1.05",
        UseDefaultValuation = false,
        WithAverageBalance = false,
        WithSectorization = false,
        WithSubledgerAccount = false,
        Level=0,

        InitialPeriod = new BalancesPeriod {
          FromDate = TestingConstants.FROM_DATE,
          ToDate = TestingConstants.TO_DATE,
          //ExchangeRateDate = new DateTime(2024, 03, 31),
          //ExchangeRateTypeUID = ExchangeRateType.ValorizacionBanxico.UID,
          //ValuateToCurrrencyUID = "01"
    },

        //FinalPeriod = new BalancesPeriod {
        //  FromDate = new DateTime(2024, 01, 01),
        //  ToDate = new DateTime(2024, 01, 31)
        //}
      };
    }

    #endregion Helpers

  } // class TrialBalanceUseCasesTests

} // namespace Empiria.FinancialAccounting.Tests.Balances
