/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart Edition                     Component : Domain Layer                            *
*  Assembly : FinancialAccounting.dll                    Pattern   : Service provider                        *
*  Type     : AccountsChartEditionRowReader              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Gets account edition commands from a spreadsheet row.                                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;

using Empiria.Office;

namespace Empiria.FinancialAccounting.AccountsChartEdition {

  /// <summary>Gets account edition commands from a spreadsheet row.</summary>
  sealed internal class AccountsChartEditionRowReader {

    private readonly Spreadsheet _spreadsheet;
    private readonly int _rowIndex;

    public AccountsChartEditionRowReader(Spreadsheet spreadsheet, int rowIndex) {
      Assertion.Require(spreadsheet, nameof(spreadsheet));
      Assertion.Require(rowIndex > 0, "rowIndex must be greater than zero.");

      _spreadsheet = spreadsheet;
      _rowIndex = rowIndex;
    }


    internal string GetAccountNumber() {
      return ReadStringValueFromColumn("B");
    }


    internal string GetAccountName() {
      return ReadStringValueFromColumn("C");
    }


    internal string GetAccountRole() {
      return ReadStringValueFromColumn("D");
    }


    internal FixedList<string> GetCurrencies() {
      string[] columns = new[] { "F", "G", "H", "I", "J" };

      return ReadStringValueFromColumns(columns);
    }


    internal bool GetIsSubsidiaryAccountFlag() {
      return ReadBoolValueFromColumn("E");
    }


    internal FixedList<string> GetOperations() {
      var value = ReadStringValueFromColumn("A");

      if (value.Length == 0) {
        return new FixedList<string>();
      }

      if (value == "Agregar cuenta") {
        return new FixedList<string>(new[] { "Create" });
      }

      if (value.StartsWith("Cambiar ")) {
        value = value.Replace("Cambiar ", string.Empty);

        var operations = new List<string> {
          "Update"
        };

        operations.AddRange(value.Split(',')
                                 .Select(x => EmpiriaString.TrimAll(x)));

        return operations.ToFixedList();
      }

      return new FixedList<string>(new[] { value });
    }


    internal string GetOperationText() {
      return ReadStringValueFromColumn("A");
    }


    internal FixedList<string> GetSectors() {
      string[] columns = new[] { "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U",
                                 "V", "W", "X", "Y", "Z", "AA", "AB", "AC", "AD" };

      return ReadStringValueFromColumns(columns);
    }

    #region Helpers

    private bool ReadBoolValueFromColumn(string column) {
      var value = _spreadsheet.ReadCellValue<string>($"{column}{_rowIndex}");

      if (value.Length != 0 && value != "-") {
        return false;
      }

      return EmpiriaString.ToBoolean(EmpiriaString.TrimAll(value));
    }


    private string ReadStringValueFromColumn(string column) {
      var value = _spreadsheet.ReadCellValue<string>($"{column}{_rowIndex}");

      return EmpiriaString.TrimAll(value);
    }

    private FixedList<string> ReadStringValueFromColumns(string[] columns) {
      var list = new List<string>();

      foreach (var column in columns) {
        var value = ReadStringValueFromColumn(column);

        if (value.Length != 0 && value != "-") {
          list.Add(value);
        }
      }

      return list.ToFixedList();
    }

    #endregion Helpers

  }  // class AccountsChartEditionRowReader

} // namespace Empiria.FinancialAccounting.AccountsChartEdition
