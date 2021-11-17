/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                 Component : Vouchers Importer                    *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll   Pattern   : Service provider                     *
*  Type     : StandardVoucherImporter                       License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Performs batch voucher importation tasks from a standard structure.                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System.Collections.Generic;
using System.Linq;

using Empiria.FinancialAccounting.Vouchers.Adapters;
using Empiria.FinancialAccounting.Vouchers.UseCases;

using Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter.Adapters;

namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter {

  /// <summary>Performs voucher importation tasks from a standard structure
  /// adapted from distinct sources.</summary>
  internal class StandardVoucherImporter {

    private readonly VoucherImportationCommand _command;
    private readonly FixedList<ToImportVoucher> _toImportVouchersList;

    #region Public methods

    internal StandardVoucherImporter(VoucherImportationCommand command) {
      Assertion.AssertObject(command, "command");

      _command = command;
      _toImportVouchersList = new FixedList<ToImportVoucher>();
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
        foreach (ToImportVoucher voucher in _toImportVouchersList) {
          VoucherImporterDataService.StoreVoucher(voucher);
          VoucherImporterDataService.StoreVoucherIssues(voucher);

          if (voucher.HasErrors) {
            continue;
          }

          VoucherFields voucherFields = MapToVoucherFields(voucher.Header);
          FixedList<VoucherEntryFields> entriesFields = MapToVoucherEntriesFields(voucher.Entries);

          usecases.ImportVoucher(voucherFields, entriesFields, true); // _command.TryToCloseVouchers)
        }
      }

      return result;
    }


    #endregion Public methods

    #region Private methods

    private FixedList<NamedEntityDto> GetImportErrors() {
      var errors = this._toImportVouchersList.SelectMany(
                              z => z.AllIssues.FindAll(w => w.Type == VoucherIssueType.Error));

      return new FixedList<NamedEntityDto>(errors.Select(x => x.ToNamedEntity()));
    }


    private FixedList<NamedEntityDto> GetImportWarnings() {
      var warnings = this._toImportVouchersList.SelectMany(
                              z => z.AllIssues.FindAll(w => w.Type == VoucherIssueType.Warning));

      return new FixedList<NamedEntityDto>(warnings.Select(x => x.ToNamedEntity()));
    }


    private FixedList<ImportVouchersTotals> GetImportVoucherTotals() {
      var importationSets = this._toImportVouchersList.Select(x => x.Header.ImportationSet)
                                                      .Distinct();

      var list = new List<ImportVouchersTotals>(importationSets.Count());

      foreach (string set in importationSets) {
        var totals = new ImportVouchersTotals {
          UID = set,
          Description = set
        };
        var setVouchers = this._toImportVouchersList.FindAll(x => x.Header.ImportationSet.Equals(set));

        totals.VouchersCount = setVouchers.Count;
        totals.ErrorsCount = setVouchers.Sum(x => x.AllIssues.Count(y => y.Type == VoucherIssueType.Error));
        totals.WarningsCount = setVouchers.Sum(x => x.AllIssues.Count(y => y.Type == VoucherIssueType.Warning));

        list.Add(totals);
      }

      return list.ToFixedList();
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
