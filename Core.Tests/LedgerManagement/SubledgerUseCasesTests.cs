/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Ledger Management                          Component : Test cases                              *
*  Assembly : Empiria.FinancialAccounting.Tests.dll      Pattern   : Use cases tests                         *
*  Type     : SubledgerUseCasesTests                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use case tests for subledgers and their accounts.                                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using Xunit;

using Empiria.FinancialAccounting.UseCases;
using Empiria.FinancialAccounting.Adapters;

namespace Empiria.FinancialAccounting.Tests {

  /// <summary>Use case tests for subledgers and their accounts.</summary>
  public class SubledgerUseCasesTests {

    #region Use cases initialization

    private readonly SubledgerUseCases _usecases;

    public SubledgerUseCasesTests() {
      CommonMethods.Authenticate();

      _usecases = SubledgerUseCases.UseCaseInteractor();
    }

    ~SubledgerUseCasesTests() {
      _usecases.Dispose();
    }

    #endregion Use cases initialization


    #region Facts

    [Fact]
    public void Should_Get_A_Subledger() {
      SubledgerDto subledger = _usecases.GetSubledger(TestingConstants.SUBLEDGER_UID);

      Assert.Equal(TestingConstants.SUBLEDGER_UID, subledger.UID);
    }


    //[Fact]
    //public void Should_Get_A_Subledger_Account() {
    //  LedgerAccountDto ledgerAccount = _usecases.GetLedgerAccount(TestingConstants.LEDGER_UID,
    //                                                              TestingConstants.LEDGER_ACCOUNT_ID);

    //  Assert.Equal(TestingConstants.LEDGER_UID, ledgerAccount.Ledger.UID);
    //  Assert.Equal(TestingConstants.LEDGER_ACCOUNT_ID, ledgerAccount.Id);
    //}


    [Fact]
    public void Should_Search_Subledger_Accounts() {
      //string KEYWORDS = "Ramírez";

      //FixedList<SubledgerAccountDto> subledgerAccounts = _usecases.SearchSubledgerAccounts(KEYWORDS);

      //Assert.NotNull(subledgerAccounts);
      //Assert.NotEmpty(subledgerAccounts);

      //subledgerAccounts = _usecases.SearchSubledgerAccounts("sedfsjot7349esjfSDJKFH");

      //Assert.NotNull(subledgerAccounts);
      //Assert.Empty(subledgerAccounts);
    }


    #endregion Facts

  }  // class SubledgerUseCasesTests

}  // namespace Empiria.FinancialAccounting.Tests
