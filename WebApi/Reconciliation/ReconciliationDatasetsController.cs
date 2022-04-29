/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reconciliation Services                      Component : Web Api                               *
*  Assembly : Empiria.FinancialAccounting.WebApi.dll       Pattern   : Controller                            *
*  Type     : ReconciliationDatasetsController             License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Web API used to retrive and set reconciliation data sets.                                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.Http;

using Empiria.Json;
using Empiria.WebApi;

using Empiria.FinancialAccounting.Reconciliation.UseCases;
using Empiria.FinancialAccounting.Datasets.Adapters;
using Empiria.FinancialAccounting.Reconciliation.Adapters;

namespace Empiria.FinancialAccounting.WebApi.Reconciliation {

  /// <summary>Web API used to retrive and set reconciliation data sets.</summary>
  public class ReconciliationDatasetsController : WebApiController {

    #region Web Apis

    [HttpGet]
    [Route("v2/financial-accounting/reconciliation/datasets/{datasetUID:guid}")]
    public SingleObjectModel GetDataset([FromUri] string datasetUID) {

      using (var usecases = ReconciliationDatasetsUseCases.UseCaseInteractor()) {
        DatasetDto dataset = usecases.GetDataset(datasetUID);

        return new SingleObjectModel(base.Request, dataset);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/reconciliation/datasets")]
    public SingleObjectModel GetDatasetsLoadStatus([FromBody] ReconciliationDatasetsCommand command) {

      using (var usecases = ReconciliationDatasetsUseCases.UseCaseInteractor()) {
        DatasetsLoadStatusDto loadStatus = usecases.GetDatasetsLoadStatus(command);

        return new SingleObjectModel(base.Request, loadStatus);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/reconciliation/datasets/import-from-file")]
    public SingleObjectModel ImportDatasetFromFile() {

      HttpRequest httpRequest = GetValidatedHttpRequest();

      FileData excelFile = GetFileDataFromRequest(httpRequest);

      ReconciliationDatasetsCommand command = GetDatasetsCommandFromRequest(httpRequest);

      using (var usecases = ReconciliationDatasetsUseCases.UseCaseInteractor()) {
        DatasetsLoadStatusDto datasets = usecases.ImportDatasetFromFile(command, excelFile);

        return new SingleObjectModel(base.Request, datasets);
      }
    }


    [HttpDelete]
    [Route("v2/financial-accounting/reconciliation/datasets/{datasetUID:guid}")]
    public SingleObjectModel RemoveDataset([FromUri] string datasetUID) {

      using (var usecases = ReconciliationDatasetsUseCases.UseCaseInteractor()) {
        DatasetsLoadStatusDto datasets = usecases.RemoveDataset(datasetUID);

        return new SingleObjectModel(base.Request, datasets);
      }
    }

    #endregion Web Apis

    #region Helper methods

    static private FileData GetFileDataFromRequest(HttpRequest httpRequest) {
      HttpPostedFile file = httpRequest.Files[0];

      return new FileData() {
        InputStream = httpRequest.Files[0].InputStream,

        MediaType = file.ContentType,
        MediaLength = file.ContentLength,
        OriginalFileName = file.FileName,
      };
    }


    private ReconciliationDatasetsCommand GetDatasetsCommandFromRequest(HttpRequest httpRequest) {
      NameValueCollection form = httpRequest.Form;

      Assertion.AssertObject(form["command"], "'command' form field is required");

      var command = new ReconciliationDatasetsCommand();

      return JsonConverter.Merge(form["command"], command);
    }


    static private HttpRequest GetValidatedHttpRequest() {
      var httpRequest = HttpContext.Current.Request;

      Assertion.AssertObject(httpRequest, "httpRequest");
      Assertion.Assert(httpRequest.Files.Count == 1,
                       "The request does not have the dataset file to be imported.");

      var form = httpRequest.Form;

      Assertion.AssertObject(form, "The request must be of type 'multipart/form-data'.");

      return httpRequest;
    }

    #endregion Helper methods

  }  // class ReconciliationDatasetsController

}  // namespace Empiria.FinancialAccounting.WebApi.Reconciliation
