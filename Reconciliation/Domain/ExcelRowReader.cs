/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reconciliation Services                    Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Reconciliation.dll     Pattern   : Service provider                        *
*  Type     : ExcelRowReader                             License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Gets reconciliation data from a spreadsheet row.                                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Json;
using Empiria.Office;

namespace Empiria.FinancialAccounting.Reconciliation {

  /// <summary>Gets reconciliation data from a spreadsheet row.</summary>
  sealed internal class ExcelRowReader {

    private readonly Spreadsheet _spreadsheet;
    private readonly int _row;

    public ExcelRowReader(Spreadsheet spreadsheet, int row) {
      _spreadsheet = spreadsheet;
      _row = row;
    }


    public string GetAccountNumber() {
      string value = GetStringValue("F");

      value = EmpiriaString.TrimAll(value);

      if (value.Contains(" ")) {
        value = value.Split(' ')[0];
      }

      return AccountsChart.IFRS.FormatAccountNumber(value);
    }


    public decimal GetCredits() {
      return GetDecimalValue("H");
    }


    public string GetCurrencyCode() {
      string value = GetStringValue("O");

      switch (value.ToUpperInvariant()) {
        case "MXN":
          return Currency.MXN.Code;
        case "USD":
          return Currency.USD.Code;
        case "JPY":
          return Currency.YEN.Code;
        case "UDI":
          return Currency.UDI.Code;
        case "EUR":
          return Currency.EUR.Code;
        default:
          throw Assertion.AssertNoReachThisCode(
            $"El registro número {_row} tiene un valor de moneda que no reconozco: '{value}'."
          );
      }
    }


    public decimal GetDebits() {
      return GetDecimalValue("G");
    }


    public decimal GetEndBalance() {
      return 0;
    }


    public JsonObject GetExtensionData() {
      var json = new JsonObject();

      json.Add("parte", GetStringValue("A"));
      json.Add("mercado", GetStringValue("B"));
      json.Add("claveoper", GetStringValue("C"));
      json.Add("fecha", GetStringValue("D"));
      json.Add("consecutivo", GetStringValue("E"));
      json.Add("submov", GetStringValue("J"));
      json.Add("emisor", GetStringValue("L"));
      json.Add("numcontrato", GetStringValue("M"));

      return json;
    }


    public decimal GetInitialBalance() {
      return 0;
    }


    public string GetLedger() {
      return "09";
    }


    public string GetSectorCode() {
      return Sector.Empty.Code;
    }


    public string GetSubledgerAccountNumber() {
      string value = GetStringValue("F");

      if (value.Contains(" ")) {
        value = value.Split(' ')[1];
      }

      return value.TrimStart('0');
    }


    public string GetTransactionSlip() {
      return GetStringValue("D") + "-" + GetStringValue("E");
    }


    public string GetUniqueKey() {
      string value = GetStringValue("C") + "-" +
                     GetStringValue("D") + "-" +
                     GetStringValue("E") + "-" +
                     GetStringValue("I") + "-" +
                     GetStringValue("J") + "-" +
                     GetStringValue("M") + "-" +
                     GetStringValue("N");

      return value;
    }

    #region Helpers

    private decimal GetDecimalValue(string column) {
      return _spreadsheet.ReadCellValue<decimal>($"{column}{_row}");
    }

    private string GetStringValue(string column) {
      var value = _spreadsheet.ReadCellValue<string>($"{column}{_row}");

      return EmpiriaString.TrimAll(value);
    }

    #endregion Helpers

  }  // class ExcelRowReader

} // namespace Empiria.FinancialAccounting.Reconciliation
