/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Accounting Rules                         Component : Test cases                      *
*  Assembly : FinancialAccounting.AccountingRules.Tests.dll      Pattern   : Domain tests                    *
*  Type     : AccountingGuideTests                               License   : Please read LICENSE.txt file    *
*                                                                                                            *
*  Summary  : Unit tests for AccountingGuide type.                                                           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Xunit;

namespace Empiria.FinancialAccounting.AccountingRules.Tests {

  /// <summary>Unit tests for AccountingGuide type.</summary>
  public class AccountingGuideTests {

    #region Facts

    [Fact]
    public void Should_Parse_All_Accounting_Guides() {
      var sut = BaseObject.GetFullList<AccountingGuide>();

      Assert.NotNull(sut);
    }


    [Fact]
    public void Should_Parse_All_Accounting_Guides_Rules() {
      var guides = BaseObject.GetFullList<AccountingGuide>();

      foreach (var guide in guides) {
        var rules = guide.Rules;

        Assert.NotNull(rules);
      }
    }


    [Fact]
    public void Should_Parse_Empty_AccountingGuide() {
      var sut = AccountingGuide.Empty;

      Assert.NotNull(sut);
    }

    #endregion Facts

  }  // class AccountingGuideTests

}  // namespace Empiria.FinancialAccounting.AccountingRules.Tests
