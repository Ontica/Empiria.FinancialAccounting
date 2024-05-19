/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reconciliation Services                      Component : Web Api                               *
*  Assembly : Empiria.FinancialAccounting.WebApi.dll       Pattern   : Controller                            *
*  Type     : OperationalDataController                    License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Web API used to retrive and set operational data for reconciliation processes.                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Web.Http;

using Empiria.Storage;
using Empiria.WebApi;

using Empiria.DynamicData.Datasets.Adapters;

using Empiria.FinancialAccounting.Reconciliation.Adapters;
using Empiria.FinancialAccounting.Reconciliation.UseCases;

namespace Empiria.FinancialAccounting.WebApi.Reconciliation {

  /// <summary>Web API used to retrive and set operational data for reconciliation processes.</summary>
  public class OperationalDataController : WebApiController {

    #region Web Apis

    [HttpGet]
    [Route("v2/financial-accounting/reconciliation/datasets/{datasetUID:guid}")]
    public SingleObjectModel GetDataset([FromUri] string datasetUID) {

      using (var usecases = OperationalDataUseCases.UseCaseInteractor()) {
        DatasetOutputDto dataset = usecases.GetDataset(datasetUID);

        return new SingleObjectModel(base.Request, dataset);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/reconciliation/datasets")]
    public SingleObjectModel GetDatasetsLoadStatus([FromBody] OperationalDataDto dto) {

      using (var usecases = OperationalDataUseCases.UseCaseInteractor()) {
        DatasetsLoadStatusDto loadStatus = usecases.GetDatasetsLoadStatus(dto);

        return new SingleObjectModel(base.Request, loadStatus);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/reconciliation/datasets/import-from-file")]
    public SingleObjectModel ImportDatasetFromFile() {

      OperationalDataDto dto = base.GetFormDataFromHttpRequest<OperationalDataDto>("command");

      InputFile excelFile = base.GetInputFileFromHttpRequest(dto.DatasetKind);

      using (var usecases = OperationalDataUseCases.UseCaseInteractor()) {
        DatasetsLoadStatusDto datasets = usecases.CreateDataset(dto, excelFile);

        return new SingleObjectModel(base.Request, datasets);
      }
    }


    [HttpDelete]
    [Route("v2/financial-accounting/reconciliation/datasets/{datasetUID:guid}")]
    public SingleObjectModel RemoveDataset([FromUri] string datasetUID) {

      using (var usecases = OperationalDataUseCases.UseCaseInteractor()) {
        DatasetsLoadStatusDto datasets = usecases.RemoveDataset(datasetUID);

        return new SingleObjectModel(base.Request, datasets);
      }
    }

    #endregion Web Apis

  }  // class OperationalDataController

}  // namespace Empiria.FinancialAccounting.WebApi.Reconciliation
