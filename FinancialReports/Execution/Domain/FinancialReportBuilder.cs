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

    private readonly FinancialReportQuery _buildQuery;
    private readonly AccountBalancesProvider _balancesProvider;

    internal FinancialReportBuilder(FinancialReportQuery buildQuery) {
      Assertion.Require(buildQuery, nameof(buildQuery));

      _buildQuery = buildQuery;

      _balancesProvider = new AccountBalancesProvider(_buildQuery);
    }


    internal FinancialReport Build() {
      FinancialReportType reportType = _buildQuery.GetFinancialReportType();

      var financialConceptsReport = new FinancialConceptsReport(_buildQuery, _balancesProvider);

      if (reportType.DesignType == FinancialReportDesignType.FixedCells ||
          reportType.DesignType == FinancialReportDesignType.FixedRows) {

        FixedList<FinancialReportEntry> entries = financialConceptsReport.Generate();

        return MapToFinancialReport(entries);

      } else if (reportType.DesignType == FinancialReportDesignType.AccountsIntegration) {

        FixedList<FinancialReportEntry> entries = financialConceptsReport.GenerateIntegration();

        return MapToFinancialReport(entries);
      }

      throw Assertion.EnsureNoReachThisCode(
                $"Unhandled financial report design type {reportType.DesignType}.");
    }


    internal FinancialReport GetBreakdown(string reportRowUID) {
      FinancialReportType reportType = _buildQuery.GetFinancialReportType();

      var financialConceptsReport = new FinancialConceptsReport(_buildQuery, _balancesProvider);

      if (reportType.DesignType == FinancialReportDesignType.FixedRows) {

        FixedList<FinancialReportEntry> entries = financialConceptsReport.GenerateBreakdown(reportRowUID);

        return MapToFinancialReport(entries);

      }

      throw Assertion.EnsureNoReachThisCode(
                $"Unhandled financial report design type {reportType.DesignType}.");
    }


    private FinancialReport MapToFinancialReport<T>(FixedList<T> reportEntries) where T : FinancialReportEntry {
      var convertedEntries = new FixedList<FinancialReportEntry>(reportEntries.Select(x => (FinancialReportEntry) x));

      return new FinancialReport(_buildQuery, convertedEntries);
    }


  } // class FinancialReportBuilder

} // namespace Empiria.FinancialAccounting.FinancialReports
