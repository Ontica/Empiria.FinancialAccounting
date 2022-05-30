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
      query.BalancesType = BalancesType.WithCurrentBalance;
      query.ShowCascadeBalances = false;
      query.WithSubledgerAccount = false;
      query.UseDefaultValuation = false;
      query.WithAverageBalance = true;

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

      query.TrialBalanceType = TrialBalanceType.BalanzaEnColumnasPorMoneda;

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


    #endregion Facts

    #region Helpers

    private TrialBalanceQuery GetDefaultTrialBalanceQuery() {
      return new TrialBalanceQuery() {
        AccountsChartUID = TestingConstants.ACCOUNTS_CHART_UID,
        BalancesType = BalancesType.WithCurrentBalanceOrMovements,
        TrialBalanceType = TrialBalanceType.Balanza,
        Ledgers = TestingConstants.BALANCE_LEDGERS_ARRAY,
        InitialPeriod = new BalancesPeriod {
          FromDate = TestingConstants.FROM_DATE,
          ToDate = TestingConstants.TO_DATE,
        },
        FinalPeriod = new BalancesPeriod {
          FromDate = new DateTime(2022, 02, 01),
          ToDate = new DateTime(2022, 02, 28)
        }
      };
    }

    #endregion Helpers

  } // class TrialBalanceUseCasesTests

} // namespace Empiria.FinancialAccounting.Tests.Balances
