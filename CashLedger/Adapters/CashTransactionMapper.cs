/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Cash Ledger                                Component : Adapters Layer                          *
*  Assembly : FinancialAccounting.CashLedger.dll         Pattern   : Mapper                                  *
*  Type     : CashTransactionMapper                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides mapping services for cash ledger transactions.                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.StateEnums;

namespace Empiria.FinancialAccounting.CashLedger.Adapters {

  /// <summary>Provides mapping services for cash ledger transactions.</summary>
  static internal class CashTransactionMapper {

    static internal CashTransactionHolderDto Map(CashTransaction transaction) {
      return new CashTransactionHolderDto {
        Transaction = MapToDescriptor(transaction),
        Entries = MapEntries(transaction.GetEntries()),
      };
    }


    static internal FixedList<CashTransactionHolderDto> Map(FixedList<CashTransaction> transactions) {
      return transactions.Select(x => Map(x))
                         .ToFixedList();
    }


    static internal FixedList<CashTransactionDescriptor> MapToDescriptor(FixedList<CashTransaction> list) {
      return list.Select(x => MapToDescriptor(x))
                 .ToFixedList();
    }


    static internal FixedList<CashEntryDescriptor> MapToDescriptor(FixedList<CashEntryExtended> list) {
      return list.Select(x => MapToDescriptor(x))
                 .ToFixedList();
    }

    #region Helpers

    static private FixedList<CashTransactionEntryDto> MapEntries(FixedList<CashEntry> entries) {
      return entries.Select(x => MapEntry(x))
                    .ToFixedList();
    }


    static private CashTransactionEntryDto MapEntry(CashEntry entry) {
      return new CashTransactionEntryDto {
        Id = entry.Id,
        AccountNumber = entry.LedgerAccount.Number,
        AccountName = entry.LedgerAccount.Name,
        ParentAccountFullName = entry.LedgerAccount.ParentFullName,
        SubledgerAccountNumber = entry.HasSubledgerAccount ? entry.SubledgerAccount.Number : string.Empty,
        SubledgerAccountName = entry.HasSubledgerAccount ? entry.SubledgerAccount.Name : string.Empty,
        SectorCode = entry.Sector.Code,
        CurrencyId = entry.Currency.Id,
        CurrencyName = entry.Currency.ISOCode,
        Credit = entry.Credit,
        Debit = entry.Debit,
        ExchangeRate = entry.ExchangeRate,
        ResponsibilityAreaCode = entry.ResponsibilityArea.Code,
        ResponsibilityAreaName = entry.ResponsibilityArea.Name,
        VerificationNumber = entry.VerificationNumber,
        BudgetCode = entry.BudgetCode,
        Date = ExecutionServer.IsMinOrMaxDate(entry.Date) ? System.DateTime.Today : entry.Date,
        Description = entry.Description,
        CashAccountId = entry.CashAccountId,
        CashAccountNo = entry.CashAccountNo,
        CashAccountAppliedRule = entry.CashAccountAppliedRule,
        CashAccountRecordedById = entry.CashAccountRecordedById,
        CashAccountRecordingTime = ExecutionServer.IsMinOrMaxDate(entry.CashAccountRecordingTime) ? System.DateTime.Today : entry.CashAccountRecordingTime,
        CuentaSistemaLegado = entry.CuentaSistemaLegado
      };
    }


    static private CashEntryDescriptor MapToDescriptor(CashEntryExtended entry) {

      return new CashEntryDescriptor {
        Id = entry.Id,
        AccountNumber = entry.LedgerAccount.Number,
        AccountName = entry.LedgerAccount.Name,
        ParentAccountFullName = entry.LedgerAccount.ParentFullName,
        SubledgerAccountNumber = entry.HasSubledgerAccount ? entry.SubledgerAccount.Number : string.Empty,
        SubledgerAccountName = entry.HasSubledgerAccount ? entry.SubledgerAccount.Name : string.Empty,
        SectorCode = entry.Sector.Code,
        CurrencyId = entry.Currency.Id,
        CurrencyName = entry.Currency.ISOCode,
        Credit = entry.Credit,
        Debit = entry.Debit,
        ExchangeRate = entry.ExchangeRate,
        ResponsibilityAreaCode = entry.ResponsibilityArea.Code,
        ResponsibilityAreaName = entry.ResponsibilityArea.Name,
        VerificationNumber = entry.VerificationNumber,
        BudgetCode = entry.BudgetCode,
        Date = ExecutionServer.IsMinOrMaxDate(entry.Date) ? System.DateTime.Today : entry.Date,
        Description = entry.Description,
        CashAccountId = entry.CashAccountId,
        TransactionId = entry.TransactionId,
        TransactionNumber = entry.TransactionNumber,
        TransactionConcept = entry.TransactionConcept,
        TransactionAccountingDate = entry.AccountingDate,
        TransactionRecordingDate = entry.RecordingDate,
        TransactionLedgerName = entry.Ledger.FullName
      };
    }


    static private CashTransactionDescriptor MapToDescriptor(CashTransaction txn) {
      TransactionStatus status = TransactionStatus.Pending;

      return new CashTransactionDescriptor {
        Id = txn.Id,
        Number = txn.Number,
        Concept = txn.Concept,
        AccountingDate = txn.AccountingDate,
        RecordingDate = txn.RecordingDate,
        LedgerName = txn.Ledger.FullName,
        TransactionTypeName = txn.TransactionType.Name,
        VoucherTypeName = txn.VoucherType.Name,
        ElaboratedBy = txn.ElaboratedBy.Name,
        AuthorizedBy = txn.AuthorizedBy.Name,
        SourceName = txn.FunctionalArea.FullName,
        Status = status.ToString(),
        StatusName = status.GetName()
      };
    }

    #endregion Helpers

  }  // class CashTransactionMapper

}  // namespace Empiria.FinancialAccounting.CashLedger.Adapters
