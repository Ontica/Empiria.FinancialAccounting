/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Domain Layer                            *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Service provider                        *
*  Type     : FinancialConceptsReport                    License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Generates a report from financial concepts (e.g. R01, R10, R12).                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;

using Empiria.Collections;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;

using Empiria.FinancialAccounting.ExternalData;
using Empiria.FinancialAccounting.FinancialReports.Adapters;
using Empiria.FinancialAccounting.FinancialConcepts;

namespace Empiria.FinancialAccounting.FinancialReports {

  /// <summary>Generates a report from financial concepts (e.g. R01, R10, R12).</summary>
  internal class FinancialConceptsReport {

    private readonly FinancialReportQuery _buildQuery;
    private readonly FinancialConceptsCalculator _conceptsCalculator;

    #region Public methods

    internal FinancialConceptsReport(FinancialReportQuery buildQuery,
                                     AccountBalancesProvider balancesProvider) {
      Assertion.Require(buildQuery, nameof(buildQuery));
      Assertion.Require(balancesProvider, nameof(balancesProvider));

      _buildQuery = buildQuery;

      _conceptsCalculator = new FinancialConceptsCalculator(_buildQuery, balancesProvider);

      FinancialReportType = _buildQuery.GetFinancialReportType();
    }


    public FinancialReportType FinancialReportType {
      get;
    }


    internal FixedList<FinancialReportEntry> Generate() {
      FixedList<FinancialReportItemDefinition> rowsAndCells = FinancialReportType.GetRowsAndCells();

      FixedList<FinancialReportEntryResult> reportEntries = CreateReportEntriesWithoutTotals(rowsAndCells);

      FillEntries(reportEntries);

      CalculateColumns(reportEntries);

      return reportEntries.Select(x => (FinancialReportEntry) x).ToFixedList();
    }


    internal FixedList<FinancialReportEntry> GenerateBreakdown(string reportItemUID) {
      FinancialReportItemDefinition reportItem = FinancialReportType.GetRow(reportItemUID);

      FinancialReportEntryResult reportItemTotals = CreateReportEntryWithoutTotals(reportItem);

      FixedList<FinancialReportBreakdownResult> breakdownEntries = CreateBreakdownEntriesWithoutTotals(reportItemTotals);

      ReportEntryTotals breakdownTotal = _conceptsCalculator.CalculateBreakdownTotalEntry(breakdownEntries);

      breakdownTotal.CopyTotalsTo(reportItemTotals);

      var reportEntries = new List<FinancialReportEntry>();

      reportEntries.AddRange(breakdownEntries);
      reportEntries.Add(reportItemTotals);

      CalculateColumns(reportEntries);

      return reportEntries.ToFixedList();
    }


    internal FixedList<FinancialReportEntry> GenerateIntegration() {
      FixedList<FinancialReportItemDefinition> rowsAndCells = FinancialReportType.GetRowsAndCells();

      rowsAndCells = FilterItemsWithIntegrationAccounts(rowsAndCells);

      var reportEntries = new List<FinancialReportEntry>();

      foreach (var item in rowsAndCells) {
        FinancialReportEntryResult reportEntry = CreateReportEntryWithoutTotals(item);

        FixedList<FinancialReportBreakdownResult> breakdownEntries = CreateBreakdownEntriesWithoutTotals(reportEntry);

        breakdownEntries = breakdownEntries.FindAll(x => x.IntegrationEntry.Type == FinancialConceptEntryType.Account);

        ReportEntryTotals breakdownTotal = _conceptsCalculator.CalculateBreakdownTotalEntry(breakdownEntries);

        reportEntries.AddRange(breakdownEntries);

        breakdownTotal.CopyTotalsTo(reportEntry);

        reportEntries.Add(reportEntry);
      }

      return reportEntries.ToFixedList();
    }


    #endregion Public methods

    #region Private methods

    private void CalculateColumns(IEnumerable<FinancialReportEntryResult> reportEntries) {
      var calculator = new FinancialReportCalculator(FinancialReportType);

      IEnumerable<FinancialReportEntry> castedEntries = reportEntries.Select(entry => (FinancialReportEntry) entry);

      var columnsToCalculate = FinancialReportType.DataColumns.FindAll(x => x.IsCalculated);

      calculator.CalculateColumns(columnsToCalculate, castedEntries);
    }


    private void CalculateColumns(IEnumerable<FinancialReportEntry> reportEntries) {
      var calculator = new FinancialReportCalculator(FinancialReportType);

      var columnsToCalculate = FinancialReportType.BreakdownColumns.FindAll(x => x.IsCalculated);

      calculator.CalculateColumns(columnsToCalculate, reportEntries);
    }


    private FixedList<FinancialReportItemDefinition> FilterItemsWithIntegrationAccounts(FixedList<FinancialReportItemDefinition> list) {
      return list.FindAll(x => !x.FinancialConcept.IsEmptyInstance &&
                               x.FinancialConcept.Integration.Contains(item => item.Type == FinancialConceptEntryType.Account));
    }


    private void FillEntries(FixedList<FinancialReportEntryResult> reportEntries) {

      foreach (var reportEntry in reportEntries) {

        if (reportEntry.FinancialConcept.IsEmptyInstance) {
          continue;
        }

        ReportEntryTotals totals = _conceptsCalculator.CalculateFinancialConcept(reportEntry.FinancialConcept);

        if (FinancialReportType.RoundDecimals) {
          totals = totals.Round();
        }

        totals.CopyTotalsTo(reportEntry);
      }
    }


    #endregion Private methods

    #region Helpers


    private FixedList<FinancialReportEntryResult> CreateReportEntriesWithoutTotals(FixedList<FinancialReportItemDefinition> reportItemsDef) {
      return reportItemsDef.Select(x => CreateReportEntryWithoutTotals(x))
                           .ToFixedList();
    }


    private FinancialReportEntryResult CreateReportEntryWithoutTotals(FinancialReportItemDefinition reportItemDefinition) {
      return new FinancialReportEntryResult(reportItemDefinition);
    }


    private FixedList<FinancialReportBreakdownResult> CreateBreakdownEntriesWithoutTotals(FinancialReportEntryResult reportEntry) {
      var integration = reportEntry.FinancialConcept.Integration;

      return integration.Select(x => new FinancialReportBreakdownResult(x))
                        .ToFixedList();
    }

    #endregion Helpers

  }  // class FinancialConceptsReport

}  // namespace Empiria.FinancialAccounting.FinancialReports
