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

namespace Empiria.FinancialAccounting.FinancialReports {

  /// <summary>Calculates financial concepts values.</summary>
  internal class FinancialConceptsCalculator {

    private readonly ExecutionContext _executionContext;

    internal FinancialConceptsCalculator(ExecutionContext executionContext) {
      Assertion.Require(executionContext, nameof(executionContext));

      _executionContext = executionContext;
    }


    #region Public methods

    public IFinancialConceptValues CalculateBreakdownTotalEntry(FixedList<FinancialReportBreakdownResult> breakdown) {

      IFinancialConceptValues granTotal = CreateFinancialConceptValuesObject();

      foreach (var breakdownItem in breakdown) {

        IFinancialConceptValues breakdownTotals = CalculateBreakdownEntry(breakdownItem.IntegrationEntry);

        if (_executionContext.FinancialReportType.RoundTo != RoundTo.DoNotRound) {
          breakdownTotals = breakdownTotals.Round(_executionContext.FinancialReportType.RoundTo);
        }

        breakdownTotals.CopyTotalsTo(breakdownItem);

        switch (breakdownItem.IntegrationEntry.Operator) {

          case OperatorType.Add:
            granTotal = granTotal.Sum(breakdownTotals);
            break;

          case OperatorType.Substract:
            granTotal = granTotal.Substract(breakdownTotals);
            break;

          case OperatorType.AbsoluteValue:
            granTotal = granTotal.Sum(breakdownTotals)
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

          return totals.Sum(CalculateAccount(integrationEntry));

        case OperatorType.Substract:

          return totals.Substract(CalculateAccount(integrationEntry));

        case OperatorType.Multiply:

          return totals.Multiply(CalculateAccount(integrationEntry));

        case OperatorType.AbsoluteValue:

          return totals.Sum(CalculateAccount(integrationEntry))
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

          return totals.Sum(CalculateExternalVariable(integrationEntry));

        case OperatorType.Substract:

          return totals.Substract(CalculateExternalVariable(integrationEntry));

        case OperatorType.Multiply:

          return totals.Multiply(CalculateExternalVariable(integrationEntry));

        case OperatorType.AbsoluteValue:

          return totals.Sum(CalculateExternalVariable(integrationEntry))
                       .AbsoluteValue();

        default:
          throw Assertion.EnsureNoReachThisCode($"Unhandled operator '{integrationEntry.Operator}'.");
      }

    }


    private IFinancialConceptValues AccumulateFinancialConceptTotals(FinancialConceptEntry integrationEntry,
                                                                     IFinancialConceptValues totals) {

      Assertion.Require(integrationEntry.Type == FinancialConceptEntryType.FinancialConceptReference,
                        "Invalid integrationEntry.Type");

      IFinancialConceptValues returnValues;

      switch (integrationEntry.Operator) {

        case OperatorType.Add:

          returnValues = totals.Sum(CalculateFinancialConcept(integrationEntry.ReferencedFinancialConcept));
          break;

        case OperatorType.Substract:

          returnValues = totals.Substract(CalculateFinancialConcept(integrationEntry.ReferencedFinancialConcept));
          break;

        case OperatorType.Multiply:

          returnValues = totals.Multiply(CalculateFinancialConcept(integrationEntry.ReferencedFinancialConcept));
          break;

        case OperatorType.AbsoluteValue:

          returnValues = totals.Sum(CalculateFinancialConcept(integrationEntry.ReferencedFinancialConcept))
                               .AbsoluteValue();
          break;

        default:
          throw Assertion.EnsureNoReachThisCode($"Unhandled operator '{integrationEntry.Operator}'.");

      }

      if (integrationEntry.CalculationRule == "CambiarElSigno") {

        returnValues = returnValues.ChangeSign();

      }

      return returnValues;
    }


    private IFinancialConceptValues CalculateAccount(FinancialConceptEntry integrationEntry) {
      if (!_executionContext.BalancesProvider.ContainsAccount(integrationEntry.AccountNumber)) {
        return CreateFinancialConceptValuesObject();
      }

      FixedList<ITrialBalanceEntryDto> accountBalances =
                                  _executionContext.BalancesProvider.GetAccountBalances(integrationEntry);

      var totals = CreateFinancialConceptValuesObject();

      foreach (var balance in accountBalances) {

        if (integrationEntry.CalculationRule == "SaldoDeudorasMenosSaldoAcreedoras") {
          totals = totals.SumDebitsOrSubstractCredits(balance);
        } else {
          totals = totals.Sum(balance);
        }

      }

      if (integrationEntry.CalculationRule == "ConsolidarEnUnaColumna") {
        totals = totals.ConsolidateTotalsInto(integrationEntry.DataColumn);
      }

      if (_executionContext.FinancialReportType.RoundTo != RoundTo.DoNotRound) {
        totals = totals.Round(_executionContext.FinancialReportType.RoundTo);
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

      if (!_executionContext.ExternalValuesProvider.ContainsVariable(integrationEntry.ExternalVariableCode)) {
        return CreateFinancialConceptValuesObject();
      }

      FixedList<ExternalValue> externalValues =
                      _executionContext.ExternalValuesProvider.GetValues(integrationEntry.ExternalVariableCode);

      IFinancialConceptValues totals = CreateFinancialConceptValuesObject();

      foreach (var value in externalValues) {
        totals = totals.Sum(value);
      }

      if (integrationEntry.CalculationRule == "ConsolidarEnUnaColumna") {
        totals = totals.ConsolidateTotalsInto(integrationEntry.DataColumn);
      }

      return totals;
    }


    #endregion Private calculation methods

    #region Helpers

    private IFinancialConceptValues CreateFinancialConceptValuesObject() {
      return new FinancialConceptValues(_executionContext.FinancialReportType.DataColumns);
    }

    #endregion Helpers

  }  // class FinancialConceptsCalculator

}  // namespace Empiria.FinancialAccounting.FinancialReports
