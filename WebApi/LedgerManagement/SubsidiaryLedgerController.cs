/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Ledger Management                            Component : Web Api                               *
*  Assembly : Empiria.FinancialAccounting.WebApi.dll       Pattern   : Query Controller                      *
*  Type     : SubsidiaryLedgerController                   License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Query web API used to get information about subsidiary ledger books and subsidiary accounts.   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Web.Http;

using Empiria.WebApi;

using Empiria.FinancialAccounting.UseCases;
using Empiria.FinancialAccounting.Adapters;

namespace Empiria.FinancialAccounting.WebApi {

  /// <summary>Query web API used to get information about subsidiary ledger
  /// books and subsidiary accounts.</summary>
  public class SubsidiaryLedgerController : WebApiController {

    #region Web Apis


    [HttpGet]
    [Route("v2/financial-accounting/subsidiary-ledgers/{subledgerUID:guid}")]
    public SingleObjectModel GetSubledger([FromUri] string subledgerUID) {

      using (var usecases = SubsidiaryLedgerUseCases.UseCaseInteractor()) {
        SubsidiaryLedgerDto subledger = usecases.GetSubsidiaryLedger(subledgerUID);

        return new SingleObjectModel(base.Request, subledger);
      }
    }


    [HttpGet]
    [Route("v2/financial-accounting/subsidiary-ledgers/{subledgerUID:guid}" +
           "/accounts/{subledgerAccountId:int}")]
    public SingleObjectModel GetSubledgerAccount([FromUri] string subledgerUID,
                                                 [FromUri] int subledgerAccountId) {

      using (var usecases = SubsidiaryLedgerUseCases.UseCaseInteractor()) {
        SubsidiaryAccountDto subledgerAccount = usecases.GetSubsidiaryAccount(subledgerUID,
                                                                              subledgerAccountId);

        return new SingleObjectModel(base.Request, subledgerAccount);
      }
    }


    [HttpGet, AllowAnonymous]
    [Route("v2/financial-accounting/{accountsChartUID:guid}/subsidiary-accounts/{keywords}")]
    public CollectionModel SearchSubledgerAccounts([FromUri] string accountsChartUID,
                                                   [FromUri] string keywords) {

      using (var usecases = SubsidiaryLedgerUseCases.UseCaseInteractor()) {
        FixedList<SubsidiaryAccountDto> list = usecases.SearchSubsidiaryAccounts(accountsChartUID, keywords);

        return new CollectionModel(base.Request, list);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/subsidiary-ledgers/{subledgerUID:guid}/accounts")]
    public SingleObjectModel CreateSubledgerAccount([FromUri] string subledgerUID,
                                                    [FromUri] SubledgerAccountFields fields) {

      using (var usecases = SubsidiaryLedgerUseCases.UseCaseInteractor()) {
        SubsidiaryAccountDto subledgerAccount = usecases.CreateSubledgerAccount(subledgerUID,
                                                                                fields);

        return new SingleObjectModel(base.Request, subledgerAccount);
      }
    }

    #endregion Web Apis

  }  // class SubsidiaryLedgerController

}  // namespace Empiria.FinancialAccounting.WebApi
