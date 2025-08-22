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

    public FixedList<CashEntryDescriptor> GetEntries(FixedList<long> ids) {
      Assertion.Require(ids, nameof(ids));
      Assertion.Require(ids.Count > 0, nameof(ids));

      FixedList<CashEntryExtended> list = CashLedgerData.GetEntries(ids);

      return CashTransactionMapper.MapToDescriptor(list);
    }


    public CashTransactionHolderDto GetTransaction(long id, bool returnLegacySystemData) {
      Assertion.Require(id > 0, nameof(id));

      CashTransaction transaction = CashLedgerData.GetTransaction(id);

      CashTransactionHolderDto mapped = CashTransactionMapper.Map(transaction);

      if (!returnLegacySystemData) {
        return mapped;
      }

      var legacyEntries = SistemaLegadoData.LeerMovimientos(id);
      var merger = new SistemaLegadoMerger(mapped.Entries, legacyEntries);

      merger.Merge();

      return mapped;
    }


    public FixedList<CashTransactionHolderDto> GetTransactions(FixedList<long> ids,
                                                               bool returnLegacySystemData) {

      Assertion.Require(ids, nameof(ids));
      Assertion.Require(ids.Count > 0, nameof(ids));

      FixedList<CashTransaction> list = CashLedgerData.GetTransactions(ids);

      FixedList<CashTransactionHolderDto> mapped = CashTransactionMapper.Map(list);

      if (!returnLegacySystemData) {
        return mapped;
      }

      var allLegacyEntries = SistemaLegadoData.LeerMovimientos(ids);

      foreach (var txn in mapped) {
        var legacyTxnEntries = allLegacyEntries.FindAll(x => x.IdPoliza == txn.Transaction.Id)
                                               .Sort((x, y) => x.IdConsecutivo.CompareTo(y.IdConsecutivo));

        var merger = new SistemaLegadoMerger(txn.Entries, legacyTxnEntries);

        merger.Merge();
      }

      return mapped;
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

      CashLedgerData.WriteCashEntriesAccounts(entries);
    }


    public CashTransactionHolderDto UpdateEntries(long transactionId, FixedList<CashEntryFields> entries) {
      Assertion.Require(entries, nameof(entries));
      Assertion.Require(entries.Count > 0, nameof(entries));

      CashTransaction transaction = CashLedgerData.GetTransaction(transactionId);

      CashLedgerData.WriteCashEntriesAccounts(entries);

      return CashTransactionMapper.Map(transaction);
    }

    #endregion Use cases

  }  // class CashLedgerUseCases

}  // namespace Empiria.FinancialAccounting.CashLedger.UseCases
