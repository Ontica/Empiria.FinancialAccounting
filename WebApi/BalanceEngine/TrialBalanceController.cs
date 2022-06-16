/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                               Component : Web Api                               *
*  Assembly : Empiria.FinancialAccounting.WebApi.dll       Pattern   : Controller                            *
*  Type     : TrialBalanceController                       License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Query web API used to retrive trial balances.                                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Threading.Tasks;
using System.Web.Http;

using Empiria.WebApi;

using Empiria.FinancialAccounting.Reporting;

using Empiria.FinancialAccounting.BalanceEngine.UseCases;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;


namespace Empiria.FinancialAccounting.WebApi.BalanceEngine {

  /// <summary>Query web API used to retrive trial balances.</summary>
  public class TrialBalanceController : WebApiController {

    #region Web Apis

    [HttpPost]
    [Route("v2/financial-accounting/balance-engine/analitico-de-cuentas")]
    public async Task<SingleObjectModel> GetAnaliticoCuentas([FromBody] TrialBalanceQuery query) {

      base.RequireBody(query);

      using (var usecases = TrialBalanceUseCases.UseCaseInteractor()) {

        AnaliticoDeCuentasDto dto = await usecases.BuildAnaliticoDeCuentas(query)
                                                  .ConfigureAwait(false);

        return new SingleObjectModel(this.Request, dto);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/balance-engine/balanza-columnas-moneda")]
    public async Task<SingleObjectModel> GetBalanzaColumnasMoneda([FromBody] TrialBalanceQuery query) {

      base.RequireBody(query);

      using (var usecases = TrialBalanceUseCases.UseCaseInteractor()) {

        BalanzaColumnasMonedaDto dto = await usecases.BuildBalanzaColumnasMoneda(query)
                                                            .ConfigureAwait(false);

        return new SingleObjectModel(this.Request, dto);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/balance-engine/balanza-comparativa")]
    public async Task<SingleObjectModel> GetBalanzaComparativa([FromBody] TrialBalanceQuery query) {

      base.RequireBody(query);

      using (var usecases = TrialBalanceUseCases.UseCaseInteractor()) {

        BalanzaComparativaDto dto = await usecases.BuildBalanzaComparativa(query)
                                                            .ConfigureAwait(false);

        return new SingleObjectModel(this.Request, dto);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/balance-engine/balanza-cascada")]
    public async Task<SingleObjectModel> GetBalanzaContabilidadesCascada([FromBody] TrialBalanceQuery query) {

      base.RequireBody(query);

      using (var usecases = TrialBalanceUseCases.UseCaseInteractor()) {

        BalanzaContabilidadesCascadaDto dto = await usecases.BuildBalanzaContabilidadesCascada(query)
                                                            .ConfigureAwait(false);

        return new SingleObjectModel(this.Request, dto);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/balance-engine/balanza-dolarizada")]
    public async Task<SingleObjectModel> GetBalanzaDolarizada([FromBody] TrialBalanceQuery query) {

      base.RequireBody(query);

      using (var usecases = TrialBalanceUseCases.UseCaseInteractor()) {

        BalanzaDolarizadaDto dto = await usecases.BuildBalanzaDolarizada(query)
                                                            .ConfigureAwait(false);

        return new SingleObjectModel(this.Request, dto);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/balance-engine/balanza-tradicional")]
    public async Task<SingleObjectModel> GetBalanzaTradicional([FromBody] TrialBalanceQuery query) {

      base.RequireBody(query);

      using (var usecases = TrialBalanceUseCases.UseCaseInteractor()) {

        BalanzaTradicionalDto dto = await usecases.BuildBalanzaTradicional(query)
                                                  .ConfigureAwait(false);

        return new SingleObjectModel(this.Request, dto);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/balance-engine/saldos-por-auxiliar")]
    public async Task<SingleObjectModel> GetSaldosPorAuxiliar([FromBody] TrialBalanceQuery query) {

      base.RequireBody(query);

      using (var usecases = TrialBalanceUseCases.UseCaseInteractor()) {

        SaldosPorAuxiliarDto dto = await usecases.BuildSaldosPorAuxiliar(query)
                                                  .ConfigureAwait(false);

        return new SingleObjectModel(this.Request, dto);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/balance-engine/saldos-por-cuenta")]
    public async Task<SingleObjectModel> GetSaldosPorCuenta([FromBody] TrialBalanceQuery query) {

      base.RequireBody(query);

      using (var usecases = TrialBalanceUseCases.UseCaseInteractor()) {

        SaldosPorCuentaDto dto = await usecases.BuildSaldosPorCuenta(query)
                                                  .ConfigureAwait(false);

        return new SingleObjectModel(this.Request, dto);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/balance-engine/trial-balance")]
    public SingleObjectModel GetTrialBalance([FromBody] TrialBalanceQuery query) {

      base.RequireBody(query);

      using (var usecases = TrialBalanceUseCases.UseCaseInteractor()) {

        TrialBalanceDto trialBalance = usecases.BuildTrialBalance(query);

        return new SingleObjectModel(this.Request, trialBalance);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/balance-engine/trial-balance/excel")]
    public SingleObjectModel ExportTrialBalanceToExcel([FromBody] TrialBalanceQuery query) {

      base.RequireBody(query);

      using (var usecases = TrialBalanceUseCases.UseCaseInteractor()) {

        TrialBalanceDto trialBalance = usecases.BuildTrialBalance(query);

        var excelExporter = new ExcelExporterService();

        FileReportDto excelFileDto = excelExporter.Export(trialBalance);

        return new SingleObjectModel(this.Request, excelFileDto);
      }
    }

    #endregion Web Apis

  } // class TrialBalanceController

} // namespace Empiria.FinancialAccounting.WebApi.BalanceEngine
