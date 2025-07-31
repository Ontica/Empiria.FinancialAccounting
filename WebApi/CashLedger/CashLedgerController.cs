/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Cash Ledger                                  Component : Web Api                               *
*  Assembly : Empiria.FinancialAccounting.WebApi.dll       Pattern   : Query Controller                      *
*  Type     : CashLedgerController                         License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Query web API used to retrive cash ledger transactions.                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System.Web.Http;

using Empiria.WebApi;

using Empiria.FinancialAccounting.CashLedger.Adapters;
using Empiria.FinancialAccounting.CashLedger.UseCases;

namespace Empiria.FinancialAccounting.WebApi.CashLedger {

  /// <summary>Query web API used to retrive cash ledger transactions.</summary>
  public class CashLedgerController : WebApiController {

    #region Web Apis

    [HttpGet]
    [Route("v2/financial-accounting/cash-ledger/transactions/{id:long}")]
    public SingleObjectModel GetCashTransaction([FromUri] long id) {

      using (var usecases = CashLedgerUseCases.UseCaseInteractor()) {
        CashTransactionHolderDto transaction = usecases.GetTransaction(id);

        return new SingleObjectModel(base.Request, transaction);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/cash-ledger/transactions/search")]
    public CollectionModel SearchCashLedgerTransactions([FromBody] CashLedgerQuery query) {
      base.RequireBody(query);

      using (var usecases = CashLedgerUseCases.UseCaseInteractor()) {
        FixedList<CashTransactionDescriptor> transactions = usecases.SearchTransactions(query);

        return new CollectionModel(base.Request, transactions);
      }
    }

    #endregion Web Apis

  }  // class CashLedgerController

}  // namespace Empiria.FinancialAccounting.WebApi.CashLedger
