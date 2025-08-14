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

    public CashTransactionHolderDto ExecuteCommand(long id, CashEntriesCommand command) {
      Assertion.Require(id > 0, nameof(id));
      Assertion.Require(command, nameof(command));

      CashTransaction transaction = CashLedgerData.GetTransaction(id);

      FixedList<CashEntry> entries = command.GetEntries(transaction);

      transaction.SetCashEntryAccount(entries, command.CashAccountId);

      return CashTransactionMapper.Map(transaction);
    }


    public CashTransactionHolderDto GetTransaction(long id) {
      Assertion.Require(id > 0, nameof(id));

      CashTransaction transaction = CashLedgerData.GetTransaction(id);

      return CashTransactionMapper.Map(transaction);
    }


    public FixedList<CashEntryDescriptor> SearchEntries(CashLedgerQuery query) {
      Assertion.Require(query, nameof(query));

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

      FixedList<CashTransaction> list = CashLedgerData.GetTransactions(filter, sort, pageSize);

      return CashTransactionMapper.MapToDescriptor(list);
    }


    public CashTransactionHolderDto UpdateEntries(long id, CashTransactionHolderDto transactionDto) {
      Assertion.Require(id > 0, nameof(id));
      Assertion.Require(transactionDto, nameof(transactionDto));

      CashTransaction transaction = CashLedgerData.GetTransaction(id);

      FixedList<CashEntry> entries = transaction.GetEntries();

      foreach (var updatedEntry in transactionDto.Entries) {
        CashEntry entry = entries.Find(x => x.Id == updatedEntry.Id);

        transaction.SetCashEntryAccount(entry, updatedEntry.CashAccountId);
      }

      return CashTransactionMapper.Map(transaction);
    }

    #endregion Use cases

  }  // class CashLedgerUseCases

}  // namespace Empiria.FinancialAccounting.CashLedger.UseCases
