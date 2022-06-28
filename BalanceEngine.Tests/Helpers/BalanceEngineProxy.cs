/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test cases                              *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Http Proxy                              *
*  Type     : BalanceEngineProxy                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use Http proxy based on Http remote calls to TrialBalanceUseCases.                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Threading.Tasks;

using Empiria.WebApi.Client;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.UseCases;

namespace Empiria.FinancialAccounting.Tests.BalanceEngine {

  static class BalanceEngineProxy {

    /// <summary>Use case proxy based on Http remote calls to TrialBalanceUseCases methods.</summary>
    static internal TrialBalanceDto BuildTrialBalance(TrialBalanceQuery query) {

      if (TestingConstants.INVOKE_USE_CASES_THROUGH_THE_WEB_API) {
        return BuildRemoteTrialBalanceUseCase(query);

      } else {
        return BuildLocalTrialBalanceUseCase(query);

      }
    }


    /// <summary>Use case proxy based on Http remote calls to TrialBalanceUseCases methods.</summary>
    static internal TrialBalanceDto<T> BuildTrialBalance<T>(TrialBalanceQuery query)
                                                            where T : ITrialBalanceEntryDto {

      if (TestingConstants.INVOKE_USE_CASES_THROUGH_THE_WEB_API) {
        return BuildRemoteTrialBalanceUseCase<T>(query);

      } else {
        return BuildLocalTrialBalanceUseCase<T>(query);

      }
    }

    static private TrialBalanceDto BuildLocalTrialBalanceUseCase(TrialBalanceQuery query) {
      using (var usecase = TrialBalanceUseCases.UseCaseInteractor()) {
        return usecase.BuildTrialBalance(query);
      }
    }


    static private TrialBalanceDto<T> BuildLocalTrialBalanceUseCase<T>(TrialBalanceQuery query)
                                                                       where T : ITrialBalanceEntryDto {
      using (var usecase = TrialBalanceUseCases.UseCaseInteractor()) {
        throw new NotImplementedException();
      }
    }


    static internal Task<AnaliticoDeCuentasDto> BuildAnaliticoDeCuentas(TrialBalanceQuery query) {
      if (TestingConstants.INVOKE_USE_CASES_THROUGH_THE_WEB_API) {
        return BuildRemoteAnaliticoDeCuentasUseCase(query);

      } else {
        return BuildLocalAnaliticoDeCuentasUseCase(query);

      }
    }


    static internal Task<BalanzaColumnasMonedaDto> BuildBalanzaColumnasMoneda(TrialBalanceQuery query) {
      if (TestingConstants.INVOKE_USE_CASES_THROUGH_THE_WEB_API) {
        return BuildRemoteBalanzaColumnasMonedaUseCase(query);

      } else {
        return BuildLocalBalanzaColumnasMonedaUseCase(query);

      }
    }


    static internal Task<BalanzaContabilidadesCascadaDto> BuildBalanzaContabilidadesCascada(
                                                TrialBalanceQuery query) {
      if (TestingConstants.INVOKE_USE_CASES_THROUGH_THE_WEB_API) {
        return BuildLocalBalanzaContabilidadesCascadaUseCase(query);

      } else {
        return BuildRemoteBalanzaContabilidadesCascadaUseCase(query);

      }
    }


    static internal Task<BalanzaTradicionalDto> BuildBalanzaTradicional(TrialBalanceQuery query) {
      if (TestingConstants.INVOKE_USE_CASES_THROUGH_THE_WEB_API) {
        return BuildRemoteBalanzaTradicionalUseCase(query);

      } else {
        return BuildLocalBalanzaTradicionalUseCase(query);

      }
    }


    private static Task<AnaliticoDeCuentasDto> BuildLocalAnaliticoDeCuentasUseCase(TrialBalanceQuery query) {
      using (var usecase = TrialBalanceUseCases.UseCaseInteractor()) {
        return usecase.BuildAnaliticoDeCuentas(query);
      }
    }


    private static async Task<AnaliticoDeCuentasDto> BuildRemoteAnaliticoDeCuentasUseCase(TrialBalanceQuery query) {
      HttpApiClient http = CreateHttpApiClient();

      var dto = await http.PostAsync<ResponseModel<AnaliticoDeCuentasDto>>(query, "v2/financial-accounting/balance-engine/analitico-de-cuentas")
                          .ConfigureAwait(false);

      return dto.Data;
    }


    private static Task<BalanzaColumnasMonedaDto> BuildLocalBalanzaColumnasMonedaUseCase(TrialBalanceQuery query) {
      using (var usecase = TrialBalanceUseCases.UseCaseInteractor()) {
        return usecase.BuildBalanzaColumnasMoneda(query);
      }
    }


    private static async Task<BalanzaColumnasMonedaDto> BuildRemoteBalanzaColumnasMonedaUseCase(TrialBalanceQuery query) {
      HttpApiClient http = CreateHttpApiClient();

      var dto = await http.PostAsync<ResponseModel<BalanzaColumnasMonedaDto>>(
                            query, "v2/financial-accounting/balance-engine/balanza-columnas-moneda")
                          .ConfigureAwait(false);

      return dto.Data;
    }


    private static Task<BalanzaContabilidadesCascadaDto> BuildLocalBalanzaContabilidadesCascadaUseCase(
                                                TrialBalanceQuery query) {
      using (var usecase = TrialBalanceUseCases.UseCaseInteractor()) {
        return usecase.BuildBalanzaContabilidadesCascada(query);
      }
    }


    private static async Task<BalanzaContabilidadesCascadaDto> BuildRemoteBalanzaContabilidadesCascadaUseCase(
                                                      TrialBalanceQuery query) {
      HttpApiClient http = CreateHttpApiClient();

      var dto = await http.PostAsync<ResponseModel<BalanzaContabilidadesCascadaDto>>(
                            query, "v2/financial-accounting/balance-engine/balanza-contabilidades-cascada")
                          .ConfigureAwait(false);

      return dto.Data;
    }


    private static Task<BalanzaTradicionalDto> BuildLocalBalanzaTradicionalUseCase(TrialBalanceQuery query) {
      using (var usecase = TrialBalanceUseCases.UseCaseInteractor()) {
        return usecase.BuildBalanzaTradicional(query);
      }
    }


    private static async Task<BalanzaTradicionalDto> BuildRemoteBalanzaTradicionalUseCase(TrialBalanceQuery query) {
      HttpApiClient http = CreateHttpApiClient();

      var dto = await http.PostAsync<ResponseModel<BalanzaTradicionalDto>>(query, "v2/financial-accounting/balance-engine/balanza-tradicional")
                          .ConfigureAwait(false);

      return dto.Data;
    }


    static private TrialBalanceDto BuildRemoteTrialBalanceUseCase(TrialBalanceQuery query) {
      HttpApiClient http = CreateHttpApiClient();

      return http.PostAsync<ResponseModel<TrialBalanceDto>>(query, "v2/financial-accounting/balance-engine/trial-balance")
                 .Result
                 .Data;
    }


    static private TrialBalanceDto<T> BuildRemoteTrialBalanceUseCase<T>(TrialBalanceQuery query)
                                                                        where T : ITrialBalanceEntryDto {
      HttpApiClient http = CreateHttpApiClient();

      return http.PostAsync<ResponseModel<TrialBalanceDto<T>>>(query, "v2/financial-accounting/balance-engine/trial-balance")
                 .Result
                 .Data;
    }


    static private HttpApiClient CreateHttpApiClient() {
      return new HttpApiClient(TestingConstants.WEB_API_BASE_ADDRESS,
                               TimeSpan.FromSeconds(TestingConstants.WEB_API_TIMEOUT_SECONDS));
    }

  }  // class BalanceEngineProxy

}  // namespace Empiria.FinancialAccounting.Tests.BalanceEngine
