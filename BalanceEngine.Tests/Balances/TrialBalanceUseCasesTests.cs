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

using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.UseCases;
using Empiria.FinancialAccounting.BalanceEngine;

namespace Empiria.FinancialAccounting.Tests.Balances {

  /// <summary>Test cases for trial balance reports.</summary>
  public class TrialBalanceUseCasesTests {

    #region Fields

    private readonly TrialBalanceUseCases _usecases;

    #endregion Fields

    #region Initialization

    public TrialBalanceUseCasesTests() {
      CommonMethods.Authenticate();

      _usecases = TrialBalanceUseCases.UseCaseInteractor();
    }

    ~TrialBalanceUseCasesTests() {
      _usecases.Dispose();
    }

    #endregion Initialization

    #region Facts

    [Fact]
    public void Should_Build_Analitico_De_Cuentas() {
      TrialBalanceCommand command = GetDefaultTrialBalanceCommand();

      command.TrialBalanceType = TrialBalanceType.AnaliticoDeCuentas;
      command.UseDefaultValuation = true;
      command.WithSubledgerAccount = false;
      command.WithAverageBalance = true;

      TrialBalanceDto trialBalance = _usecases.BuildTrialBalance(command);

      Assert.NotNull(trialBalance);
      Assert.Equal(command, trialBalance.Command);
      Assert.NotEmpty(trialBalance.Entries);
    }


    [Fact]
    public void Should_Build_A_Traditional_Trial_Balance() {
      TrialBalanceCommand command = GetDefaultTrialBalanceCommand();

      command.TrialBalanceType = TrialBalanceType.SaldosPorAuxiliar;
      command.BalancesType = BalancesType.WithCurrentBalanceOrMovements;
      command.ShowCascadeBalances = false;
      command.SubledgerAccount = "90000000000342548";
      command.WithSubledgerAccount = true;

      TrialBalanceDto trialBalance = _usecases.BuildTrialBalance(command);

      Assert.NotNull(trialBalance);
      Assert.Equal(command, trialBalance.Command);
      Assert.NotEmpty(trialBalance.Entries);
    }


    [Fact]
    public void Should_Build_A_Traditional_No_Consolidated_Trial_Balance() {
      TrialBalanceCommand command = GetDefaultTrialBalanceCommand();

      command.TrialBalanceType = TrialBalanceType.Balanza;
      command.ShowCascadeBalances = true;

      TrialBalanceDto trialBalance = _usecases.BuildTrialBalance(command);

      Assert.NotNull(trialBalance);
      Assert.Equal(command, trialBalance.Command);
      Assert.NotEmpty(trialBalance.Entries);
    }


    [Fact]
    public void Should_Build_Balanza_Consolidada_Por_Moneda() {
      TrialBalanceCommand command = GetDefaultTrialBalanceCommand();

      command.TrialBalanceType = TrialBalanceType.BalanzaConsolidadaPorMoneda;
      TrialBalanceDto trialBalance = _usecases.BuildTrialBalance(command);

      Assert.NotNull(trialBalance);
      Assert.Equal(command, trialBalance.Command);
      Assert.NotEmpty(trialBalance.Entries);
    }

    #endregion Facts

    #region Helpers

    private TrialBalanceCommand GetDefaultTrialBalanceCommand() {
      return new TrialBalanceCommand() {
        AccountsChartUID = TestingConstants.ACCOUNTS_CHART_UID,
        BalancesType = BalancesType.WithCurrentBalanceOrMovements,
        TrialBalanceType = TrialBalanceType.Balanza,
        Ledgers = TestingConstants.BALANCE_LEDGERS_ARRAY,
        InitialPeriod = new TrialBalanceCommandPeriod() {
          FromDate = TestingConstants.FROM_DATE,
          ToDate = TestingConstants.TO_DATE
        },
        FinalPeriod = new TrialBalanceCommandPeriod() {
          FromDate = new DateTime(2021, 07, 01),
          ToDate = new DateTime(2021, 07, 31)
        }

      };
    }

    #endregion Helpers

  } // class TrialBalanceUseCasesTests

} // namespace Empiria.FinancialAccounting.Tests.Balances