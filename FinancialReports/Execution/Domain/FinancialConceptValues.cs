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
using System.Collections.Generic;

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

      return ApplyForEach(absoluteValue);
    }


    public void CopyTotalsTo(FinancialReportEntry copyTo) {
      foreach (var fieldName in DynamicFields.GetDynamicMemberNames()) {
        decimal value = DynamicFields.GetTotalField(fieldName);

        copyTo.SetTotalField(fieldName, value);
      }
    }


    public IFinancialConceptValues Round() {
      DecimalFunction round = (x) => Math.Round(x, 0);

      return ApplyForEach(round);
    }


    public IFinancialConceptValues Substract(IFinancialConceptValues values,
                                             string dataColumn) {
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


    public IFinancialConceptValues Substract(ITrialBalanceEntryDto balance, string dataColumn) {
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

      return ApplyRuleIfNeeded(substracted, dataColumn);
    }


    public IFinancialConceptValues Sum(IFinancialConceptValues values, string dataColumn) {
      var casted = (FinancialConceptValues) values;

      var sum = new FinancialConceptValues(_columns);

      foreach (var fieldName in DynamicFields.GetDynamicMemberNames()) {
        decimal newValue = DynamicFields.GetTotalField(fieldName) +
                           casted.DynamicFields.GetTotalField(fieldName);

        sum.DynamicFields.SetTotalField(fieldName, newValue);
      }

      return sum;
    }


    public IFinancialConceptValues Sum(ITrialBalanceEntryDto balance, string dataColumn) {
      var casted = (DynamicTrialBalanceEntry) balance;

      var sum = new FinancialConceptValues(_columns);

      foreach (var fieldName in DynamicFields.GetDynamicMemberNames()) {
        decimal newValue = DynamicFields.GetTotalField(fieldName) +
                           casted.GetTotalField(fieldName);

        sum.DynamicFields.SetTotalField(fieldName, newValue);
      }

      return ApplyRuleIfNeeded(sum, dataColumn);
    }


    public IFinancialConceptValues Sum(ExternalValue value, string dataColumn) {
      var sum = new FinancialConceptValues(_columns);

      foreach (var fieldName in DynamicFields.GetDynamicMemberNames()) {
        decimal newValue = DynamicFields.GetTotalField(fieldName) +
                           value.GetTotalField(fieldName);

        sum.DynamicFields.SetTotalField(fieldName, newValue);
      }

      return ApplyRuleIfNeeded(sum, dataColumn);
    }


    public IFinancialConceptValues SumDebitsOrSubstractCredits(ITrialBalanceEntryDto balance, string dataColumn) {
      var analiticoBalance = (DynamicTrialBalanceEntry) balance;

      if (analiticoBalance.DebtorCreditor == DebtorCreditorType.Deudora) {
        return Sum(balance, dataColumn);
      } else {
        return Substract(balance, dataColumn);
      }
    }


    #region Helpers

    private IFinancialConceptValues ApplyForEach(DecimalFunction function) {
      var temp = new FinancialConceptValues(_columns);

      foreach (var fieldName in DynamicFields.GetDynamicMemberNames()) {
        decimal value = DynamicFields.GetTotalField(fieldName);

        temp.DynamicFields.SetTotalField(fieldName, function(value));
      }

      return temp;
    }


    private IFinancialConceptValues ApplyRuleIfNeeded(FinancialConceptValues entry, string dataColumn) {
      if (dataColumn.Length == 0 || dataColumn.ToLower() == "default") {
        return entry;
      }

      return ConsolidateTotalsInto(entry, dataColumn);
    }


    private IFinancialConceptValues ConsolidateTotalsInto(FinancialConceptValues values, string consolidatedFieldName) {

      Assertion.Require(consolidatedFieldName, nameof(consolidatedFieldName));

      decimal consolidatedTotal = 0;

      foreach (var fieldName in values.DynamicFields.GetDynamicMemberNames()) {
        consolidatedTotal += values.DynamicFields.GetTotalField(fieldName);
      }

      var consolidated = new FinancialConceptValues(_columns);

      consolidated.DynamicFields.SetTotalField(consolidatedFieldName, consolidatedTotal);

      return consolidated;
    }

    #endregion Helpers

  }  // class FinancialConceptsValues

}  // namespace Empiria.FinancialAccounting.FinancialReports
