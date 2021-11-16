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

using Empiria.FinancialAccounting.Vouchers.Adapters;
using Empiria.FinancialAccounting.Adapters;

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


    public VoucherDto AppendEntry(int voucherId, VoucherEntryFields fields) {
      Assertion.Assert(voucherId > 0, "voucherId");
      Assertion.AssertObject(fields, "fields");

      var voucher = Voucher.Parse(voucherId);

      var voucherEntry = voucher.AppendEntry(fields);

      voucherEntry.Save();

      return VoucherMapper.Map(voucher);
    }


    public VoucherDto AppendEntries(int voucherId, FixedList<VoucherEntryFields> entries) {
      Assertion.Assert(voucherId > 0, "voucherId");
      Assertion.AssertObject(entries, "entries");

      var voucher = Voucher.Parse(voucherId);

      foreach (var entryFields in entries) {
        entryFields.VoucherId = voucherId;
        if (entryFields.LedgerAccountId == -1 && entryFields.StandardAccountIdForCreateLedgerAccount != -1) {
          var la = AssignVoucherLedgerStandardAccount(voucher.Id, entryFields.StandardAccountIdForCreateLedgerAccount);
          entryFields.LedgerAccountId = la.Id;
        }
        if (entryFields.CreateSubledgerAccount) {
          SubledgerAccount sa = voucher.Ledger.CreateSubledgerAccount(entryFields.SubledgerAccountNoToCreate,
                                                                      SubledgerType.Pending);

          sa.Save();

          entryFields.SubledgerAccountId = sa.Id;
        }

        var voucherEntry = voucher.AppendEntry(entryFields);

        voucherEntry.Save();
      }

      return VoucherMapper.Map(voucher);
    }


    public LedgerAccountDto AssignVoucherLedgerStandardAccount(int voucherId,
                                                               int standardAccountId) {
      Assertion.Assert(voucherId > 0, "voucherId");
      Assertion.Assert(standardAccountId > 0, "standardAccountId");

      var voucher = Voucher.Parse(voucherId);

      Assertion.Assert(voucher.IsOpened,
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


    public VoucherDto CloseVoucher(int voucherId) {
      Assertion.Assert(voucherId > 0, "voucherId");

      var voucher = Voucher.Parse(voucherId);

      voucher.Close();

      return VoucherMapper.Map(voucher);
    }


    public VoucherDto CreateVoucher(VoucherFields fields) {
      Assertion.AssertObject(fields, "fields");

      fields.EnsureIsValid();

      var voucher = new Voucher(fields);

      voucher.Save();

      return VoucherMapper.Map(voucher);
    }


    public VoucherDto ImportVoucher(VoucherFields voucherFields,
                                    FixedList<VoucherEntryFields> entriesFields,
                                    bool tryToClose) {
      Assertion.AssertObject(voucherFields, "voucherFields");
      Assertion.AssertObject(entriesFields, "entriesFields");

      VoucherDto voucher = CreateVoucher(voucherFields);

      voucher = AppendEntries(voucher.Id, entriesFields);

      if (tryToClose) {
        return CloseVoucher(voucher.Id);
      } else {
        return voucher;
      }
    }


    public VoucherDto DeleteEntry(int voucherId, int voucherEntryId) {
      Assertion.Assert(voucherId > 0, "voucherId");
      Assertion.Assert(voucherEntryId > 0, "voucherEntryId");

      var voucher = Voucher.Parse(voucherId);

      VoucherEntry entry = voucher.GetEntry(voucherEntryId);

      voucher.DeleteEntry(entry);

      return VoucherMapper.Map(voucher);
    }


    public void DeleteVoucher(int voucherId) {
      Assertion.Assert(voucherId > 0, "voucherId");

      var voucher = Voucher.Parse(voucherId);

      voucher.Delete();
    }


    public VoucherEntryDto GetCopyOfLastEntry(int voucherId) {
      Assertion.Assert(voucherId > 0, "voucherId");

      var voucher = Voucher.Parse(voucherId);

      VoucherEntry copy = voucher.GetCopyOfLastEntry();

      return VoucherMapper.MapEntry(copy);
    }


    public VoucherDto UpdateVoucher(int voucherId, VoucherFields fields) {
      Assertion.Assert(voucherId > 0, "voucherId");
      Assertion.AssertObject(fields, "fields");

      fields.EnsureIsValid();

      var voucher = Voucher.Parse(voucherId);

      voucher.Update(fields);

      voucher.Save();

      return VoucherMapper.Map(voucher);
    }


    public VoucherDto UpdateEntry(int voucherId, int voucherEntryId,
                                  VoucherEntryFields fields) {
      Assertion.Assert(voucherId > 0, "voucherId");
      Assertion.Assert(voucherEntryId > 0, "voucherEntryId");
      Assertion.AssertObject(fields, "fields");

      var voucher = Voucher.Parse(voucherId);

      VoucherEntry entry = voucher.GetEntry(voucherEntryId);

      voucher.UpdateEntry(entry, fields);

      return VoucherMapper.Map(voucher);
    }

    public FixedList<string> ValidateVoucher(int voucherId) {
      Assertion.Assert(voucherId > 0, "voucherId");

      var voucher = Voucher.Parse(voucherId);

      return voucher.ValidationResult();
    }


    #endregion Use cases

  }  // class VoucherEditionUseCases

}  // namespace Empiria.FinancialAccounting.Vouchers.UseCases
