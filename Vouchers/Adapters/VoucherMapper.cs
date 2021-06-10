/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Vouchers Management                        Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Vouchers.dll           Pattern   : Mapper class                            *
*  Type     : VoucherMapper                              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Mapping methods for accounting vouchers.                                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Vouchers.Adapters {

  /// <summary>Mapping methods for accounting vouchers.</summary>
  static public class VoucherMapper {

    static internal VoucherDto Map(Voucher voucher) {
      return new VoucherDto {
        Id = voucher.Id,
        Ledger = voucher.Ledger.MapToNamedEntity(),
        Number = voucher.Number,
        Concept = voucher.Concept,
        TransactionType = voucher.TransactionType.MapToNamedEntity(),
        VoucherType = voucher.VoucherType.MapToNamedEntity(),
        FunctionalArea = FunctionalArea.Parse(678).MapToNamedEntity(),
        AccountingDate = voucher.AccountingDate,
        RecordingDate = voucher.RecordingDate,
        ElaboratedBy = "María Luisa Jiménez",
        AuthorizedBy = "Rebeca Martínez Solís",
        Status = voucher.IsOpened ? "Pediente" : "Cerrado",
        Entries = MapEntries(voucher.Entries)
      };
    }

    static private FixedList<VoucherEntryDto> MapEntries(FixedList<VoucherEntry> list) {
      return new FixedList<VoucherEntryDto>(list.Select((x) => MapEntry(x)));
    }

    private static VoucherEntryDto MapEntry(VoucherEntry entry) {
      return new VoucherEntryDto {
        Id = entry.Id,
        VoucherEntryType = entry.VoucherEntryType,
        LedgerAccount = entry.LedgerAccount.MapToNumberedNamedEntity(),
        Sector = entry.Sector.MapToNamedEntity(),
        SubledgerAccount = entry.SubledgerAccount.MapToNumberedNamedEntity(),
        Concept = entry.Concept,
        Date = entry.Date,
        ResponsibilityArea = FunctionalArea.Parse(675).MapToNamedEntity(),
        BudgetConcept = entry.BudgetConcept,
        AvailabilityCode = entry.AvailabilityCode,
        // EventType = entry.EventType,
        VerificationNumber = entry.VerificationNumber,
        Currency = entry.Currency.MapToNamedEntity(),
        Debit = entry.Debit,
        Credit = entry.Credit,
        Amount = entry.Amount,
        BaseCurrencyAmount = entry.BaseCurrrencyAmount,
        ExchangeRate = entry.ExchangeRate
      };
    }

    static internal FixedList<VoucherDescriptorDto> MapToVoucherDescriptor(FixedList<Voucher> list) {
      return new FixedList<VoucherDescriptorDto>(list.Select((x) => MapToVoucherDescriptor(x)));
    }

    static internal VoucherDescriptorDto MapToVoucherDescriptor(Voucher voucher) {
      return new VoucherDescriptorDto {
        Id = voucher.Id,
        LedgerName = voucher.Ledger.FullName,
        Number = voucher.Number,
        Concept = voucher.Concept,
        TransactionTypeName = voucher.TransactionType.Name,
        VoucherTypeName = voucher.VoucherType.Name,
        SourceName = "Banobras",
        AccountingDate = voucher.AccountingDate,
        RecordingDate = voucher.RecordingDate,
        ElaboratedBy = "María Luisa Jiménez",
        AuthorizedBy = "Rebeca Martínez Solís",
        Status = voucher.IsOpened ? "Pediente" : "Cerrado"
      };
    }

  }  // class VoucherMapper

}  // namespace Empiria.FinancialAccounting.Vouchers.Adapters
