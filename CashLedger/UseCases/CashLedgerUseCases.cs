/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Cash Ledger                                Component : Use cases Layer                         *
*  Assembly : FinancialAccounting.CashLedger.dll         Pattern   : Use case interactor class               *
*  Type     : CashLedgerUseCases                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases used to retrive and manage cash ledger transactions.                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.Services;

using Empiria.FinancialAccounting.CashLedger.Adapters;
using Empiria.FinancialAccounting.CashLedger.Data;

namespace Empiria.FinancialAccounting.CashLedger.UseCases {

  /// <summary>Use cases used to retrive and manage cash ledger transactions.</summary>
  public class CashLedgerUseCases : UseCase {

    #region Constructors and parsers

    protected CashLedgerUseCases() {
      // no-op
    }

    static public CashLedgerUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<CashLedgerUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases

    public FixedList<CashEntryDescriptor> GetEntries(FixedList<long> transactionsIds) {
      Assertion.Require(transactionsIds, nameof(transactionsIds));
      Assertion.Require(transactionsIds.Count > 0, nameof(transactionsIds));

      FixedList<CashEntryExtended> list = CashLedgerData.GetEntries(transactionsIds);

      return CashTransactionMapper.MapToDescriptor(list);
    }


    public CashTransactionHolderDto GetTransaction(long transactionId, bool returnLegacySystemData) {
      Assertion.Require(transactionId > 0, nameof(transactionId));

      CashTransaction transaction = CashLedgerData.GetTransaction(transactionId);

      return CashTransactionMapper.Map(transaction);
    }


    public FixedList<CashTransactionHolderDto> GetTransactions(FixedList<long> transactionsIds,
                                                               bool returnLegacySystemData) {

      Assertion.Require(transactionsIds, nameof(transactionsIds));
      Assertion.Require(transactionsIds.Count > 0, nameof(transactionsIds));

      FixedList<CashTransaction> list = CashLedgerData.GetTransactions(transactionsIds);

      return CashTransactionMapper.Map(list);
    }


    public FixedList<CashEntryDescriptor> SearchEntries(CashLedgerQuery query) {
      Assertion.Require(query, nameof(query));

      query.SearchEntries = true;
      query.EnsureIsValid();

      string filter = query.MapToFilterString();
      string sort = query.MapToSortString();
      int pageSize = query.CalculatePageSize();

      FixedList<CashEntryExtended> list = CashLedgerData.SearchEntries(filter, sort, pageSize);

      return CashTransactionMapper.MapToDescriptor(list);
    }


    public FixedList<CashTransactionDescriptor> SearchTransactions(CashLedgerQuery query) {
      Assertion.Require(query, nameof(query));

      query.EnsureIsValid();

      string filter = query.MapToFilterString();
      string sort = query.MapToSortString();
      int pageSize = query.CalculatePageSize();

      FixedList<CashTransaction> list = CashLedgerData.SearchTransactions(filter, sort, pageSize);

      return CashTransactionMapper.MapToDescriptor(list);
    }


    public void UpdateBulkEntries(FixedList<CashEntryFields> entries) {
      Assertion.Require(entries, nameof(entries));
      Assertion.Require(entries.Count > 0, nameof(entries));

      CashLedgerData.UpdateCashEntriesAccounts(entries);
    }


    public CashTransactionHolderDto UpdateEntries(long transactionId, FixedList<CashEntryFields> entries) {
      Assertion.Require(entries, nameof(entries));
      Assertion.Require(entries.Count > 0, nameof(entries));

      CashTransaction transaction = CashLedgerData.GetTransaction(transactionId);

      CashLedgerData.UpdateCashEntriesAccounts(entries);

      return CashTransactionMapper.Map(transaction);
    }

    #endregion Use cases

  }  // class CashLedgerUseCases

}  // namespace Empiria.FinancialAccounting.CashLedger.UseCases
