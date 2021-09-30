/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                Component : Web Api                               *
*  Assembly : Empiria.FinancialAccounting.WebApi.dll       Pattern   : Query Controller                      *
*  Type     : ExportBalancesController                     License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Query web API used to export balances to other systems.                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Web.Http;

using Empiria.WebApi;

using Empiria.FinancialAccounting.BanobrasIntegration.BalancesExporter.Adapters;
using Empiria.FinancialAccounting.BanobrasIntegration.BalancesExporter.UseCases;

namespace Empiria.FinancialAccounting.WebApi.BanobrasIntegration {

  /// <summary>Query web API used to return balances used by other systems (Banobras).</summary>
  public class ExportBalancesController : WebApiController {

    #region Web Apis

    [HttpPost, AllowAnonymous]
    [Route("v2/financial-accounting/integration/balances-by-day")]
    public CollectionModel ExportBalancesByDay([FromBody] ExportBalancesCommand command) {

      base.RequireBody(command);

      command.GuardarSaldos = false;

      using (var usecases = ExportBalancesUseCases.UseCaseInteractor()) {

        FixedList<ExportedBalancesDto> balancesDto = usecases.ExportBalancesByDay(command);

        return new CollectionModel(this.Request, balancesDto);
      }
    }


    [HttpPost, AllowAnonymous]
    [Route("v2/financial-accounting/integration/balances-by-month")]
    public CollectionModel ExportBalancesByMonth([FromBody] ExportBalancesCommand command) {

      base.RequireBody(command);

      command.GuardarSaldos = true;

      using (var usecases = ExportBalancesUseCases.UseCaseInteractor()) {

        FixedList<ExportedBalancesDto> balancesDto = usecases.ExportBalancesByMonth(command);

        return new CollectionModel(this.Request, balancesDto);
      }
    }

    #endregion Web Apis

  } // class ExportBalancesController

} // namespace Empiria.FinancialAccounting.WebApi.BanobrasIntegration
