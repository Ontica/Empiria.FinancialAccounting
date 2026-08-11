/* Empiria Financial ******************************************************************************************
*                                                                                                             *
*  Module   : Ledger Management                            Component : Test cases                             *
*  Assembly : FinancialAccounting.FinancialConcepts.Tests  Pattern   : Unit Test                              *
*  Type     : FinancialConceptMacroTests                   License   : Please read LICENSE.txt file           *
*                                                                                                             *
*  Summary  : Unit tests for Financial Concept Macro.                                                         *
*                                                                                                             *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. ***/
using Xunit;


namespace Empiria.FinancialAccounting.FinancialConcepts.Tests {

  /// <summary>Unit tests for Financial Concept Macro.</summary>
  public class FinancialConceptMacroTests {

    #region Facts

    [Fact]
    public void Should_Get_All_FinancialConceptMacro() {
      var sut = FinancialConceptMacro.GetList();

      Assert.NotNull(sut);
      Assert.NotEmpty(sut);
    }


    [Fact]
    public void Should_Parse_All_FinancialConceptMacro() {
      var workMAcroList = FinancialConceptMacro.GetList();

      foreach (var sut in workMAcroList) {
        Assert.NotNull(sut.Name);
      }
    }

    #endregion Facts

  }  // class FinancialConceptMacroTests

}  // namespace Empiria.FinancialAccounting.FinancialConcepts.Tests
