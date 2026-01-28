/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Domain Layer                            *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Information holder                      *
*  Type     : ExecutionContext                           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides data about the execution context of financial report's concepts generation.           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.FinancialAccounting.FinancialConcepts;

using Empiria.FinancialAccounting.FinancialReports.Adapters;
using Empiria.FinancialAccounting.FinancialReports.Providers;

namespace Empiria.FinancialAccounting.FinancialReports {

  /// <summary>Provides data about the execution context of financial report's concepts generation.</summary>
  internal class ExecutionContext {

    private readonly Lazy<AccountBalancesProvider> _balancesProvider;
    private readonly Lazy<ExternalValuesProvider> _externalValuesProvider;
    private readonly Lazy<FinancialConceptsCalculator> _conceptsCalculator;
    private readonly Lazy<FixedList<CalculatedConcept>> _precalculatedConcepts;

    public ExecutionContext(FinancialReportQuery buildQuery) {
      Assertion.Require(buildQuery, nameof(buildQuery));

      this.BuildQuery = buildQuery;

      this.FinancialReportType = BuildQuery.GetFinancialReportType();

      _balancesProvider = new Lazy<AccountBalancesProvider>(() =>
                new AccountBalancesProvider(BuildQuery));

      _externalValuesProvider = new Lazy<ExternalValuesProvider>(() =>
                new ExternalValuesProvider(FinancialReportType.ExternalVariablesSets,
                                           BuildQuery.ToDate));

      _conceptsCalculator = new Lazy<FinancialConceptsCalculator>(() =>
                new FinancialConceptsCalculator(this));

      _precalculatedConcepts = new Lazy<FixedList<CalculatedConcept>>(() => BuildPrecalculatedConcepts());
    }


    #region Properties

    public FinancialReportQuery BuildQuery {
      get;
    }


    public FinancialReportType FinancialReportType {
      get;
    }


    public AccountBalancesProvider BalancesProvider {
      get {
        return _balancesProvider.Value;
      }
    }


    public ExternalValuesProvider ExternalValuesProvider {
      get {
        return _externalValuesProvider.Value;
      }
    }


    internal FixedList<CalculatedConcept> PrecalculatedConcepts {
      get {
        return _precalculatedConcepts.Value;
      }
    }


    internal FinancialConceptsCalculator ConceptsCalculator {
      get {
        return _conceptsCalculator.Value;
      }
    }

    #endregion Properties

    #region Methods

    internal ExecutionContext CreateCopy(FinancialReportType reportType) {
      var buildQuery = BuildQuery.Clone();

      buildQuery.ReportType = reportType.UID;

      return new ExecutionContext(buildQuery);
    }

    #endregion Methods

    #region Helpers

    private FixedList<CalculatedConcept> BuildPrecalculatedConcepts() {
      if (!FinancialReportType.PrecalculateConcepts) {
        return new FixedList<CalculatedConcept>();
      }

      FinancialReportQuery query = this.BuildQuery.Clone();

      query.ReportType = FinancialReportType.PrecalculateConceptsFromReportType.UID;

      FinancialConceptsReport report = new FinancialConceptsReport(query);

      FixedList<FinancialConcept> concepts = GetPrecalculatedConcepts();

      return report.CalculateConcepts(concepts);
    }


    private FixedList<FinancialConcept> GetPrecalculatedConcepts() {

      DateTime fromDate = this.BuildQuery.FromDate;
      DateTime toDate = this.BuildQuery.ToDate;

      FixedList<FinancialReportItemDefinition> reportItems = FinancialReportType.GetItems()
                                                                                .FindAll(x => fromDate <= x.EndDate && x.StartDate <= toDate);

      FixedList<FinancialConcept> reportConcepts = reportItems.FindAll(x => !x.FinancialConcept.IsEmptyInstance)
                                                              .Select(x => x.FinancialConcept)
                                                              .ToFixedList();

      var list = new List<FinancialConcept>(256);

      foreach (var reportConcept in reportConcepts) {
        LoadConceptAndItsIntegration(reportConcept, list);
      }

      return list.ToFixedList();
    }


    private void LoadConceptAndItsIntegration(FinancialConcept reportConcept,
                                              List<FinancialConcept> list) {

      var precalculatedGroups = this.FinancialReportType.PrecalculatedConceptsGroups;

      if (precalculatedGroups.Contains(reportConcept.Group)) {
        if (!list.Contains(reportConcept)) {
          list.Add(reportConcept);
        }
        return;
      }

      var candidates = reportConcept.Integration.FindAll(x => !x.ReferencedFinancialConcept.IsEmptyInstance)
                                                .Select(x => x.ReferencedFinancialConcept);

      foreach (var candidate in candidates) {
        LoadConceptAndItsIntegration(candidate, list);
      }

    }

    #endregion Helpers

  }  // class ExecutionContext

}  // namespace Empiria.FinancialAccounting.FinancialReports
