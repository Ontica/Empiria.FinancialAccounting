/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Vouchers Management                        Component : Use cases Layer                         *
*  Assembly : FinancialAccounting.Vouchers.dll           Pattern   : Use case interactor class               *
*  Type     : VoucherEditionUseCases                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases used to edit vouchers and their postings.                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.FinancialAccounting.Adapters;
using Empiria.FinancialAccounting.Vouchers.Adapters;

namespace Empiria.FinancialAccounting.Vouchers.UseCases {

  /// <summary>Use cases used to edit vouchers and their postings.</summary>
  public class VoucherEditionUseCases : UseCase {

    #region Constructors and parsers

    protected VoucherEditionUseCases() {
      // no-op
    }

    static public VoucherEditionUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<VoucherEditionUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases


    public VoucherDto AppendEntry(long voucherId, VoucherEntryFields fields) {
      Assertion.Require(voucherId > 0, "voucherId");
      Assertion.Require(fields, "fields");

      var voucher = Voucher.Parse(voucherId);

      voucher.AppendAndSaveEntry(fields);

      return VoucherMapper.Map(voucher);
    }


    public VoucherDto AppendEntries(long voucherId, FixedList<VoucherEntryFields> entries) {
      Assertion.Require(voucherId > 0, "voucherId");
      Assertion.Require(entries, "entries");

      var voucher = Voucher.Parse(voucherId);

      Assertion.Require(voucher.IsOpened,
                        "Esta operación sólo está disponible para pólizas abiertas.");

      foreach (var entryFields in entries) {
        entryFields.VoucherId = voucherId;

        if (entryFields.LedgerAccountId <= 0) {
          entryFields.LedgerAccountId = AssignVoucherLedgerStandardAccount(voucherId, entryFields.StandardAccountId).Id;
        }

        if (entryFields.SubledgerAccountId <= 0 && entryFields.SubledgerAccountNumber.Length != 0) {
          var newSubledgerAccount = voucher.Ledger.CreateSubledgerAccount(entryFields.SubledgerAccountNumber,
                                                                         SubledgerType.Pending);
          newSubledgerAccount.Save();

          entryFields.SubledgerAccountId = newSubledgerAccount.Id;
        }


        voucher.AppendAndSaveEntry(entryFields);
     }

      return VoucherMapper.Map(voucher);
    }


    public LedgerAccountDto AssignVoucherLedgerStandardAccount(long voucherId,
                                                               int standardAccountId) {
      Assertion.Require(voucherId > 0, "voucherId");
      Assertion.Require(standardAccountId > 0, "standardAccountId");

      var voucher = Voucher.Parse(voucherId);

      Assertion.Require(voucher.IsOpened,
                        "Esta operación sólo está disponible para pólizas abiertas.");

      var standardAccount = StandardAccount.Parse(standardAccountId);

      LedgerAccount ledgerAccount;

      if (voucher.Ledger.Contains(standardAccount)) {
        ledgerAccount = voucher.Ledger.GetAccount(standardAccount);
      } else {
        ledgerAccount = voucher.Ledger.AssignAccount(standardAccount);
      }

      return LedgerMapper.MapAccount(ledgerAccount, voucher.AccountingDate);
    }


    public string BulkClose(int[] voucherIdsArray) {
      Assertion.Require(voucherIdsArray, "voucherIdsArray");
      Assertion.Require(voucherIdsArray.Length > 0, "voucherIdsArray must have one or more values.");

      int closedCounter = 0;

      foreach (var voucherId in voucherIdsArray) {
        var voucher = Voucher.Parse(voucherId);

        if (!voucher.IsOpened) {
          continue;
        }

        if (!voucher.CanBeClosedBy(Participant.Current)) {
          continue;
        }

        try {
          voucher.Close();
          closedCounter++;
        } finally {
          // no-op
        }
      }

      return $"Se enviaron al diario {closedCounter} pólizas de {voucherIdsArray.Length} seleccionadas.";
    }


    public string BulkDelete(int[] voucherIdsArray) {
      Assertion.Require(voucherIdsArray, "voucherIdsArray");
      Assertion.Require(voucherIdsArray.Length > 0, "voucherIdsArray must have one or more values.");

      int deletedCounter = 0;

      foreach (var voucherId in voucherIdsArray) {
        var voucher = Voucher.Parse(voucherId);

        if (!voucher.IsOpened) {
          continue;
        }

        if (!(voucher.ElaboratedBy.Equals(Participant.Current) ||
              voucher.AuthorizedBy.Equals(Participant.Current))) {
          continue;
        }

        try {
          voucher.Delete();
          deletedCounter++;
        } finally {
          // no-op
        }
      }

      return $"Se eliminaron {deletedCounter} pólizas de {voucherIdsArray.Length} seleccionadas.";
    }

    public string BulkSendToSupervisor(int[] voucherIdsArray) {
      Assertion.Require(voucherIdsArray, "voucherIdsArray");
      Assertion.Require(voucherIdsArray.Length > 0, "voucherIdsArray must have one or more values.");

      int sentCounter = 0;

      foreach (var voucherId in voucherIdsArray) {
        var voucher = Voucher.Parse(voucherId);

        if (!voucher.IsOpened) {
          continue;
        }

        if (!voucher.ElaboratedBy.Equals(Participant.Current)) {
          continue;
        }

        try {
          voucher.SendToSupervisor();
          sentCounter++;
        } finally {
          // no-op
        }
      }

      return $"Se enviaron {sentCounter} pólizas al supervisor de {voucherIdsArray.Length} seleccionadas.";
    }


    public VoucherDto CloseVoucher(long voucherId, bool fromImporter) {
      Assertion.Require(voucherId > 0, "voucherId");

      var voucher = Voucher.Parse(voucherId);

      if (!fromImporter && !voucher.CanBeClosedBy(Participant.Current)) {
        Assertion.RequireFail($"La póliza no puede enviarse directamente al diario " +
                              $"por el usuario {Participant.Current.Name}.");

      } else if (fromImporter && !voucher.IsAccountingDateOpened) {
        EmpiriaLog.Info($"Se intentó cerrar la póliza {voucherId} desde el importador, pero tiene fecha valor.");

        return VoucherMapper.Map(voucher);
      }

      voucher.Close();

      return VoucherMapper.Map(voucher);
    }


    public VoucherDto CreateVoucher(VoucherFields fields) {
      Assertion.Require(fields, "fields");

      fields.EnsureValid();

      var voucher = new Voucher(fields);

      voucher.Save();

      return VoucherMapper.Map(voucher);
    }


    public VoucherDto ImportVoucher(VoucherFields voucherFields,
                                    FixedList<VoucherEntryFields> entriesFields,
                                    bool tryToClose) {
      Assertion.Require(voucherFields, "voucherFields");
      Assertion.Require(entriesFields, "entriesFields");

      FixedList<string> issues = ValidateVoucherToImport(voucherFields, entriesFields);

      Assertion.Require(issues.Count == 0, "There are one ore more problems with voucher data to be imported.");

      VoucherDto voucher = CreateVoucher(voucherFields);

      voucher = AppendEntries(voucher.Id, entriesFields);

      if (tryToClose) {
        return CloseVoucher(voucher.Id, true);
      } else {
        return voucher;
      }
    }


    public VoucherDto DeleteEntry(long voucherId, long voucherEntryId) {
      Assertion.Require(voucherId > 0, "voucherId");
      Assertion.Require(voucherEntryId > 0, "voucherEntryId");

      var voucher = Voucher.Parse(voucherId);

      VoucherEntry entry = voucher.GetEntry(voucherEntryId);

      voucher.DeleteEntry(entry);

      return VoucherMapper.Map(voucher);
    }


    public void DeleteVoucher(long voucherId) {
      Assertion.Require(voucherId > 0, "voucherId");

      var voucher = Voucher.Parse(voucherId);

      if (!(voucher.ElaboratedBy.Equals(Participant.Current) ||
            voucher.AuthorizedBy.Equals(Participant.Current))) {
        Assertion.RequireFail("La póliza no puede ser eliminada debido a que la tiene otro usuario.");
      }

      voucher.Delete();
    }


    public VoucherEntryDto GetCopyOfLastEntry(long voucherId) {
      Assertion.Require(voucherId > 0, "voucherId");

      var voucher = Voucher.Parse(voucherId);

      VoucherEntry copy = voucher.GetCopyOfLastEntry();

      return VoucherMapper.MapEntry(copy);
    }


    public VoucherDto SendVoucherToSupervisor(long voucherId) {
      Assertion.Require(voucherId > 0, "voucherId");

      var voucher = Voucher.Parse(voucherId);

      voucher.SendToSupervisor();

      return VoucherMapper.Map(voucher);
    }


    public VoucherDto UpdateVoucher(long voucherId, VoucherFields fields) {
      Assertion.Require(voucherId > 0, "voucherId");
      Assertion.Require(fields, "fields");

      fields.EnsureValid();

      var voucher = Voucher.Parse(voucherId);

      voucher.Update(fields);

      voucher.Save();

      return VoucherMapper.Map(voucher);
    }


    public VoucherDto UpdateEntry(long voucherId, long voucherEntryId,
                                  VoucherEntryFields fields) {
      Assertion.Require(voucherId > 0, "voucherId");
      Assertion.Require(voucherEntryId > 0, "voucherEntryId");
      Assertion.Require(fields, "fields");

      var voucher = Voucher.Parse(voucherId);

      var validator = new VoucherEntryValidator(voucher.Ledger, voucher.AccountingDate);

      FixedList<string> issues = validator.Validate(fields);

      Assertion.Require(issues.Count == 0,
                       "No se pudo guardar el movimiento debido a: " + EmpiriaString.ToString(issues));

      VoucherEntry entry = voucher.GetEntry(voucherEntryId);

      voucher.UpdateEntry(entry, fields);

      return VoucherMapper.Map(voucher);
    }


    public FixedList<string> ValidateVoucher(long voucherId) {
      Assertion.Require(voucherId > 0, "voucherId");

      var voucher = Voucher.Parse(voucherId);

      return voucher.ValidationResult(true);
    }


    public FixedList<string> ValidateVoucherToImport(VoucherFields voucher,
                                                     FixedList<VoucherEntryFields> entries) {
      voucher.EnsureValid();

      Ledger ledger = Ledger.Parse(voucher.LedgerUID);

      var validator = new VoucherValidator(ledger, voucher.AccountingDate);

      return validator.Validate(entries);
    }


    public FixedList<string> ValidateVoucherEntryToImport(VoucherFields voucher,
                                                          VoucherEntryFields entry) {

      Ledger ledger = Ledger.Parse(voucher.LedgerUID);

      var validator = new VoucherEntryValidator(ledger, voucher.AccountingDate);

      return validator.Validate(entry);
    }

    #endregion Use cases

  }  // class VoucherEditionUseCases

}  // namespace Empiria.FinancialAccounting.Vouchers.UseCases
