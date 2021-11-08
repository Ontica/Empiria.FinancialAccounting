/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Ledger Management                            Component : Web Api                               *
*  Assembly : Empiria.FinancialAccounting.WebApi.dll       Pattern   : Query Controller                      *
*  Type     : SubledgerController                          License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Query web API used to get information about subledger books and subledger accounts.            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Web.Http;

using Empiria.WebApi;

using Empiria.FinancialAccounting.UseCases;
using Empiria.FinancialAccounting.Adapters;

namespace Empiria.FinancialAccounting.WebApi {

  /// <summary>Query web API used to get information about subledger books and subledger accounts.</summary>
  public class SubledgerController : WebApiController {

    #region Web Apis


    [HttpGet]
    [Route("v2/financial-accounting/subledgers/{subledgerUID:guid}")]
    public SingleObjectModel GetSubledger([FromUri] string subledgerUID) {

      using (var usecases = SubledgerUseCases.UseCaseInteractor()) {
        SubledgerDto subledger = usecases.GetSubledger(subledgerUID);

        return new SingleObjectModel(base.Request, subledger);
      }
    }


    [HttpGet]
    [Route("v2/financial-accounting/subledgers/{subledgerUID:guid}" +
           "/accounts/{subledgerAccountId:int}")]
    public SingleObjectModel GetSubledgerAccount([FromUri] string subledgerUID,
                                                 [FromUri] int subledgerAccountId) {

      using (var usecases = SubledgerUseCases.UseCaseInteractor()) {
        SubledgerAccountDto subledgerAccount = usecases.GetSubledgerAccount(subledgerUID,
                                                                              subledgerAccountId);

        return new SingleObjectModel(base.Request, subledgerAccount);
      }
    }


    [HttpPost, AllowAnonymous]
    [Route("v2/financial-accounting/{accountsChartUID:guid}/subledger-accounts")]
    public CollectionModel SearchSubledgerAccountsWithCommand([FromUri] string accountsChartUID,
                                                              [FromBody] SearchSubledgerAccountCommand command) {

      using (var usecases = SubledgerUseCases.UseCaseInteractor()) {
        FixedList<SubledgerAccountDto> list = usecases.SearchSubledgerAccounts(accountsChartUID, command);

        return new CollectionModel(base.Request, list);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/subledgers/{subledgerUID:guid}/accounts")]
    public SingleObjectModel CreateSubledgerAccount([FromUri] string subledgerUID,
                                                    [FromUri] SubledgerAccountFields fields) {

      using (var usecases = SubledgerUseCases.UseCaseInteractor()) {
        SubledgerAccountDto subledgerAccount = usecases.CreateSubledgerAccount(subledgerUID, fields);

        return new SingleObjectModel(base.Request, subledgerAccount);
      }
    }

    #endregion Web Apis

  }  // class SubledgerController

}  // namespace Empiria.FinancialAccounting.WebApi
