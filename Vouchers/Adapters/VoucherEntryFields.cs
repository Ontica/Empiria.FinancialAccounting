/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Vouchers Management                        Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Vouchers.dll           Pattern   : Input Data Holder                       *
*  Type     : VoucherEntryFields                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Data structure that serves as an adapter to create or update voucher entries data.             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Vouchers.Adapters {

  /// <summary>Data structure that serves as an adapter to create or update vouchers entries data.</summary>
  public class VoucherEntryFields : IVoucherEntry {

    public int VoucherId {
      get; set;
    }

    public int StandardAccountId {
      get; set;
    }

    public int LedgerAccountId {
      get; set;
    }

    public int SectorId {
      get; set;
    }


    public int SubledgerAccountId {
      get; set;
    }


    public string SubledgerAccountNumber {
      get; set;
    } = string.Empty;


    public int ReferenceEntryId {
      get; set;
    }


    public int ResponsibilityAreaId {
      get; set;
    }


    public string BudgetConcept {
      get; set;
    } = string.Empty;


    public int EventTypeId {
      get; set;
    }


    public string VerificationNumber {
      get; set;
    } = string.Empty;


    public VoucherEntryType VoucherEntryType {
      get; set;
    }


    public DateTime Date {
      get; set;
    } = ExecutionServer.DateMinValue;


    public string Concept {
      get; set;
    } = string.Empty;


    public string CurrencyUID {
      get; set;
    }


    public decimal Amount {
      get; set;
    }


    public decimal ExchangeRate {
      get; set;
    }


    public decimal BaseCurrencyAmount {
      get; set;
    }


    public bool Protected {
      get; set;
    }


    public bool HasEventType {
      get {
        return this.EventTypeId > 0;
      }
    }


    public bool HasSector {
      get {
        return this.SectorId > 0;
      }
    }


    public bool HasSubledgerAccount {
      get {
        return this.SubledgerAccountId > 0 || this.SubledgerAccountNumber.Length != 0;
      }
    }


    public Currency Currency {
      get {
        return Currency.Parse(this.CurrencyUID);
      }
    }


    public Sector Sector {
      get {
        return Sector.Parse(this.SectorId);
      }
    }


    Account IVoucherEntry.GetAccount(DateTime accountingDate) {
      if (this.LedgerAccountId > 0) {
        return LedgerAccount.Parse(this.LedgerAccountId)
                            .GetHistoric(accountingDate);
      }

      return StandardAccount.Parse(this.StandardAccountId)
                            .GetHistoric(accountingDate);
    }


  }  // class VoucherEntryFields

}  // namespace Empiria.FinancialAccounting.Vouchers.Adapters
