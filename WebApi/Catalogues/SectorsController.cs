/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Catalogues                                   Component : Web Api                               *
*  Assembly : Empiria.FinancialAccounting.WebApi.dll       Pattern   : Controller                            *
*  Type     : SectorsController                            License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Web apis used to get information about sectors.                                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Web.Http;

using Empiria.WebApi;

using Empiria.FinancialAccounting.UseCases;

namespace Empiria.FinancialAccounting.WebApi {

  /// <summary>Web apis used to get information about sectors.</summary>
  public class SectorsController : WebApiController {

    #region Web Api

    [HttpGet]
    [Route("v2/financial-accounting/catalogues/sectors")]
    public CollectionModel GetSectors() {

      using (var usecases = SectorsUseCases.UseCaseInteractor()) {
        FixedList<NamedEntityDto> sectors = usecases.GetSectors();

        return new CollectionModel(base.Request, sectors);
      }
    }

    #endregion Web Api

  }  // class SectorsController

}  // namespace Empiria.FinancialAccounting.WebApi
