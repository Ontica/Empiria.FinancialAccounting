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

using Empiria.FinancialAccounting.FinancialReports.Adapters;
using Empiria.FinancialAccounting.FinancialReports.Providers;

namespace Empiria.FinancialAccounting.FinancialReports {

  /// <summary>Provides data about the execution context of financial report's concepts generation.</summary>
  internal class ExecutionContext {

    private readonly Lazy<AccountBalancesProvider> _balancesProvider;
    private readonly Lazy<ExternalValuesProvider> _externalValuesProvider;
    private readonly Lazy<FinancialConceptsCalculator> _conceptsCalculator;

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
                new FinancialConceptsCalculator(FinancialReportType.DataColumns,
                                                BalancesProvider,
                                                ExternalValuesProvider,
                                                FinancialReportType.RoundTo));
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


    internal FinancialConceptsCalculator ConceptsCalculator {
      get {
        return _conceptsCalculator.Value;
      }
    }

    #endregion Properties

  }  // class ExecutionContext

}  // namespace Empiria.FinancialAccounting.FinancialReports
