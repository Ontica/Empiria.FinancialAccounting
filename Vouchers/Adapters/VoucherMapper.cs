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
  static public class VoucherMapper {

    static public VoucherDto Map(Voucher voucher) {
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
        Status = voucher.IsOpened ? "Pendiente" : "Cerrado",
        Actions = MapVoucherActions(voucher),
        Entries = MapToVoucherEntriesDescriptorWithTotals(voucher)
      };
      return dto;
    }

    private static VoucherActionsDto MapVoucherActions(Voucher voucher) {
      if (!voucher.IsOpened) {
        return new VoucherActionsDto();
      }
      if (!voucher.IsValid()) {
        return new VoucherActionsDto {
          ReviewVoucher = true
        };
      }
      if (voucher.CanBeClosedBy(Participant.Current)) {
        return new VoucherActionsDto {
          SendToLedger = true
        };
      } else {
        return new VoucherActionsDto {
          SendToSupervisor = true
        };
      }
    }


    private static FixedList<VoucherEntryDescriptorDto> MapToVoucherEntriesDescriptorWithTotals(Voucher voucher) {
      var list = new List<VoucherEntryDescriptorDto>(voucher.Entries.Select((x) => MapToVoucherEntryDescriptor(x)));

      FixedList<VoucherTotal> totals = voucher.GetTotals();

      list.AddRange(MapTotalsEntries(totals));

      return list.ToFixedList();
    }


    private static FixedList<VoucherEntryDescriptorDto> MapTotalsEntries(FixedList<VoucherTotal> totals) {
      return new FixedList<VoucherEntryDescriptorDto>(totals.Select((x) => MapTotalsEntry(x)));
    }


    private static VoucherEntryDescriptorDto MapTotalsEntry(VoucherTotal total) {
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

    private static VoucherEntryDescriptorDto MapToVoucherEntryDescriptor(VoucherEntry entry) {
      return new VoucherEntryDescriptorDto {
        Id = entry.Id,
        VoucherEntryType = entry.VoucherEntryType,
        AccountNumber = entry.LedgerAccount.Number,
        AccountName = entry.LedgerAccount.Name,
        Sector = entry.Sector.Code,
        SubledgerAccountNumber = entry.HasSubledgerAccount ? entry.SubledgerAccount.Number : string.Empty,
        SubledgerAccountName = entry.HasSubledgerAccount ? entry.SubledgerAccount.Name : string.Empty,
        ResponsibilityArea = entry.ResponsibilityArea.Code == "NULL" ? string.Empty : entry.ResponsibilityArea.Code,
        VerificationNumber = entry.VerificationNumber,
        Currency = $"{entry.Currency.Code} / {entry.Currency.Abbrev}",
        ExchangeRate = entry.ExchangeRate,
        Debit = entry.Debit,
        Credit = entry.Credit,
        ItemType = VoucherEntryItemType.AccountEntry
      };
    }

    public static VoucherEntryDto MapEntry(VoucherEntry entry) {
      return new VoucherEntryDto {
        Id = entry.Id,
        VoucherEntryType = entry.VoucherEntryType,
        LedgerAccount = LedgerMapper.MapAccount(entry.LedgerAccount, entry.Voucher.AccountingDate),
        Sector = entry.HasSector ?
                 LedgerMapper.MapSectorRule(entry.SectorRule) : null,
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
