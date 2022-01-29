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
using System.Collections.Generic;

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
      command.AccountsChartUID = "47ec2ec7-0f4f-482e-9799-c23107b60d8a";
      command.BalancesType = BalancesType.WithCurrentBalance;
      command.FromAccount = "8.10";
      command.ToAccount = "8.10";
      command.UseDefaultValuation = true;
      command.WithSubledgerAccount = false;
      command.WithAverageBalance = false;
      command.ShowCascadeBalances = false;

      TrialBalanceDto trialBalance = _usecases.BuildTrialBalance(command);

      Assert.NotNull(trialBalance);
      Assert.Equal(command, trialBalance.Command);
      Assert.NotEmpty(trialBalance.Entries);
    }


    [Fact]
    public void Should_Build_A_Traditional_Trial_Balance() {
      TrialBalanceCommand command = GetDefaultTrialBalanceCommand();

      command.TrialBalanceType = TrialBalanceType.SaldosPorCuenta;
      command.BalancesType = BalancesType.WithCurrentBalance;
      //command.AccountsChartUID = "47ec2ec7-0f4f-482e-9799-c23107b60d8a";
      command.FromAccount = "2503-90-00-00-01";
      command.ToAccount = "2503-90-00-00-01";
      command.ShowCascadeBalances = true;
      command.WithSubledgerAccount = false;
      command.UseDefaultValuation = false;

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

      command.TrialBalanceType = TrialBalanceType.BalanzaEnColumnasPorMoneda;
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
          ToDate = TestingConstants.TO_DATE,
          //ExchangeRateDate = new DateTime(2021, 06, 30),
          //ExchangeRateTypeUID = "96c617f6-8ed9-47f3-8d2d-f1240e446e1d",
          //ValuateToCurrrencyUID = "01"
        },
        FinalPeriod = new TrialBalanceCommandPeriod() {
          FromDate = new DateTime(2021, 06, 01),
          ToDate = new DateTime(2021, 06, 30)
        }

      };
    }

    #endregion Helpers

  } // class TrialBalanceUseCasesTests

} // namespace Empiria.FinancialAccounting.Tests.Balances