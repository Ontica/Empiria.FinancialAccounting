/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                Component : Vouchers Importer                     *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll  Pattern   : Information Holder                    *
*  Type     : ToImportVoucherEntry                         License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  :                                                                                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter {

  internal class ToImportVoucherEntry {

    public string BaseAccount {
      get; internal set;
    }

    public string Subaccount {
      get; internal set;
    }

    public string CurrencyCode {
      get; internal set;
    }

    public string Area {
      get; internal set;
    }

    public decimal Debit {
      get; internal set;
    }

    public decimal Credit {
      get; internal set;
    }

    public string SubledgerAccount {
      get; internal set;
    }

    public decimal ExchangeRate {
      get; internal set;
    }

    internal Currency GetCurrency() {
      return Currency.Parse(this.CurrencyCode);
    }


    internal Sector GetSector() {
      string sectorCode = this.Subaccount.Substring(this.Subaccount.Length - 2);

      return Sector.Parse(sectorCode);
    }


    internal LedgerAccount GetLedgerAccount(Ledger ledger) {
      string accountNumber = this.BaseAccount + "-" + this.Subaccount;

      var account = ledger.AccountsChart.GetAccount(accountNumber);

      var standardAccount = StandardAccount.Parse(account.StandardAccountId);

      return ledger.GetAccount(standardAccount);
    }


    internal SubsidiaryAccount GetSubledgerAccount(Ledger ledger) {
      throw new NotImplementedException();
    }

  }  // class ExcelSourceData

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter
