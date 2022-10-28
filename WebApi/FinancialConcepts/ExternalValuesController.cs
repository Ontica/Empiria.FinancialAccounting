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

using Empiria.Storage;

using Empiria.FinancialAccounting.Datasets.Adapters;

using Empiria.FinancialAccounting.FinancialConcepts.Adapters;
using Empiria.FinancialAccounting.FinancialConcepts.UseCases;

using Empiria.FinancialAccounting.Reporting;

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


    [HttpPost]
    [Route("v2/financial-accounting/financial-concepts/external-values/excel")]
    public SingleObjectModel ExportExternalValuesToExcel([FromBody] ExternalValuesQuery query) {

      base.RequireBody(query);

      using (var usecases = ExternalValuesUseCases.UseCaseInteractor()) {
        ExternalValuesDto result = usecases.GetExternalValues(query);

        var excelExporter = new ExcelExporterService();

        FileReportDto excelFileDto = excelExporter.Export(result);

        return new SingleObjectModel(this.Request, excelFileDto);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/financial-concepts/external-values/datasets")]
    public SingleObjectModel GetExternalValuesDatasetsLoadStatus([FromBody] ExternalValuesDatasetDto dto) {

      using (var usecases = ExternalValuesUseCases.UseCaseInteractor()) {
        DatasetsLoadStatusDto loadStatus = usecases.GetDatasetsLoadStatus(dto);

        return new SingleObjectModel(base.Request, loadStatus);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/financial-concepts/external-values/import-from-file")]
    public SingleObjectModel ImportExternalValuesDatasetFromFile() {

      ExternalValuesDatasetDto dto = base.GetFormDataFromHttpRequest<ExternalValuesDatasetDto>("command");

      InputFile excelFile = base.GetInputFileFromHttpRequest(dto.DatasetKind);

      using (var usecases = ExternalValuesUseCases.UseCaseInteractor()) {
        DatasetsLoadStatusDto datasets = usecases.CreateDataset(dto, excelFile);

        return new SingleObjectModel(base.Request, datasets);
      }
    }


    [HttpDelete]
    [Route("v2/financial-accounting/financial-concepts/external-values/datasets/{datasetUID:guid}")]
    public SingleObjectModel RemoveExternalValuesDataset([FromUri] string datasetUID) {

      using (var usecases = ExternalValuesUseCases.UseCaseInteractor()) {
        DatasetsLoadStatusDto datasets = usecases.RemoveDataset(datasetUID);

        return new SingleObjectModel(base.Request, datasets);
      }
    }

    #endregion Web Apis

  }  // class ExternalValuesController

}  // namespace Empiria.FinancialAccounting.WebApi.FinancialConcepts
