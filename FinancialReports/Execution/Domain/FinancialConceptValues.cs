/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Domain Layer                            *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Value type                              *
*  Type     : FinancialConceptValues                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Default value type that holds and performs operations over financial concepts fields.          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.ExternalData;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;

using Empiria.FinancialAccounting.FinancialReports.Providers;

namespace Empiria.FinancialAccounting.FinancialReports {

  delegate decimal DecimalFunction(decimal value);

  /// <summary>Default value type that holds and performs operations over financial concepts fields.</summary>
  internal class FinancialConceptValues : IFinancialConceptValues {

    private readonly FixedList<DataTableColumn> _columns;
    private readonly FixedList<string> _sumInsteadOfSubstractColumns;

    internal FinancialConceptValues(FixedList<DataTableColumn> columns) {
      _columns = columns;

      _sumInsteadOfSubstractColumns = columns.FindAll(x => x.Tags.Contains("sumInsteadOfSubstract"))
                                             .Select(x => x.Field)
                                             .ToFixedList();

      foreach (var column in columns.FindAll(x => x.Type == "decimal")) {
        DynamicFields.SetTotalField(column.Field, 0m);
      }
    }


    public DynamicFields DynamicFields {
      get; private set;
    } = new DynamicFields();


    public IFinancialConceptValues AbsoluteValue() {
      DecimalFunction absoluteValue = (x) => Math.Abs(x);

      return ApplyToAllFields(absoluteValue);
    }


    public IFinancialConceptValues ChangeSign() {
      DecimalFunction changeSign = (x) => -1m * x;

      return ApplyToAllFields(changeSign);
    }


    public IFinancialConceptValues ConsolidateTotalsInto(string consolidatedFieldName) {

      Assertion.Require(consolidatedFieldName, nameof(consolidatedFieldName));

      decimal consolidatedTotal = 0;

      foreach (var fieldName in this.DynamicFields.GetDynamicMemberNames()) {
        consolidatedTotal += this.DynamicFields.GetTotalField(fieldName);
      }

      var consolidated = new FinancialConceptValues(_columns);

      consolidated.DynamicFields.SetTotalField(consolidatedFieldName, consolidatedTotal);

      return consolidated;
    }


    public void CopyTotalsTo(DynamicFields copyTo) {
      foreach (var fieldName in DynamicFields.GetDynamicMemberNames()) {
        decimal value = DynamicFields.GetTotalField(fieldName);

        copyTo.SetTotalField(fieldName, value);
      }
    }


    public IFinancialConceptValues Multiply(IFinancialConceptValues values) {
      var casted = (FinancialConceptValues) values;

      var product = new FinancialConceptValues(_columns);

      foreach (var fieldName in DynamicFields.GetDynamicMemberNames()) {
        decimal newValue = DynamicFields.GetTotalField(fieldName) *
                           casted.DynamicFields.GetTotalField(fieldName);

        product.DynamicFields.SetTotalField(fieldName, newValue);
      }

      return product;
    }


    public IFinancialConceptValues Multiply(ITrialBalanceEntryDto balance) {
      var casted = (DynamicTrialBalanceEntry) balance;

      var product = new FinancialConceptValues(_columns);

      foreach (var fieldName in DynamicFields.GetDynamicMemberNames()) {
        decimal newValue = DynamicFields.GetTotalField(fieldName) *
                           casted.GetTotalField(fieldName);

        product.DynamicFields.SetTotalField(fieldName, newValue);
      }

      return product;
    }


    public IFinancialConceptValues Multiply(ExternalValue value) {
      var product = new FinancialConceptValues(_columns);

      foreach (var fieldName in DynamicFields.GetDynamicMemberNames()) {
        decimal newValue = DynamicFields.GetTotalField(fieldName) *
                           value.GetTotalField(fieldName);

        product.DynamicFields.SetTotalField(fieldName, newValue);
      }

      return product;
    }


    public IFinancialConceptValues Round(RoundTo roundTo) {
      DecimalFunction round;

      switch (roundTo) {

        case RoundTo.DoNotRound:
          return this;

        case RoundTo.Units:
          round = (x) => Math.Round(x, 0);
          break;

        case RoundTo.Thousands:
          round = (x) => Math.Round(x / 1000m, 0);
          break;

        case RoundTo.Millions:
          round = (x) => Math.Round(x / 1000000m, 0);
          break;

        default:
          throw Assertion.EnsureNoReachThisCode($"Unhandled roundTo operation '{roundTo}'");

      }

      return ApplyToAllFields(round);
    }


    public IFinancialConceptValues Substract(IFinancialConceptValues values) {
      var casted = (FinancialConceptValues) values;

      var substracted = new FinancialConceptValues(_columns);

      foreach (var fieldName in DynamicFields.GetDynamicMemberNames()) {

        decimal newValue;

        if (_sumInsteadOfSubstractColumns.Contains(fieldName)) {
          newValue = DynamicFields.GetTotalField(fieldName) +
                             casted.DynamicFields.GetTotalField(fieldName);
        } else {
          newValue = DynamicFields.GetTotalField(fieldName) -
                             casted.DynamicFields.GetTotalField(fieldName);
        }

        substracted.DynamicFields.SetTotalField(fieldName, newValue);
      }

      return substracted;
    }


    public IFinancialConceptValues Substract(ITrialBalanceEntryDto balance) {
      var casted = (DynamicTrialBalanceEntry) balance;

      var substracted = new FinancialConceptValues(_columns);

      foreach (var fieldName in DynamicFields.GetDynamicMemberNames()) {

        decimal newValue;

        if (_sumInsteadOfSubstractColumns.Contains(fieldName)) {
          newValue = DynamicFields.GetTotalField(fieldName) +
                             casted.GetTotalField(fieldName);
        } else {
          newValue = DynamicFields.GetTotalField(fieldName) -
                             casted.GetTotalField(fieldName);
        }

        substracted.DynamicFields.SetTotalField(fieldName, newValue);
      }

      return substracted;
    }


    public IFinancialConceptValues Sum(IFinancialConceptValues values) {
      var casted = (FinancialConceptValues) values;

      var sum = new FinancialConceptValues(_columns);

      foreach (var fieldName in DynamicFields.GetDynamicMemberNames()) {
        decimal newValue = DynamicFields.GetTotalField(fieldName) +
                           casted.DynamicFields.GetTotalField(fieldName);

        sum.DynamicFields.SetTotalField(fieldName, newValue);
      }

      return sum;
    }


    public IFinancialConceptValues Sum(ITrialBalanceEntryDto balance) {
      var casted = (DynamicTrialBalanceEntry) balance;

      var sum = new FinancialConceptValues(_columns);

      foreach (var fieldName in DynamicFields.GetDynamicMemberNames()) {
        decimal newValue = DynamicFields.GetTotalField(fieldName) +
                           casted.GetTotalField(fieldName);

        sum.DynamicFields.SetTotalField(fieldName, newValue);
      }

      return sum;
    }


    public IFinancialConceptValues Sum(ExternalValue value) {
      var sum = new FinancialConceptValues(_columns);

      foreach (var fieldName in DynamicFields.GetDynamicMemberNames()) {
        decimal newValue = DynamicFields.GetTotalField(fieldName) +
                           value.GetTotalField(fieldName);

        sum.DynamicFields.SetTotalField(fieldName, newValue);
      }

      return sum;
    }


    public IFinancialConceptValues SumDebitsOrSubstractCredits(ITrialBalanceEntryDto balance) {
      var analiticoBalance = (DynamicTrialBalanceEntry) balance;

      if (analiticoBalance.DebtorCreditor == DebtorCreditorType.Deudora) {
        return Sum(balance);
      } else {
        return Substract(balance);
      }
    }


    #region Helpers

    private IFinancialConceptValues ApplyToAllFields(DecimalFunction function) {
      var temp = new FinancialConceptValues(_columns);

      foreach (var fieldName in DynamicFields.GetDynamicMemberNames()) {
        decimal value = DynamicFields.GetTotalField(fieldName);

        temp.DynamicFields.SetTotalField(fieldName, function(value));
      }

      return temp;
    }


    #endregion Helpers

  }  // class FinancialConceptsValues

}  // namespace Empiria.FinancialAccounting.FinancialReports
