/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Catalogues Management                        Component : Web Api                               *
*  Assembly : Empiria.FinancialAccounting.WebApi.dll       Pattern   : Controller                            *
*  Type     : FunctionalAreasController                    License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Web API for functional areas.                                                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System.Web.Http;

using Empiria.WebApi;

using Empiria.FinancialAccounting.UseCases;

namespace Empiria.FinancialAccounting.WebApi {

  /// <summary>Web API for functional areas.</summary>
  public class FunctionalAreasController : WebApiController {

    #region Web Apis

    [HttpGet]
    [Route("v2/financial-accounting/catalogues/functional-areas")]
    public CollectionModel GetFunctionalAreas() {

      using (var usecases = FunctionalAreasUseCases.UseCaseInteractor()) {
        FixedList<NamedEntityDto> list = usecases.FunctionalAreas();

        return new CollectionModel(base.Request, list);
      }
    }

    #endregion Web Apis

  }  // class FunctionalAreasController

}  // namespace Empiria.FinancialAccounting.WebApi
