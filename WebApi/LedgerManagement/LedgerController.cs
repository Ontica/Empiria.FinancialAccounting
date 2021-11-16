/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Ledger Management                            Component : Web Api                               *
*  Assembly : Empiria.FinancialAccounting.WebApi.dll       Pattern   : Query Controller                      *
*  Type     : LedgerController                             License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Query web API used to get information about accounting ledger books and their accounts.        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Web.Http;

using Empiria.WebApi;

using Empiria.FinancialAccounting.UseCases;
using Empiria.FinancialAccounting.Adapters;

namespace Empiria.FinancialAccounting.WebApi {

  /// <summary>Query web API used to get information about accounting ledger
  /// books and their accounts.</summary>
  public class LedgerController : WebApiController {

    #region Web Apis


    [HttpGet]
    [Route("v2/financial-accounting/ledgers/{ledgerUID:guid}")]
    public SingleObjectModel GetLedger([FromUri] string ledgerUID) {

      using (var usecases = LedgerUseCases.UseCaseInteractor()) {
        LedgerDto ledger = usecases.GetLedger(ledgerUID);

        return new SingleObjectModel(base.Request, ledger);
      }
    }


    [HttpGet]
    [Route("v2/financial-accounting/ledgers/{ledgerUID:guid}/accounts/{accountId:int}")]
    public SingleObjectModel GetLedgerAccount([FromUri] string ledgerUID,
                                              [FromUri] int accountId) {

      using (var usecases = LedgerUseCases.UseCaseInteractor()) {
        LedgerAccountDto ledgerAccount = usecases.GetLedgerAccount(ledgerUID, accountId);

        return new SingleObjectModel(base.Request, ledgerAccount);
      }
    }


    [HttpGet]
    [Route("v2/financial-accounting/ledgers/{ledgerUID:guid}/subledgers")]
    public CollectionModel GetLedgerSubledgers([FromUri] string ledgerUID) {

      using (var usecases = LedgerUseCases.UseCaseInteractor()) {
        FixedList<SubledgerDto> subledgers = usecases.GetSubledgers(ledgerUID);

        return new CollectionModel(base.Request, subledgers);
      }
    }


    #endregion Web Apis

  }  // class LedgerController

}  // namespace Empiria.FinancialAccounting.WebApi
