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

        VoucherDto voucher = usecases.GetVoucher(2804);
        FileReportDto sut = exporter.Export(voucher);

        Assert.NotNull(sut);
      }
      
    }


    [Fact]
    public void ExportVouchersWithMovementesTest() {

      using (var usecases = VoucherUseCases.UseCaseInteractor()) {

        var exporter = new ExcelExporterService();

        int[] ids = new int[] { 8381251, 8742526 };
        FixedList<VoucherDto> vouchers = usecases.GetVouchers(ids);
        FileReportDto sut = exporter.Export(vouchers);

        Assert.NotNull(sut);
      }

    }


  } // class ComparativoDeCuentasTests

} //
