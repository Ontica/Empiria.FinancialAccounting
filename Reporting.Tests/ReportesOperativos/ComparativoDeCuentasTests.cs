/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test cases                              *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Use cases tests                         *
*  Type     : ComparativoDeCuentasTests                  License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Test cases for Comparativo de cuentas.                                                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.FinancialAccounting.Reporting;
using Xunit;

namespace Empiria.FinancialAccounting.Tests.Reporting {

  /// <summary>Test cases for Comparativo de cuentas.</summary>
  public class ComparativoDeCuentasTests {


    [Fact]
    public void Should_Build_Comparativo_De_Cuentas() {

      ReportBuilderQuery query = new ReportBuilderQuery {
        AccountsChartUID = "47ec2ec7-0f4f-482e-9799-c23107b60d8a",
        AccountNumber = "9",
        OutputType = "d551b39a-ae52-4c35-abbe-96d71e4a10bf",
        ReportType = ReportTypes.ComparativoDeCuentas,
        FromDate = new DateTime(2022, 06, 01),
        ToDate = new DateTime(2022, 06, 30)
      };

      using (var service = ReportingService.ServiceInteractor()) {
        ReportDataDto sut = service.GenerateReport(query);

        //FileReportDto fileReportDto = service.ExportReport(query, sut);

        Assert.NotNull(sut);
        Assert.Equal(query, sut.Query);
        Assert.NotEmpty(sut.Entries);
      }


    }


  } // class ComparativoDeCuentasTests

} // 
