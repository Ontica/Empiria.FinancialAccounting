/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Cash Ledger                                Component : Adapters Layer                          *
*  Assembly : FinancialAccounting.CashLedger.dll         Pattern   : Mapper                                  *
*  Type     : CashTransactionMapper                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides mapping services for cash ledger transactions.                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.FinancialAccounting.CashLedger.Adapters {

  /// <summary>Provides mapping services for cash ledger transactions.</summary>
  static internal class CashTransactionMapper {

    static internal CashTransactionHolderDto Map(CashTransaction transaction) {
      return new CashTransactionHolderDto {
        Transaction = MapToDescriptor(transaction),
        Entries = MapEntries(transaction.GetEntries()),
      };
    }


    static internal FixedList<CashTransactionDescriptor> MapToDescriptor(FixedList<CashTransaction> list) {
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
        SubledgerAccountNumber = entry.HasSubledgerAccount ? entry.SubledgerAccount.Number : string.Empty,
        SubledgerAccountName = entry.HasSubledgerAccount ? entry.SubledgerAccount.Name : string.Empty,
        SectorCode = entry.Sector.Code,
        CurrencyName = entry.Currency.ShortName,
        Credit = entry.Credit,
        Debit = entry.Debit,
        ExchangeRate = entry.ExchangeRate,
        ResponsibilityAreaName = entry.ResponsibilityArea.Name,
        VerificationNumber = entry.VerificationNumber,
        CashAccountId = entry.CashAccountId
      };
    }


    static private CashTransactionDescriptor MapToDescriptor(CashTransaction txn) {
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
         SourceName = txn.FunctionalArea.FullName,
         StatusName = txn.StatusName
      };
    }

    #endregion Helpers

  }  // class CashTransactionMapper

}  // namespace Empiria.FinancialAccounting.CashLedger.Adapters
