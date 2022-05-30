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


    static private TrialBalanceDto BuildRemoteTrialBalanceUseCase(TrialBalanceQuery query) {
      HttpApiClient http = CreateHttpApiClient();

      return http.PostAsync<ResponseModel<TrialBalanceDto>>(query, "v2/financial-accounting/trial-balance")
                 .Result
                 .Data;
    }


    static private TrialBalanceDto<T> BuildRemoteTrialBalanceUseCase<T>(TrialBalanceQuery query)
                                                                        where T : ITrialBalanceEntryDto {
      HttpApiClient http = CreateHttpApiClient();

      return http.PostAsync<ResponseModel<TrialBalanceDto<T>>>(query, "v2/financial-accounting/trial-balance")
                 .Result
                 .Data;
    }


    static private HttpApiClient CreateHttpApiClient() {
      return new HttpApiClient(TestingConstants.WEB_API_BASE_ADDRESS,
                               TimeSpan.FromSeconds(TestingConstants.WEB_API_TIMEOUT_SECONDS));
    }

  }  // class BalanceEngineProxy

}  // namespace Empiria.FinancialAccounting.Tests.BalanceEngine
