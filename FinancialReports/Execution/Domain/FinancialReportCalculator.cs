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

using Empiria.Expressions;

namespace Empiria.FinancialAccounting.FinancialReports {

  /// <summary>Performs data calculation over financial reports data.</summary>
  internal class FinancialReportCalculator {

    public FinancialReportCalculator() {
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
      foreach (var column in columns) {
        CalculateEntryColumn(column, entry);
      }
    }


    #region Helpers

    private void CalculateEntryColumn(DataTableColumn column, FinancialReportEntry entry) {
      var expression = new Expression(column.Formula);

      IDictionary<string, object> dictionary = ConvertReportEntryToDictionary(entry);

      decimal result = expression.Evaluate<decimal>(dictionary);

      entry.SetTotalField(column.Field, result);
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
