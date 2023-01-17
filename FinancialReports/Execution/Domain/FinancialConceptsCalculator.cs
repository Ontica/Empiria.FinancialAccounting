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

    public IFinancialConceptValues Calculate(FinancialConcept financialConcept) {
      Assertion.Require(financialConcept, nameof(financialConcept));

      Assertion.Require(!financialConcept.IsEmptyInstance,
                    "Cannot process the empty FinancialConcept instance.");

      IFinancialConceptValues totals = this.CalculateFinancialConcept(financialConcept);

      if (financialConcept.HasScript) {
        ExecuteConceptScript(financialConcept, totals);
      }

      return totals;
    }


    public IFinancialConceptValues CalculateBreakdownTotalEntry(FixedList<FinancialReportBreakdownResult> breakdown) {

      IFinancialConceptValues granTotal = CreateFinancialConceptValuesObject();

      foreach (var breakdownItem in breakdown) {

        IFinancialConceptValues breakdownValues = CalculateBreakdownEntry(breakdownItem.IntegrationEntry);

        if (_executionContext.FinancialReportType.RoundTo != RoundTo.DoNotRound) {
          breakdownValues = breakdownValues.Round(_executionContext.FinancialReportType.RoundTo);
        }

        CalculateFormulaBasedColumns(breakdownItem.FinancialConcept, breakdownValues);

        breakdownValues.CopyTotalsTo(breakdownItem);

        granTotal = ApplyOperator(breakdownItem.IntegrationEntry.Operator,
                                  granTotal, breakdownValues);

      } // foreach

      return granTotal;
    }


    private IFinancialConceptValues CalculateFinancialConcept(FinancialConcept financialConcept) {
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

      CalculateFormulaBasedColumns(financialConcept, totals);

      return totals;
    }


    #endregion Public methods

    #region Private calculation methods

    private IFinancialConceptValues AccumulateAccountTotals(FinancialConceptEntry integrationEntry,
                                                            IFinancialConceptValues totals) {

      Assertion.Require(integrationEntry.Type == FinancialConceptEntryType.Account,
                        "Invalid integrationEntry.Type");

      var value = CalculateAccount(integrationEntry);

      return ApplyOperator(integrationEntry.Operator, totals, value);
    }


    private IFinancialConceptValues AccumulateExternalVariableTotals(FinancialConceptEntry integrationEntry,
                                                                     IFinancialConceptValues totals) {

      Assertion.Require(integrationEntry.Type == FinancialConceptEntryType.ExternalVariable,
                       "Invalid integrationEntry.Type");

      var value = CalculateExternalVariable(integrationEntry);

      return ApplyOperator(integrationEntry.Operator, totals, value);
    }


    private IFinancialConceptValues AccumulateFinancialConceptTotals(FinancialConceptEntry integrationEntry,
                                                                     IFinancialConceptValues totals) {

      Assertion.Require(integrationEntry.Type == FinancialConceptEntryType.FinancialConceptReference,
                        "Invalid integrationEntry.Type");

      var value = CalculateFinancialConcept(integrationEntry.ReferencedFinancialConcept);

      IFinancialConceptValues returnValues = ApplyOperator(integrationEntry.Operator, totals, value);

      if (integrationEntry.CalculationRule == "CambiarElSigno") {
        returnValues = returnValues.ChangeSign();
      }

      return returnValues;
    }


    private IFinancialConceptValues ApplyOperator(OperatorType @operator,
                                                  IFinancialConceptValues totals,
                                                  IFinancialConceptValues value) {
      switch (@operator) {

        case OperatorType.Add:

          return totals.Sum(value);

        case OperatorType.Substract:

          return totals.Substract(value);

        case OperatorType.Multiply:

          return totals.Multiply(value);

        case OperatorType.AbsoluteValue:

          return totals.Sum(value)
                       .AbsoluteValue();

        default:
          throw Assertion.EnsureNoReachThisCode($"Unhandled operator '{@operator}'.");
      }
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

    private void CalculateFormulaBasedColumns(FinancialConcept financialConcept,
                                              IFinancialConceptValues totals) {

      var expressions = new FinancialConceptExpressions(_executionContext);

      FixedList<DataTableColumn> formulaBasedColumns =
            _executionContext.FinancialReportType.DataColumns.FindAll(x => x.IsCalculated);

      expressions.CalculateColumns(financialConcept, formulaBasedColumns, totals);
    }


    private IFinancialConceptValues CreateFinancialConceptValuesObject() {
      return new FinancialConceptValues(_executionContext.FinancialReportType.DataColumns);
    }


    private void ExecuteConceptScript(FinancialConcept financialConcept,
                                      IFinancialConceptValues totals) {
      var expressions = new FinancialConceptExpressions(_executionContext);

      expressions.ExecuteConceptScript(financialConcept, totals);
    }

    #endregion Helpers

  }  // class FinancialConceptsCalculator

}  // namespace Empiria.FinancialAccounting.FinancialReports
