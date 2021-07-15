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
      //command.ExchangeRateDate = new DateTime(2021, 01, 15);
      //command.ExchangeRateTypeUID = "5923136d-8533-4975-81b9-c8ec3bf18dea";
      command.FromDate = TestingConstants.FromDate;
      command.ShowCascadeBalances = false;
      command.ToDate = TestingConstants.ToDate;
      command.TrialBalanceType = BalanceEngine.TrialBalanceType.SaldosPorAuxiliar;
      //command.ValuateToCurrrencyUID = "01";
      command.FromAccount = "1503";
      command.Ledgers = new string[1] {"3426b979-4797-4e13-4643-937361bc0fd9"};
      //command.ToAccount = "1103";

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
      command.FromDate = TestingConstants.FromDate;
      command.ToDate = TestingConstants.ToDate;

      TrialBalanceDto trialBalance = _usecases.BuildTrialBalance(command);

      Assert.NotNull(trialBalance);
      Assert.Equal(command, trialBalance.Command);
      Assert.NotEmpty(trialBalance.Entries);
    }

    #endregion Facts

  } // class TrialBalanceUseCasesTests

} // namespace Empiria.FinancialAccounting.Tests.Balances
