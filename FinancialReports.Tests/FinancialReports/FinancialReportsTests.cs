/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                            Component : Test cases                            *
*  Assembly : FinancialAccounting.Tests.FinancialReports   Pattern   : Unit tests                            *
*  Type     : FinancialReportsTests                        License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Unit tests for financial report types.                                                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Xunit;

using Empiria.FinancialAccounting.FinancialReports;

namespace Empiria.FinancialAccounting.Tests.FinancialReports {

  /// <summary>Unit tests for financial report types.</summary>
  public class FinancialReportTypeTests {

    #region Facts

    [Fact]
    public void Should_Parse_All_Financial_Report_Types() {
      var reportypeLists = FinancialReportType.GetList();

      foreach (var sut in reportypeLists) {
        Assert.NotEmpty(sut.Name);
      }
    }


    [Fact]
    public void Should_Read_All_Financial_Report_Types() {
      var sut = FinancialReportType.GetList();

      Assert.NotNull(sut);
      Assert.NotEmpty(sut);
    }

    #endregion Facts

  } // class FinancialReportTypeTests

} // namespace Empiria.FinancialAccounting.Tests.FinancialReports
