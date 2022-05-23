/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Concepts                           Component : Web Api                               *
*  Assembly : Empiria.FinancialAccounting.WebApi.dll       Pattern   : Query Controller                      *
*  Type     : FinancialConceptsController                  License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Query web API used to retrive financial concepts.                                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Web.Http;

using Empiria.WebApi;

using Empiria.FinancialAccounting.FinancialConcepts.UseCases;
using Empiria.FinancialAccounting.FinancialConcepts.Adapters;

using Empiria.FinancialAccounting.Reporting;

namespace Empiria.FinancialAccounting.WebApi.FinancialConcepts {

  /// <summary>Query web API used to retrive financial concepts.</summary>
  public class FinancialConceptsController : WebApiController {

    #region Web Apis

    [HttpGet]
    [Route("v2/financial-accounting/financial-concepts/{financialConceptUID:guid}")]
    public SingleObjectModel GetFinancialConcept([FromUri] string financialConceptUID) {

      using (var usecases = FinancialConceptsUseCases.UseCaseInteractor()) {
        FinancialConceptDto concept = usecases.GetFinancialConcept(financialConceptUID);

        return new SingleObjectModel(base.Request, concept);
      }
    }


    [HttpGet]
    [Route("v2/financial-accounting/financial-concepts/in-group/{groupUID:guid}")]
    public CollectionModel GetFinancialConceptsInGroup([FromUri] string groupUID) {

      using (var usecases = FinancialConceptsUseCases.UseCaseInteractor()) {
        FixedList<FinancialConceptDescriptorDto> concepts = usecases.GetGroupFinancialConcepts(groupUID);

        return new CollectionModel(base.Request, concepts);
      }
    }


    [HttpGet]
    [Route("v2/financial-accounting/financial-concepts/groups/{groupUID:guid}/flat-tree")]
    public CollectionModel GetGroupFinancialConceptsEntriesAsTree([FromUri] string groupUID) {

      using (var usecases = FinancialConceptsUseCases.UseCaseInteractor()) {
        FixedList<FinancialConceptEntryAsTreeNodeDto> treeNodes =
                            usecases.GetGroupFinancialConceptsEntriesAsTree(groupUID);

        return new CollectionModel(base.Request, treeNodes);
      }
    }


    [HttpGet]
    [Route("v2/financial-accounting/financial-concepts/groups/{groupUID:guid}/excel")]
    [Route("v2/financial-accounting/financial-concepts/groups/{groupUID:guid}/flat-tree/excel")]
    public SingleObjectModel ExportGroupFinancialConceptsEntriesAsTree([FromUri] string groupUID) {

      using (var usecases = FinancialConceptsUseCases.UseCaseInteractor()) {
        FixedList<FinancialConceptEntryAsTreeNodeDto> treeNodes =
                             usecases.GetGroupFinancialConceptsEntriesAsTree(groupUID);

        var excelExporter = new ExcelExporterService();

        FileReportDto excelFileDto = excelExporter.Export(treeNodes);

        return new SingleObjectModel(this.Request, excelFileDto);
      }
    }


    [HttpGet]
    [Route("v2/financial-accounting/financial-concepts/{financialConceptUID:guid}/integration")]
    public CollectionModel GetFinancialConceptIntegration([FromUri] string financialConceptUID) {

      using (var usecases = FinancialConceptsUseCases.UseCaseInteractor()) {
        FixedList<FinancialConceptEntryDto> integration =
                             usecases.GetFinancialConceptIntegration(financialConceptUID);

        return new CollectionModel(base.Request, integration);
      }
    }


    [HttpGet]
    [Route("v2/financial-accounting/financial-concepts/account-chart-groups/{accountsChartUID:guid}")]
    public CollectionModel GetFinancialConceptsGroups([FromUri] string accountsChartUID) {

      using (var usecases = FinancialConceptsUseCases.UseCaseInteractor()) {
        FixedList<NamedEntityDto> rules = usecases.GetFinancialConceptsGroups(accountsChartUID);

        return new CollectionModel(base.Request, rules);
      }
    }


    #endregion Web Apis

  }  // class FinancialConceptsController

}  // namespace Empiria.FinancialAccounting.WebApi.FinancialConcepts
