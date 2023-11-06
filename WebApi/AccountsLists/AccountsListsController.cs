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

using Empiria.FinancialAccounting.AccountsLists.UseCases;
using Empiria.FinancialAccounting.AccountsLists.Adapters;
using Empiria.FinancialAccounting.AccountsLists.SpecialCases;

namespace Empiria.FinancialAccounting.WebApi {

  /// <summary>Query web API used to get information about accounts lists.</summary>
  public class AccountsListsController : WebApiController {

    #region Query Web Apis

    [HttpGet]
    [Route("v2/financial-accounting/accounts-lists-for-edition/{accountsListUID}")]
    public SingleObjectModel GetEditableAccountsList([FromUri] string accountsListUID,
                                                     [FromUri] string keywords = "") {

      using (var usecases = AccountsListsUseCases.UseCaseInteractor()) {
        AccountsListDto list = usecases.GetEditableAccountsList(accountsListUID, keywords);

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

    [HttpPost]
    [Route("v2/financial-accounting/accounts-lists-for-edition/set-keywords")]
    public NoDataModel SetKeywords() {

      using (var usecases = AccountsListsUseCases.UseCaseInteractor()) {
        usecases.SetKeywords();

        return new NoDataModel(base.Request);
      }
    }

    #endregion Query Web Apis

    #region Conciliación de derivados

    [HttpPost]
    [Route("v2/financial-accounting/accounts-lists-for-edition/ConciliacionDerivados")]
    public SingleObjectModel AddConciliacionDerivadosListItem([FromBody] ConciliacionDerivadosListItemFields fields) {

      RequireBody(fields);

      using (var usecases = AccountsListsUseCases.UseCaseInteractor()) {
        ConciliacionDerivadosListItemDto item = usecases.AddConciliacionDerivadosListItem(fields);

        return new SingleObjectModel(base.Request, item);
      }
    }


    [HttpDelete]
    [Route("v2/financial-accounting/accounts-lists-for-edition/ConciliacionDerivados/{itemUID:guid}")]
    public NoDataModel RemoveConciliacionDerivadosListItem([FromUri] string itemUID,
                                                           [FromBody] ConciliacionDerivadosListItemFields fields) {
      RequireBody(fields);

      Assertion.Require(itemUID == fields.UID, "Unrecognized list item UID.");

      using (var usecases = AccountsListsUseCases.UseCaseInteractor()) {
        usecases.RemoveConciliacionDerivadosListItem(fields);

        return new NoDataModel(base.Request);
      }
    }


    [HttpPut, HttpPatch]
    [Route("v2/financial-accounting/accounts-lists-for-edition/ConciliacionDerivados/{itemUID:guid}")]
    public SingleObjectModel UpdateConciliacionDerivadosListItem([FromUri] string itemUID,
                                                                 [FromBody] ConciliacionDerivadosListItemFields fields) {
      RequireBody(fields);

      Assertion.Require(itemUID == fields.UID, "Unrecognized list item UID.");

      using (var usecases = AccountsListsUseCases.UseCaseInteractor()) {
        ConciliacionDerivadosListItemDto item = usecases.UpdateConciliacionDerivadosListItem(fields);

        return new SingleObjectModel(base.Request, item);
      }
    }

    #endregion Conciliación de derivados

    #region Depreciación activo fijo

    [HttpPost]
    [Route("v2/financial-accounting/accounts-lists-for-edition/DepreciacionActivoFijo")]
    public SingleObjectModel AddDepreciacionActivoFijoListItem([FromBody] DepreciacionActivoFijoListItemFields fields) {

      RequireBody(fields);

      using (var usecases = AccountsListsUseCases.UseCaseInteractor()) {
        DepreciacionActivoFijoListItemDto item = usecases.AddDepreciacionActivoFijoListItem(fields);

        return new SingleObjectModel(base.Request, item);
      }
    }


    [HttpDelete]
    [Route("v2/financial-accounting/accounts-lists-for-edition/DepreciacionActivoFijo/{itemUID:guid}")]
    public NoDataModel RemoveDepreciacionActivoFijoListItem([FromUri] string itemUID,
                                                            [FromBody] DepreciacionActivoFijoListItemFields fields) {

      RequireBody(fields);

      Assertion.Require(itemUID == fields.UID, "Unrecognized list item UID.");

      using (var usecases = AccountsListsUseCases.UseCaseInteractor()) {
        usecases.RemoveDepreciacionActivoFijoListItem(fields);

        return new NoDataModel(base.Request);
      }
    }


    [HttpPut, HttpPatch]
    [Route("v2/financial-accounting/accounts-lists-for-edition/DepreciacionActivoFijo/{itemUID:guid}")]
    public SingleObjectModel UpdateDepreciacionActivoFijoListItem([FromUri] string itemUID,
                                                                  [FromBody] DepreciacionActivoFijoListItemFields fields) {

      RequireBody(fields);

      Assertion.Require(itemUID == fields.UID, "Unrecognized list item UID.");

      using (var usecases = AccountsListsUseCases.UseCaseInteractor()) {
        DepreciacionActivoFijoListItemDto item = usecases.UpdateDepreciacionActivoFijoListItem(fields);

        return new SingleObjectModel(base.Request, item);
      }
    }

    #endregion Depreciación activo fijo

    #region Swaps Cobertura

    [HttpGet]
    [Route("v2/financial-accounting/accounts-lists-for-edition/SwapsCobertura/classifications")]
    public CollectionModel GetSwapsCoberturaClassifications() {

      using (var usecases = AccountsListsUseCases.UseCaseInteractor()) {
        FixedList<string> classifications = usecases.SwapsCoberturaClassifications();

        return new CollectionModel(base.Request, classifications);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/accounts-lists-for-edition/SwapsCobertura")]
    public SingleObjectModel AddSwapsCoberturaListItem([FromBody] SwapsCoberturaListItemFields fields) {

      RequireBody(fields);

      using (var usecases = AccountsListsUseCases.UseCaseInteractor()) {
        SwapsCoberturaListItemDto item = usecases.AddSwapsCoberturaListItem(fields);

        return new SingleObjectModel(base.Request, item);
      }
    }


    [HttpDelete]
    [Route("v2/financial-accounting/accounts-lists-for-edition/SwapsCobertura/{itemUID:guid}")]
    public NoDataModel RemoveSwapsCoberturaListItem([FromUri] string itemUID,
                                                    [FromBody] SwapsCoberturaListItemFields fields) {

      RequireBody(fields);

      Assertion.Require(itemUID == fields.UID, "Unrecognized list item UID.");

      using (var usecases = AccountsListsUseCases.UseCaseInteractor()) {
        usecases.RemoveSwapsCoberturaListItem(fields);

        return new NoDataModel(base.Request);
      }
    }


    [HttpPut, HttpPatch]
    [Route("v2/financial-accounting/accounts-lists-for-edition/SwapsCobertura/{itemUID:guid}")]
    public SingleObjectModel UpdateSwapsCoberturaListItem([FromUri] string itemUID,
                                                          [FromBody] SwapsCoberturaListItemFields fields) {

      RequireBody(fields);

      Assertion.Require(itemUID == fields.UID, "Unrecognized list item UID.");

      using (var usecases = AccountsListsUseCases.UseCaseInteractor()) {
        SwapsCoberturaListItemDto item = usecases.UpdateSwapsCoberturaListItem(fields);

        return new SingleObjectModel(base.Request, item);
      }
    }

    #endregion Swaps Cobertura

    #region Préstamos interbancarios

    [HttpGet]
    [Route("v2/financial-accounting/accounts-lists-for-edition/PrestamosInterbancarios/prestamos")]
    public CollectionModel GetPrestamos() {

      using (var usecases = AccountsListsUseCases.UseCaseInteractor()) {
        FixedList<Prestamo> prestamos = usecases.PrestamosList();

        prestamos.Sort((x, y) => x.Order.CompareTo(y.Order));

        return new CollectionModel(base.Request, prestamos);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/accounts-lists-for-edition/PrestamosInterbancarios")]
    public SingleObjectModel AddPrestamoInterbancarioListItem([FromBody] PrestamosInterbancariosListItemFields fields) {

      RequireBody(fields);

      using (var usecases = AccountsListsUseCases.UseCaseInteractor()) {
        PrestamosInterbancariosListItemDto item = usecases.AddPrestamoInterbancarioListItem(fields);

        return new SingleObjectModel(base.Request, item);
      }
    }


    [HttpDelete]
    [Route("v2/financial-accounting/accounts-lists-for-edition/PrestamosInterbancarios/{itemUID:guid}")]
    public NoDataModel RemovePrestamoInterbancarioListItem([FromUri] string itemUID,
                                                           [FromBody] PrestamosInterbancariosListItemFields fields) {

      RequireBody(fields);

      Assertion.Require(itemUID == fields.UID, "Unrecognized list item UID.");

      using (var usecases = AccountsListsUseCases.UseCaseInteractor()) {
        usecases.RemovePrestamoInterbancarioListItem(fields);

        return new NoDataModel(base.Request);
      }
    }


    [HttpPut, HttpPatch]
    [Route("v2/financial-accounting/accounts-lists-for-edition/PrestamosInterbancarios/{itemUID:guid}")]
    public SingleObjectModel UpdatePrestamoInterbancarioListItem([FromUri] string itemUID,
                                                                 [FromBody] PrestamosInterbancariosListItemFields fields) {

      RequireBody(fields);

      Assertion.Require(itemUID == fields.UID, "Unrecognized list item UID.");

      using (var usecases = AccountsListsUseCases.UseCaseInteractor()) {
        PrestamosInterbancariosListItemDto item = usecases.UpdatePrestamoInterbancarioListItem(fields);

        return new SingleObjectModel(base.Request, item);
      }
    }

    #endregion Préstamos interbancarios

  }  // class AccountsListsController

}  // namespace Empiria.FinancialAccounting.WebApi.AccountsLists
