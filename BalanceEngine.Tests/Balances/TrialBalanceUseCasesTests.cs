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
      TrialBalanceCommand command = GetDefaultTrialBalanceCommand();

      command.TrialBalanceType = TrialBalanceType.BalanzaConContabilidadesEnCascada;
      command.BalancesType = BalancesType.WithCurrentBalanceOrMovements;
      command.UseDefaultValuation = false;
      command.WithSubledgerAccount = false;
      command.WithAverageBalance = false;
      command.ShowCascadeBalances = true;

      TrialBalanceDto sut = BalanceEngineProxy.BuildTrialBalance(command);

      Assert.NotNull(sut);
      Assert.Equal(command, sut.Command);
      Assert.NotEmpty(sut.Entries);
    }


    [Fact]
    public void Should_Build_Comparacion_Entre_Periodos() {
      TrialBalanceCommand command = GetDefaultTrialBalanceCommand();

      command.TrialBalanceType = TrialBalanceType.BalanzaValorizadaComparativa;
      command.BalancesType = BalancesType.WithCurrentBalanceOrMovements;
      command.UseDefaultValuation = true;
      command.WithSubledgerAccount = true;
      command.WithAverageBalance = false;
      command.ShowCascadeBalances = false;

      TrialBalanceDto sut = BalanceEngineProxy.BuildTrialBalance(command);

      Assert.NotNull(sut);
      Assert.Equal(command, sut.Command);
      Assert.NotEmpty(sut.Entries);
    }


    [Fact]
    public void Should_Build_A_Traditional_Trial_Balance() {
      TrialBalanceCommand command = GetDefaultTrialBalanceCommand();

      command.TrialBalanceType = TrialBalanceType.Balanza;
      command.BalancesType = BalancesType.WithCurrentBalance;
      command.ShowCascadeBalances = false;
      command.WithSubledgerAccount = false;
      command.UseDefaultValuation = false;
      command.WithAverageBalance = true;

      TrialBalanceDto sut = BalanceEngineProxy.BuildTrialBalance(command);

      Assert.NotNull(sut);
      Assert.Equal(command, sut.Command);
      Assert.NotEmpty(sut.Entries);
    }


    [Fact]
    public void Should_Build_A_Traditional_No_Consolidated_Trial_Balance() {
      TrialBalanceCommand command = GetDefaultTrialBalanceCommand();

      command.TrialBalanceType = TrialBalanceType.Balanza;
      command.ShowCascadeBalances = true;

      TrialBalanceDto sut = BalanceEngineProxy.BuildTrialBalance(command);

      Assert.NotNull(sut);
      Assert.Equal(command, sut.Command);
      Assert.NotEmpty(sut.Entries);
    }


    [Fact]
    public void Should_Build_Balanza_Consolidada_Por_Moneda() {
      TrialBalanceCommand command = GetDefaultTrialBalanceCommand();

      command.TrialBalanceType = TrialBalanceType.BalanzaEnColumnasPorMoneda;

      TrialBalanceDto sut = BalanceEngineProxy.BuildTrialBalance(command);

      Assert.NotNull(sut);
      Assert.Equal(command, sut.Command);
      Assert.NotEmpty(sut.Entries);
    }


    [Fact]
    public void Should_Build_Balanza_Dolarizada() {
      TrialBalanceCommand command = GetDefaultTrialBalanceCommand();
      command.TrialBalanceType = TrialBalanceType.BalanzaDolarizada;
      command.BalancesType = BalancesType.WithCurrentBalanceOrMovements;
      command.UseDefaultValuation = true;
      command.ShowCascadeBalances = false;
      command.FromAccount = "1";
      command.ToAccount = "1";

      TrialBalanceDto sut = BalanceEngineProxy.BuildTrialBalance(command);

      Assert.NotNull(sut);
      Assert.Equal(command, sut.Command);
      Assert.NotEmpty(sut.Entries);
    }


    #endregion Facts

    #region Helpers

    private TrialBalanceCommand GetDefaultTrialBalanceCommand() {
      return new TrialBalanceCommand() {
        AccountsChartUID = TestingConstants.ACCOUNTS_CHART_UID,
        BalancesType = BalancesType.WithCurrentBalanceOrMovements,
        TrialBalanceType = TrialBalanceType.Balanza,
        Ledgers = TestingConstants.BALANCE_LEDGERS_ARRAY,
        InitialPeriod = new BalanceEngineCommandPeriod() {
          FromDate = TestingConstants.FROM_DATE,
          ToDate = TestingConstants.TO_DATE,
          //ExchangeRateDate = new DateTime(2021, 06, 30),
          //ExchangeRateTypeUID = "96c617f6-8ed9-47f3-8d2d-f1240e446e1d",
          //ValuateToCurrrencyUID = "01"
        },
        FinalPeriod = new BalanceEngineCommandPeriod() {
          FromDate = new DateTime(2022, 02, 01),
          ToDate = new DateTime(2022, 02, 28)
        }

      };
    }

    #endregion Helpers

  } // class TrialBalanceUseCasesTests

} // namespace Empiria.FinancialAccounting.Tests.Balances
