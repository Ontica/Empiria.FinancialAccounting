/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Transactions Services                  Component : Test cases                        *
*  Assembly : FinancialAccounting.Transactions.Tests           Pattern   : Unit tests                        *
*  Type     : TransactionalSystemTests                         License   : Please read LICENSE.txt file      *
*                                                                                                            *
*  Summary  : Unit tests for TransactionalSystem type.                                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Xunit;

namespace Empiria.FinancialAccounting.Transactions.Tests {

  /// <summary>Unit tests for TransactionalSystem type.</summary>
  public class TransactionalSystemTests {

    #region Facts

    [Fact]
    public void Should_Parse_Transactional_Systems_WithCode() {
      var systemCodes = TransactionalSystem.GetList().Select(x => x.Code);

      foreach (var code in systemCodes) {
        var sut = TransactionalSystem.ParseWithCode(code);

        Assert.NotNull(sut);
      }
    }


    [Fact]
    public void Should_Read_Transactional_Systems() {
      var sut = TransactionalSystem.GetList();

      Assert.NotEmpty(sut);
      Assert.NotNull(sut);
    }


    [Fact]
    public void Should_Read_Transactional_Systems_Rules() {
      var list = TransactionalSystem.GetList();

      foreach (var system in list) {
        var sut = system.Rules;

        Assert.NotNull(sut);
      }
    }

    #endregion Facts

  }  // class TransactionalSystemTests

}  // namespace Empiria.FinancialAccounting.Transactions.Tests
