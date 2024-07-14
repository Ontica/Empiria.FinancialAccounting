/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Vouchers Management                        Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Vouchers.dll           Pattern   : Information Holder                      *
*  Type     : VoucherDto                                 License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO for an accounting voucher.                                                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.Adapters;

namespace Empiria.FinancialAccounting.Vouchers.Adapters {

  /// <summary>Output DTO for an accounting voucher.</summary>
  public class VoucherDto {

    internal VoucherDto() {
      // no-op
    }

    public long Id {
      get; internal set;
    }

    public string Number {
      get; internal set;
    }

    public NamedEntityDto AccountsChart {
      get; internal set;
    }

    public NamedEntityDto Ledger {
      get; internal set;
    }

    public string Concept {
      get; internal set;
    }

    public NamedEntityDto TransactionType {
      get; internal set;
    }

    public NamedEntityDto VoucherType {
      get; internal set;
    }

    public NamedEntityDto FunctionalArea {
      get; internal set;
    }

    public DateTime AccountingDate {
      get; internal set;
    }

    public DateTime RecordingDate {
      get; internal set;
    }

    public string ElaboratedBy {
      get; internal set;
    }

    public string AuthorizedBy {
      get; internal set;
    }

    public string ClosedBy {
      get; internal set;
    }

    public string Status {
      get; internal set;
    }

    public bool IsClosed {
      get; internal set;
    }

    public bool AllEntriesAreInBaseCurrency {
      get; internal set;
    }

    public VoucherActions Actions {
      get; internal set;
    }

    public FixedList<VoucherEntryDescriptorDto> Entries {
      get; internal set;
    }

  }  // class VoucherDto


  /// <summary>Output DTO for an accounting voucher.</summary>
  public class VoucherEntryDto {

    internal VoucherEntryDto() {
      // no-op
    }

    public long Id {
      get; internal set;
    }

    public VoucherEntryType VoucherEntryType {
      get; internal set;
    }

    public LedgerAccountDto LedgerAccount {
      get; internal set;
    }

    public SectorRuleShortDto Sector {
      get; internal set;
    }

    public SubledgerAccountDescriptorDto SubledgerAccount {
      get; internal set;
    }

    public string Concept {
      get; internal set;
    }

    public DateTime Date {
      get; internal set;
    }

    public NamedEntityDto ResponsibilityArea {
      get; internal set;
    }

    public string BudgetConcept {
      get; internal set;
    }

    public NamedEntityDto EventType {
      get; internal set;
    }

    public string VerificationNumber {
      get; internal set;
    }

    public NamedEntityDto Currency {
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

  }  // class VoucherEntryDto

}  // namespace Empiria.FinancialAccounting.Vouchers.Adapters
