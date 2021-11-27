/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                 Component : Vouchers Importer                    *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll   Pattern   : Structurer                           *
*  Type     : TextFileStructurer                            License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Provides information structure services for vouchers contained in text Files.                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;

using Empiria.FinancialAccounting.Vouchers;
using Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter.Adapters;

namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter {

  /// <summary>Provides information structure services for vouchers contained in text Files.</summary>
  internal class TextFileStructurer {

    private readonly ImportVouchersCommand _command;
    private readonly string[] _textFileLines;

    private readonly FixedList<TextFileVoucherEntry> _entries;

    public TextFileStructurer(ImportVouchersCommand command, string[] textFileLines) {
      Assertion.AssertObject(command, "command");
      Assertion.AssertObject(textFileLines, "textFileLines");

      _command = command;
      _textFileLines = textFileLines;

      _entries = ParseTextLinesToTextFileVoucherEntries();
    }


    public FixedList<TextFileVoucherEntry> Entries {
      get {
        return _entries;
      }
    }

    #region Methods

    internal FixedList<ToImportVoucher> GetToImportVouchersList() {
      string[] voucherUniqueIDsArray = GetVoucherUniqueIds();

      var vouchersListToImport = new List<ToImportVoucher>(voucherUniqueIDsArray.Length);

      foreach (string voucherUniqueID in voucherUniqueIDsArray) {
        ToImportVoucher voucherToImport = BuildVoucherToImport(voucherUniqueID);

        vouchersListToImport.Add(voucherToImport);
      }

      return vouchersListToImport.ToFixedList();
    }


    private ToImportVoucher BuildVoucherToImport(string voucherUniqueID) {
      ToImportVoucherHeader header = MapToImportVoucherHeader(voucherUniqueID);
      FixedList<ToImportVoucherEntry> entries = MapToImportVoucherEntries(header);

      return new ToImportVoucher(header, entries);
    }


    private ToImportVoucherHeader MapToImportVoucherHeader(string voucherUniqueID) {
      var sourceHeader = _entries.Find(x => x.VoucherUniqueID == voucherUniqueID);

      var header = new ToImportVoucherHeader();

      header.ImportationSet = sourceHeader.GetImportationSet();
      header.UniqueID = sourceHeader.VoucherUniqueID;
      header.Ledger = sourceHeader.GetLedger();
      header.Concept = sourceHeader.GetConcept();
      header.AccountingDate = sourceHeader.GetAccountingDate();
      header.VoucherType = VoucherType.Parse(_command.VoucherTypeUID);
      header.TransactionType = TransactionType.Parse(_command.TransactionTypeUID);
      header.FunctionalArea = sourceHeader.GetFunctionalArea();
      header.RecordingDate = DateTime.Today;
      header.ElaboratedBy = Participant.Current;

      header.Issues = sourceHeader.GetHeaderIssues();

      return header;
    }


    private FixedList<ToImportVoucherEntry> MapToImportVoucherEntries(ToImportVoucherHeader header) {
      var entries = _entries.FindAll(x => x.VoucherUniqueID == header.UniqueID);

      var mapped = entries.Select(entry => MapToImportVoucherEntry(header, entry));

      return new List<ToImportVoucherEntry>(mapped).ToFixedList();
    }


    private ToImportVoucherEntry MapToImportVoucherEntry(ToImportVoucherHeader header,
                                                         TextFileVoucherEntry sourceEntry) {
      var entry = new ToImportVoucherEntry(header);

      entry.StandardAccount = sourceEntry.GetStandardAccount();
      entry.Sector = sourceEntry.GetSector();
      entry.SubledgerAccount = sourceEntry.GetSubledgerAccount();
      entry.SubledgerAccountNo = sourceEntry.GetSubledgerAccountNo();
      entry.ResponsibilityArea = sourceEntry.GetResponsibilityArea();
      entry.BudgetConcept = string.Empty;
      entry.EventType = sourceEntry.GetEventType();
      entry.VerificationNumber = string.Empty;
      entry.VoucherEntryType = sourceEntry.GetVoucherEntryType();
      entry.Date = header.AccountingDate;
      entry.Currency = sourceEntry.GetCurrency();
      entry.Amount = sourceEntry.GetAmount();
      entry.ExchangeRate = sourceEntry.GetExchangeRate();
      entry.BaseCurrencyAmount = sourceEntry.GetBaseCurrencyAmount();
      entry.Protected = false;

      entry.AddIssues(sourceEntry.GetEntryIssues());

      return entry;
    }


    private FixedList<TextFileVoucherEntry> ParseTextLinesToTextFileVoucherEntries() {
      List<TextFileVoucherEntry> entries = new List<TextFileVoucherEntry>(_textFileLines.Length);

      for (int lineIndex = 0; lineIndex < _textFileLines.Length; lineIndex++) {
        entries.Add(new TextFileVoucherEntry(_textFileLines[lineIndex], lineIndex + 1));
      }

      return entries.ToFixedList();
    }


    private string[] GetVoucherUniqueIds() {
      return this.Entries.Select(x => x.VoucherUniqueID)
                         .Distinct()
                         .ToArray();
    }


    #endregion Methods

  }  // class TextFileStructurer

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter
