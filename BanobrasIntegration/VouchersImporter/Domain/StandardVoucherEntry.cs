/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                Component : Vouchers Importer                     *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll  Pattern   : Information Holder                    *
*  Type     : StandardVoucherEntry                         License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  :                                                                                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.FinancialAccounting.Vouchers;
using Empiria.FinancialAccounting.Vouchers.Adapters;

namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter {

  internal class StandardVoucherEntry {

    internal StandardVoucherEntry(Voucher voucher, ToImportVoucherEntry sourceData) {
      Assertion.AssertObject(voucher, "voucher");
      Assertion.AssertObject(sourceData, "sourceData");

      this.Voucher = voucher;
      this.SourceData = sourceData;

      LoadFields();
    }

    private void LoadFields() {
      this.LedgerAccount = this.SourceData.GetLedgerAccount(this.Voucher.Ledger);
      this.Sector = this.SourceData.GetSector();
      this.SubledgerAccount = this.SourceData.GetSubledgerAccount(this.Voucher.Ledger);
      this.Currency = this.SourceData.GetCurrency();
    }

    internal Voucher Voucher {
      get;
    }

    public ToImportVoucherEntry SourceData {
      get;
    }

    internal VoucherEntryType VoucherEntryType {
      get {
        if (this.SourceData.Debit != 0) {
          return VoucherEntryType.Debit;
        } else {
          return VoucherEntryType.Credit;
        }
      }
    }

    internal LedgerAccount LedgerAccount {
      get; set;
    } = LedgerAccount.Empty;


    internal Sector Sector {
      get; set;
    } = Sector.Empty;


    internal SubsidiaryAccount SubledgerAccount {
      get; set;
    } = SubsidiaryAccount.Empty;


    internal Currency Currency {
      get; set;
    } = Currency.Empty;


    internal decimal Amount {
      get {
        return this.SourceData.Debit;
      }
    }


    internal decimal ExchangeRate {
      get {
        return this.SourceData.ExchangeRate;
      }
    }


    internal decimal BaseCurrencyAmount {
      get {
        return this.Amount * ExchangeRate;
      }
    }


    internal VoucherEntryFields MapToVoucherEntryFields() {
      return new VoucherEntryFields {
        VoucherId = this.Voucher.Id,
        VoucherEntryType = this.VoucherEntryType,
        LedgerAccountId = this.LedgerAccount.Id,
        SectorId = this.Sector.Id,
        SubledgerAccountId = this.SubledgerAccount.Id,
        CurrencyId = this.Currency.Id,
        Amount = this.Amount,
        ExchangeRate = this.ExchangeRate,
        BaseCurrencyAmount = this.BaseCurrencyAmount,
      };
    }

  }  // class StandardVoucherEntry

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter
