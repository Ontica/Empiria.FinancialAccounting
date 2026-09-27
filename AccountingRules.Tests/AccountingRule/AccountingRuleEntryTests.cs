/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Accounting Rules                         Component : Test cases                      *
*  Assembly : FinancialAccounting.AccountingRules.Tests.dll      Pattern   : Domain tests                    *
*  Type     : AccountingRuleEntryTests                           License   : Please read LICENSE.txt file    *
*                                                                                                            *
*  Summary  : Unit tests for AccountingRuleEntry type.                                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Xunit;

namespace Empiria.FinancialAccounting.AccountingRules.Tests {

  /// <summary>Unit tests for AccountingRuleEntry type.</summary>
  public class AccountingRuleEntryTests {

    #region Facts

    [Fact]
    public void Should_Parse_All_Accounting_Rules_Entries() {
      var sut = BaseObject.GetFullList<AccountingRuleEntry>();

      Assert.NotNull(sut);
    }


    [Fact]
    public void Should_Parse_Empty_AccountingRuleEntry() {
      var sut = AccountingRuleEntry.Empty;

      Assert.NotNull(sut);
    }

    #endregion Facts

  }  // class AccountingRuleEntryTests

}  // namespace Empiria.FinancialAccounting.AccountingRules.Tests
