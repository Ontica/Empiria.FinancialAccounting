/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Accounting Core                  Component : Test cases                              *
*  Assembly : Empiria.FinancialAccounting.Tests.dll      Pattern   : Domain tests                            *
*  Type     : LedgerAccountTests                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Test cases for ledger accounts.                                                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using Xunit;

namespace Empiria.FinancialAccounting.Tests {

  /// <summary>Test cases for an account balance.</summary>
  public class LedgerAccountTests {

    #region Facts

    [Fact]
    public void Should_Parse_Ledger_Account() {
      var account = LedgerAccount.Parse(TestingConstants.ACCOUNT_NUMBER);

      Assert.Equal(TestingConstants.ACCOUNT_NUMBER, account.AccountNumber);
      Assert.Equal(TestingConstants.ACCOUNT_NAME, account.Name);
    }


    [Fact]
    public void Should_Parse_Empty_Ledger_Account() {
      var account = LedgerAccount.Empty;

      Assert.Equal(-1, account.Id);
      Assert.Equal("Empty", account.UID);
    }

    #endregion Facts

  }  // class LedgerAccountTests

}  // namespace Empiria.FinancialAccounting.Tests
