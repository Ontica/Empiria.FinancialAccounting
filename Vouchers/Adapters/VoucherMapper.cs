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
using System.Collections.Generic;

using Empiria.FinancialAccounting.Adapters;

namespace Empiria.FinancialAccounting.Vouchers.Adapters {

  /// <summary>Mapping methods for accounting vouchers.</summary>
  static internal class VoucherMapper {

    static internal VoucherDto Map(Voucher voucher) {

      var dto = new VoucherDto {
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
        ClosedBy = voucher.IsClosed ? voucher.ClosedBy.Name : string.Empty,
        Status = voucher.StatusName,
        IsClosed = voucher.IsClosed,
        AllEntriesAreInBaseCurrency = !voucher.Entries.Contains(x => !x.Currency.Equals(voucher.Ledger.BaseCurrency)),
        Actions = voucher.Actions,
        Entries = MapToVoucherEntriesDescriptorWithTotals(voucher)
      };

      return dto;
    }


    static internal VoucherFields MapToVoucherFields(Voucher voucher) {
      return new VoucherFields {
         AccountingDate = voucher.AccountingDate,
         Concept = voucher.Concept,
         ElaboratedById = voucher.ElaboratedBy.Id,
         FunctionalAreaId = voucher.FunctionalArea.Id,
         LedgerUID = voucher.Ledger.UID,
         RecordingDate = voucher.RecordingDate,
         TransactionTypeUID = voucher.TransactionType.UID,
         VoucherTypeUID = voucher.VoucherType.UID
      };
    }


    static internal VoucherEntryFields MapToVoucherEntryFields(VoucherEntry entry) {
      return new VoucherEntryFields {
         Amount = entry.Amount,
         BaseCurrencyAmount = entry.BaseCurrencyAmount,
         BudgetConcept = entry.BudgetConcept,
         Concept = entry.Concept,
         CurrencyUID = entry.Currency.UID,
         Date = entry.Date,
         EventTypeId = entry.EventType.Id,
         ExchangeRate = entry.ExchangeRate,
         LedgerAccountId = entry.LedgerAccount.Id,
         Protected = entry.Protected,
         ReferenceEntryId = entry.ReferenceEntryId,
         ResponsibilityAreaId = entry.ResponsibilityArea.Id,
         SectorId = entry.Sector.Id,
         SubledgerAccountId = entry.SubledgerAccount.Id,
         VerificationNumber = entry.VerificationNumber,
         VoucherEntryType = entry.VoucherEntryType,
         VoucherId = entry.Voucher.Id
      };
    }


    static internal VoucherEntryDto MapEntry(VoucherEntry entry) {
      return new VoucherEntryDto {
        Id = entry.Id,
        VoucherEntryType = entry.VoucherEntryType,
        LedgerAccount = LedgerMapper.MapAccount(entry.LedgerAccount, entry.Voucher.AccountingDate),
        Sector = entry.HasSector ?
                 LedgerMapper.MapSectorRulesShort(entry.SectorRule) : null,
        SubledgerAccount = entry.HasSubledgerAccount ?
                           SubledgerMapper.MapToSubledgerAccountDescriptor(entry.SubledgerAccount) : null,
        Concept = entry.Concept,
        Date = entry.Date,
        ResponsibilityArea = !entry.ResponsibilityArea.IsEmptyInstance ?
                              entry.ResponsibilityArea.MapToNamedEntity() : null,
        BudgetConcept = entry.BudgetConcept,
        EventType = !entry.EventType.IsEmptyInstance ?
                     entry.EventType.MapToNamedEntity() : null,
        VerificationNumber = entry.VerificationNumber,
        Currency = entry.Currency.MapToNamedEntity(),
        Amount = entry.Amount,
        BaseCurrencyAmount = entry.BaseCurrencyAmount,
        ExchangeRate = entry.ExchangeRate
      };
    }


    static internal FixedList<VoucherDescriptorDto> MapToDescriptor(FixedList<Voucher> list) {
      return new FixedList<VoucherDescriptorDto>(list.Select((x) => MapToDescriptor(x)));
    }


    static private VoucherDescriptorDto MapToDescriptor(Voucher voucher) {
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
        Status = voucher.StatusName
      };
    }


    static private VoucherEntryDescriptorDto MapToDescriptor(VoucherEntry entry) {
      return new VoucherEntryDescriptorDto {
        Id = entry.Id,
        VoucherEntryType = entry.VoucherEntryType,
        AccountNumber = entry.LedgerAccount.Number,
        AccountName = entry.LedgerAccount.Name,
        Sector = entry.Sector.Code,
        SubledgerAccountNumber = entry.HasSubledgerAccount ? entry.SubledgerAccount.Number : string.Empty,
        SubledgerAccountName = entry.HasSubledgerAccount ? entry.SubledgerAccount.Name : string.Empty,
        ResponsibilityArea = entry.ResponsibilityArea.Code,
        VerificationNumber = entry.VerificationNumber,
        Currency = $"{entry.Currency.Code} / {entry.Currency.Abbrev}",
        ExchangeRate = entry.ExchangeRate,
        Debit = entry.Debit,
        Credit = entry.Credit,
        ItemType = VoucherEntryItemType.AccountEntry
      };
    }


    #region Helpers

    static private FixedList<VoucherEntryDescriptorDto> MapToVoucherEntriesDescriptorWithTotals(Voucher voucher) {
      var list = new List<VoucherEntryDescriptorDto>(voucher.Entries.Select((x) => MapToDescriptor(x)));

      FixedList<VoucherTotal> totals = voucher.GetTotals();

      list.AddRange(MapTotalsEntries(totals));

      return list.ToFixedList();
    }


    static private FixedList<VoucherEntryDescriptorDto> MapTotalsEntries(FixedList<VoucherTotal> totals) {
      return new FixedList<VoucherEntryDescriptorDto>(totals.Select((x) => MapTotalsEntry(x)));
    }


    static private VoucherEntryDescriptorDto MapTotalsEntry(VoucherTotal total) {
      return new VoucherEntryDescriptorDto {
        ItemType = VoucherEntryItemType.TotalsEntry,
        AccountName = total.IsBalanced ?
                         $"Sumas iguales en {total.Currency.Name}" :
                         $"Diferencia de {total.Difference.ToString("C2")} en {total.Currency.Name}",
        Currency = $"{total.Currency.Code} / {total.Currency.Abbrev}",
        Credit = total.CreditTotal,
        Debit = total.DebitTotal
      };
    }

    #endregion Helpers

  }  // class VoucherMapper


}  // namespace Empiria.FinancialAccounting.Vouchers.Adapters
