/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Transaction Slips                             Component : Interface adapters                   *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll   Pattern   : Mapper class                         *
*  Type     : TransactionSlipMapper                         License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Mapping methods for transaction slips (volantes).                                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.Vouchers.Adapters;

namespace Empiria.FinancialAccounting.BanobrasIntegration.TransactionSlips.Adapters {

  /// <summary>Mapping methods for transaction slips (volantes).</summary>
  static internal class TransactionSlipMapper {

    #region Public mappers

    static internal FixedList<TransactionSlipDto> Map(FixedList<TransactionSlip> list, bool skipEntries = false) {
      return new FixedList<TransactionSlipDto>(list.Select(x => Map(x, skipEntries)));
    }


    static internal TransactionSlipDto Map(TransactionSlip transactionSlip, bool skipEntries = false) {
      var entries = new FixedList<TransactionSlipEntry>();

      if (!skipEntries) {
        entries = transactionSlip.Entries();
      }

      return new TransactionSlipDto {
        Header = MapToDescriptor(transactionSlip),
        Entries = MapEntries(entries),
        Issues = MapIssues(transactionSlip.Issues()),
        Voucher = TryMapVoucher(transactionSlip)
      };
    }


    static internal FixedList<TransactionSlipDescriptorDto> MapToDescriptors(FixedList<TransactionSlip> list) {
      return new FixedList<TransactionSlipDescriptorDto>(list.Select(x => MapToDescriptor(x)));
    }


    #endregion Public mappers


    #region Helper methods

    static private FixedList<TransactionSlipEntryDto> MapEntries(FixedList<TransactionSlipEntry> entries) {
      return new FixedList<TransactionSlipEntryDto>(entries.Select(x => MapEntry(x)));
    }


    static private TransactionSlipEntryDto MapEntry(TransactionSlipEntry entry) {
      return new TransactionSlipEntryDto {
        UID = entry.UID,
        EntryNumber = entry.EntryNumber,
        AccountNumber = entry.AccountNumber,
        SectorCode = entry.SectorCode,
        SubledgerAccount = entry.SubledgerAccount,
        CurrencyCode = entry.CurrencyCode.ToString("00"),
        FunctionalArea = entry.FunctionalArea,
        VerificationNumber = entry.VerificationNumber.ToString(),
        Description = EmpiriaString.Clean(entry.Description),
        ExchangeRate = entry.ExchangeRate,
        Debit  = entry.Debit,
        Credit = entry.Credit
      };
    }


    static private TransactionSlipIssueDto MapIssue(TransactionSlipIssue issue) {
      return new TransactionSlipIssueDto {
         Description = issue.Description
      };
    }


    static private FixedList<TransactionSlipIssueDto> MapIssues(FixedList<TransactionSlipIssue> issues) {
      return new FixedList<TransactionSlipIssueDto>(issues.Select(x => MapIssue(x)));
    }


    static private TransactionSlipDescriptorDto MapToDescriptor(TransactionSlip slip) {
      return new TransactionSlipDescriptorDto {
         UID = slip.UID,
         SlipNumber = slip.Number.ToString("00000"),
         Concept = EmpiriaString.Clean(slip.Concept),
         AccountingDate = slip.AccountingDate,
         RecordingDate = slip.RecordingDate,
         FunctionalArea = slip.FunctionalArea,
         VerificationNumber = slip.VerificationNumber.ToString(),
         AccountingVoucherId = slip.AccountingVoucherId,
         ElaboratedBy = slip.ElaboratedBy.Trim(),
         ControlTotal = slip.ControlTotal,
         Status = slip.Status,
         StatusName = slip.StatusName,
         AccountsChartId = slip.AccountingChartId,
         ProcessingDate = slip.ProcessingDate,
         SystemId = slip.SystemId
      };
    }


    static private NamedEntityDto TryMapVoucher(TransactionSlip slip) {
      if (!slip.HasVoucher) {
        return null;
      }

      VoucherDto voucher = slip.GetVoucher();

      return new NamedEntityDto(voucher.Id.ToString(), voucher.Number);
    }


    #endregion Helper methods

  }  // class TransactionSlipMapper

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.TransactionSlips.Adapters
