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
using System.Collections.Generic;
using System.Linq;

using Empiria.FinancialAccounting.Vouchers.Adapters;
using Empiria.FinancialAccounting.Vouchers.UseCases;

using Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter.Adapters;

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
      using (var usecases = VoucherEditionUseCases.UseCaseInteractor()) {

        foreach (ToImportVoucher voucher in _toImportVouchersList) {

          if (voucher.HasErrors) {
            continue;
          }

          VoucherFields voucherFields = MapToVoucherFields(voucher.Header);

          foreach (var entry in voucher.Entries) {
            var mappedEntry = MapToVoucherEntryFields(entry);

            FixedList<string> entryIssues = usecases.ValidateVoucherEntryToImport(voucherFields, mappedEntry);

            foreach (var issueText in entryIssues) {
              var issue = new ToImportVoucherIssue(VoucherIssueType.Error,
                                                   voucher.Header.ImportationSet,
                                                   entry.DataSource,
                                                   issueText);

              entry.AddIssue(issue);
            }
          }

          if (voucher.HasErrors) {
            continue;
          }

          FixedList<VoucherEntryFields> entriesFields = MapToVoucherEntriesFields(voucher.Entries);

          FixedList<string> errors = usecases.ValidateVoucherToImport(voucherFields, entriesFields);

          foreach (var error in errors) {
            voucher.AddError(error);
          }

        }  // foreach

      }  // using

      return GetDryRunResult();
    }


    internal void DryRunImportOne(ToImportVoucher voucher) {
      using (var usecases = VoucherEditionUseCases.UseCaseInteractor()) {

        if (voucher.HasErrors) {
          return;
        }

        VoucherFields voucherFields = MapToVoucherFields(voucher.Header);

        foreach (var entry in voucher.Entries) {
          var mappedEntry = MapToVoucherEntryFields(entry);

          FixedList<string> entryIssues = usecases.ValidateVoucherEntryToImport(voucherFields, mappedEntry);

          foreach (var issueText in entryIssues) {
            var issue = new ToImportVoucherIssue(VoucherIssueType.Error,
                                                  voucher.Header.ImportationSet,
                                                  entry.DataSource,
                                                  issueText);

            entry.AddIssue(issue);
          }
        }

        if (voucher.HasErrors) {
          return;
        }

        FixedList<VoucherEntryFields> entriesFields = MapToVoucherEntriesFields(voucher.Entries);

        FixedList<string> errors = usecases.ValidateVoucherToImport(voucherFields, entriesFields);

        foreach (var error in errors) {
          voucher.AddError(error);
        }

      }  // using
    }


    internal ImportVouchersResult Import() {
      ImportVouchersResult result = this.DryRunImport();

      EmpiriaLog.Info("Dry run importation ends. Actual importation starts ...");

      using (var usecases = VoucherEditionUseCases.UseCaseInteractor()) {

        foreach (ToImportVoucher voucher in _toImportVouchersList) {
          VoucherImporterDataService.StoreVoucher(voucher);
          VoucherImporterDataService.StoreVoucherIssues(voucher);

          if (voucher.HasErrors) {
            foreach (var issue in voucher.Issues) {
              EmpiriaLog.Info($"Póliza '{voucher.Header.UniqueID}': {issue.Description}");
            }
            continue;
          }

          VoucherFields voucherFields = MapToVoucherFields(voucher.Header);
          FixedList<VoucherEntryFields> entriesFields = MapToVoucherEntriesFields(voucher.Entries);

          try {
            usecases.ImportVoucher(voucherFields, entriesFields, _command.TryToCloseVouchers);

          } catch (Exception e) {
            EmpiriaLog.Error(e);
            EmpiriaLog.Info($"Póliza '{voucher.Header.UniqueID}': {e.Message}");

          }
        }  // foreach

      }  // using

      return result;
    }


    internal VoucherDto TryImportOne(ToImportVoucher voucher) {
      this.DryRunImportOne(voucher);

      using (var usecases = VoucherEditionUseCases.UseCaseInteractor()) {
        VoucherImporterDataService.StoreVoucher(voucher);
        VoucherImporterDataService.StoreVoucherIssues(voucher);

        if (voucher.HasErrors) {
          foreach (var issue in voucher.Issues) {
            EmpiriaLog.Info($"Póliza '{voucher.Header.UniqueID}': {issue.Description}");
          }
          return null;
        }

        VoucherFields voucherFields = MapToVoucherFields(voucher.Header);
        FixedList<VoucherEntryFields> entriesFields = MapToVoucherEntriesFields(voucher.Entries);

        try {
          return usecases.ImportVoucher(voucherFields, entriesFields, _command.TryToCloseVouchers);

        } catch (Exception e) {
          EmpiriaLog.Error(e);
          EmpiriaLog.Info($"Póliza '{voucher.Header.UniqueID}': {e.Message}");
          return null;
        }
      }  // using
    }

    #endregion Public methods

    #region Private methods

    private ImportVouchersResult GetDryRunResult() {
      var result = new ImportVouchersResult();

      result.VoucherTotals = GetImportVoucherTotals();

      result.VouchersCount = result.VoucherTotals.Sum(x => x.VouchersCount);

      result.Errors = GetImportErrors();
      result.Warnings = GetImportWarnings();

      return result;
    }


    private FixedList<NamedEntityDto> GetImportErrors() {
      var errors = _toImportVouchersList.SelectMany(
                              z => z.AllIssues.FindAll(w => w.Type == VoucherIssueType.Error));

      return new FixedList<NamedEntityDto>(errors.Select(x => x.ToNamedEntity()));
    }


    private FixedList<NamedEntityDto> GetImportWarnings() {
      var warnings = _toImportVouchersList.SelectMany(
                              z => z.AllIssues.FindAll(w => w.Type == VoucherIssueType.Warning));

      return new FixedList<NamedEntityDto>(warnings.Select(x => x.ToNamedEntity()));
    }


    private FixedList<ImportVouchersTotals> GetImportVoucherTotals() {
      var importationSets = _toImportVouchersList.Select(x => x.Header.ImportationSet)
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
        StandardAccountId = entry.StandardAccount.Id,
        LedgerAccountId = entry.LedgerAccount.Id,
        SubledgerAccountId = entry.SubledgerAccount.Id,
        SubledgerAccountNumber = entry.SubledgerAccountNo,
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
        Protected = entry.Protected
      };
    }

    #endregion Private methods

  }  // class VoucherImporter

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter
