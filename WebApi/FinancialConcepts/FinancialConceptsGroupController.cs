/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Concepts                           Component : Web Api                               *
*  Assembly : Empiria.FinancialAccounting.WebApi.dll       Pattern   : Query Controller                      *
*  Type     : FinancialConceptsGroupController             License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Query web API used to retrive financial concepts in groups.                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Web.Http;

using Empiria.WebApi;

using Empiria.FinancialAccounting.FinancialConcepts.UseCases;
using Empiria.FinancialAccounting.FinancialConcepts.Adapters;

using Empiria.FinancialAccounting.Reporting;

namespace Empiria.FinancialAccounting.WebApi.FinancialConcepts {

  /// <summary>Query web API used to retrive financial concepts in groups.</summary>
  public class FinancialConceptsGroupController : WebApiController {

    #region Web Apis

    [HttpGet]
    [Route("v2/financial-accounting/financial-concepts/groups")]
    public CollectionModel GetFinancialConceptsGroups() {

      using (var usecases = FinancialConceptsGroupUseCases.UseCaseInteractor()) {
        FixedList<FinancialConceptsGroupDto> groups = usecases.GetFinancialConceptsGroups();

        return new CollectionModel(base.Request, groups);
      }
    }


    [HttpGet]
    [Route("v2/financial-accounting/financial-concepts/in-group/{groupUID:guid}")]
    public CollectionModel GetFinancialConceptsInGroup([FromUri] string groupUID,
                                                       [FromUri] DateTime? date = null) {
      if (!date.HasValue) {
        date = DateTime.Today;
      }

      using (var usecases = FinancialConceptsGroupUseCases.UseCaseInteractor()) {
        FixedList<FinancialConceptDescriptorDto> concepts =
                                usecases.GetFinancialConceptsInGroup(groupUID, date.Value);

        return new CollectionModel(base.Request, concepts);
      }
    }


    [HttpGet]
    [Route("v2/financial-accounting/financial-concepts/groups/{groupUID:guid}/flat-tree")]
    public CollectionModel GetGroupIntegrationEntriesAsTree([FromUri] string groupUID) {

      using (var usecases = FinancialConceptsGroupUseCases.UseCaseInteractor()) {
        FixedList<FinancialConceptEntryAsTreeNodeDto> treeNodes =
                            usecases.GetGroupIntegrationEntriesAsTree(groupUID);

        return new CollectionModel(base.Request, treeNodes);
      }
    }


    [HttpGet]
    [Route("v2/financial-accounting/financial-concepts/groups/{groupUID:guid}/excel")]
    [Route("v2/financial-accounting/financial-concepts/groups/{groupUID:guid}/flat-tree/excel")]
    public SingleObjectModel ExportGroupIntegrationEntriesAsTree([FromUri] string groupUID) {

      using (var usecases = FinancialConceptsGroupUseCases.UseCaseInteractor()) {
        FixedList<FinancialConceptEntryAsTreeNodeDto> treeNodes =
                             usecases.GetGroupIntegrationEntriesAsTree(groupUID);

        var excelExporter = new ExcelExporterService();

        FileReportDto excelFileDto = excelExporter.Export(treeNodes);

        return new SingleObjectModel(this.Request, excelFileDto);
      }
    }


    #endregion Web Apis

  }  // class FinancialConceptsController

}  // namespace Empiria.FinancialAccounting.WebApi.FinancialConcepts
