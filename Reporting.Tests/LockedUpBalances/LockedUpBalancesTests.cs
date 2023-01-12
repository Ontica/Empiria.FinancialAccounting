/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test cases                              *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Use cases tests                         *
*  Type     : AccountStatementsTests                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Test cases for Locked up balances.                                                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.UseCases;
using Empiria.FinancialAccounting.Reporting;
using Empiria.FinancialAccounting.Reporting.LockedUpBalances.Adapters;
using Xunit;

namespace Empiria.FinancialAccounting.Tests.Reporting.LockedUpBalances {

  /// <summary>Test cases for Locked up balances.</summary>
  public class LockedUpBalancesTests {

    [Fact]
    public void ShouldBuildLockedUpBalances() {

      using (var service = LockedUpBalancesService.ServiceInteractor()) {

        ReportBuilderQuery buildQuery = new ReportBuilderQuery {
          AccountsChartUID = "47ec2ec7-0f4f-482e-9799-c23107b60d8a",
          FromDate = new DateTime(2022, 02, 01),
          ToDate = new DateTime(2022, 02, 28)
        };

        LockedUpBalancesDto sut = service.GenerateReport(buildQuery);

        Assert.NotNull(sut);
        Assert.NotEmpty(sut.Entries);
      }

    }

  } // class LockedUpBalancesTests

} // namespace Empiria.FinancialAccounting.Tests.Reporting.LockedUpBalances
