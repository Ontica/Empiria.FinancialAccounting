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

  internal class ToImportVoucherEntry {

    internal ToImportVoucherEntry(ToImportVoucherHeader header) {
      this.ToImportVoucherHeader = header;
    }

    public ToImportVoucherHeader ToImportVoucherHeader {
      get;
    }

    public LedgerAccount LedgerAccount {
      get; internal set;
    }

    public Sector Sector {
      get; internal set;
    }

    public SubsidiaryAccount SubledgerAccount {
      get; internal set;
    }

    public FunctionalArea ResponsibilityArea {
      get; internal set;
    }

    public string BudgetConcept {
      get; internal set;
    } = string.Empty;


    public EventType EventType {
      get; internal set;
    }

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
    }

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

    public FixedList<ToImportVoucherIssue> Issues {
      get; internal set;
    }

  }  // class StandardVoucherEntry

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter
