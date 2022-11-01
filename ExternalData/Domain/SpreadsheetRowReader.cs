/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : External Data                              Component : Domain Layer                            *
*  Assembly : FinancialAccounting.ExternalData.dll       Pattern   : Service provider                        *
*  Type     : SpreadsheetRowReader                       License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Gets financial external values data from a spreadsheet row.                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Office;

namespace Empiria.FinancialAccounting.ExternalData {

  /// <summary>Gets financial external values data from a spreadsheet row.</summary>
  sealed internal class SpreadsheetRowReader {

    private readonly Spreadsheet _spreadsheet;
    private readonly int _rowIndex;

    public SpreadsheetRowReader(Spreadsheet spreadsheet, int rowIndex) {
      Assertion.Require(spreadsheet, nameof(spreadsheet));
      Assertion.Require(rowIndex > 0, "rowIndex must be greater than zero.");

      _spreadsheet = spreadsheet;
      _rowIndex = rowIndex;
    }


    internal DynamicFields GetDynamicFields(FixedList<DataTableColumn> columns) {
      var fields = new DynamicFields();

      foreach (var column in columns) {
        decimal value = ReadDecimalValueFromColumn(column.Column);

        if (value != 0) {
          fields.SetTotalField(column.Field, value);
        }
      }

      return fields;
    }


    internal string GetVariableCode() {
      return ReadStringValueFromColumn("A");
    }

    #region Helpers

    private decimal ReadDecimalValueFromColumn(string column) {
      return _spreadsheet.ReadCellValue<decimal>($"{column}{_rowIndex}");
    }

    private string ReadStringValueFromColumn(string column) {
      var value = _spreadsheet.ReadCellValue<string>($"{column}{_rowIndex}");

      return EmpiriaString.TrimAll(value);
    }

    #endregion Helpers

  }  // class SpreadsheetRowReader

} // namespace Empiria.FinancialAccounting.ExternalData
