/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Domain Layer                            *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Service provider                        *
*  Type     : FinancialConceptExpressions                License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Performs financial concepts runtime expressions calculation and scripts execution.             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.FinancialAccounting.FinancialConcepts;
using Empiria.FinancialAccounting.FinancialReports.Providers;

namespace Empiria.FinancialAccounting.FinancialReports {

  /// <summary>Performs financial concepts runtime expressions calculation and scripts execution.</summary>
  internal class FinancialConceptExpressions {

    private readonly ExecutionContext _executionContext;

    internal FinancialConceptExpressions(ExecutionContext executionContext) {
      _executionContext = executionContext;
    }


    internal void CalculateColumns(FinancialConcept financialConcept,
                                   FixedList<DataTableColumn> columns,
                                   IFinancialConceptValues baseValues) {
      IDictionary<string, object> data = ConvertToDictionary(financialConcept, baseValues);

      if (financialConcept.Code.StartsWith("3300") && !financialConcept.Code.EndsWith("0008")) {
        columns = columns.FindAll(x => !x.Tags.Contains("skipFormulaForConceptsStartingWith3"));
      }

      foreach (var column in columns) {
        decimal result = CalculateColumnEntry(column.Formula, data);

        if (!data.ContainsKey(column.Field)) {
          data.Add(column.Field, result);
        } else {
          data[column.Field] = result;
        }

        baseValues.SetTotalField(column.Field, result);
      }
    }


    internal IFinancialConceptValues ExecuteConceptScript(FinancialConcept financialConcept,
                                                          IFinancialConceptValues baseValues) {

      if (!financialConcept.HasScript) {
        return baseValues;
      }

      IDictionary<string, object> data = ConvertToDictionary(financialConcept, baseValues);

      var compiler = new RuntimeCompiler(_executionContext);

      try {
        compiler.ExecuteScript(financialConcept.CalculationScript, data);

      } catch (Exception ex) {
        throw new InvalidOperationException($"No se pudo ejecutar el script del concepto " +
                    $"{financialConcept.Code} - {financialConcept.Name} ({financialConcept.Id}): " +
                    $"{financialConcept.CalculationScript}", ex);
      }

      if (MustReturnThisValue(financialConcept)) {
        return (IFinancialConceptValues) data["this"];
      }

      foreach (var item in data) {
        if (item.Value is decimal || item.Value is int) {
          baseValues.SetTotalField(item.Key, (decimal) item.Value);
        }
      }

      return baseValues;
    }

    #region Helpers

    private decimal CalculateColumnEntry(string formula, IDictionary<string, object> inputValues) {
      var compiler = new RuntimeCompiler(_executionContext);

      return compiler.EvaluateExpression<decimal>(formula, inputValues);
    }


    private IDictionary<string, object> ConvertToDictionary(FinancialConcept financialConcept,
                                                            IFinancialConceptValues baseValues) {
      IDictionary<string, object> dictionary = baseValues.ToDictionary();

      var conceptCode = financialConcept.Code;

      dictionary.Add("concepto", financialConcept);

      dictionary.Add("conceptCode", conceptCode);

      dictionary.Add("this", baseValues);

      return dictionary;
    }


    private bool MustReturnThisValue(FinancialConcept financialConcept) {
      var script = EmpiriaString.TrimAll(financialConcept.CalculationScript);

      return script.StartsWith("this := ");
    }

    #endregion Helpers

  }   // class FinancialConceptExpressions

}  // namespace Empiria.FinancialAccounting.FinancialReports
