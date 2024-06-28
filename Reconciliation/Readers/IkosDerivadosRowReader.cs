/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reconciliation Services                    Component : Adapters Layer                          *
*  Assembly : FinancialAccounting.Reconciliation.dll     Pattern   : Data reader provider                    *
*  Type     : IkosDerivadosRowReader                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Lee el archivo de concilación de IKOS derivados.                                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Json;
using Empiria.Office;

namespace Empiria.FinancialAccounting.Reconciliation.Readers {

  /// <summary>Lee el archivo de concilación de IKOS derivados.</summary>
  sealed internal class IkosDerivadosRowReader : IReconciliationRowReader {

    private readonly Spreadsheet _spreadsheet;
    private readonly int _rowIndex;

    public IkosDerivadosRowReader(Spreadsheet spreadsheet, int rowIndex) {
      Assertion.Require(spreadsheet, nameof(spreadsheet));
      Assertion.Require(rowIndex > 0, "rowIndex must be greater than zero.");

      _spreadsheet = spreadsheet;
      _rowIndex = rowIndex;
    }


    public string GetAccountNumber() {
      string accountNumber = ReadStringValueFromColumn("F");

      accountNumber = EmpiriaString.TrimAll(accountNumber);

      if (accountNumber.Contains(" ")) {
        accountNumber = accountNumber.Split(' ')[0];
      }

      return AccountsChart.IFRS.FormatAccountNumber(accountNumber);
    }


    public decimal GetCredits() {
      return ReadDecimalValueFromColumn("H");
    }


    public string GetCurrencyCode() {
      string isoCode = ReadStringValueFromColumn("O");

      var currency = Currency.TryParseISOCode(isoCode.ToUpperInvariant());

      Assertion.Require(currency,
                        $"El registro número {_rowIndex} tiene una clave de moneda que no reconozco: '{isoCode}'.");

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
      string subledgerAccountNumber = ReadStringValueFromColumn("F");

      if (subledgerAccountNumber.Contains(" ")) {
        subledgerAccountNumber = subledgerAccountNumber.Split(' ')[1];
      }

      return subledgerAccountNumber.TrimStart('0');
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

  }  // class IkosDerivadosRowReader

} // namespace Empiria.FinancialAccounting.Reconciliation.Readers
