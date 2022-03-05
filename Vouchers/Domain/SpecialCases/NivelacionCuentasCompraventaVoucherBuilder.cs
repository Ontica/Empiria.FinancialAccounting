/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Vouchers Management                        Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Vouchers.dll           Pattern   : Concrete Builder                        *
*  Type     : NivelacionCuentasCompraventaVoucherBuilder License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Builds a voucher that mantains the right valued balance for a list of accounts pairs,          *
*             typically accounts handled in different currencies at different times.                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.Vouchers.Adapters;

namespace Empiria.FinancialAccounting.Vouchers.SpecialCases {

  /// <summary>Builds a voucher that mantains the right valued balance for a list of accounts pairs,
  /// typically accounts handled in different currencies at different times.</summary>
  internal class NivelacionCuentasCompraventaVoucherBuilder : VoucherBuilder {

    internal NivelacionCuentasCompraventaVoucherBuilder() {
      // no-op
    }

    internal override FixedList<string> DryRun() {
      FixedList<VoucherEntryFields> entries = GetCancelationEntries();

      return ImplementsDryRun(entries);
    }


    internal override Voucher GenerateVoucher() {
      FixedList<VoucherEntryFields> entries = GetCancelationEntries();

      FixedList<string> issues = this.ImplementsDryRun(entries);

      Assertion.Assert(issues.Count == 0,
          "There were one or more issues generating 'Nivelación de cuentas de compraventa' voucher: " +
          EmpiriaString.ToString(issues));

      var voucher = new Voucher(base.Fields);

      voucher.Save();

      CreateVoucherEntries(voucher, entries);

      return voucher;
    }


    private void CreateVoucherEntries(Voucher voucher, FixedList<VoucherEntryFields> entries) {
      foreach (var entry in entries) {

        entry.VoucherId = voucher.Id;

        VoucherEntry cancelationEntry = voucher.AppendEntry(entry);

        cancelationEntry.Save();
      }
    }

    private AccountsList GetNivelacionAccountsList() {
      return base.SpecialCaseType.AccountsList;
    }


    private FixedList<VoucherEntryFields> GetCancelationEntries() {
      return new FixedList<VoucherEntryFields>();
    }

    private FixedList<string> ImplementsDryRun(FixedList<VoucherEntryFields> entries) {
      var validator = new VoucherValidator(Ledger.Parse(base.Fields.LedgerUID),
                                           base.Fields.AccountingDate);

      return validator.Validate(entries);
    }

  }  // class NivelacionCuentasCompraventaVoucherBuilder

}  // namespace Empiria.FinancialAccounting.Vouchers.SpecialCases
