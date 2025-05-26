/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test cases                              *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Use cases tests                         *
*  Type     : BalanzaSatVsCoreBalancesTests              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Test cases for Listado de pólizas.                                                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Linq;
using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.Reporting;
using Empiria.FinancialAccounting.Reporting.FiscalReports.Builders;
using Xunit;

namespace Empiria.FinancialAccounting.Tests.Reporting {

  /// <summary>Test cases for Comparativo de cuentas.</summary>
  public class BalanzaSatVsCoreBalancesTests {


    [Theory]
    [InlineData("2024-01-01", "2024-01-31")]
    [InlineData("2024-02-01", "2024-02-29")]
    [InlineData("2024-03-01", "2024-03-31")]
    [InlineData("2024-04-01", "2024-04-30")]
    [InlineData("2024-05-01", "2024-05-31")]
    [InlineData("2024-06-01", "2024-06-30")]
    [InlineData("2024-07-01", "2024-07-31")]
    [InlineData("2024-08-01", "2024-08-31")]
    [InlineData("2024-09-01", "2024-09-30")]
    [InlineData("2024-10-01", "2024-10-31")]
    [InlineData("2024-11-01", "2024-11-30")]
    [InlineData("2024-12-01", "2024-12-31")]
    [InlineData("2025-01-01", "2025-01-31")]
    [InlineData("2025-02-01", "2025-02-28")]
    [InlineData("2025-03-01", "2025-03-31")]
    [InlineData("2025-04-01", "2025-04-30")]
    public void Should_Have_Same_Entries(string fromDate, string toDate) {

      CoreBalanceEntries coreBalances = TestsHelpers.GetCoreBalanceEntries(DateTime.Parse(fromDate),
                                                                           DateTime.Parse(toDate),
                                                                           ExchangeRateType.ValorizacionBanxico);

      FixedList<BalanzaSatEntry> balanzaSat = TestsHelpers.GetBalanzaSat(DateTime.Parse(fromDate),
                                                                                       DateTime.Parse(toDate));

      var _balanzaSat = balanzaSat.FindAll(x => x.Cuenta != "1.10.01.02" && // MAYO 2024
                                           x.Cuenta != "6.05.03.02.05" && // FEBRERO 2025
                                           x.Cuenta != "6.05.13.03.01" // FEBRERO 2025
                                           );

      foreach (var sut in balanzaSat) {
       
        var filtered = coreBalances.GetCoreEntriesByAccount(sut.Cuenta)
                                   .FindAll(x=>x.Account.DebtorCreditor == sut.DebtorCreditor);

        var totalInitialBalance = filtered.Sum(x => x.InitialBalance);
        var totalDebit = filtered.Sum(x => x.Debit);
        var totalCredit = filtered.Sum(x => x.Credit);
        var totalCurrentBalance = filtered.Sum(x => x.CurrentBalance);

        Assert.True(Math.Abs(totalInitialBalance - sut.SaldoInicial) <= 1,
                    TestsHelpers.BalanceDiffMsg("Saldo inicial", $"{sut.Cuenta} ({sut.DebtorCreditor})",
                                                totalInitialBalance, sut.SaldoInicial));

        Assert.True(Math.Abs(totalDebit - sut.Debe) <= 1,
                    TestsHelpers.BalanceDiffMsg("Debe", $"{sut.Cuenta} ({sut.DebtorCreditor})",
                                                totalDebit, sut.Debe));

        Assert.True(Math.Abs(totalCredit - sut.Haber) <= 1,
                    TestsHelpers.BalanceDiffMsg("Haber", $"{sut.Cuenta} ({sut.DebtorCreditor})",
                                                totalCredit, sut.Haber));

        Assert.True(Math.Abs(totalCurrentBalance - sut.SaldoFinal) <= 1,
                    TestsHelpers.BalanceDiffMsg("Saldo final", $"{sut.Cuenta} ({sut.DebtorCreditor})",
                                                totalCurrentBalance, sut.SaldoFinal));
      }

      Assert.True(balanzaSat.Count > 500);
    }


    [Fact]
    public void Should_Build_Balanza_Sat() {

      ReportBuilderQuery query = new ReportBuilderQuery {
        AccountsChartUID = "47ec2ec7-0f4f-482e-9799-c23107b60d8a",
        ReportType = ReportTypes.BalanzaSAT,
        FromDate = new DateTime(2025, 01, 01),
        ToDate = new DateTime(2025, 01, 31),
        ExportTo = Storage.FileType.Xml
      };

      using (var service = ReportingService.ServiceInteractor()) {
        ReportDataDto sut = service.GenerateReport(query);

        Assert.NotNull(sut);
        Assert.Equal(query, sut.Query);
        Assert.NotEmpty(sut.Entries);
      }
    }


  } // class ListadoDePolizasTests

} // namespace Empiria.FinancialAccounting.Tests.Reporting
