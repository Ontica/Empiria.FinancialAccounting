/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Cash Ledger                                  Component : Web Api                               *
*  Assembly : Empiria.FinancialAccounting.WebApi.dll       Pattern   : Web Api Controller                    *
*  Type     : CashLedgerController                         License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Web API used to retrive and update cash ledger transactions.                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System.Web.Http;

using Empiria.WebApi;

using Empiria.FinancialAccounting.CashLedger.Adapters;
using Empiria.FinancialAccounting.CashLedger.UseCases;

namespace Empiria.FinancialAccounting.WebApi.CashLedger {

  /// <summary>Web API used to retrive and update cash ledger transactions.</summary>
  public class CashLedgerController : WebApiController {

    #region Query web apis

    [HttpGet]
    [Route("v2/financial-accounting/cash-ledger/transactions/{id:long}")]
    public SingleObjectModel GetCashTransaction([FromUri] long id) {

      using (var usecases = CashLedgerUseCases.UseCaseInteractor()) {
        CashTransactionHolderDto transaction = usecases.GetTransaction(id);

        return new SingleObjectModel(base.Request, transaction);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/cash-ledger/entries/search")]
    public CollectionModel SearchCashEntries([FromBody] CashLedgerQuery query) {

      using (var usecases = CashLedgerUseCases.UseCaseInteractor()) {
        FixedList<CashEntryDescriptor> entries = usecases.SearchEntries(query);

        return new CollectionModel(base.Request, entries);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/cash-ledger/transactions/search")]
    public CollectionModel SearchCashLedgerTransactions([FromBody] CashLedgerQuery query) {

      using (var usecases = CashLedgerUseCases.UseCaseInteractor()) {
        FixedList<CashTransactionDescriptor> transactions = usecases.SearchTransactions(query);

        return new CollectionModel(base.Request, transactions);
      }
    }

    #endregion Query web apis

    #region Command web apis

    [HttpPost]
    [Route("v2/financial-accounting/cash-ledger/transactions/{id:long}/execute-command")]
    public SingleObjectModel ExecuteCommand([FromUri] long id, [FromBody] CashEntriesCommand command) {

      using (var usecases = CashLedgerUseCases.UseCaseInteractor()) {
        CashTransactionHolderDto transaction = usecases.ExecuteCommand(id, command);

        return new SingleObjectModel(base.Request, transaction);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/cash-ledger/transactions/{id:long}/update-entries")]
    public SingleObjectModel UpdateEntries([FromUri] long id, [FromBody] CashTransactionHolderDto transactionDto) {

      using (var usecases = CashLedgerUseCases.UseCaseInteractor()) {
        CashTransactionHolderDto transaction = usecases.UpdateEntries(id, transactionDto);

        return new SingleObjectModel(base.Request, transaction);
      }
    }

    #endregion Command web apis

  }  // class CashLedgerController

}  // namespace Empiria.FinancialAccounting.WebApi.CashLedger
