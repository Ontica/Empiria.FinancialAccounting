/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Ledger Management                            Component : Web Api                               *
*  Assembly : Empiria.FinancialAccounting.WebApi.dll       Pattern   : Query Controller                      *
*  Type     : SubledgerAccountsController                  License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Query web API used to get information about subledger accounts.                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Web.Http;

using Empiria.WebApi;

using Empiria.FinancialAccounting.Adapters;
using Empiria.FinancialAccounting.UseCases;

namespace Empiria.FinancialAccounting.WebApi {

  /// <summary>Query web API used to get information about subledger subledger accounts.</summary>
  public class SubledgerAccountsController : WebApiController {

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
    [Route("v2/financial-accounting/subledger-accounts/{subledgerAccountId:int}")]
    public SingleObjectModel GetSubledgerAccount([FromUri] int subledgerAccountId) {

      using (var usecases = SubledgerUseCases.UseCaseInteractor()) {
        SubledgerAccountDto subledgerAccount = usecases.GetSubledgerAccount(subledgerAccountId);

        return new SingleObjectModel(base.Request, subledgerAccount);
      }
    }


    [HttpPost, AllowAnonymous]
    [Route("v2/financial-accounting/subledger-accounts/search")]
    public CollectionModel SearchSubledgerAccounts([FromBody] SearchSubledgerAccountCommand command) {

      using (var usecases = SubledgerUseCases.UseCaseInteractor()) {
        FixedList<SubledgerAccountDescriptorDto> list = usecases.SearchSubledgerAccounts(command);

        return new CollectionModel(base.Request, list);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/subledger-accounts")]
    public SingleObjectModel CreateSubledgerAccount([FromBody] SubledgerAccountFields fields) {

      using (var usecases = SubledgerUseCases.UseCaseInteractor()) {
        SubledgerAccountDto subledgerAccount = usecases.CreateSubledgerAccount(fields);

        return new SingleObjectModel(base.Request, subledgerAccount);
      }
    }


    [HttpPut, HttpPatch]
    [Route("v2/financial-accounting/subledger-accounts/{subledgerAccountId:int}")]
    public SingleObjectModel UpdateSubledgerAccount([FromUri] int subledgerAccountId,
                                                    [FromBody] SubledgerAccountFields fields) {

      using (var usecases = SubledgerUseCases.UseCaseInteractor()) {
        SubledgerAccountDto subledgerAccount = usecases.UpdateSubledgerAccount(subledgerAccountId, fields);

        return new SingleObjectModel(base.Request, subledgerAccount);
      }
    }

    #endregion Web Apis

  }  // class SubledgerAccountsController

}  // namespace Empiria.FinancialAccounting.WebApi
