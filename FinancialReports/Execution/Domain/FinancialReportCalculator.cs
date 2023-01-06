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

using Empiria.FinancialAccounting.FinancialConcepts;
using Empiria.FinancialAccounting.FinancialReports.Providers;

namespace Empiria.FinancialAccounting.FinancialReports {

  /// <summary>Performs data calculation over financial reports data.</summary>
  internal class FinancialReportCalculator {

    private readonly ExecutionContext _executionContext;

    internal FinancialReportCalculator(ExecutionContext executionContext) {
      _executionContext = executionContext;
    }


    internal void CalculateColumns(FinancialConcept financialConcept,
                                   FixedList<DataTableColumn> columns,
                                   IFinancialConceptValues baseValues) {
      IDictionary<string, object> entryValues = ConvertToDictionary(financialConcept, baseValues);

      foreach (var column in columns) {
        decimal result = CalculateColumnEntry(column.Formula, entryValues);

        if (!entryValues.ContainsKey(column.Field)) {
          entryValues.Add(column.Field, result);
        } else {
          entryValues[column.Field] = result;
        }

        baseValues.SetTotalField(column.Field, result);
      }
    }

    #region Helpers

    private decimal CalculateColumnEntry(string formula, IDictionary<string, object> values) {
      var compiler = new RuntimeCompiler(_executionContext);

      return compiler.EvaluateExpression<decimal>(formula, values);
    }


    private IDictionary<string, object> ConvertToDictionary(FinancialConcept financialConcept,
                                                            IFinancialConceptValues baseValues) {
      IDictionary<string, object> dictionary = baseValues.ToDictionary();

      var conceptCode = financialConcept.Code;

      dictionary.Add("conceptCode", conceptCode);

      return dictionary;
    }

    #endregion Helpers

  }   // class FinancialReportCalculator

}  // namespace Empiria.FinancialAccounting.FinancialReports
