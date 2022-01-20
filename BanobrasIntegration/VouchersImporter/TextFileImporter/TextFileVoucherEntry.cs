/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                 Component : Vouchers Importer                    *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll   Pattern   : Information Holder                   *
*  Type     : TextFileVoucherEntry                          License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Data structure that holds information in a text file line with imported voucher entry data.    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.FinancialAccounting.Vouchers;

namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter {

  /// <summary>Data structure that holds information in a text file line
  /// with imported voucher entry data.</summary>
  public class TextFileVoucherEntry {

    static internal readonly int STANDARD_TEXT_LINE_LENGTH = 441;

    static private readonly int STANDARD_ACCOUNT_NUMBER_LENGTH = 16;
    static private readonly int WIDE_ACCOUNT_NUMBER_LENGTH = 50;

    static internal readonly int WIDE_TEXT_LINE_LENGTH =
            STANDARD_TEXT_LINE_LENGTH + (WIDE_ACCOUNT_NUMBER_LENGTH - STANDARD_ACCOUNT_NUMBER_LENGTH);

    private readonly List<ToImportVoucherIssue> _entryIssues = new List<ToImportVoucherIssue>();


    internal TextFileVoucherEntry(AccountsChart accountsChart, string textLine, int textLineIndex) {
      Assertion.AssertObject(accountsChart, "accountsChart");
      Assertion.AssertObject(textLine, "textLine");

      AccountsChart = accountsChart;
      TextLine = textLine;
      TextLineIndex = textLineIndex;

      CheckTextLineLength(textLine);
      LoadTextLine(textLine);
    }


    private AccountsChart AccountsChart {
      get;
    }


    public string TextLine {
      get;
    }


    public int TextLineIndex {
      get;
    }


    private bool TextLineIsInWideFormat {
      get {
        return this.TextLine.Length == WIDE_TEXT_LINE_LENGTH;
      }
    }


    public string VoucherUniqueID {
      get {
        return $"{this.LedgerNumber}||{this.TransactionNumber}||{this.AccountingDate.ToString("yyyyMMdd")}";
      }
    }


    internal string GetImportationSet() {
      Ledger ledger = this.GetLedger();

      return ledger.FullName;
    }


    private string LedgerNumber {
      get; set;
    }


    private string TransactionNumber {
      get; set;
    }


    private string TransactionConcept {
      get; set;
    }


    private string Source {
      get; set;
    }


    private DateTime AccountingDate {
      get; set;
    }


    private decimal Amount {
      get; set;
    }


    private string CurrencyCode {
      get; set;
    }


    private string SectorCode {
      get; set;
    }


    private decimal ExchangeRate {
      get; set;
    }


    private string SubledgerAccountNumber {
      get; set;
    }


    private string ResponsibilityArea {
      get; set;
    }


    private string AccountNumber {
      get; set;
    }

    #region Private methods


    private void CheckTextLineLength(string textLine) {
      Assertion.AssertObject(textLine, "textLine");

      Assertion.Assert(textLine.Length == STANDARD_TEXT_LINE_LENGTH ||
                       textLine.Length == WIDE_TEXT_LINE_LENGTH,
                       $"La línea de texto {this.TextLineIndex} tiene una longitud " +
                       $"({textLine.Length}) que no reconozco.");
    }


    private void LoadTextLine(string textLine) {
      int lineLengthExcess;

      if (textLine.Length == STANDARD_TEXT_LINE_LENGTH) {
        lineLengthExcess = 0;
      } else if (textLine.Length == WIDE_TEXT_LINE_LENGTH) {
        lineLengthExcess = WIDE_TEXT_LINE_LENGTH - STANDARD_TEXT_LINE_LENGTH;
      } else {
        throw Assertion.AssertNoReachThisCode($"Invalid text line length of {textLine.Length}");
      }

      this.Source = textLine.Substring(0, 6);

      this.LedgerNumber = textLine.Substring(6, 4);
      if (this.LedgerNumber.StartsWith("00")) {
        this.LedgerNumber = this.LedgerNumber.Substring(2, 2);
      }

      this.TransactionNumber = textLine.Substring(10, 12);

      this.AccountingDate = new DateTime(int.Parse(textLine.Substring(45, 4)),
                                         int.Parse(textLine.Substring(43, 2)),
                                         int.Parse(textLine.Substring(41, 2)));

      this.TransactionConcept = textLine.Substring(49, 240);

      this.AccountNumber = textLine.Substring(289, 16 + lineLengthExcess);

      this.AccountNumber = this.AccountsChart.FormatAccountNumber(this.AccountNumber);

      this.SectorCode = textLine.Substring(305 + lineLengthExcess, 2);

      this.Amount = decimal.Parse(textLine.Substring(316 + lineLengthExcess, 13) + "." +
                                  textLine.Substring(329 + lineLengthExcess, 6));

      this.Amount = Math.Round(this.Amount, 2);

      this.CurrencyCode = textLine.Substring(335 + lineLengthExcess, 2);

      this.ExchangeRate = decimal.Parse(textLine.Substring(337 + lineLengthExcess, 7) + "." +
                                        textLine.Substring(344 + lineLengthExcess, 8));

      this.ExchangeRate = Math.Round(this.ExchangeRate, 6);

      this.SubledgerAccountNumber = textLine.Substring(371 + lineLengthExcess, 20);

      // this.AvailabilityCode = textLine.Substring(403 + lineLengthExcess, 4);

      this.ResponsibilityArea = textLine.Substring(419 + lineLengthExcess, 6);

      // this.AccountsChartId = textLine.Substring(425 + lineLengthExcess, 4);
    }


    internal Sector GetSector() {
      try {
        return Sector.Parse(this.SectorCode);
      } catch {
        AddError($"El sector '{this.SectorCode}' no existe en el catálogo de sectores.");

        return Sector.Empty;
      }
    }


    internal SubledgerAccount GetSubledgerAccount() {
      Ledger ledger = this.GetLedger();

      var formattedAccountNo = GetSubledgerAccountNo();

      SubledgerAccount subledgerAccount = ledger.TryGetSubledgerAccount(formattedAccountNo);

      if (subledgerAccount == null) {
        return SubledgerAccount.Empty;
      }

      if (subledgerAccount.Suspended) {
        AddError($"El auxiliar '{formattedAccountNo}' ({subledgerAccount.Name}) está suspendido, " +
                 $"por lo que no permite operaciones de registro.");
      }

      return subledgerAccount;
    }


    internal string GetSubledgerAccountNo() {
      Ledger ledger = this.GetLedger();

      return ledger.FormatSubledgerAccount(this.SubledgerAccountNumber);
    }


    internal FunctionalArea GetResponsibilityArea() {
      try {
        return FunctionalArea.Parse(this.ResponsibilityArea);
      } catch {
        AddError($"El área de responsabilidad '{this.ResponsibilityArea}' no existe en el catálogo de áreas.");

        return FunctionalArea.Empty;
      }
    }


    internal EventType GetEventType() {
      return EventType.Empty;

      //try {
      //  return EventType.Parse(int.Parse(this.AvailabilityCode));
      //} catch {
      //  AddError($"El evento '{this.AvailabilityCode}' no existe en el catálogo de eventos.");

      //  return EventType.Empty;
      //}
    }

    internal Currency GetCurrency() {
      try {
        return Currency.Parse(this.CurrencyCode);
      } catch {
        AddError($"La moneda '{this.CurrencyCode}' no existe en el catálogo de monedas.");

        return Currency.Empty;
      }
    }


    internal decimal GetAmount() {
      return Math.Abs(this.Amount);
    }


    internal decimal GetBaseCurrencyAmount() {
      return Math.Abs(this.Amount) * this.ExchangeRate;
    }


    internal FixedList<ToImportVoucherIssue> GetEntryIssues() {
      return _entryIssues.ToFixedList();
    }


    internal decimal GetExchangeRate() {
      return this.ExchangeRate;
    }


    internal VoucherEntryType GetVoucherEntryType() {
      return this.Amount >= 0 ? VoucherEntryType.Debit : VoucherEntryType.Credit;
    }


    internal StandardAccount GetStandardAccount() {
      try {
        StandardAccount stdAccount = this.AccountsChart.TryGetStandardAccount(this.AccountNumber);

        if (stdAccount != null) {
          return stdAccount;
        } else {
          AddError($"La cuenta '{this.AccountNumber}' no existe en el catálogo de cuentas.");

          return StandardAccount.Empty;
        }

      } catch (Exception e) {
        AddError($"Ocurrió un problema al leer el catálogo de cuentas. " + e.Message);

        return StandardAccount.Empty;
      }
    }

    private void AddError(string msg) {
      _entryIssues.Add(new ToImportVoucherIssue(VoucherIssueType.Error,
                                                this.GetImportationSet(),
                                                $"Línea {this.TextLineIndex}",
                                                msg));
    }


    private void AddWarning(string msg) {
      _entryIssues.Add(new ToImportVoucherIssue(VoucherIssueType.Warning,
                                                this.GetImportationSet(),
                                                $"Línea {this.TextLineIndex}",
                                                msg));
    }


    internal FixedList<ToImportVoucherIssue> GetHeaderIssues() {
      return new FixedList<ToImportVoucherIssue>();
    }


    internal FunctionalArea GetFunctionalArea() {
      try {
        return FunctionalArea.Parse(this.Source);

      } catch {
        AddError($"El área funcional '{this.Source}' no existe en el catálogo de áreas.");

        return FunctionalArea.Empty;
      }
    }


    internal DateTime GetAccountingDate() {
      return this.AccountingDate;
    }


    internal string GetConcept() {
      string temp = this.TransactionConcept.Replace("-", " ");

      return EmpiriaString.TrimAll(temp);
    }


    internal Ledger GetLedger() {
      try {
        Ledger ledger = this.AccountsChart.TryGetLedger(this.LedgerNumber);

        if (ledger == null) {
          AddError($"No existe una contabilidad con número '{this.LedgerNumber}' " +
                   $"para el catálogo de cuentas {this.AccountsChart.Name}.");
        }

        return ledger ?? Ledger.Empty;

      } catch {
        AddError($"No existe una contabilidad con número '{this.LedgerNumber}'" +
                 $"para el catálogo de cuentas {this.AccountsChart.Name}.");

        return Ledger.Empty;
      }
    }


    #endregion Private methods

  }  // class TextFileVoucherEntry

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter
