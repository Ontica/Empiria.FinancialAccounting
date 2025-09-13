/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Cash Ledger                                Component : Data Layer                              *
*  Assembly : FinancialAccounting.WebApiClient.dll       Pattern   : Web api client                          *
*  Type     : CashTransactionServices                    License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides financial accounting cash ledger transactions services using a web proxy.             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System.Threading.Tasks;

using Empiria.Storage;

using Empiria.CashFlow.CashLedger.Adapters;

namespace Empiria.FinancialAccounting.ClientServices {

  /// <summary>Provides financial accounting cash ledger transactions services using a web proxy.</summary>
  public class CashTransactionServices : BaseService {

    public Task<T> GetTransaction<T>(long id) {

      string path = $"v2/financial-accounting/cash-ledger/transactions/{id}";

      return WebApiClient.GetAsync<T>(path);
    }


    public Task<FileDto> GetTransactionAsPdfFile(long id) {

      string path = $"v2/financial-accounting/vouchers/{id}/print";

      return WebApiClient.GetAsync<FileDto>(path);
    }


    public Task<FixedList<T>> GetTransactions<T>(FixedList<long> transactionIds) {

      string path = $"v2/financial-accounting/cash-ledger/transactions/bulk-operation/get-transactions";

      return WebApiClient.PostAsync<FixedList<T>>(transactionIds, path);
    }


    public Task<FixedList<CashEntryExtendedDto>> GetTransactionsEntries(FixedList<long> entriesIds) {

      string path = $"v2/financial-accounting/cash-ledger/entries/bulk-operation/get-entries";

      return WebApiClient.PostAsync<FixedList<CashEntryExtendedDto>>(entriesIds, path);
    }


    public Task<FixedList<CashEntryExtendedDto>> SearchEntries(BaseCashLedgerQuery query) {

      string path = "v2/financial-accounting/cash-ledger/entries/search";

      return WebApiClient.PostAsync<FixedList<CashEntryExtendedDto>>(query, path);
    }


    public Task<FixedList<CashTransactionDescriptor>> SearchTransactions(BaseCashLedgerQuery query) {

      string path = "v2/financial-accounting/cash-ledger/transactions/search";

      return WebApiClient.PostAsync<FixedList<CashTransactionDescriptor>>(query, path);
    }


    public Task UpdateBulkEntries(FixedList<CashEntryFields> bulkEntries) {

      string path = $"v2/financial-accounting/cash-ledger/transactions/bulk-operation/update-entries";

      return WebApiClient.PostAsync(bulkEntries, path);
    }


    public Task<T> UpdateEntries<T>(FixedList<CashEntryFields> entries) {

      string path = $"v2/financial-accounting/cash-ledger/transactions/{entries[0].TransactionId}/update-entries";

      return WebApiClient.PostAsync<T>(entries, path);
    }

  }  // class CashTransactionServices

}  // namespace Empiria.FinancialAccounting.ClientServices
