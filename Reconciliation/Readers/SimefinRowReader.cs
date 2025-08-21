/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reconciliation Services                    Component : Adapters Layer                          *
*  Assembly : FinancialAccounting.Reconciliation.dll     Pattern   : Data reader provider                    *
*  Type     : SimefinRowReader                           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Lee el archivo de conciliación del sistema SIMEFIN.                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.Json;
using Empiria.Office;

namespace Empiria.FinancialAccounting.Reconciliation.Readers {

  /// <summary>Lee el archivo de conciliación del sistema SIMEFIN.</summary>
  sealed internal class SimefinRowReader : IReconciliationRowReader {

    private readonly Spreadsheet _spreadsheet;
    private readonly int _rowIndex;

    public SimefinRowReader(Spreadsheet spreadsheet, int rowIndex) {
      Assertion.Require(spreadsheet, nameof(spreadsheet));
      Assertion.Require(rowIndex > 0, "rowIndex must be greater than zero.");

      _spreadsheet = spreadsheet;
      _rowIndex = rowIndex;
    }


    public string GetAccountNumber() {
      string accountNumber = ReadStringValueFromColumn("D");

      accountNumber = EmpiriaString.TrimAll(accountNumber);

      if (accountNumber.Contains(" ")) {
        accountNumber = accountNumber.Split(' ')[0];
      }

      return AccountsChart.IFRS.FormatAccountNumber(accountNumber);
    }


    public decimal GetCredits() {
      return ReadDecimalValueFromColumn("F");
    }


    public string GetCurrencyCode() {
      string isoCode = ReadStringValueFromColumn("G");

      var currency = Currency.TryParseISOCode(isoCode.ToUpperInvariant());

      Assertion.Require(currency,
                        $"El registro número {_rowIndex} tiene una clave de moneda que no reconozco: '{isoCode}'.");

      return currency.Code;
    }


    public decimal GetDebits() {
      return ReadDecimalValueFromColumn("E");
    }


    public decimal GetEndBalance() {
      return 0;
    }


    public JsonObject GetExtensionData() {
      var json = new JsonObject();

      json.Add("portafolio",  ReadStringValueFromColumn("J"));
      json.Add("idOperacion", ReadStringValueFromColumn("L"));
      json.Add("nombreOperacion", ReadStringValueFromColumn("M"));
      json.Add("sector",       ReadStringValueFromColumn("N"));
      json.Add("contraparte", ReadStringValueFromColumn("Q"));

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
      return string.Empty;
    }


    public string GetTransactionSlip() {
      return ReadStringValueFromColumn("A");
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

  }  // class SimefinRowReader

} // namespace Empiria.FinancialAccounting.Reconciliation.Readers
