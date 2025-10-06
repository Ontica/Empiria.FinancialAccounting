/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Lists                             Component : Test cases                              *
*  Assembly : Empiria.FinancialAccounting.Tests.dll      Pattern   : Unit tests                              *
*  Type     : AccountClassificatorListTests              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Unit tests for AccountClassificatorList type.                                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using Xunit;

using Empiria.FinancialAccounting.AccountsLists;

namespace Empiria.FinancialAccounting.Tests {

  /// <summary>Unit tests for AccountClassificatorList type.</summary>
  public class AccountClassificatorListTests {

    #region Facts

    [Fact]
    public void Should_Parse_An_AccountsList() {
      var sut = AccountClassificatorList.Parse("BitacoraCuentasValorizacion");

      Assert.NotNull(sut);
      Assert.NotNull(sut.TryGetAccount("2.13.01.02.01.02"));
      Assert.NotNull(sut.TryGetAccountValue("2.13.01.02.01.02", "Rubro"));
    }



    [Fact]
    public void Should_Parse_All_AccountsList_Items() {
      var list = AccountClassificatorList.Parse("BitacoraCuentasValorizacion");

      foreach (AccountClassificator sut in list.Items) {
        Assert.NotNull(sut.Classificators);
        Assert.NotEmpty(sut.Classificators);
        Assert.NotEmpty(sut.GetClassificatorValue("ERI"));
        Assert.NotEmpty(sut.GetClassificatorValue("Completo"));
        Assert.NotEmpty(sut.GetClassificatorValue("Detallado"));
        Assert.NotEmpty(sut.GetClassificatorValue("Rubro"));
      }
    }

    #endregion Facts

  }  // class AccountClassificatorList

}  // namespace Empiria.FinancialAccounting.Tests
