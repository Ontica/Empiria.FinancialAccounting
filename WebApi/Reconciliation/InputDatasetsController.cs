/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reconciliation Services                      Component : Web Api                               *
*  Assembly : Empiria.FinancialAccounting.WebApi.dll       Pattern   : Controller                            *
*  Type     : InputDatasetsController                      License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Web API used to retrive and set reconciliation input data sets.                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.Http;

using Empiria.Json;
using Empiria.WebApi;

using Empiria.FinancialAccounting.Datasets.UseCases;
using Empiria.FinancialAccounting.Datasets.Adapters;

namespace Empiria.FinancialAccounting.WebApi.Reconciliation {

  /// <summary>Web API used to retrive and set reconciliation input data sets.</summary>
  public class InputDatasetsController : WebApiController {

    #region Web Apis

    [HttpGet]
    [Route("v2/financial-accounting/reconciliation/input-datasets/{inputDatasetUID:guid}")]
    public SingleObjectModel GetInputDataset([FromUri] string inputDatasetUID) {

      using (var usecases = InputDatasetsUseCases.UseCaseInteractor()) {
        InputDatasetDto dataset = usecases.GetDataset(inputDatasetUID);

        return new SingleObjectModel(base.Request, dataset);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/reconciliation/input-datasets")]
    public SingleObjectModel GetInputDatasetsList([FromBody] GetInputDatasetsCommand command) {

      using (var usecases = InputDatasetsUseCases.UseCaseInteractor()) {
         ReconciliationDatasetsDto datasets = usecases.GetDatasetsList(command);

        return new SingleObjectModel(base.Request, datasets);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/reconciliation/input-datasets/import-from-file")]
    public SingleObjectModel ImportInputDatasetFile() {

      HttpRequest httpRequest = GetValidatedHttpRequest();

      FileData excelFile = GetFileDataFromRequest(httpRequest);

      StoreInputDatasetCommand command = GetStoreInputDataSetCommandFromRequest(httpRequest);

      using (var usecases = InputDatasetsUseCases.UseCaseInteractor()) {
        ReconciliationDatasetsDto datasets = usecases.ImportDatasetFromFile(command, excelFile);

        return new SingleObjectModel(base.Request, datasets);
      }
    }


    [HttpDelete]
    [Route("v2/financial-accounting/reconciliation/input-datasets/{inputDatasetUID:guid}")]
    public SingleObjectModel RemoveInputDataset([FromUri] string inputDatasetUID) {

      using (var usecases = InputDatasetsUseCases.UseCaseInteractor()) {
        ReconciliationDatasetsDto datasets = usecases.RemoveDataset(inputDatasetUID);

        return new SingleObjectModel(base.Request, datasets);
      }
    }

    #endregion Web Apis

    #region Helper methods

    static private FileData GetFileDataFromRequest(HttpRequest httpRequest) {
      HttpPostedFile file = httpRequest.Files[0];

      var fileData = new FileData();

      fileData.InputStream = httpRequest.Files[0].InputStream;

      fileData.MediaType = file.ContentType;
      fileData.MediaLength = file.ContentLength;
      fileData.OriginalFileName = file.FileName;

      return fileData;
    }


    private StoreInputDatasetCommand GetStoreInputDataSetCommandFromRequest(HttpRequest httpRequest) {
      NameValueCollection form = httpRequest.Form;

      Assertion.AssertObject(form["command"], "'command' form field is required");

      var command = new StoreInputDatasetCommand();

      return JsonConverter.Merge(form["command"], command);
    }


    static private HttpRequest GetValidatedHttpRequest() {
      var httpRequest = HttpContext.Current.Request;

      Assertion.AssertObject(httpRequest, "httpRequest");
      Assertion.Assert(httpRequest.Files.Count == 1, "The request does not have the reconcilation file to be imported.");

      var form = httpRequest.Form;

      Assertion.AssertObject(form, "The request must be of type 'multipart/form-data'.");

      return httpRequest;
    }

    #endregion Helper methods

  }  // class InputDatasetsController

}  // namespace Empiria.FinancialAccounting.WebApi.Reconciliation
