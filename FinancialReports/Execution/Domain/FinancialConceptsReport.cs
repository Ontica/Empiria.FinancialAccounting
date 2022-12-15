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

using Empiria.FinancialAccounting.FinancialConcepts;

using Empiria.FinancialAccounting.FinancialReports.Adapters;
using Empiria.FinancialAccounting.FinancialReports.Providers;

namespace Empiria.FinancialAccounting.FinancialReports {

  /// <summary>Generates a report from financial concepts (e.g. R01, R10, R12).</summary>
  internal class FinancialConceptsReport {

    private readonly FinancialReportType _financialReportType;
    private readonly FinancialConceptsCalculator _conceptsCalculator;

    #region Public methods

    internal FinancialConceptsReport(FinancialReportQuery buildQuery) {
      Assertion.Require(buildQuery, nameof(buildQuery));

      _financialReportType = buildQuery.GetFinancialReportType();

      var balancesProvider = new AccountBalancesProvider(buildQuery);

      var externalValuesProvider = new ExternalValuesProvider(_financialReportType.ExternalVariablesSets,
                                                              buildQuery.ToDate);

      _conceptsCalculator = new FinancialConceptsCalculator(_financialReportType.DataColumns,
                                                            balancesProvider,
                                                            externalValuesProvider,
                                                            _financialReportType.RoundTo);
    }


    internal FixedList<FinancialReportEntry> Generate() {
      FixedList<FinancialReportItemDefinition> reportItems = _financialReportType.GetItems();

      FixedList<FinancialReportEntryResult> reportEntries = CreateReportEntriesWithoutTotals(reportItems);

      FillEntries(reportEntries);

      CalculateFormulaBasedColumns(reportEntries);

      return reportEntries.Select(x => (FinancialReportEntry) x).ToFixedList();
    }


    internal FixedList<FinancialReportEntry> GenerateBreakdown(string reportItemUID) {
      FinancialReportItemDefinition reportItem = _financialReportType.GetItem(reportItemUID);

      FinancialReportEntryResult reportItemTotals = CreateReportEntryWithoutTotals(reportItem);

      FixedList<FinancialReportBreakdownResult> breakdownEntries = CreateBreakdownEntriesWithoutTotals(reportItemTotals);

      IFinancialConceptValues breakdownTotal = _conceptsCalculator.CalculateBreakdownTotalEntry(breakdownEntries);

      breakdownTotal.CopyTotalsTo(reportItemTotals);

      var reportEntries = new List<FinancialReportEntry>();

      reportEntries.AddRange(breakdownEntries);
      reportEntries.Add(reportItemTotals);

      CalculateFormulaBasedColumns(reportEntries);

      return reportEntries.ToFixedList();
    }


    internal FixedList<FinancialReportEntry> GenerateIntegration() {
      FixedList<FinancialReportItemDefinition> reportItems = _financialReportType.GetItems();

      reportItems = FilterItemsWithIntegrationAccounts(reportItems);

      var reportEntries = new List<FinancialReportEntry>();

      foreach (var item in reportItems) {
        FinancialReportEntryResult reportEntry = CreateReportEntryWithoutTotals(item);

        FixedList<FinancialReportBreakdownResult> breakdownEntries = CreateBreakdownEntriesWithoutTotals(reportEntry);

        breakdownEntries = breakdownEntries.FindAll(x => x.IntegrationEntry.Type == FinancialConceptEntryType.Account);

        IFinancialConceptValues breakdownTotal = _conceptsCalculator.CalculateBreakdownTotalEntry(breakdownEntries);

        reportEntries.AddRange(breakdownEntries);

        breakdownTotal.CopyTotalsTo(reportEntry);

        reportEntries.Add(reportEntry);
      }

      return reportEntries.ToFixedList();
    }


    #endregion Public methods

    #region Private methods

    private void CalculateFormulaBasedColumns(IEnumerable<FinancialReportEntryResult> reportEntries) {
      var calculator = new FinancialReportCalculator();

      IEnumerable<FinancialReportEntry> castedEntries = reportEntries.Select(entry => (FinancialReportEntry) entry);

      var columnsToCalculate = _financialReportType.DataColumns.FindAll(x => x.IsCalculated);

      calculator.CalculateColumns(columnsToCalculate, castedEntries);
    }


    private void CalculateFormulaBasedColumns(IEnumerable<FinancialReportEntry> reportEntries) {
      var calculator = new FinancialReportCalculator();

      var columnsToCalculate = _financialReportType.BreakdownColumns.FindAll(x => x.IsCalculated);

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

        IFinancialConceptValues totals = _conceptsCalculator.CalculateFinancialConcept(reportEntry.FinancialConcept);

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
