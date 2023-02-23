/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : External Data                                Component : Web Api                               *
*  Assembly : Empiria.FinancialAccounting.WebApi.dll       Pattern   : Query Controller                      *
*  Type     : ExternalVariablesController                  License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Web API used to retrive and edit external variables.                                           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Web.Http;

using Empiria.WebApi;

using Empiria.FinancialAccounting.ExternalData.Adapters;
using Empiria.FinancialAccounting.ExternalData.UseCases;

namespace Empiria.FinancialAccounting.WebApi.ExternalData {

  /// <summary>Web API used to retrive and edit external variables.</summary>
  public class ExternalVariablesController : WebApiController {

    #region Web Apis

    [HttpGet]
    [Route("v2/financial-accounting/financial-concepts/external-variables-sets")]
    public CollectionModel GetExternalVariablesSets() {

      using (var usecases = ExternalVariablesUseCases.UseCaseInteractor()) {
        FixedList<ExternalVariablesSetDto> sets = usecases.GetVariablesSets();

        return new CollectionModel(Request, sets);
      }
    }


    [HttpGet]
    [Route("v2/financial-accounting/financial-concepts/" +
           "external-variables-sets/{setUID:guid}/variables")]
    public CollectionModel GetExternalVariables([FromUri] string setUID,
                                                [FromUri] DateTime? date = null) {
      if (!date.HasValue) {
        date = DateTime.Today;
      }

      using (var usecases = ExternalVariablesUseCases.UseCaseInteractor()) {
        FixedList<ExternalVariableDto> variables = usecases.GetVariables(setUID, date.Value);

        return new CollectionModel(Request, variables);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/financial-concepts/" +
           "external-variables-sets/{setUID:guid}/variables")]
    public SingleObjectModel AddVariable([FromUri] string setUID,
                                         [FromBody] ExternalVariableFields fields) {

      RequireBody(fields);

      using (var usecases = ExternalVariablesUseCases.UseCaseInteractor()) {
        ExternalVariableDto dto = usecases.AddVariable(setUID, fields);

        return new SingleObjectModel(Request, dto);
      }
    }


    [HttpDelete]
    [Route("v2/financial-accounting/financial-concepts/" +
           "external-variables-sets/{setUID:guid}/variables/{variableUID:guid}")]
    public NoDataModel RemoveVariable([FromUri] string setUID,
                                      [FromUri] string variableUID) {

      using (var usecases = ExternalVariablesUseCases.UseCaseInteractor()) {
        usecases.RemoveVariable(setUID, variableUID);

        return new NoDataModel(Request);
      }
    }


    [HttpPut, HttpPatch]
    [Route("v2/financial-accounting/financial-concepts/" +
           "external-variables-sets/{setUID:guid}/variables/{variableUID:guid}")]
    public SingleObjectModel UpdateVariable([FromUri] string setUID,
                                            [FromUri] string variableUID,
                                            [FromBody] ExternalVariableFields fields) {
      RequireBody(fields);

      using (var usecases = ExternalVariablesUseCases.UseCaseInteractor()) {
        ExternalVariableDto dto = usecases.UpdateVariable(setUID, variableUID, fields);

        return new SingleObjectModel(Request, dto);
      }
    }


    #endregion Web Apis

  }  // class ExternalVariablesController

}  // namespace Empiria.FinancialAccounting.WebApi.ExternalData
