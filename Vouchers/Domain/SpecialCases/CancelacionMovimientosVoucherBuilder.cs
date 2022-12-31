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

    internal CancelacionMovimientosVoucherBuilder(VoucherSpecialCaseFields fields) : base(fields) {

      Voucher voucherToCancel = Voucher.TryParse(base.Ledger, fields.OnVoucherNumber);

      if (voucherToCancel == null) {
        Assertion.EnsureNoReachThisCode(
          $"La póliza '{fields.OnVoucherNumber}' no existe en la contabilidad {base.Ledger.FullName}."
        );
      }

      _voucherToCancel = voucherToCancel;
    }


    #region Abstract Implements

    protected override FixedList<VoucherEntryFields> BuildVoucherEntries() {
      return _voucherToCancel.Entries.Select(x => MapToCancelationEntry(x))
                                     .ToFixedList();
    }

    #endregion Abstract Implements

    #region Helpers

    private VoucherEntryFields MapToCancelationEntry(VoucherEntry toCancelEntry) {
      VoucherEntryFields cancelationEntryFields = VoucherMapper.MapToVoucherEntryFields(toCancelEntry);

      if (cancelationEntryFields.VoucherEntryType == VoucherEntryType.Credit) {
        cancelationEntryFields.VoucherEntryType = VoucherEntryType.Debit;
      } else {
        cancelationEntryFields.VoucherEntryType = VoucherEntryType.Credit;
      }

      return cancelationEntryFields;
    }

    #endregion Helpers

  }  // class CancelacionMovimientosVoucherBuilder

}  // namespace Empiria.FinancialAccounting.Vouchers.SpecialCases
