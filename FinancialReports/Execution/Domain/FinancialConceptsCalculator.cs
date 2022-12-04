/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Domain Layer                            *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Service provider                        *
*  Type     : FinancialConceptsCalculator                License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Calculates financial concepts values.                                                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;

using Empiria.FinancialAccounting.ExternalData;
using Empiria.FinancialAccounting.FinancialReports.Adapters;
using Empiria.FinancialAccounting.FinancialConcepts;

namespace Empiria.FinancialAccounting.FinancialReports {

  /// <summary>Calculates financial concepts values.</summary>
  internal class FinancialConceptsCalculator {

    private readonly FinancialReportQuery _buildQuery;
    private readonly FinancialReportType _financialReportType;

    private readonly AccountBalancesProvider _balancesProvider;

    internal FinancialConceptsCalculator(FinancialReportQuery buildQuery,
                                         AccountBalancesProvider balancesProvider) {
      Assertion.Require(buildQuery, nameof(buildQuery));
      Assertion.Require(balancesProvider, nameof(balancesProvider));

      _buildQuery = buildQuery;
      _financialReportType = _buildQuery.GetFinancialReportType();

      _balancesProvider = balancesProvider;
    }


    #region Public methods

    public ReportEntryTotals CalculateBreakdownTotalEntry(FixedList<FinancialReportBreakdownResult> breakdown) {

      ReportEntryTotals granTotal = CreateReportEntryTotalsObject();

      foreach (var breakdownItem in breakdown) {

        ReportEntryTotals breakdownTotals = CalculateBreakdownEntry(breakdownItem.IntegrationEntry);

        if (_financialReportType.RoundDecimals) {
          breakdownTotals = breakdownTotals.Round();
        }

        breakdownTotals.CopyTotalsTo(breakdownItem);

        switch (breakdownItem.IntegrationEntry.Operator) {

          case OperatorType.Add:
            granTotal = granTotal.Sum(breakdownTotals, breakdownItem.IntegrationEntry.DataColumn);
            break;

          case OperatorType.Substract:
            granTotal = granTotal.Substract(breakdownTotals, breakdownItem.IntegrationEntry.DataColumn);
            break;

          case OperatorType.AbsoluteValue:
            granTotal = granTotal.Sum(breakdownTotals, breakdownItem.IntegrationEntry.DataColumn)
                                 .AbsoluteValue();
            break;

        } // switch

      } // foreach

      return granTotal;
    }


    public ReportEntryTotals CalculateFinancialConcept(FinancialConcept financialConcept) {
      Assertion.Require(!financialConcept.IsEmptyInstance,
                        "Cannot process the empty FinancialConcept instance.");

      ReportEntryTotals totals = CreateReportEntryTotalsObject();

      foreach (var integrationItem in financialConcept.Integration) {

        switch (integrationItem.Type) {
          case FinancialConceptEntryType.FinancialConceptReference:

            totals = AccumulateFinancialConceptTotals(integrationItem, totals);
            break;

          case FinancialConceptEntryType.Account:
            totals = AccumulateAccountTotals(integrationItem, totals);
            break;

          case FinancialConceptEntryType.ExternalVariable:
            totals = AccumulateExternalVariableTotals(integrationItem, totals);
            break;
        }

      }  // foreach

      return totals;
    }


    #endregion Public methods

    #region Private calculation methods

    private ReportEntryTotals AccumulateAccountTotals(FinancialConceptEntry integrationEntry,
                                                       ReportEntryTotals totals) {

      Assertion.Require(integrationEntry.Type == FinancialConceptEntryType.Account,
                        "Invalid integrationEntry.Type");

      switch (integrationEntry.Operator) {

        case OperatorType.Add:

          return totals.Sum(CalculateAccount(integrationEntry),
                            integrationEntry.DataColumn);

        case OperatorType.Substract:

          return totals.Substract(CalculateAccount(integrationEntry),
                                  integrationEntry.DataColumn);

        case OperatorType.AbsoluteValue:

          return totals.Sum(CalculateAccount(integrationEntry),
                            integrationEntry.DataColumn)
                       .AbsoluteValue();

        default:
          throw Assertion.EnsureNoReachThisCode($"Unhandled operator '{integrationEntry.Operator}'.");
      }

    }


    private ReportEntryTotals AccumulateExternalVariableTotals(FinancialConceptEntry integrationEntry,
                                                               ReportEntryTotals totals) {

      Assertion.Require(integrationEntry.Type == FinancialConceptEntryType.ExternalVariable,
                       "Invalid integrationEntry.Type");

      switch (integrationEntry.Operator) {

        case OperatorType.Add:

          return totals.Sum(CalculateExternalVariable(integrationEntry),
                            integrationEntry.DataColumn);

        case OperatorType.Substract:

          return totals.Substract(CalculateExternalVariable(integrationEntry),
                                  integrationEntry.DataColumn);

        case OperatorType.AbsoluteValue:

          return totals.Sum(CalculateExternalVariable(integrationEntry),
                            integrationEntry.DataColumn)
                       .AbsoluteValue();

        default:
          throw Assertion.EnsureNoReachThisCode($"Unhandled operator '{integrationEntry.Operator}'.");
      }

    }


    private ReportEntryTotals AccumulateFinancialConceptTotals(FinancialConceptEntry integrationEntry,
                                                               ReportEntryTotals totals) {

      Assertion.Require(integrationEntry.Type == FinancialConceptEntryType.FinancialConceptReference,
                      "Invalid integrationEntry.Type");

      switch (integrationEntry.Operator) {

        case OperatorType.Add:

          return totals.Sum(CalculateFinancialConcept(integrationEntry.ReferencedFinancialConcept),
                            integrationEntry.DataColumn);

        case OperatorType.Substract:

          return totals.Substract(CalculateFinancialConcept(integrationEntry.ReferencedFinancialConcept),
                                  integrationEntry.DataColumn);

        case OperatorType.AbsoluteValue:

          return totals.Sum(CalculateFinancialConcept(integrationEntry.ReferencedFinancialConcept),
                            integrationEntry.DataColumn)
                       .AbsoluteValue();

        default:
          throw Assertion.EnsureNoReachThisCode($"Unhandled operator '{integrationEntry.Operator}'.");

      }

    }

    private ReportEntryTotals CalculateAccount(FinancialConceptEntry integrationEntry) {
      if (!_balancesProvider.ContainsAccount(integrationEntry.AccountNumber)) {
        return CreateReportEntryTotalsObject();
      }

      FixedList<ITrialBalanceEntryDto> accountBalances = _balancesProvider.GetAccountBalances(integrationEntry);

      var totals = CreateReportEntryTotalsObject();

      foreach (var balance in accountBalances) {

        if (integrationEntry.CalculationRule == "SaldoDeudorasMenosSaldoAcreedoras") {
          totals = totals.SumDebitsOrSubstractCredits(balance, integrationEntry.DataColumn);
        } else {
          totals = totals.Sum(balance, integrationEntry.DataColumn);
        }
      }

      if (_financialReportType.RoundDecimals) {
        totals = totals.Round();
      }

      if (integrationEntry.Operator == OperatorType.AbsoluteValue) {
        totals = totals.AbsoluteValue();
      }

      return totals;
    }


    private ReportEntryTotals CalculateBreakdownEntry(FinancialConceptEntry integrationEntry) {

      switch (integrationEntry.Type) {

        case FinancialConceptEntryType.FinancialConceptReference:
          return CalculateFinancialConcept(integrationEntry.ReferencedFinancialConcept);

        case FinancialConceptEntryType.Account:
          return CalculateAccount(integrationEntry);

        case FinancialConceptEntryType.ExternalVariable:
          return CalculateExternalVariable(integrationEntry);

        default:
          throw Assertion.EnsureNoReachThisCode();
      }
    }


    private ReportEntryTotals CalculateExternalVariable(FinancialConceptEntry integrationEntry) {
      var variable = ExternalVariable.TryParseWithCode(integrationEntry.ExternalVariableCode);

      ExternalValue value = ExternalValue.Empty;

      if (variable != null) {
        value = variable.GetValue(_buildQuery.ToDate);
      }

      var totals = CreateReportEntryTotalsObject();

      return totals.Sum(value, integrationEntry.DataColumn);
    }


    #endregion Private calculation methods

    #region Helpers

    private ReportEntryTotals CreateReportEntryTotalsObject() {
      switch (_financialReportType.DataSource) {

        case FinancialReportDataSource.AnaliticoCuentas:
          return new DynamicReportEntryTotals(_financialReportType.DataColumns);

        case FinancialReportDataSource.BalanzaEnColumnasPorMoneda:
          return new BalanzaEnColumnasPorMonedaReportEntryTotals();

        case FinancialReportDataSource.BalanzaTradicional:
          return new DynamicReportEntryTotals(_financialReportType.DataColumns);

        default:
          throw Assertion.EnsureNoReachThisCode($"Unhandled data source {_financialReportType.DataSource}.");
      }
    }

    #endregion Helpers

  }  // class FinancialConceptsCalculator

}  // namespace Empiria.FinancialAccounting.FinancialReports
