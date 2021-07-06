/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Ledger Management                          Component : Test cases                              *
*  Assembly : Empiria.FinancialAccounting.Tests.dll      Pattern   : Use cases tests                         *
*  Type     : SubsidiaryLedgerUseCasesTests              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use case tests for subsidiary ledgers and their accounts.                                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using Xunit;

using Empiria.FinancialAccounting.UseCases;
using Empiria.FinancialAccounting.Adapters;

namespace Empiria.FinancialAccounting.Tests {

  /// <summary>Use case tests for subsidiary ledgers and their accounts.</summary>
  public class SubsidiaryLedgerUseCasesTests {

    #region Use cases initialization

    private readonly SubsidiaryLedgerUseCases _usecases;

    public SubsidiaryLedgerUseCasesTests() {
      CommonMethods.Authenticate();

      _usecases = SubsidiaryLedgerUseCases.UseCaseInteractor();
    }

    ~SubsidiaryLedgerUseCasesTests() {
      _usecases.Dispose();
    }

    #endregion Use cases initialization


    #region Facts

    [Fact]
    public void Should_Get_A_Subsidiary_Ledger() {
      SubsidiaryLedgerDto subledger = _usecases.GetSubsidiaryLedger(TestingConstants.SUBSIDIARY_LEDGER_UID);

      Assert.Equal(TestingConstants.SUBSIDIARY_LEDGER_UID, subledger.UID);
    }


    //[Fact]
    //public void Should_Get_A_Subsidiary_Ledger_Account() {
    //  LedgerAccountDto ledgerAccount = _usecases.GetLedgerAccount(TestingConstants.LEDGER_UID,
    //                                                              TestingConstants.LEDGER_ACCOUNT_ID);

    //  Assert.Equal(TestingConstants.LEDGER_UID, ledgerAccount.Ledger.UID);
    //  Assert.Equal(TestingConstants.LEDGER_ACCOUNT_ID, ledgerAccount.Id);
    //}


    [Fact]
    public void Should_Search_Subsidiary_Accounts() {
      string KEYWORDS = "Ramírez";

      FixedList<SubsidiaryAccountDto> subledgerAccounts = _usecases.SearchSubsidiaryAccounts(KEYWORDS);

      Assert.NotNull(subledgerAccounts);
      Assert.NotEmpty(subledgerAccounts);

      subledgerAccounts = _usecases.SearchSubsidiaryAccounts("sedfsjot7349esjfSDJKFH");

      Assert.NotNull(subledgerAccounts);
      Assert.Empty(subledgerAccounts);
    }


    #endregion Facts

  }  // class SubsidiaryLedgerUseCasesTests

}  // namespace Empiria.FinancialAccounting.Tests
