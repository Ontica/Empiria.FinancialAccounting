/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Domain Layer                            *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Service provider                        *
*  Type     : DynamicReportEntryTotals                   License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Generates a dynamic fields report entry to be used as a FinancialReportEntry.                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.FinancialAccounting.ExternalData;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.FinancialReports {

  delegate decimal DecimalFunction(decimal value);

  /// <summary>Generates a dynamic fields report entry to be used as a FinancialReportEntry.</summary>
  internal class DynamicReportEntryTotals : ReportEntryTotals {

    private DynamicReportEntryTotals() {
      // no-op
    }


    internal DynamicReportEntryTotals(FixedList<DataTableColumn> columns) {
      foreach (var column in columns.FindAll(x => x.Type == "decimal")) {
        DynamicFields.SetTotalField(column.Field, 0m);
      }
    }


    public DynamicReportEntryTotals(IEnumerable<string> fieldNames) {
      foreach (var fieldName in fieldNames) {
        DynamicFields.SetTotalField(fieldName, 0m);
      }
    }


    public DynamicFields DynamicFields {
      get; private set;
    } = new DynamicFields();


    public override ReportEntryTotals AbsoluteValue() {
      DecimalFunction absoluteValue = (x) => Math.Abs(x);

      return ApplyForEach(absoluteValue);
    }


    public override void CopyTotalsTo(FinancialReportEntry copyTo) {
      foreach (var fieldName in DynamicFields.GetDynamicMemberNames()) {
        decimal value = DynamicFields.GetTotalField(fieldName);

        copyTo.SetTotalField(fieldName, value);
      }
    }


    public override ReportEntryTotals Round() {
      DecimalFunction round = (x) => Math.Round(x, 0);

      return ApplyForEach(round);
    }


    public override ReportEntryTotals Substract(ReportEntryTotals total, string dataColumn) {
      var casted = (DynamicReportEntryTotals) total;

      var substracted = new DynamicReportEntryTotals();

      foreach (var fieldName in DynamicFields.GetDynamicMemberNames()) {
        decimal newValue = DynamicFields.GetTotalField(fieldName) -
                           casted.DynamicFields.GetTotalField(fieldName);

        substracted.DynamicFields.SetTotalField(fieldName, newValue);
      }

      return substracted;
    }


    public override ReportEntryTotals Substract(ITrialBalanceEntryDto balance, string dataColumn) {
      var casted = (DynamicTrialBalanceEntryDto) balance;

      var substracted = new DynamicReportEntryTotals();

      foreach (var fieldName in DynamicFields.GetDynamicMemberNames()) {
        decimal newValue = DynamicFields.GetTotalField(fieldName) -
                           casted.GetTotalField(fieldName);

        substracted.DynamicFields.SetTotalField(fieldName, newValue);
      }

      return substracted;
    }


    public override ReportEntryTotals Sum(ReportEntryTotals total, string dataColumn) {
      var casted = (DynamicReportEntryTotals) total;

      var sum = new DynamicReportEntryTotals();

      foreach (var fieldName in DynamicFields.GetDynamicMemberNames()) {
        decimal newValue = DynamicFields.GetTotalField(fieldName) +
                           casted.DynamicFields.GetTotalField(fieldName);

        sum.DynamicFields.SetTotalField(fieldName, newValue);
      }

      return sum;
    }


    public override ReportEntryTotals Sum(ITrialBalanceEntryDto balance, string dataColumn) {
      var casted = (DynamicTrialBalanceEntryDto) balance;

      var sum = new DynamicReportEntryTotals();

      foreach (var fieldName in DynamicFields.GetDynamicMemberNames()) {
        decimal newValue = DynamicFields.GetTotalField(fieldName) +
                           casted.GetTotalField(fieldName);

        sum.DynamicFields.SetTotalField(fieldName, newValue);
      }

      return sum;
    }


    public override ReportEntryTotals Sum(ExternalValue value, string dataColumn) {
      var sum = new DynamicReportEntryTotals();

      foreach (var fieldName in DynamicFields.GetDynamicMemberNames()) {
        decimal newValue = DynamicFields.GetTotalField(fieldName) +
                           value.GetTotalField(fieldName);

        sum.DynamicFields.SetTotalField(fieldName, newValue);
      }

      return sum;
    }


    public override ReportEntryTotals SumDebitsOrSubstractCredits(ITrialBalanceEntryDto balance, string dataColumn) {
      var analiticoBalance = (DynamicTrialBalanceEntryDto) balance;

      if (analiticoBalance.DebtorCreditor == DebtorCreditorType.Deudora) {
        return Sum(balance, dataColumn);
      } else {
        return Substract(balance, dataColumn);
      }
    }


    private ReportEntryTotals ApplyForEach(DecimalFunction function) {
      var temp = new DynamicReportEntryTotals();

      foreach (var fieldName in DynamicFields.GetDynamicMemberNames()) {
        decimal value = DynamicFields.GetTotalField(fieldName);

        temp.DynamicFields.SetTotalField(fieldName, function(value));
      }

      return temp;
    }


  }  // class DynamicReportEntryTotals

}  // namespace Empiria.FinancialAccounting.FinancialReports
