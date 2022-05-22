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
    [Route("v2/financial-accounting/financial-concepts/in-group/{groupUID:guid}")]
    public CollectionModel GeFinancialConceptsInGroup([FromUri] string groupUID) {

      using (var usecases = FinancialConceptsUseCases.UseCaseInteractor()) {
        FixedList<FinancialConceptDto> concepts = usecases.FinancialConceptsInGroup(groupUID);

        return new CollectionModel(base.Request, concepts);
      }
    }


    [HttpGet]
    [Route("v2/financial-accounting/rules/grouping-rules/{groupUID:guid}/flat-tree")]
    public CollectionModel GetGroupingRulesFlatTree([FromUri] string groupUID) {

      using (var usecases = FinancialConceptsUseCases.UseCaseInteractor()) {
        FixedList<GroupingRulesTreeItemDto> rulesTreeItems = usecases.GroupingRulesFlatTree(groupUID);

        return new CollectionModel(base.Request, rulesTreeItems);
      }
    }


    [HttpGet]
    [Route("v2/financial-accounting/rules/grouping-rules/{groupUID:guid}/excel")]
    [Route("v2/financial-accounting/rules/grouping-rules/{groupUID:guid}/flat-tree/excel")]
    public SingleObjectModel ExportGroupingRulesFlatTreeToExcel([FromUri] string groupUID) {

      using (var usecases = FinancialConceptsUseCases.UseCaseInteractor()) {
        FixedList<GroupingRulesTreeItemDto> rulesTreeItems = usecases.GroupingRulesFlatTree(groupUID);

        var excelExporter = new ExcelExporterService();

        FileReportDto excelFileDto = excelExporter.Export(rulesTreeItems);

        return new SingleObjectModel(this.Request, excelFileDto);
      }
    }


    [HttpGet]
    [Route("v2/financial-accounting/rules/grouping-rule-items/{financialConceptUID:guid}")]
    public CollectionModel GetGroupingRuleItems([FromUri] string financialConceptUID) {

      using (var usecases = FinancialConceptsUseCases.UseCaseInteractor()) {
        FixedList<GroupingRuleItemDto> rules = usecases.GroupingRuleItems(financialConceptUID);

        return new CollectionModel(base.Request, rules);
      }
    }


    [HttpGet]
    [Route("v2/financial-accounting/financial-concepts/groups/{accountsChartUID:guid}")]
    public CollectionModel GetFinancialConceptsGroups([FromUri] string accountsChartUID) {

      using (var usecases = FinancialConceptsUseCases.UseCaseInteractor()) {
        FixedList<NamedEntityDto> rules = usecases.FinancialConceptsGroups(accountsChartUID);

        return new CollectionModel(base.Request, rules);
      }
    }


    #endregion Web Apis

  }  // class FinancialConceptsController

}  // namespace Empiria.FinancialAccounting.WebApi.FinancialConcepts
