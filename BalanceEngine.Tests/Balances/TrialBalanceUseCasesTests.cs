/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test cases                              *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Use cases tests                         *
*  Type     : AccountBalanceUseCasesTests                License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Test cases for trial balance report.                                                           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Xunit;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.UseCases;

namespace Empiria.FinancialAccounting.Tests.Balances {

  /// <summary>Test cases for trial balance report.</summary>
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
    public void Should_Build_A_Traditional_Consolidated_Trial_Balance() {
      TrialBalanceCommand command = new TrialBalanceCommand();

      command.AccountsChartUID = "b2328e67-3f2e-45b9-b1f6-93ef6292204e";
      command.BalancesType = BalanceEngine.BalancesType.AllAccounts;
      command.ConsolidateBalancesToTargetCurrency = false;
      command.TrialBalanceType = BalanceEngine.TrialBalanceType.BalanzaValorizadaComparativa;
      command.FromAccount = "1103";
      command.ToAccount = "1103";
      command.Ledgers = new string[] {"2584a757-865c-2025-8025-fa633f200c49"};  

      command.InitialPeriod.FromDate = new DateTime(2021, 01, 01);
      command.InitialPeriod.ToDate = new DateTime(2021, 01, 31);
      command.InitialPeriod.ValuateToCurrrencyUID = "01";
      command.InitialPeriod.ExchangeRateDate = new DateTime(2021, 01, 31);
      command.InitialPeriod.ExchangeRateTypeUID = "5923136d-8533-4975-81b9-c8ec3bf18dea";
      //command.InitialPeriod.UseDefaultValuation = false;

      command.FinalPeriod.FromDate = new DateTime(2021, 02, 01);
      command.FinalPeriod.ToDate = new DateTime(2021, 02, 28);
      command.FinalPeriod.ValuateToCurrrencyUID = "01";
      command.FinalPeriod.ExchangeRateDate = new DateTime(2021, 02, 28);
      command.FinalPeriod.ExchangeRateTypeUID = "5923136d-8533-4975-81b9-c8ec3bf18dea";

      TrialBalanceDto trialBalance = _usecases.BuildTrialBalance(command);

      Assert.NotNull(trialBalance);
      Assert.Equal(command, trialBalance.Command);
      Assert.NotEmpty(trialBalance.Entries);
    }


    [Fact]
    public void Should_Build_A_Traditional_No_Consolidated_Trial_Balance() {
      TrialBalanceCommand command = new TrialBalanceCommand();

      command.TrialBalanceType = BalanceEngine.TrialBalanceType.Balanza;
      command.ShowCascadeBalances = true;
      command.InitialPeriod.FromDate = TestingConstants.FromDate;
      command.InitialPeriod.ToDate = TestingConstants.ToDate;

      TrialBalanceDto trialBalance = _usecases.BuildTrialBalance(command);

      Assert.NotNull(trialBalance);
      Assert.Equal(command, trialBalance.Command);
      Assert.NotEmpty(trialBalance.Entries);
    }

    #endregion Facts

  } // class TrialBalanceUseCasesTests

} // namespace Empiria.FinancialAccounting.Tests.Balances
