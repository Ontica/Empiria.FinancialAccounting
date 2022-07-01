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
using System.Collections.Specialized;
using System.Web;
using System.Web.Http;

using Empiria.Json;
using Empiria.Storage;
using Empiria.WebApi;

using Empiria.FinancialAccounting.Reconciliation.UseCases;
using Empiria.FinancialAccounting.Datasets.Adapters;
using Empiria.FinancialAccounting.Reconciliation.Adapters;

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

      HttpRequest httpRequest = GetValidatedHttpRequest();

      InputFile excelFile = base.GetInputFileFromHttpRequest();

      OperationalDataDto dto = BuildOperationalDataDtoFromRequest(httpRequest);

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

    #region Helper methods


    private OperationalDataDto BuildOperationalDataDtoFromRequest(HttpRequest httpRequest) {
      NameValueCollection form = httpRequest.Form;

      Assertion.Require(form["command"], "'command' form field is required");

      var command = new OperationalDataDto();

      return JsonConverter.Merge(form["command"], command);
    }


    static private HttpRequest GetValidatedHttpRequest() {
      var httpRequest = HttpContext.Current.Request;

      Assertion.Require(httpRequest, "httpRequest");
      Assertion.Require(httpRequest.Files.Count == 1,
                       "The request does not have the dataset file to be imported.");

      var form = httpRequest.Form;

      Assertion.Require(form, "The request must be of type 'multipart/form-data'.");

      return httpRequest;
    }

    #endregion Helper methods

  }  // class OperationalDataController

}  // namespace Empiria.FinancialAccounting.WebApi.Reconciliation
