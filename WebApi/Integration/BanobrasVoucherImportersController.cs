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
using System.Web.Http;

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
    public SingleObjectModel ImportVouchersFromExcelFile() {

      InputFile excelFile = base.GetInputFileFromHttpRequest("VoucherImporterExcelFile");

      ImportVouchersCommand command = base.GetFormDataFromHttpRequest<ImportVouchersCommand>("command");

      using (var usecases = ImportVouchersUseCases.UseCaseInteractor()) {
        ImportVouchersResult result = usecases.ImportVouchersFromExcelFile(command, excelFile);

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
    public SingleObjectModel ImportVouchersFromTextFile() {

      InputFile textFile = base.GetInputFileFromHttpRequest("VoucherImporterTextFile");

      ImportVouchersCommand command = base.GetFormDataFromHttpRequest<ImportVouchersCommand>("command");

      using (var usecases = ImportVouchersUseCases.UseCaseInteractor()) {
        ImportVouchersResult result = usecases.ImportVouchersFromTextFile(command, textFile);

        return new SingleObjectModel(base.Request, result);
      }
    }


    #endregion Voucher importers

    #region Helper methods

    private bool RouteContainsDryRunFlag() {
      return base.Request.RequestUri.PathAndQuery.EndsWith("/dry-run");
    }

    #endregion Helper methods

  }  // class BanobrasVoucherImportersController

}  // namespace Empiria.FinancialAccounting.WebApi.BanobrasIntegration
