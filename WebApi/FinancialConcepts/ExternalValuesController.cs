/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Concepts                           Component : Web Api                               *
*  Assembly : Empiria.FinancialAccounting.WebApi.dll       Pattern   : Query Controller                      *
*  Type     : ExternalValuesController                     License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Web API used to retrive and upload external variables values.                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Web.Http;

using Empiria.WebApi;

using Empiria.FinancialAccounting.FinancialConcepts.Adapters;
using Empiria.FinancialAccounting.FinancialConcepts.UseCases;

namespace Empiria.FinancialAccounting.WebApi.FinancialConcepts {

  /// <summary>Web API used to retrive and upload external variables.</summary>
  public class ExternalValuesController : WebApiController {

    #region Web Apis

    [HttpPost]
    [Route("v2/financial-accounting/financial-concepts/external-values")]
    public SingleObjectModel GetExternalValues([FromBody] ExternalValuesQuery query) {

      base.RequireBody(query);

      using (var usecases = ExternalValuesUseCases.UseCaseInteractor()) {
        ExternalValuesDto result = usecases.GetExternalValues(query);

        return new SingleObjectModel(base.Request, result);
      }
    }

    #endregion Web Apis

  }  // class ExternalValuesController

}  // namespace Empiria.FinancialAccounting.WebApi.FinancialConcepts
