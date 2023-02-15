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

using Empiria.FinancialAccounting.FinancialConcepts;

using Empiria.FinancialAccounting.FinancialReports.Adapters;

namespace Empiria.FinancialAccounting.FinancialReports {

  /// <summary>Generates a report from financial concepts (e.g. R01, R10, R12).</summary>
  internal class FinancialConceptsReport {

    #region Constructor and fields

    private readonly ExecutionContext _executionContext;

    internal FinancialConceptsReport(FinancialReportQuery buildQuery) {
      Assertion.Require(buildQuery, nameof(buildQuery));

      _executionContext = new ExecutionContext(buildQuery);
    }

    #endregion Constructor and fields

    #region Properties

    private FinancialReportType FinancialReportType {
      get {
        return _executionContext.FinancialReportType;
      }
    }


    private FinancialConceptsCalculator ConceptsCalculator {
      get {
        return _executionContext.ConceptsCalculator;
      }
    }

    #endregion Properties

    #region Public methods

    internal FixedList<CalculatedConcept> CalculateConcepts(FixedList<FinancialConcept> concepts) {
      Assertion.Require(concepts, nameof(concepts));

      var calculatedList = new List<CalculatedConcept>(concepts.Count);

      foreach (var concept in concepts) {
        IFinancialConceptValues values = this.ConceptsCalculator.Calculate(concept);

        var calculatedConcept = new CalculatedConcept(concept, values);

        calculatedList.Add(calculatedConcept);
      }

      return calculatedList.ToFixedList();
    }


    internal FixedList<FinancialReportEntry> Generate() {
      FixedList<FinancialReportItemDefinition> reportItems = FinancialReportType.GetItems();

      FixedList<FinancialReportEntryResult> reportEntries = CreateReportEntriesWithoutTotals(reportItems);

      FillEntries(reportEntries);

      return reportEntries.Select(x => (FinancialReportEntry) x).ToFixedList();
    }


    internal FixedList<FinancialReportEntry> GenerateBreakdown(FinancialReportItemDefinition reportItem) {

      FinancialReportEntryResult reportItemTotals = CreateReportEntryWithoutTotals(reportItem);

      FixedList<FinancialReportBreakdownResult> breakdownEntries = CreateBreakdownEntriesWithoutTotals(reportItemTotals);

      IFinancialConceptValues breakdownTotal = this.ConceptsCalculator.CalculateBreakdownTotalEntry(breakdownEntries);

      breakdownTotal.CopyTotalsTo(reportItemTotals);

      var reportEntries = new List<FinancialReportEntry>();

      reportEntries.AddRange(breakdownEntries);
      reportEntries.Add(reportItemTotals);

      return reportEntries.ToFixedList();
    }


    internal FixedList<FinancialReportEntry> GenerateIntegration() {
      FixedList<FinancialReportItemDefinition> reportItems = FinancialReportType.GetItems();

      reportItems = FilterItemsWithIntegrationAccounts(reportItems);

      var reportEntries = new List<FinancialReportEntry>();

      foreach (var item in reportItems) {
        FinancialReportEntryResult reportEntry = CreateReportEntryWithoutTotals(item);

        FixedList<FinancialReportBreakdownResult> breakdownEntries = CreateBreakdownEntriesWithoutTotals(reportEntry);

        breakdownEntries = breakdownEntries.FindAll(x => x.IntegrationEntry.Type == FinancialConceptEntryType.Account);

        IFinancialConceptValues breakdownTotal = this.ConceptsCalculator.CalculateBreakdownTotalEntry(breakdownEntries);

        reportEntries.AddRange(breakdownEntries);

        breakdownTotal.CopyTotalsTo(reportEntry);

        reportEntries.Add(reportEntry);
      }

      return reportEntries.ToFixedList();
    }

    #endregion Public methods

    #region Private methods

    private FixedList<FinancialReportItemDefinition> FilterItemsWithIntegrationAccounts(FixedList<FinancialReportItemDefinition> list) {
      return list.FindAll(x => !x.FinancialConcept.IsEmptyInstance &&
                               x.FinancialConcept.Integration.Contains(item => item.Type == FinancialConceptEntryType.Account));
    }


    private void FillEntries(FixedList<FinancialReportEntryResult> reportEntries) {

      foreach (var reportEntry in reportEntries) {

        FinancialConcept financialConcept = reportEntry.FinancialConcept;

        if (financialConcept.IsEmptyInstance) {
          continue;
        }

        IFinancialConceptValues totals = this.ConceptsCalculator.Calculate(financialConcept);

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
