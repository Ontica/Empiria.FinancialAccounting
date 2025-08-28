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

    public async Task<T> GetTransaction<T>(long id) {

      string path = $"v2/financial-accounting/cash-ledger/transactions/{id}";

      return await WebApiClient.GetAsync<T>(path);
    }


    public async Task<FileDto> GetTransactionAsPdfFile(long id) {

      string path = $"v2/financial-accounting/vouchers/{id}/print";

      return await WebApiClient.GetAsync<FileDto>(path);
    }


    public async Task<FixedList<T>> GetTransactions<T>(FixedList<long> transactionIds) {

      string path = $"v2/financial-accounting/cash-ledger/transactions/bulk-operation/get-transactions";

      return await WebApiClient.PostAsync<FixedList<T>>(transactionIds, path);
    }


    public async Task<FixedList<CashEntryDescriptor>> GetTransactionsEntries(FixedList<long> entriesIds) {

      string path = $"v2/financial-accounting/cash-ledger/entries/bulk-operation/get-entries";

      return await WebApiClient.PostAsync<FixedList<CashEntryDescriptor>>(entriesIds, path);
    }


    public async Task<FixedList<CashEntryDescriptor>> SearchEntries(BaseCashLedgerQuery query) {

      string path = "v2/financial-accounting/cash-ledger/entries/search";

      return await WebApiClient.PostAsync<FixedList<CashEntryDescriptor>>(query, path);
    }


    public async Task<FixedList<CashTransactionDescriptor>> SearchTransactions(BaseCashLedgerQuery query) {

      string path = "v2/financial-accounting/cash-ledger/transactions/search";

      return await WebApiClient.PostAsync<FixedList<CashTransactionDescriptor>>(query, path);
    }


    public async Task UpdateBulkEntries(FixedList<CashEntryFields> bulkEntries) {

      string path = $"v2/financial-accounting/cash-ledger/transactions/bulk-operation/update-entries";

      await WebApiClient.PostAsync(bulkEntries, path);
    }


    public async Task<T> UpdateEntries<T>(FixedList<CashEntryFields> entries) {

      string path = $"v2/financial-accounting/cash-ledger/transactions/{entries[0].TransactionId}/update-entries";

      return await WebApiClient.PostAsync<T>(entries, path);
    }

  }  // class CashTransactionServices

}  // namespace Empiria.FinancialAccounting.ClientServices
