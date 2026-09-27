/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Accounting Rules                         Component : Test cases                      *
*  Assembly : FinancialAccounting.AccountingRules.Tests.dll      Pattern   : Domain tests                    *
*  Type     : AccountingRuleTests                                License   : Please read LICENSE.txt file    *
*                                                                                                            *
*  Summary  : Unit tests for AccountingRule type.                                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Xunit;

namespace Empiria.FinancialAccounting.AccountingRules.Tests {

  /// <summary>Unit tests for AccountingRule type.</summary>
  public class AccountingRuleTests {

    #region Facts

    [Fact]
    public void Should_Parse_All_Accounting_Rules() {
      var sut = BaseObject.GetFullList<AccountingRule>();

      Assert.NotNull(sut);
    }


    [Fact]
    public void Should_Parse_All_Accounting_Rules_Entries() {
      var rules = BaseObject.GetFullList<AccountingRule>();

      foreach (var rule in rules) {
        var sut = rule.Entries;

        Assert.NotNull(sut);
      }
    }


    [Fact]
    public void Should_Parse_Empty_AccountingRule() {
      var sut = AccountingRule.Empty;

      Assert.NotNull(sut);
    }

    #endregion Facts

  }  // class AccountingRuleTests

}  // namespace Empiria.FinancialAccounting.AccountingRules.Tests
