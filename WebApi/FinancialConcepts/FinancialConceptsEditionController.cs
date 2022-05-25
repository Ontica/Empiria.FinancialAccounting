/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Concepts                           Component : Web Api                               *
*  Assembly : Empiria.FinancialAccounting.WebApi.dll       Pattern   : Command Controller                    *
*  Type     : FinancialConceptsEditionController           License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Command web API used to edit financial concepts.                                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Web.Http;

using Empiria.WebApi;

using Empiria.FinancialAccounting.FinancialConcepts.UseCases;
using Empiria.FinancialAccounting.FinancialConcepts.Adapters;

namespace Empiria.FinancialAccounting.WebApi.FinancialConcepts {

  /// <summary>Command web API used to edit financial concepts.</summary>
  public class FinancialConceptsEditionController : WebApiController {

    #region Web Apis

    [HttpGet]
    [Route("v2/financial-accounting/financial-concepts/{financialConceptUID:guid}")]
    public SingleObjectModel GetFinancialConcept([FromUri] string financialConceptUID) {

      using (var usecases = FinancialConceptsUseCases.UseCaseInteractor()) {
        FinancialConceptDto concept = usecases.GetFinancialConcept(financialConceptUID);

        return new SingleObjectModel(base.Request, concept);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/financial-concepts")]
    public SingleObjectModel InsertFinancialConcept([FromBody] FinancialConceptEditionCommand command) {

      base.RequireBody(command);

      using (var usecases = FinancialConceptsUseCases.UseCaseInteractor()) {
        FinancialConceptDto concept = usecases.InsertFinancialConcept(command);

        return new SingleObjectModel(base.Request, concept);
      }
    }


    [HttpDelete]
    [Route("v2/financial-accounting/financial-concepts/{financialConceptUID:guid}")]
    public CollectionModel RemoveFinancialConcept([FromUri] string financialConceptUID) {

      using (var usecases = FinancialConceptsUseCases.UseCaseInteractor()) {
        FixedList<FinancialConceptDescriptorDto> concepts = usecases.RemoveFinancialConcept(financialConceptUID);

        return new CollectionModel(base.Request, concepts);
      }
    }


    [HttpPut, HttpPatch]
    [Route("v2/financial-accounting/financial-concepts/{financialConceptUID:guid}")]
    public SingleObjectModel UpdateFinancialConcept([FromUri] string financialConceptUID,
                                                    [FromBody] FinancialConceptEditionCommand command) {

      base.RequireBody(command);

      Assertion.Assert(financialConceptUID == command.FinancialConceptUID,
                       "command.FinancialConceptUID does not match url.");

      using (var usecases = FinancialConceptsUseCases.UseCaseInteractor()) {
        FinancialConceptDto concept = usecases.UpdateFinancialConcept(command);

        return new SingleObjectModel(base.Request, concept);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/financial-concepts/clean-up")]
    public NoDataModel CleanupAllRules() {
      using (var usecases = FinancialConceptsGroupUseCases.UseCaseInteractor()) {

        var groups = usecases.GetFinancialConceptsGroups(AccountsChart.IFRS.UID);

        foreach (var group in groups) {
          usecases.CleanupFinancialConceptGroup(group.UID);
        }

        return new NoDataModel(base.Request);
      }
    }

    #endregion Web Apis

  }  // class FinancialConceptsEditionController

}  // namespace Empiria.FinancialAccounting.WebApi.FinancialConcepts
