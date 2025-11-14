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
using System.Security.Principal;
using System.Threading.Tasks;
using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.UseCases;
using Empiria.FinancialAccounting.Reporting;
using Empiria.FinancialAccounting.Reporting.Balances;
using Empiria.FinancialAccounting.Vouchers.Adapters;
using Empiria.FinancialAccounting.Vouchers.UseCases;
using Empiria.Services;
using Empiria.Storage;
using Xunit;

namespace Empiria.FinancialAccounting.Tests.Reporting {

  /// <summary>Test cases for Comparativo de cuentas.</summary>
  public class ExportersTests {


    [Fact]
    public void ExportTrialBalanceTest() {
      using (var usecases = TrialBalanceUseCases.UseCaseInteractor()) {

        var query = new TrialBalanceQuery() {
          AccountsChartUID = AccountsChart.IFRS.UID,
          BalancesType = BalancesType.WithCurrentBalanceOrMovements,
          TrialBalanceType = TrialBalanceType.ResumenAjusteAnual,
          Ledgers = new string[] { },
          ShowCascadeBalances = false,
          UseDefaultValuation = false,
          WithSubledgerAccount = false,
          InitialPeriod = new BalancesPeriod {
            FromDate = new DateTime(2025, 08, 01),
            ToDate = new DateTime(2025, 08, 31)
          }
        };

        TrialBalanceDto trialBalance = usecases.BuildTrialBalance(query);

        var excelExporter = new BalancesExcelExporterService();

        FileDto excelFileDto = excelExporter.Export(trialBalance);

        Assert.NotNull(excelFileDto);
      }
    }


    [Fact]
    public async Task ExportSaldosEncerradosTest() {
      
      using (var service = TrialBalanceUseCases.UseCaseInteractor()) {

        SaldosEncerradosQuery query = new SaldosEncerradosQuery {
          AccountsChartUID = AccountsChart.IFRS.UID,
          FromDate = new DateTime(2022, 01, 01),
          ToDate = new DateTime(2025, 09, 30),
        };

        SaldosEncerradosDto reportData = await service.BuildSaldosEncerrados(query);

        var excelExporter = new BalancesExcelExporterService();

        FileDto excelFileDto = excelExporter.Export(reportData);

        Assert.NotNull(excelFileDto);
      }
    }


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

        int[] ids = new int[] { 8926717, 8770140 };
        FixedList<VoucherDto> vouchers = usecases.GetVouchersToExport(ids);
        FileDto sut = exporter.Export(vouchers);

        Assert.NotNull(sut);
      }

    }


    [Fact]
    public void ExportListadoMovimientosPorPolizasTest() {

      using (var service = ReportingService.ServiceInteractor()) {

        ReportBuilderQuery buildQuery = new ReportBuilderQuery {
          AccountsChartUID = AccountsChart.IFRS.UID,
          VoucherIds = QueryVoucherArray.GetVouchersIdArray(),
          WithSubledgerAccount = true,
          ReportType = ReportTypes.ListadoMovimientosPorPolizas,
          ExportTo = FileType.Excel
        };

        ReportDataDto reportData = service.GenerateReport(buildQuery);
        FileDto sut = service.ExportReport(buildQuery, reportData);
        Assert.NotNull(sut);
      }
    }


    [Fact]
    public void ExportBalanzaSATTest() {

      using (var service = ReportingService.ServiceInteractor()) {

        ReportBuilderQuery query = new ReportBuilderQuery {
          AccountsChartUID = "47ec2ec7-0f4f-482e-9799-c23107b60d8a",
          ReportType = ReportTypes.BalanzaSAT,
          FromDate = new DateTime(2025, 09, 01),
          ToDate = new DateTime(2025, 09, 30),
          ExportTo = FileType.Excel
        };

        ReportDataDto reportData = service.GenerateReport(query);
        FileDto sut = service.ExportReport(query, reportData);
        Assert.NotNull(sut);
      }
    }


    [Fact]
    public void ExportBalanzaDeterminarImpuestosTest() {

      using (var service = ReportingService.ServiceInteractor()) {

        ReportBuilderQuery query = new ReportBuilderQuery {
          AccountsChartUID = "47ec2ec7-0f4f-482e-9799-c23107b60d8a",
          ReportType = ReportTypes.BalanzaDeterminarImpuestos,
          FromDate = new DateTime(2025, 09, 01),
          ToDate = new DateTime(2025, 09, 30),
          ExportTo = FileType.Excel
        };

        ReportDataDto reportData = service.GenerateReport(query);
        FileDto sut = service.ExportReport(query, reportData);
        Assert.NotNull(sut);
      }
    }


    [Fact]
    public void ExportValorizacionEstimacionTest() {

      using (var service = ReportingService.ServiceInteractor()) {

        ReportBuilderQuery query = new ReportBuilderQuery {
          AccountsChartUID = "47ec2ec7-0f4f-482e-9799-c23107b60d8a",
          ReportType = ReportTypes.ValorizacionEstimacionPreventiva,
          FromDate = new DateTime(2024, 01, 01),
          ToDate = new DateTime(2024, 03, 16),
          ExportTo = FileType.Excel
        };

        ReportDataDto reportData = service.GenerateReport(query);
        FileDto sut = service.ExportReport(query, reportData);
        Assert.NotNull(sut);
      }
    }


  } // class ComparativoDeCuentasTests

} //
