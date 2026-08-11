/* Empiria Financial ******************************************************************************************
*                                                                                                             *
*  Module   : Ledger Management                            Component : Test cases                             *
*  Assembly : FinancialAccounting.FinancialConcepts.Tests  Pattern   : Unit Test                              *
*  Type     : FinancialConceptGroupTests                   License   : Please read LICENSE.txt file           *
*                                                                                                             *
*  Summary  : Unit tests for Financial Concept Group.                                                         *
*                                                                                                             *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. ***/
using Xunit;


namespace Empiria.FinancialAccounting.FinancialConcepts.Tests {

  /// <summary>Unit tests for Financial Concept Group.</summary>
  public class FinancialConceptGroupTests {

    #region Facts

    [Fact]
    public void Should_Get_All_FinancialConceptGroups() {
      var sut = FinancialConceptGroup.GetList();

      Assert.NotNull(sut);
      Assert.NotEmpty(sut);
    }


    [Fact]
    public void Should_Parse_All_FinancialConceptGroups() {
      var workGroupList = FinancialConceptGroup.GetList();

      foreach (var sut in workGroupList) {
        Assert.NotNull(sut.Name);
      }
    }

    #endregion Facts

  }  // class FinancialConceptGroupTests

}  // namespace Empiria.FinancialAccounting.FinancialConcepts.Tests
