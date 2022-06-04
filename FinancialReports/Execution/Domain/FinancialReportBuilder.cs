/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Domain Layer                            *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Service provider                        *
*  Type     : FinancialReportBuilder                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides services to generate financial and regulatory reports.                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.FinancialReports.Adapters;

namespace Empiria.FinancialAccounting.FinancialReports {

  /// <summary>Provides services to generate financial and regulatory reports.</summary>
  internal class FinancialReportBuilder {

    internal FinancialReportBuilder(FinancialReportQuery buildQuery) {
      Assertion.Require(buildQuery, nameof(buildQuery));

      this.BuildQuery = buildQuery;
    }


    public FinancialReportQuery BuildQuery {
      get;
    }


    internal FinancialReport Build() {
      FinancialReportType reportType = this.BuildQuery.GetFinancialReportType();

      switch (reportType.DesignType) {
        case FinancialReportDesignType.FixedRows:
          var fixedRows = new FixedRowFinancialConceptsReport(this.BuildQuery);

          return fixedRows.Generate();

        case FinancialReportDesignType.AccountsIntegration:
          var conceptsIntegration = new FixedRowFinancialConceptsReport(this.BuildQuery);

          return conceptsIntegration.GenerateIntegration();

        default:
          throw Assertion.EnsureNoReachThisCode(
                    $"Unhandled financial report design type {reportType.DesignType}.");
      }
    }


    internal FinancialReport GetBreakdown(string reportRowUID) {
      FinancialReportType reportType = this.BuildQuery.GetFinancialReportType();

      switch (reportType.DesignType) {
        case FinancialReportDesignType.FixedRows:
          var fixedRows = new FixedRowFinancialConceptsReport(this.BuildQuery);

          return fixedRows.GenerateBreakdown(reportRowUID);

        default:
          throw Assertion.EnsureNoReachThisCode(
                    $"Unhandled financial report design type {reportType.DesignType}.");
      }
    }

  } // class FinancialReportBuilder

} // namespace Empiria.FinancialAccounting.FinancialReports
