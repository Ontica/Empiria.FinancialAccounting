/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reconciliation Services                      Component : Web Api                               *
*  Assembly : Empiria.FinancialAccounting.WebApi.dll       Pattern   : Query Controller                      *
*  Type     : ReconciliationTypesController                License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Query web API used to get information about accounts balances reconcilation types.             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Web.Http;

using Empiria.WebApi;

using Empiria.FinancialAccounting.Reconciliation.UseCases;
using Empiria.FinancialAccounting.Reconciliation.Adapters;

namespace Empiria.FinancialAccounting.WebApi.Reconciliation {

  /// <summary>Query web API used to get information about accounts balances reconcilation types.</summary>
  public class ReconciliationTypesController : WebApiController {

    #region Web Apis

    [HttpGet]
    [Route("v2/financial-accounting/reconciliation/reconciliation-types")]
    public CollectionModel GetReconciliationTypes() {

      using (var usecases = ReconciliationTypesUseCases.UseCaseInteractor()) {
        FixedList<NamedEntityDto> types = usecases.GetReconciliationTypes();

        return new CollectionModel(base.Request, types);
      }
    }

    #endregion Web Apis

  }  // class ReconciliationTypesController

}  // namespace Empiria.FinancialAccounting.WebApi.Reconciliation
