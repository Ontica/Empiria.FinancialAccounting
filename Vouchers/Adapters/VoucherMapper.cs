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
using Empiria.FinancialAccounting.Adapters;

namespace Empiria.FinancialAccounting.Vouchers.Adapters {

  /// <summary>Mapping methods for accounting vouchers.</summary>
  static public class VoucherMapper {

    static internal VoucherDto Map(Voucher voucher) {
      return new VoucherDto {
        Id = voucher.Id,
        AccountsChart = voucher.Ledger.AccountsChart.MapToNamedEntity(),
        Ledger = voucher.Ledger.MapToNamedEntity(),
        Number = voucher.Number,
        Concept = voucher.Concept,
        TransactionType = voucher.TransactionType.MapToNamedEntity(),
        VoucherType = voucher.VoucherType.MapToNamedEntity(),
        FunctionalArea = voucher.FunctionalArea.MapToNamedEntity(),
        AccountingDate = voucher.AccountingDate,
        RecordingDate = voucher.RecordingDate,
        ElaboratedBy = voucher.ElaboratedBy.Name,
        AuthorizedBy = voucher.AuthorizedBy.Name,
        Status = voucher.IsOpened ? "Pendiente" : "Cerrado",
        Entries = MapToVoucherEntriesDescriptor(voucher.Entries)
      };
    }

    private static FixedList<VoucherEntryDescriptorDto> MapToVoucherEntriesDescriptor(FixedList<VoucherEntry> entries) {
      return new FixedList<VoucherEntryDescriptorDto>(entries.Select((x) => MapToVoucherEntryDescriptor(x)));
    }


    private static VoucherEntryDescriptorDto MapToVoucherEntryDescriptor(VoucherEntry entry) {
      return new VoucherEntryDescriptorDto {
        Id = entry.Id,
        VoucherEntryType = entry.VoucherEntryType,
        AccountNumber = entry.HasSubledgerAccount ? entry.SubledgerAccount.Number : entry.LedgerAccount.Number,
        AccountName = entry.HasSubledgerAccount ? entry.SubledgerAccount.Name : entry.LedgerAccount.Name,
        Sector = entry.Sector.Code,
        ResponsibilityArea = entry.ResponsibilityArea.Name,
        VerificationNumber = entry.VerificationNumber,
        Currency = $"{entry.Currency.Code} / {entry.Currency.Abbrev}",
        ExchangeRate = entry.ExchangeRate,
        Partial = entry.VoucherEntryType == VoucherEntryType.Debit ? entry.Debit : entry.Credit,
        Debit = entry.Debit,
        Credit = entry.Credit,
        ItemType = entry.HasSubledgerAccount ? VoucherEntryItemType.PartialEntry : VoucherEntryItemType.AccountEntry
      };
    }

    public static VoucherEntryDto MapEntry(VoucherEntry entry) {
      return new VoucherEntryDto {
        Id = entry.Id,
        VoucherEntryType = entry.VoucherEntryType,
        LedgerAccount = LedgerMapper.MapAccount(entry.LedgerAccount, entry.Voucher.AccountingDate),
        Sector = entry.HasSector ? LedgerMapper.MapSector(entry.SectorRule) : null,
        SubledgerAccount = entry.SubledgerAccount.MapToNumberedNamedEntity(),
        Concept = entry.Concept,
        Date = entry.Date,
        ResponsibilityArea = entry.ResponsibilityArea.MapToNamedEntity(),
        BudgetConcept = entry.BudgetConcept,
        EventType = entry.EventType.MapToNamedEntity(),
        VerificationNumber = entry.VerificationNumber,
        Currency = entry.Currency.MapToNamedEntity(),
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
        SourceName = voucher.FunctionalArea.FullName,
        AccountingDate = voucher.AccountingDate,
        RecordingDate = voucher.RecordingDate,
        ElaboratedBy = voucher.ElaboratedBy.Name,
        AuthorizedBy = voucher.AuthorizedBy.Name,
        Status = voucher.IsOpened ? "Pendiente" : "Cerrado"
      };
    }

  }  // class VoucherMapper

}  // namespace Empiria.FinancialAccounting.Vouchers.Adapters
