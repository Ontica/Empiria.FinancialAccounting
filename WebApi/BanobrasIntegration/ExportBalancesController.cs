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

using Empiria.FinancialAccounting.BalanceEngine.UseCases;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

using Empiria.FinancialAccounting.BanobrasIntegration.Adapters;

namespace Empiria.FinancialAccounting.WebApi.BanobrasIntegration {

  /// <summary>Query web API used to return balances used by other systems (Banobras).</summary>
  public class ExportBalancesController : WebApiController {

    #region Web Apis

    [HttpPost, AllowAnonymous]
    [Route("v2/financial-accounting/integration/balances-by-day")]
    public CollectionModel ExportBalancesByDay([FromBody] ExportBalancesCommand command) {
      base.RequireBody(command);

      using (var usecases = TrialBalanceUseCases.UseCaseInteractor()) {

        TrialBalanceCommand trialBalanceCommand = command.MapToTrialBalanceCommandForBalancesByDay();

        TrialBalanceDto trialBalance = usecases.BuildTrialBalance(trialBalanceCommand);

        FixedList<ExportedBalancesDto> balancesDto = ExportBalancesMapper.MapToExportedBalances(command, trialBalance);

        return new CollectionModel(this.Request, balancesDto);
      }
    }


    [HttpPost, AllowAnonymous]
    [Route("v2/financial-accounting/integration/balances-by-month")]
    public CollectionModel ExportBalancesByMonth([FromBody] ExportBalancesCommand command) {
      base.RequireBody(command);

      using (var usecases = TrialBalanceUseCases.UseCaseInteractor()) {

        TrialBalanceCommand trialBalanceCommand = command.MapToTrialBalanceCommandForBalancesByMonth();

        TrialBalanceDto trialBalance = usecases.BuildTrialBalance(trialBalanceCommand);

        FixedList<ExportedBalancesDto> balancesDto = ExportBalancesMapper.MapToExportedBalances(command, trialBalance);

        return new CollectionModel(this.Request, balancesDto);
      }
    }

    #endregion Web Apis

  } // class ExportBalancesController

} // namespace Empiria.FinancialAccounting.WebApi.BanobrasIntegration
