/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                 Component : Vouchers Importer                    *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll   Pattern   : Service provider                     *
*  Type     : VoucherImporter                               License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Performs voucher importation tasks from a standard structure adapted from distinct sources.    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter.Adapters;
using Empiria.FinancialAccounting.Vouchers.Adapters;
using Empiria.FinancialAccounting.Vouchers.UseCases;

namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter {

  /// <summary>Performs voucher importation tasks from a standard structure
  /// adapted from distinct sources.</summary>
  internal class VoucherImporter {

    private readonly ImportVouchersCommand _command;
    private readonly FixedList<ToImportVoucher> _toImportVouchersList;

    #region Public methods

    internal VoucherImporter(ImportVouchersCommand command,
                             FixedList<ToImportVoucher> toImportVouchersList) {
      Assertion.AssertObject(command, "command");
      Assertion.AssertObject(toImportVouchersList, "toImportVouchersList");

      _command = command;
      _toImportVouchersList = toImportVouchersList;
    }


    internal ImportVouchersResult DryRunImport() {
      var result = new ImportVouchersResult();

      result.VoucherTotals = GetImportVoucherTotals();

      result.Errors = GetImportErrors();
      result.Warnings = GetImportWarnings();

      return result;
    }


    internal ImportVouchersResult Import() {
      ImportVouchersResult result = this.DryRunImport();

      //if (result.HasErrors) {
      //  return result;
      //}

      using (var usecases = VoucherEditionUseCases.UseCaseInteractor()) {
        foreach (var voucher in _toImportVouchersList) {
          VoucherImporterDataService.StoreVoucher(voucher);
          VoucherImporterDataService.StoreVoucherIssues(voucher);

          if (voucher.HasErrors) {
            continue;
          }

          VoucherFields voucherFields = MapToVoucherFields(voucher.Header);
          FixedList<VoucherEntryFields> entriesFields = MapToVoucherEntriesFields(voucher.Entries);

          // usecases.ImportVoucher(voucherFields, entriesFields, _command.TryToCloseVouchers);
        }
      }

      return result;
    }


    #endregion Public methods

    #region Private methods

    private FixedList<NamedEntityDto> GetImportErrors() {
      return new FixedList<NamedEntityDto>();
    }


    private FixedList<NamedEntityDto> GetImportWarnings() {
      return new FixedList<NamedEntityDto>();
    }


    private FixedList<ImportVouchersTotals> GetImportVoucherTotals() {
      return new FixedList<ImportVouchersTotals>();
    }


    private FixedList<VoucherEntryFields> MapToVoucherEntriesFields(FixedList<ToImportVoucherEntry> entries) {
      return new FixedList<VoucherEntryFields>(entries.Select(x => MapToVoucherEntryFields(x)));
    }


    private VoucherFields MapToVoucherFields(ToImportVoucherHeader header) {
      return new VoucherFields {
        Concept = header.Concept,
        AccountingDate = header.AccountingDate,
        RecordingDate = header.RecordingDate,
        ElaboratedByUID = header.ElaboratedBy.UID,
        LedgerUID = header.Ledger.UID,
        TransactionTypeUID = header.TransactionType.UID,
        VoucherTypeUID = header.VoucherType.UID,
        FunctionalAreaId = header.FunctionalArea.Id
      };
    }


    private VoucherEntryFields MapToVoucherEntryFields(ToImportVoucherEntry entry) {
      return new VoucherEntryFields {
        LedgerAccountId = entry.LedgerAccount.Id,
        SubledgerAccountId = entry.SubledgerAccount.Id,
        StandardAccountIdForCreateLedgerAccount = entry.StandardAccount.Id,
        SubledgerAccountNoToCreate = entry.SubledgerAccountNo,
        SectorId = entry.Sector.Id,
        ResponsibilityAreaId = entry.ResponsibilityArea.Id,
        BudgetConcept = entry.BudgetConcept,
        EventTypeId = entry.EventType.Id,
        VerificationNumber = entry.VerificationNumber,
        Date = entry.Date,
        Concept = entry.Concept,
        VoucherEntryType = entry.VoucherEntryType,
        CurrencyUID = entry.Currency.UID,
        Amount = entry.Amount,
        ExchangeRate = entry.ExchangeRate,
        BaseCurrencyAmount = entry.BaseCurrencyAmount,
        Protected = entry.Protected,
        CreateLedgerAccount = entry.CreateLedgerAccount,
        CreateSubledgerAccount = entry.CreateSubledgerAccount
      };
    }

    #endregion Private methods

  }  // class StandardVoucherImporter

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter
