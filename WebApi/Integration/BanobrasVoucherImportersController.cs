/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                Component : Voucher importation                   *
*  Assembly : Empiria.FinancialAccounting.WebApi.dll       Pattern   : Command Controller                    *
*  Type     : BanobrasVoucherImportersController           License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Web API that imports vouchers from Banobras' Excel and text files and from                     *
*             'interfaz única' data structures.                                                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.Http;

using Empiria.Json;
using Empiria.Storage;
using Empiria.WebApi;

using Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter.Adapters;
using Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter.UseCases;

namespace Empiria.FinancialAccounting.WebApi.BanobrasIntegration {

  /// <summary>Web API that imports vouchers from Banobras' Excel and text files and
  /// from 'interfaz única' data structures.</summary>
  public class BanobrasVoucherImportersController : WebApiController {

    #region Voucher importers

    [HttpPost]
    [Route("v2/financial-accounting/vouchers/import-from-excel")]
    [Route("v2/financial-accounting/vouchers/import-from-excel/dry-run")]
    public SingleObjectModel ImportVouchersFromExcelFile() {

      HttpRequest httpRequest = GetValidatedHttpRequest();

      InputFile excelFile = base.GetInputFileFromHttpRequest();

      ImportVouchersCommand command = GetImportVoucherCommandFromRequest(httpRequest);

      bool dryRun = RouteContainsDryRunFlag();

      using (var usecases = ImportVouchersUseCases.UseCaseInteractor()) {
        ImportVouchersResult result = usecases.ImportVouchersFromExcelFile(command, excelFile, dryRun);

        return new SingleObjectModel(base.Request, result);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/vouchers/import-from-interfaz-unica")]
    [Route("v2/financial-accounting/vouchers/import-from-interfaz-unica/dry-run")]
    public SingleObjectModel InterfazUnicaVoucherImporter([FromBody] InterfazUnicaImporterCommand command) {

      base.RequireBody(command);

      bool dryRun = RouteContainsDryRunFlag();

      using (var usecases = ImportVouchersUseCases.UseCaseInteractor()) {
        ImportVouchersResult result = usecases.ImportVouchersFromInterfazUnica(command, dryRun);

        return new SingleObjectModel(base.Request, result);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/vouchers/import-from-text-file")]
    [Route("v2/financial-accounting/vouchers/import-from-text-file/dry-run")]
    public SingleObjectModel ImportVouchersFromTextFile() {

      HttpRequest httpRequest = GetValidatedHttpRequest();

      InputFile textFile = base.GetInputFileFromHttpRequest();

      ImportVouchersCommand command = GetImportVoucherCommandFromRequest(httpRequest);

      bool dryRun = RouteContainsDryRunFlag();

      using (var usecases = ImportVouchersUseCases.UseCaseInteractor()) {
        ImportVouchersResult result = usecases.ImportVouchersFromTextFile(command, textFile, dryRun);

        return new SingleObjectModel(base.Request, result);
      }
    }


    #endregion Voucher importers

    #region Helper methods

    private ImportVouchersCommand GetImportVoucherCommandFromRequest(HttpRequest httpRequest) {
      NameValueCollection form = httpRequest.Form;

      Assertion.Require(form["command"], "'command' form field is required");

      var command = new ImportVouchersCommand();

      return JsonConverter.Merge(form["command"], command);
    }


    static private HttpRequest GetValidatedHttpRequest() {
      var httpRequest = HttpContext.Current.Request;

      Assertion.Require(httpRequest, "httpRequest");
      Assertion.Require(httpRequest.Files.Count == 1, "The request does not have the file to be imported.");

      var form = httpRequest.Form;

      Assertion.Require(form, "The request must be of type 'multipart/form-data'.");

      return httpRequest;
    }


    private bool RouteContainsDryRunFlag() {
      return base.Request.RequestUri.PathAndQuery.EndsWith("/dry-run");
    }

    #endregion Helper methods

  }  // class BanobrasVoucherImportersController

}  // namespace Empiria.FinancialAccounting.WebApi.BanobrasIntegration
