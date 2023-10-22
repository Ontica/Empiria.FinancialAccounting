/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Lists                               Component : Web Api                               *
*  Assembly : Empiria.FinancialAccounting.WebApi.dll       Pattern   : Query Controller                      *
*  Type     : AccountsListsController                      License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Query web API used to get information about accounts lists.                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Web.Http;

using Empiria.WebApi;

using Empiria.FinancialAccounting.Adapters;
using Empiria.FinancialAccounting.UseCases;

namespace Empiria.FinancialAccounting.WebApi {

  /// <summary>Query web API used to get information about accounts lists.</summary>
  public class AccountsListsController : WebApiController {

    #region Web Apis

    [HttpGet]
    [Route("v2/financial-accounting/accounts-lists-for-edition/{accountsListUID}")]
    public SingleObjectModel GetEditableAccountsList([FromUri] string accountsListUID) {

      using (var usecases = AccountsListsUseCases.UseCaseInteractor()) {
        AccountsListDto list = usecases.GetEditableAccountsList(accountsListUID);

        return new SingleObjectModel(base.Request, list);
      }
    }


    [HttpGet]
    [Route("v2/financial-accounting/accounts-lists-for-edition")]
    public CollectionModel GetAccountsListsForEdition() {

      using (var usecases = AccountsListsUseCases.UseCaseInteractor()) {
        FixedList<NamedEntityDto> lists = usecases.GetAccountsListsForEdition();

        return new CollectionModel(base.Request, lists);
      }
    }

    #endregion Web Apis

  }  // class AccountsListsController

}  // namespace Empiria.FinancialAccounting.WebApi.AccountsLists
