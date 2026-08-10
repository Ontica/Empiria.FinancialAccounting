/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Ledger Management                          Component : Test cases                              *
*  Assembly : Empiria.FinancialAccounting.Tests.dll      Pattern   : Unit Test                               *
*  Type     : SubLedgerTypeTests                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Unit tests for SubLedger Types.                                                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using Xunit;


namespace Empiria.FinancialAccounting.Tests {

  /// <summary>Unit tests for SubLedger Types.</summary>
  public class SubLedgerTypeTests {

    #region Facts

    [Fact]
    public void Should_Get_All_SubledgerTypes() {
      var sut = SubledgerType.GetList();

      Assert.NotNull(sut);
      Assert.NotEmpty(sut);
    }


    [Fact]
    public void Should_Parse_All_SubledgerTypes() {
      var rules = SubledgerType.GetList();

      foreach (var sut in rules) {
        Assert.NotNull(sut.Name);
      }
    }


    [Fact]
    public void Should_Parse_Empty_SubledgerTypes() {
      var sut = SubledgerType.Empty;

      Assert.NotNull(sut);
    }

    #endregion Facts

  }  // class SubLedgerTests

}  // namespace Empiria.FinancialAccounting.Tests
