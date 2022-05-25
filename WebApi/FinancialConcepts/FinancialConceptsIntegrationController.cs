/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Concepts                           Component : Web Api                               *
*  Assembly : Empiria.FinancialAccounting.WebApi.dll       Pattern   : Command Controller                    *
*  Type     : FinancialConceptsIntegrationController       License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Command web API used to edit financial concept's integration entries.                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Web.Http;

using Empiria.WebApi;

using Empiria.FinancialAccounting.FinancialConcepts.UseCases;
using Empiria.FinancialAccounting.FinancialConcepts.Adapters;

namespace Empiria.FinancialAccounting.WebApi.FinancialConcepts {

  /// <summary>Command web API used to edit financial concept's integration entries.</summary>
  public class FinancialConceptsIntegrationController : WebApiController {

    #region Web Apis

    [HttpGet]
    [Route("v2/financial-accounting/financial-concepts/{financialConceptUID:guid}/integration")]
    public CollectionModel GetFinancialConceptEntries([FromUri] string financialConceptUID) {

      using (var usecases = FinancialConceptIntegrationUseCases.UseCaseInteractor()) {
        FixedList<FinancialConceptEntryDto> entries =
                                  usecases.GetFinancialConceptEntries(financialConceptUID);

        return new CollectionModel(base.Request, entries);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/financial-concepts/{financialConceptUID:guid}/integration")]
    public SingleObjectModel InsertFinancialConceptEntry([FromUri] string financialConceptUID,
                                                         [FromBody] FinancialConceptEntryEditionCommand command) {

      base.RequireBody(command);

      Assertion.Assert(financialConceptUID == command.FinancialConceptUID,
                       "command.FinancialConceptUID does not match url.");

      using (var usecases = FinancialConceptIntegrationUseCases.UseCaseInteractor()) {
        FinancialConceptEntryDto entry = usecases.InsertFinancialConceptEntry(command);

        return new SingleObjectModel(base.Request, entry);
      }
    }


    [HttpDelete]
    [Route("v2/financial-accounting/financial-concepts/{financialConceptUID:guid}/integration/{financialConceptEntryUID:guid}")]
    public NoDataModel RemoveFinancialConceptEntry([FromUri] string financialConceptUID,
                                                   [FromUri] string financialConceptEntryUID) {

      using (var usecases = FinancialConceptIntegrationUseCases.UseCaseInteractor()) {
        usecases.RemoveFinancialConceptEntry(financialConceptUID, financialConceptEntryUID);

        return new NoDataModel(base.Request);
      }
    }


    [HttpPut, HttpPatch]
    [Route("v2/financial-accounting/financial-concepts/{financialConceptUID:guid}/integration/{financialConceptEntryUID:guid}")]
    public SingleObjectModel UpdateFinancialConceptEntry([FromUri] string financialConceptUID,
                                                         [FromUri] string financialConceptEntryUID,
                                                         [FromBody] FinancialConceptEntryEditionCommand command) {

      base.RequireBody(command);

      Assertion.Assert(financialConceptUID == command.FinancialConceptUID,
                       "command.FinancialConceptUID does not match url.");

      Assertion.Assert(financialConceptEntryUID == command.FinancialConceptEntryUID,
                 "command.financialConceptEntryUID does not match url.");

      using (var usecases = FinancialConceptIntegrationUseCases.UseCaseInteractor()) {
        FinancialConceptEntryDto entry = usecases.UpdateFinancialConceptEntry(command);

        return new SingleObjectModel(base.Request, entry);
      }
    }

    #endregion Web Apis

  }  // class FinancialConceptsIntegrationController

}  // namespace Empiria.FinancialAccounting.WebApi.FinancialConcepts
