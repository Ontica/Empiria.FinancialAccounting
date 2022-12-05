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
using Empiria.FinancialAccounting.FinancialConcepts;

using Empiria.FinancialAccounting.FinancialReports.Providers;

namespace Empiria.FinancialAccounting.FinancialReports {

  /// <summary>Calculates financial concepts values.</summary>
  internal class FinancialConceptsCalculator {

    private readonly FixedList<DataTableColumn> _dataColumns;
    private readonly AccountBalancesProvider _balancesProvider;
    private readonly ExternalValuesProvider _externalValuesProvider;
    private readonly bool _roundDecimals;

    internal FinancialConceptsCalculator(FixedList<DataTableColumn> dataColumns,
                                         AccountBalancesProvider balancesProvider,
                                         ExternalValuesProvider externalValuesProvider,
                                         bool roundDecimals) {
      Assertion.Require(dataColumns, nameof(dataColumns));
      Assertion.Require(balancesProvider, nameof(balancesProvider));
      Assertion.Require(externalValuesProvider, nameof(externalValuesProvider));

      _dataColumns = dataColumns;
      _balancesProvider = balancesProvider;
      _externalValuesProvider = externalValuesProvider;
      _roundDecimals = roundDecimals;
    }


    #region Public methods

    public IFinancialConceptValues CalculateBreakdownTotalEntry(FixedList<FinancialReportBreakdownResult> breakdown) {

      IFinancialConceptValues granTotal = CreateFinancialConceptValuesObject();

      foreach (var breakdownItem in breakdown) {

        IFinancialConceptValues breakdownTotals = CalculateBreakdownEntry(breakdownItem.IntegrationEntry);

        if (_roundDecimals) {
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


    public IFinancialConceptValues CalculateFinancialConcept(FinancialConcept financialConcept) {
      Assertion.Require(!financialConcept.IsEmptyInstance,
                        "Cannot process the empty FinancialConcept instance.");

      IFinancialConceptValues totals = CreateFinancialConceptValuesObject();

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

    private IFinancialConceptValues AccumulateAccountTotals(FinancialConceptEntry integrationEntry,
                                                            IFinancialConceptValues totals) {

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


    private IFinancialConceptValues AccumulateExternalVariableTotals(FinancialConceptEntry integrationEntry,
                                                                     IFinancialConceptValues totals) {

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


    private IFinancialConceptValues AccumulateFinancialConceptTotals(FinancialConceptEntry integrationEntry,
                                                                     IFinancialConceptValues totals) {

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


    private IFinancialConceptValues CalculateAccount(FinancialConceptEntry integrationEntry) {
      if (!_balancesProvider.ContainsAccount(integrationEntry.AccountNumber)) {
        return CreateFinancialConceptValuesObject();
      }

      FixedList<ITrialBalanceEntryDto> accountBalances = _balancesProvider.GetAccountBalances(integrationEntry);

      var totals = CreateFinancialConceptValuesObject();

      foreach (var balance in accountBalances) {

        if (integrationEntry.CalculationRule == "SaldoDeudorasMenosSaldoAcreedoras") {
          totals = totals.SumDebitsOrSubstractCredits(balance, integrationEntry.DataColumn);
        } else {
          totals = totals.Sum(balance, integrationEntry.DataColumn);
        }
      }

      if (_roundDecimals) {
        totals = totals.Round();
      }

      if (integrationEntry.Operator == OperatorType.AbsoluteValue) {
        totals = totals.AbsoluteValue();
      }

      return totals;
    }


    private IFinancialConceptValues CalculateBreakdownEntry(FinancialConceptEntry integrationEntry) {

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


    private IFinancialConceptValues CalculateExternalVariable(FinancialConceptEntry integrationEntry) {

      if (!_externalValuesProvider.ContainsVariable(integrationEntry.ExternalVariableCode)) {
        return CreateFinancialConceptValuesObject();
      }

      FixedList<ExternalValue> externalValues = _externalValuesProvider.GetValues(integrationEntry.ExternalVariableCode);

      IFinancialConceptValues totals = CreateFinancialConceptValuesObject();

      foreach (var value in externalValues) {
        totals = totals.Sum(value, integrationEntry.DataColumn);
      }

      return totals;
    }


    #endregion Private calculation methods

    #region Helpers

    private IFinancialConceptValues CreateFinancialConceptValuesObject() {
      return new FinancialConceptValues(_dataColumns);
    }

    #endregion Helpers

  }  // class FinancialConceptsCalculator

}  // namespace Empiria.FinancialAccounting.FinancialReports
