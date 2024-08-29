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
using Empiria.FinancialAccounting.Vouchers.Adapters;
using Empiria.FinancialAccounting.Vouchers.UseCases;
using Empiria.Storage;
using Xunit;

namespace Empiria.FinancialAccounting.Tests.Reporting {

  /// <summary>Test cases for Comparativo de cuentas.</summary>
  public class ExportersTests {


    [Fact]
    public void ExportVoucherMovementesTest() {

      using (var usecases = VoucherUseCases.UseCaseInteractor()) {

        var exporter = new ExcelExporterService();

        VoucherDto voucher = usecases.GetVoucher(8381251);
        FileDto sut = exporter.Export(voucher);

        Assert.NotNull(sut);
      }
      
    }


    [Fact]
    public void ExportVouchersWithMovementesTest() {

      using (var usecases = VoucherUseCases.UseCaseInteractor()) {

        var exporter = new ExcelExporterService();

        int[] ids = new int[] { 8298449, 8298419, 8841127 }; //, 8742526 
        FixedList<VoucherDto> vouchers = usecases.GetVouchersToExport(ids);
        FileDto sut = exporter.Export(vouchers);

        Assert.NotNull(sut);
      }

    }


    [Fact]
    public void ExportVouchersByVerificationNumbersTest() {

      using (var service = ReportingService.ServiceInteractor()) {

        ReportBuilderQuery buildQuery = new ReportBuilderQuery {
          AccountsChartUID = "47ec2ec7-0f4f-482e-9799-c23107b60d8a",
          //AccountNumber = "1.05.01.03.02.03.01",
          VerificationNumbers = new string[] { "1", "2", "3" },
          ReportType = ReportTypes.MovimientosPorNumeroDeVerificacion,
          FromDate = new DateTime(2023, 06, 30),
          ToDate = new DateTime(2023, 06, 30),
          ExportTo = FileType.Excel
        };

        ReportDataDto reportData = service.GenerateReport(buildQuery);
        FileDto sut = service.ExportReport(buildQuery, reportData);
        Assert.NotNull(sut);
      }

    }


  } // class ComparativoDeCuentasTests

} //
