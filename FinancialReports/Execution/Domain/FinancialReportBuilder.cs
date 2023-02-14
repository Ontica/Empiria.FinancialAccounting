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

    internal FinancialReportBuilder(FinancialReportQuery buildQuery) {
      Assertion.Require(buildQuery, nameof(buildQuery));

      _buildQuery = buildQuery;
    }


    internal FinancialReport Build() {
      FinancialReportType reportType = _buildQuery.GetFinancialReportType();

      var financialConceptsReport = new FinancialConceptsReport(_buildQuery);

      if (reportType.DesignType != FinancialReportDesignType.AccountsIntegration) {

        FixedList<FinancialReportEntry> entries = financialConceptsReport.Generate();

        return MapToFinancialReport(entries);

      } else {

        FixedList<FinancialReportEntry> entries = financialConceptsReport.GenerateIntegration();

        return MapToFinancialReport(entries);
      }
    }


    internal FinancialReport GetBreakdown(FinancialReportItemDefinition reportItem) {
      Assertion.Require(reportItem, nameof(reportItem));

      var financialConceptsReport = new FinancialConceptsReport(_buildQuery);

      FixedList<FinancialReportEntry> entries = financialConceptsReport.GenerateBreakdown(reportItem);

      return MapToFinancialReport(entries);
    }


    private FinancialReport MapToFinancialReport<T>(FixedList<T> reportEntries) where T : FinancialReportEntry {
      var convertedEntries = reportEntries.Select(x => (FinancialReportEntry) x)
                                          .ToFixedList();

      return new FinancialReport(_buildQuery, convertedEntries);
    }


  } // class FinancialReportBuilder

} // namespace Empiria.FinancialAccounting.FinancialReports
