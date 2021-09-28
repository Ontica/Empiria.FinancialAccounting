/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                Component : Voucher Text file importer            *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll  Pattern   : Command Controller                    *
*  Type     : ImportVouchersController                     License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Command API used to import vouchers and voucher entries form DB, Excel and text files.         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.Http;

using Empiria.Json;
using Empiria.WebApi;

using Empiria.FinancialAccounting.Vouchers.Adapters;

using Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter.Adapters;
using Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter.UseCases;

namespace Empiria.FinancialAccounting.WebApi.BanobrasIntegration {

  /// <summary>Command API used to import vouchers and voucher entries form DB, Excel and text files.</summary>
  public class ImportVouchersController : WebApiController {


    #region Database importers


    [HttpPost]
    [Route("v2/financial-accounting/vouchers/import-from-database/dry-run")]
    public SingleObjectModel DryRunImportVouchersFromDatabase([FromBody] ImportVouchersCommand command) {

      base.RequireBody(command);

      using (var usecases = ImportVouchersUseCases.UseCaseInteractor()) {
        ImportVouchersResult result = usecases.DryRunImportVouchersFromDatabase(command);

        return new SingleObjectModel(base.Request, result);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/vouchers/import-from-database")]
    public SingleObjectModel ImportVouchersFromDatabase([FromBody] ImportVouchersCommand command) {

      base.RequireBody(command);

      using (var usecases = ImportVouchersUseCases.UseCaseInteractor()) {
        var result = usecases.ImportVouchersFromDatabase(command);

        return new SingleObjectModel(base.Request, result);
      }
    }


    #endregion Database importers


    #region Excel file importers


    [HttpPost]
    [Route("v2/financial-accounting/vouchers/import-from-excel/dry-run")]
    public SingleObjectModel DryRunImportVouchersFromExcelFile() {

      HttpRequest httpRequest = GetValidatedHttpRequest();

      FileData excelFile = GetFileDataFromRequest(httpRequest);
      ImportVouchersCommand command = GetImportVoucherCommandFromRequest(httpRequest);

      using (var usecases = ImportVouchersUseCases.UseCaseInteractor()) {
        ImportVouchersResult result = usecases.DryRunImportVouchersFromExcelFile(command, excelFile);

        return new SingleObjectModel(base.Request, result);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/vouchers/import-from-excel")]
    public SingleObjectModel ImportVouchersFromExcelFile() {

      HttpRequest httpRequest = GetValidatedHttpRequest();

      FileData excelFile = GetFileDataFromRequest(httpRequest);
      ImportVouchersCommand command = GetImportVoucherCommandFromRequest(httpRequest);

      using (var usecases = ImportVouchersUseCases.UseCaseInteractor()) {
        ImportVouchersResult result = usecases.ImportVouchersFromExcelFile(command, excelFile);

        return new SingleObjectModel(base.Request, result);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/vouchers/{voucherId:int}/entries/import-from-excel/dry-run")]
    public SingleObjectModel DryRunImportVoucherEntriesFromExcelFile([FromUri] int voucherId) {

      HttpRequest httpRequest = GetValidatedHttpRequest();

      FileData excelFile = GetFileDataFromRequest(httpRequest);
      ImportVouchersCommand command = GetImportVoucherCommandFromRequest(httpRequest);

      using (var usecases = ImportVouchersUseCases.UseCaseInteractor()) {
        ImportVouchersResult result = usecases.DryRunImportVoucherEntriesFromExcelFile(voucherId, command, excelFile);

        return new SingleObjectModel(base.Request, result);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/vouchers/{voucherId:int}/entries/import-from-excel")]
    public SingleObjectModel ImportVoucherEntriesFromExcelFile([FromUri] int voucherId) {

      HttpRequest httpRequest = GetValidatedHttpRequest();

      FileData excelFile = GetFileDataFromRequest(httpRequest);
      ImportVouchersCommand command = GetImportVoucherCommandFromRequest(httpRequest);

      using (var usecases = ImportVouchersUseCases.UseCaseInteractor()) {
        ImportVouchersResult result = usecases.ImportVoucherEntriesFromExcelFile(voucherId, command, excelFile);

        return new SingleObjectModel(base.Request, result);
      }
    }


    #endregion Excel file importers


    #region Text file importers


    [HttpPost]
    [Route("v2/financial-accounting/vouchers/import-from-text-file/dry-run")]
    public SingleObjectModel DryRunImportVouchersFromTextFile() {

      HttpRequest httpRequest = GetValidatedHttpRequest();

      FileData textFile = GetFileDataFromRequest(httpRequest);
      ImportVouchersCommand command = GetImportVoucherCommandFromRequest(httpRequest);

      using (var usecases = ImportVouchersUseCases.UseCaseInteractor()) {

        ImportVouchersResult result = usecases.DryRunImportVouchersFromTextFile(command, textFile);

        return new SingleObjectModel(base.Request, result);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/vouchers/import-from-text-file")]
    public SingleObjectModel ImportVouchersFromTextFile() {

      HttpRequest httpRequest = GetValidatedHttpRequest();

      FileData textFile = GetFileDataFromRequest(httpRequest);
      ImportVouchersCommand command = GetImportVoucherCommandFromRequest(httpRequest);

      using (var usecases = ImportVouchersUseCases.UseCaseInteractor()) {
        ImportVouchersResult result = usecases.ImportVouchersFromTextFile(command, textFile);

        return new SingleObjectModel(base.Request, result);
      }
    }

    #endregion Text file importers


    #region Helper methods


    private ImportVouchersCommand GetImportVoucherCommandFromRequest(HttpRequest httpRequest) {
      NameValueCollection form = httpRequest.Form;

      Assertion.AssertObject(form["command"], "'command' form field is required");

      var command = new ImportVouchersCommand();

      return JsonConverter.Merge(form["command"], command);
    }


    static private FileData GetFileDataFromRequest(HttpRequest httpRequest) {
      HttpPostedFile file = httpRequest.Files[0];

      var fields = new FileData();

      fields.InputStream = httpRequest.Files[0].InputStream;

      fields.MediaType = file.ContentType;
      fields.MediaLength = file.ContentLength;
      fields.OriginalFileName = file.FileName;

      return fields;
    }


    static private HttpRequest GetValidatedHttpRequest() {
      var httpRequest = HttpContext.Current.Request;

      Assertion.AssertObject(httpRequest, "httpRequest");
      Assertion.Assert(httpRequest.Files.Count == 1, "The request does not have the file to be imported.");

      var form = httpRequest.Form;

      Assertion.AssertObject(form, "The request must be of type 'multipart/form-data'.");

      return httpRequest;
    }

    #endregion Helper methods

  }  // class ImportVouchersController

}  // namespace Empiria.FinancialAccounting.WebApi.BanobrasIntegration
