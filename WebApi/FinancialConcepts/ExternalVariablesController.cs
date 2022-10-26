/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Concepts                           Component : Web Api                               *
*  Assembly : Empiria.FinancialAccounting.WebApi.dll       Pattern   : Query Controller                      *
*  Type     : ExternalVariablesController                  License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Web API used to retrive and edit external variables.                                           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Web.Http;

using Empiria.WebApi;

using Empiria.FinancialAccounting.FinancialConcepts.Adapters;
using Empiria.FinancialAccounting.FinancialConcepts.UseCases;

namespace Empiria.FinancialAccounting.WebApi.FinancialConcepts {

  /// <summary>Web API used to retrive and edit external variables.</summary>
  public class ExternalVariablesController : WebApiController {

    #region Web Apis

    [HttpGet]
    [Route("v2/financial-accounting/financial-concepts/external-variables-sets")]
    public CollectionModel GetExternalVariablesSets() {

      using (var usecases = ExternalVariablesUseCases.UseCaseInteractor()) {
        FixedList<ExternalVariablesSetDto> sets = usecases.GetExternalVariablesSets();

        return new CollectionModel(base.Request, sets);
      }
    }


    [HttpGet]
    [Route("v2/financial-accounting/financial-concepts/external-variables-sets/{setUID:guid}")]
    public CollectionModel GetExternalVariables([FromUri] string setUID) {

      using (var usecases = ExternalVariablesUseCases.UseCaseInteractor()) {
        FixedList<ExternalVariableDto> variables = usecases.GetExternalVariables(setUID);

        return new CollectionModel(base.Request, variables);
      }
    }

    #endregion Web Apis

  }  // class ExternalVariablesController

}  // namespace Empiria.FinancialAccounting.WebApi.FinancialConcepts
