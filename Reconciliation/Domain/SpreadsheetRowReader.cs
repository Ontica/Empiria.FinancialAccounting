/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reconciliation Services                    Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Reconciliation.dll     Pattern   : Service provider                        *
*  Type     : SpreadsheetRowReader                       License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Gets reconciliation data from a spreadsheet row.                                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Json;
using Empiria.Office;

namespace Empiria.FinancialAccounting.Reconciliation {

  /// <summary>Gets reconciliation data from a spreadsheet row.</summary>
  sealed internal class SpreadsheetRowReader {

    private readonly Spreadsheet _spreadsheet;
    private readonly int _rowIndex;

    public SpreadsheetRowReader(Spreadsheet spreadsheet, int rowIndex) {
      Assertion.Require(spreadsheet, nameof(spreadsheet));
      Assertion.Require(rowIndex > 0, "rowIndex must be greater than zero.");

      _spreadsheet = spreadsheet;
      _rowIndex = rowIndex;
    }


    public string GetAccountNumber() {
      string value = ReadStringValueFromColumn("F");

      value = EmpiriaString.TrimAll(value);

      if (value.Contains(" ")) {
        value = value.Split(' ')[0];
      }

      return AccountsChart.IFRS.FormatAccountNumber(value);
    }


    public decimal GetCredits() {
      return ReadDecimalValueFromColumn("H");
    }


    public string GetCurrencyCode() {
      string value = ReadStringValueFromColumn("O");

      var currency = Currency.TryParseByCurrencyCode(value.ToUpperInvariant());

      Assertion.Require(currency,
                        $"El registro número {_rowIndex} tiene un valor de moneda que no reconozco: '{value}'.");

      return currency.Code;
    }


    public decimal GetDebits() {
      return ReadDecimalValueFromColumn("G");
    }


    public decimal GetEndBalance() {
      return 0;
    }


    public JsonObject GetExtensionData() {
      var json = new JsonObject();

      json.Add("parte",       ReadStringValueFromColumn("A"));
      json.Add("mercado",     ReadStringValueFromColumn("B"));
      json.Add("claveoper",   ReadStringValueFromColumn("C"));
      json.Add("fecha",       ReadStringValueFromColumn("D"));
      json.Add("consecutivo", ReadStringValueFromColumn("E"));
      json.Add("submov",      ReadStringValueFromColumn("J"));
      json.Add("emisor",      ReadStringValueFromColumn("L"));
      json.Add("numcontrato", ReadStringValueFromColumn("M"));

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
      string value = ReadStringValueFromColumn("F");

      if (value.Contains(" ")) {
        value = value.Split(' ')[1];
      }

      return value.TrimStart('0');
    }


    public string GetTransactionSlip() {
      return ReadStringValueFromColumn("D") + "-" + ReadStringValueFromColumn("E");
    }


    public string GetUniqueKey() {
      string value = ReadStringValueFromColumn("C") + "-" +
                     ReadStringValueFromColumn("D") + "-" +
                     ReadStringValueFromColumn("E") + "-" +
                     ReadStringValueFromColumn("I") + "-" +
                     ReadStringValueFromColumn("J") + "-" +
                     ReadStringValueFromColumn("M") + "-" +
                     ReadStringValueFromColumn("N");

      return value;
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

} // namespace Empiria.FinancialAccounting.Reconciliation
