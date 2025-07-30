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

    static internal FixedList<CashTransactionDescriptor> MapToDescriptor(FixedList<CashTransaction> list) {
      return list.Select(x => MapToDescriptor(x))
                 .ToFixedList();
    }

    #region Helpers

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
         StageName = txn.StageName
      };
    }

    #endregion Helpers

  }  // class CashTransactionMapper

}  // namespace Empiria.FinancialAccounting.CashLedger.Adapters
