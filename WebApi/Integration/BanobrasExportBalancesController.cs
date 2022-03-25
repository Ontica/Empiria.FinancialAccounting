/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                Component : Web Api                               *
*  Assembly : Empiria.FinancialAccounting.WebApi.dll       Pattern   : Query Controller                      *
*  Type     : BanobrasExportBalancesController             License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Query web API used to return balances used by other systems (Banobras).                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Web.Http;

using Empiria.WebApi;

using Empiria.FinancialAccounting.BanobrasIntegration.BalancesExporter.Adapters;
using Empiria.FinancialAccounting.BanobrasIntegration.BalancesExporter.UseCases;

namespace Empiria.FinancialAccounting.WebApi.BanobrasIntegration {

  /// <summary>Query web API used to return balances used by other systems (Banobras).</summary>
  public class BanobrasExportBalancesController : WebApiController {

    #region Web Apis


    [HttpPost, AllowAnonymous]
    [Route("v2/financial-accounting/integration/export-balances")]
    public SingleObjectModel ExportBalances([FromBody] ExportBalancesCommand command) {

      base.RequireBody(command);

      command.AccountsChartId = AccountsChart.IFRS.Id;

      using (var usecases = ExportBalancesUseCases.UseCaseInteractor()) {

        usecases.Export(command);

        return new SingleObjectModel(this.Request, "La exportación de saldos se realizó con éxito.");
      }
    }


    [HttpPost, AllowAnonymous]
    [Route("v2/financial-accounting/integration/balances-by-day")]
    public CollectionModel ExportBalancesByDay([FromBody] BanobrasExportBalancesCommand banobrasCommand) {

      base.RequireBody(banobrasCommand);

      ExportBalancesCommand command = banobrasCommand.ConvertToExportBalancesCommandByDay();

      using (var usecases = ExportBalancesUseCases.UseCaseInteractor()) {

        FixedList<ExportedBalancesDto> balancesDto = usecases.Export(command);

        return new CollectionModel(this.Request, balancesDto);
      }
    }


    [HttpPost, AllowAnonymous]
    [Route("v2/financial-accounting/integration/balances-by-month")]
    public CollectionModel ExportBalancesByMonth([FromBody] BanobrasExportBalancesCommand banobrasCommand) {

      base.RequireBody(banobrasCommand);

      ExportBalancesCommand command = banobrasCommand.ConvertToExportBalancesCommandByMonth();

      using (var usecases = ExportBalancesUseCases.UseCaseInteractor()) {

        FixedList<ExportedBalancesDto> balancesDto = usecases.Export(command);

        return new CollectionModel(this.Request, balancesDto);
      }
    }

    #endregion Web Apis

  } // class BanobrasExportBalancesController

} // namespace Empiria.FinancialAccounting.WebApi.BanobrasIntegration
