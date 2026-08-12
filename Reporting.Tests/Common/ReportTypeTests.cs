/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                           Component : Test cases                            *
*  Assembly : .FinancialAccounting.Tests.ReportTypes       Pattern   : Unit tests                            *
*  Type     : ReportTypeTests                              License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Unit tests for financial accounting report types.                                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Xunit;

using Empiria.FinancialAccounting.Reporting;

namespace Empiria.FinancialAccounting.Tests {

  /// <summary>Unit tests for financial accounting report types.</summary>
  public class ReportTypeTests {

    #region Facts

    [Fact]
    public void Should_Parse_All_Report_Types() {
      var reportTypes = ReportType.GetList();

      foreach (var sut in reportTypes) {
        Assert.NotEmpty(sut.Name);
      }
    }


    [Fact]
    public void Should_Read_All_Report_Types() {
      var sut = ReportType.GetList();

      Assert.NotNull(sut);
      Assert.NotEmpty(sut);
    }

    #endregion Facts

  } // class ReportTypeTests

} // namespace Empiria.FinancialAccounting.Tests.ReportTypes
