/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                Component : Vouchers Importer                     *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll  Pattern   : Structurer                            *
*  Type     : ExcelFileStructurer                          License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Provides information structure services for vouchers contained in Excel Files.                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;

using Empiria.FinancialAccounting.Vouchers;
using Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter.Adapters;


namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter {

  /// <summary>Provides information structure services for vouchers contained in Excel Files.</summary>
  internal class ExcelFileStructurer {

    private readonly ImportVouchersCommand _command;
    private readonly FixedList<ExcelVoucherEntry> _entries;

    public ExcelFileStructurer(ImportVouchersCommand command,
                               FixedList<ExcelVoucherEntry> excelEntries) {
      Assertion.AssertObject(command, "command");
      Assertion.AssertObject(excelEntries, "excelEntries");

      _command = command;
      _entries = excelEntries;

    }

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

      var header = new ToImportVoucherHeader {
        ImportationSet = sourceHeader.ImportationSet,
        UniqueID = sourceHeader.VoucherUniqueID,
        Ledger = sourceHeader.GetLedger(),
        Concept = sourceHeader.VoucherConcept,
        AccountingDate = _command.AccountingDate,
        VoucherType = VoucherType.Parse(_command.VoucherTypeUID),
        TransactionType = TransactionType.Parse(_command.TransactionTypeUID),
        FunctionalArea = sourceHeader.GetFunctionalArea(),
        RecordingDate = DateTime.Today,
        ElaboratedBy = Participant.Current,

        Issues = sourceHeader.GetHeaderIssues()
      };

      return header;
    }


    private FixedList<ToImportVoucherEntry> MapToImportVoucherEntries(ToImportVoucherHeader header) {
      var entries = _entries.FindAll(x => x.VoucherUniqueID == header.UniqueID);

      var mapped = entries.Select(entry => MapToImportVoucherEntry(header, entry));

      return new List<ToImportVoucherEntry>(mapped).ToFixedList();
    }


    private ToImportVoucherEntry MapToImportVoucherEntry(ToImportVoucherHeader header,
                                                         ExcelVoucherEntry sourceEntry) {
      var entry = new ToImportVoucherEntry(header) {
        StandardAccount = sourceEntry.GetStandardAccount(),
        Sector = sourceEntry.GetSector(),
        SubledgerAccount = sourceEntry.GetSubledgerAccount(),
        SubledgerAccountNo = sourceEntry.GetSubledgerAccountNo(),
        ResponsibilityArea = header.FunctionalArea,
        BudgetConcept = string.Empty,
        EventType = EventType.Empty,
        VerificationNumber = string.Empty,
        VoucherEntryType = sourceEntry.VoucherEntryType,
        Date = header.AccountingDate,
        Currency = sourceEntry.GetCurrency(),
        Amount = sourceEntry.GetAmount(),
        ExchangeRate = sourceEntry.GetExchangeRate(),
        BaseCurrencyAmount = sourceEntry.GetBaseCurrencyAmount(),
        Protected = false,

        Issues = sourceEntry.GetEntryIssues()
      };

      return entry;
    }


    private string[] GetVoucherUniqueIds() {
      return _entries.Select(x => x.VoucherUniqueID)
                     .Distinct()
                     .ToArray();
    }

  }  // class ExcelFileStructurer

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter
