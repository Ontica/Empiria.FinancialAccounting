/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                 Component : Vouchers Importer                    *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll   Pattern   : Structurer                           *
*  Type     : StandardVoucherEntry                          License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Holds a voucher's structure coming from database tables.                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.FinancialAccounting.Vouchers;

namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter {

  public class ToImportVoucherEntry {

    internal ToImportVoucherEntry(ToImportVoucherHeader header) {
      this.ToImportVoucherHeader = header;
    }


    public ToImportVoucherHeader ToImportVoucherHeader {
      get;
    }


    public LedgerAccount LedgerAccount {
      get; internal set;
    } = LedgerAccount.Empty;


    public StandardAccount StandardAccount {
      get; internal set;
    } = StandardAccount.Empty;


    public Sector Sector {
      get; internal set;
    } = Sector.Empty;


    public SubsidiaryAccount SubledgerAccount {
      get; internal set;
    } = SubsidiaryAccount.Empty;


    public string SubledgerAccountNo {
      get; internal set;
    } = string.Empty;


    public FunctionalArea ResponsibilityArea {
      get; internal set;
    } = FunctionalArea.Empty;


    public string BudgetConcept {
      get; internal set;
    } = string.Empty;


    public EventType EventType {
      get; internal set;
    } = EventType.Empty;


    public string VerificationNumber {
      get; internal set;
    } = string.Empty;


    public VoucherEntryType VoucherEntryType {
      get; internal set;
    }


    public DateTime Date {
      get; internal set;
    } = ExecutionServer.DateMinValue;


    public string Concept {
      get; internal set;
    } = string.Empty;


    public Currency Currency {
      get; internal set;
    } = Currency.Empty;


    public decimal Amount {
      get; internal set;
    }


    public decimal ExchangeRate {
      get; internal set;
    }


    public decimal BaseCurrencyAmount {
      get; internal set;
    }


    public bool Protected {
      get; internal set;
    }


    public bool CreateLedgerAccount {
      get {
        return this.LedgerAccount.IsEmptyInstance &&
               !this.StandardAccount.IsEmptyInstance;
      }
    }


    public bool CreateSubledgerAccount {
      get {
        return this.SubledgerAccount.IsEmptyInstance &&
               this.SubledgerAccountNo.Length != 0;
      }
    }


    public FixedList<ToImportVoucherIssue> Issues {
      get; internal set;
    }


  }  // class StandardVoucherEntry

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter
