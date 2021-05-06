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
    public void Should_Build_A_Trial_Balance() {
      TrialBalanceCommand command = new TrialBalanceCommand();

      command.InitialDate = TestingConstants.InitialDate;
      command.FinalDate = TestingConstants.FinalDate;
      command.Fields = " LedgerId, CurrencyId, SectorId, LedgerAccountId, AccountId, " +
                        "-1 AS SubsidiaryAccountId, '' AS BudgetKey, '' AS AccountNumber ";
      command.Condition = "AND ID_SECTOR > 0 ";
      command.Grouping = " LedgerId, CurrencyId, SectorId, LedgerAccountId, AccountId ";
      command.Having = "";
      command.Ordering = "";

      if (command.Grouping.Contains("SectorId")) {
        command.Grouping = "GROUPING SETS((),"+
						"(LedgerId),"+
						"(LedgerId, CurrencyId),"+
						"(LedgerId, CurrencyId, SectorId),"+
						"(LedgerId, CurrencyId, SectorId, LedgerAccountId),"+
						"(LedgerId, CurrencyId, SectorId, LedgerAccountId, AccountId)"+
					  ")";
      }

      TrialBalanceDto trialBalance = _usecases.BuildTrialBalance(command);

      Assert.NotNull(trialBalance);
      Assert.Equal(command, trialBalance.Command);
      Assert.NotEmpty(trialBalance.Entries);
    }

    #endregion Facts

  } // class TrialBalanceUseCasesTests

} // namespace Empiria.FinancialAccounting.Tests.Balances
