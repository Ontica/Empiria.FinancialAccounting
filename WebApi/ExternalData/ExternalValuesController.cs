/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : External Data                                Component : Web Api                               *
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

using Empiria.DynamicData.Datasets.Adapters;
using Empiria.DynamicData.ExternalData.Adapters;

using Empiria.DynamicData.ExternalData.UseCases;

using Empiria.FinancialAccounting.Reporting;

namespace Empiria.FinancialAccounting.WebApi.ExternalData {

  /// <summary>Web API used to retrive and upload external variables.</summary>
  public class ExternalValuesController : WebApiController {

    #region Web Apis

    [HttpPost]
    [Route("v2/financial-accounting/financial-concepts/external-values")]
    public SingleObjectModel GetExternalValues([FromBody] ExternalValuesQuery query) {

      base.RequireBody(query);

      using (var usecases = ExternalValuesUseCases.UseCaseInteractor()) {
        ExternalValuesDto result = usecases.GetExternalValues(query);

        return new SingleObjectModel(Request, result);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/financial-concepts/external-values/export")]
    public SingleObjectModel ExportExternalValuesToExcel([FromBody] ExternalValuesQuery query) {

      base.RequireBody(query);

      using (var usecases = ExternalValuesUseCases.UseCaseInteractor()) {
        ExternalValuesDto result = usecases.GetExternalValues(query);

        var excelExporter = new ExcelExporterService();

        FileReportDto exportedFile = excelExporter.Export(result);

        return new SingleObjectModel(Request, exportedFile);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/financial-concepts/external-values/datasets")]
    public SingleObjectModel GetExternalValuesDatasetsLoadStatus([FromBody] ExternalValuesDatasetDto dto) {

      using (var usecases = ExternalValuesUseCases.UseCaseInteractor()) {
        DatasetsLoadStatusDto loadStatus = usecases.GetDatasetsLoadStatus(dto);

        return new SingleObjectModel(Request, loadStatus);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/financial-concepts/external-values/import-from-file")]
    public SingleObjectModel ImportExternalValuesDatasetFromFile() {

      ExternalValuesDatasetDto dto = GetFormDataFromHttpRequest<ExternalValuesDatasetDto>("command");

      InputFile excelFile = base.GetInputFileFromHttpRequest(dto.DatasetKind);

      using (var usecases = ExternalValuesUseCases.UseCaseInteractor()) {
        DatasetsLoadStatusDto datasets = usecases.CreateDataset(dto, excelFile);

        return new SingleObjectModel(Request, datasets);
      }
    }


    [HttpDelete]
    [Route("v2/financial-accounting/financial-concepts/external-values/datasets/{datasetUID:guid}")]
    public SingleObjectModel RemoveExternalValuesDataset([FromUri] string datasetUID) {

      using (var usecases = ExternalValuesUseCases.UseCaseInteractor()) {
        DatasetsLoadStatusDto datasets = usecases.RemoveDataset(datasetUID);

        return new SingleObjectModel(Request, datasets);
      }
    }

    #endregion Web Apis

  }  // class ExternalValuesController

}  // namespace Empiria.FinancialAccounting.WebApi.ExternalData
