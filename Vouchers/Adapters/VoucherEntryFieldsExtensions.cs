/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Vouchers Management                        Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Vouchers.dll           Pattern   : Type Extension methods                  *
*  Type     : VoucherEntryFieldsExtensions               License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Extension methods for VoucherFields interface adapter.                                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Vouchers.Adapters {

  /// <summary>Extension methods for SearchVoucherCommand interface adapter.</summary>
  static internal class VoucherEntryFieldsExtensions {

    #region Extension methods

    static internal void EnsureValidFor(this VoucherEntryFields fields, Voucher voucher) {
      fields.EnsureValidData();

      Assertion.Assert(fields.GetVoucher().Equals(voucher),
                       "fields.VoucherId does not match the given voucher.");

      LedgerAccount account = voucher.Ledger.GetAccountWithId(fields.LedgerAccountId);

      account.CheckIsNotSummary(voucher.AccountingDate);

      account.CheckCurrencyRule(fields.GetCurrency(), voucher.AccountingDate);

      if (fields.HasSector) {
        account.CheckSectorRule(fields.GetSector(), voucher.AccountingDate);
      } else {
        account.CheckNoSectorRule(voucher.AccountingDate);
      }

      if (fields.HasSubledgerAccount) {
        account.CheckSubledgerAccountRule(fields.GetSubledgerAccount(), voucher.AccountingDate);
      } else {
        account.CheckNoSubledgerAccountRule(voucher.AccountingDate);
      }
    }


    static private void EnsureValidData(this VoucherEntryFields fields) {
      Assertion.Assert(fields.VoucherId > 0, "fields.VoucherId");
      Assertion.Assert(fields.LedgerAccountId > 0, "fields.LedgerAccountId");
      Assertion.Assert(fields.CurrencyId > 0, "fields.CurrencyId");
      Assertion.Assert(fields.VoucherEntryType == VoucherEntryType.Credit ||
                       fields.VoucherEntryType == VoucherEntryType.Debit,
                       "fields.VoucherEntryType");

      Assertion.Assert(fields.Amount > 0, "fields.Amount");

      if (fields.UsesBaseCurrency()) {
        Assertion.Assert(
            fields.BaseCurrencyAmount == 0 || fields.BaseCurrencyAmount == fields.Amount,
            "Invalid value for fields.BaseCurrencyAmount. Must be zero or equals to fields.Amount.");

      } else {
        Assertion.Assert(fields.BaseCurrencyAmount > 0,
                         "fields.BaseCurrencyAmount must be greater than zero");
      }

      Assertion.Assert(fields.Amount > 0, "fields.Amount");
    }


    static internal Currency GetCurrency(this VoucherEntryFields fields) {
      return Currency.Parse(fields.CurrencyId);
    }


    static internal LedgerAccount GetLedgerAccount(this VoucherEntryFields fields) {
      return LedgerAccount.Parse(fields.LedgerAccountId);
    }


    static internal FunctionalArea GetResponsibilityArea(this VoucherEntryFields fields) {
      return FunctionalArea.Parse(fields.ResponsibilityAreaId);
    }


    static internal Sector GetSector(this VoucherEntryFields fields) {
      return Sector.Parse(fields.SectorId);
    }


    static internal SubsidiaryAccount GetSubledgerAccount(this VoucherEntryFields fields) {
      return SubsidiaryAccount.Parse(fields.SubledgerAccountId);
    }


    static internal Voucher GetVoucher(this VoucherEntryFields fields) {
      return Voucher.Parse(fields.VoucherId);
    }


    static internal bool UsesBaseCurrency(this VoucherEntryFields fields) {
      return fields.GetCurrency().Equals(fields.GetLedgerAccount().Ledger.BaseCurrency);
    }

    #endregion Extension methods

  }  // class VoucherEntryFieldsExtensions

} // namespace Empiria.FinancialAccounting.Vouchers.Adapters
