/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Domain Layer                            *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Service provider                        *
*  Type     : FinancialReportCalculator                  License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Performs data calculation over financial reports data.                                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.FinancialAccounting.FinancialReports.Providers;

namespace Empiria.FinancialAccounting.FinancialReports {

  /// <summary>Performs data calculation over financial reports data.</summary>
  internal class FinancialReportCalculator {

    internal FinancialReportCalculator() {
      // no-op
    }


    internal void CalculateColumns(FixedList<DataTableColumn> columns,
                                   IEnumerable<FinancialReportEntry> entries) {
      foreach (var entry in entries) {
        CalculateColumns(columns, entry);
      }
    }


    internal void CalculateColumns(FixedList<DataTableColumn> columns,
                                   FinancialReportEntry entry) {
      IDictionary<string, object> entryValues = ConvertReportEntryToDictionary(entry);

      foreach (var column in columns) {
        decimal result = CalculateColumnEntry(column.Formula, entryValues);

        entryValues[column.Field] = result;
        entry.SetTotalField(column.Field, result);
      }
    }


    #region Helpers

    private decimal CalculateColumnEntry(string formula, IDictionary<string, object> values) {
      var evaluator = new ExpressionEvaluationProvider();

      return evaluator.Evaluate<decimal>(formula, values);
    }


    private IDictionary<string, object> ConvertReportEntryToDictionary(FinancialReportEntry entry) {
      IDictionary<string, object> dictionary = entry.ToDictionary();

      var conceptCode = ((IFinancialReportResult) entry).FinancialConcept.Code;

      dictionary.Add("conceptCode", conceptCode);

      return dictionary;
    }


    #endregion Helpers

  }   // class FinancialReportCalculator

}  // namespace Empiria.FinancialAccounting.FinancialReports
