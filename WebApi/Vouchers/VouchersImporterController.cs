/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Vouchers Management                          Component : Web Api                               *
*  Assembly : Empiria.FinancialAccounting.WebApi.dll       Pattern   : Command Controller                    *
*  Type     : VouchersImporterController                   License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Command API used to import whole vouchers designed for external systems interaction.           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System.Web.Http;

using Empiria.WebApi;

using Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter.Adapters;
using Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter.UseCases;

namespace Empiria.FinancialAccounting.WebApi.Vouchers {

  /// <summary>Command API used to import whole vouchers designed for external systems interaction.</summary>
  public class VouchersImporterController : WebApiController {

    #region Web api

    [HttpPost]
    [Route("v2/financial-accounting/vouchers/import")]
    [Route("v2/financial-accounting/vouchers/import/dry-run")]
    public SingleObjectModel ImportVouchers([FromBody] VoucherImportationCommand command) {

      base.RequireBody(command);

      bool dryRun = base.Request.RequestUri.PathAndQuery.EndsWith("/dry-run");

      using (var usecases = ImportVouchersUseCases.UseCaseInteractor()) {
        ImportVouchersResult result = usecases.StandardVoucherImportation(command, dryRun);

        return new SingleObjectModel(base.Request, result);
      }
    }

    #endregion Web api

  }  // class VouchersImporterController

}  // namespace Empiria.FinancialAccounting.WebApi.Vouchers
