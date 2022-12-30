/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Vouchers Management                        Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Vouchers.dll           Pattern   : Concrete Builder                        *
*  Type     : CancelacionMovimientosVoucherBuilder       License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Builds a voucher that cancels (reverses) the credits and debits included in another voucher.   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.Vouchers.Adapters;

namespace Empiria.FinancialAccounting.Vouchers.SpecialCases {

  /// <summary>Builds a voucher that cancels (reverses) the credits and debits
  /// included in another voucher.</summary>
  internal class CancelacionMovimientosVoucherBuilder : VoucherBuilder {

    private readonly Voucher _voucherToCancel;

    internal CancelacionMovimientosVoucherBuilder(Voucher voucherToCancel) {
      Assertion.Require(voucherToCancel, "voucherToCancel");

      _voucherToCancel = voucherToCancel;
    }


    internal override FixedList<string> DryRun() {
      FixedList<VoucherEntryFields> entries = BuildVoucherEntries();

      return ImplementsDryRun(entries);
    }


    internal override Voucher GenerateVoucher() {
      FixedList<VoucherEntryFields> entries = BuildVoucherEntries();

      FixedList<string> issues = this.ImplementsDryRun(entries);

      Assertion.Require(issues.Count == 0,
          "There were one or more issues generating 'Cancelación de movimientos' voucher: " +
          EmpiriaString.ToString(issues));


      var voucher = new Voucher(base.Fields);

      voucher.Save();

      CreateVoucherEntries(voucher, entries);

      return voucher;
    }

    #region Private methods


    private void CreateVoucherEntries(Voucher voucher, FixedList<VoucherEntryFields> entries) {
      foreach (var entry in entries) {

        entry.VoucherId = voucher.Id;

        voucher.AppendAndSaveEntry(entry);
      }
    }


    private FixedList<VoucherEntryFields> BuildVoucherEntries() {
      return new FixedList<VoucherEntryFields>(_voucherToCancel.Entries.Select(x => MapToCancelationEntry(x)));
    }


    private VoucherEntryFields MapToCancelationEntry(VoucherEntry toCancelEntry) {
      VoucherEntryFields cancelationEntryFields = VoucherMapper.MapToVoucherEntryFields(toCancelEntry);

      if (cancelationEntryFields.VoucherEntryType == VoucherEntryType.Credit) {
        cancelationEntryFields.VoucherEntryType = VoucherEntryType.Debit;
      } else {
        cancelationEntryFields.VoucherEntryType = VoucherEntryType.Credit;
      }

      return cancelationEntryFields;
    }


    private FixedList<string> ImplementsDryRun(FixedList<VoucherEntryFields> entries) {
      var validator = new VoucherValidator(_voucherToCancel.Ledger,
                                           base.Fields.AccountingDate);

      return validator.Validate(entries);
    }


    #endregion Private methods

  }  // class CancelacionMovimientosVoucherBuilder

}  // namespace Empiria.FinancialAccounting.Vouchers.SpecialCases
