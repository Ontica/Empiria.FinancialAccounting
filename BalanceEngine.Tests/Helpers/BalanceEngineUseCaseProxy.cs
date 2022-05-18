/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test cases                              *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Http Proxy                              *
*  Type     : BalanceEngineUseCaseProxy                  License   : Please read LICENSE.txt file            *
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

  static class BalanceEngineUseCaseProxy {

    private const int WEB_API_TIMEOUT_SECONDS = 50;

    /// <summary>Use case proxy based on Http remote calls to TrialBalanceUseCases methods.</summary>
    static internal TrialBalanceDto BuildTrialBalance(TrialBalanceCommand command) {

      if (TestingConstants.INVOKE_USE_CASES_THROUGH_THE_WEB_API) {
        return BuildRemoteTrialBalanceUseCase(command);

      } else {
        return BuildLocalTrialBalanceUseCase(command);

      }
    }


    /// <summary>Use case proxy based on Http remote calls to TrialBalanceUseCases methods.</summary>
    static internal TrialBalanceDto<T> BuildTrialBalance<T>(TrialBalanceCommand command)
                                                            where T : ITrialBalanceEntryDto {

      if (TestingConstants.INVOKE_USE_CASES_THROUGH_THE_WEB_API) {
        return BuildRemoteTrialBalanceUseCase<T>(command);

      } else {
        return BuildLocalTrialBalanceUseCase<T>(command);

      }
    }

    static private TrialBalanceDto BuildLocalTrialBalanceUseCase(TrialBalanceCommand command) {
      using (var usecase = TrialBalanceUseCases.UseCaseInteractor()) {
        return usecase.BuildTrialBalance(command);
      }
    }


    static private TrialBalanceDto<T> BuildLocalTrialBalanceUseCase<T>(TrialBalanceCommand command)
                                                                       where T : ITrialBalanceEntryDto {
      using (var usecase = TrialBalanceUseCases.UseCaseInteractor()) {
        throw new NotImplementedException();
        //  return usecase.BuildTrialBalance<T>(command);
      }
    }


    static internal Task<AnaliticoDeCuentasDto> BuildAnaliticoDeCuentas(TrialBalanceCommand command) {
      if (TestingConstants.INVOKE_USE_CASES_THROUGH_THE_WEB_API) {
        return BuildRemoteAnaliticoDeCuentasUseCase(command);

      } else {
        return BuildLocalAnaliticoDeCuentasUseCase(command);

      }
    }


    private static Task<AnaliticoDeCuentasDto> BuildLocalAnaliticoDeCuentasUseCase(TrialBalanceCommand command) {
      using (var usecase = TrialBalanceUseCases.UseCaseInteractor()) {
        return usecase.BuildAnaliticoDeCuentas(command);
      }
    }


    private static async Task<AnaliticoDeCuentasDto> BuildRemoteAnaliticoDeCuentasUseCase(TrialBalanceCommand command) {
      HttpApiClient http = CreateHttpApiClient();

      var dto = await http.PostAsync<ResponseModel<AnaliticoDeCuentasDto>>(command, "v2/financial-accounting/balance-engine/analitico-de-cuentas")
                          .ConfigureAwait(false);

      return dto.Data;
    }


    static private TrialBalanceDto BuildRemoteTrialBalanceUseCase(TrialBalanceCommand command) {
      HttpApiClient http = CreateHttpApiClient();

      return http.PostAsync<ResponseModel<TrialBalanceDto>>(command, "v2/financial-accounting/trial-balance")
                 .Result
                 .Data;
    }


    static private TrialBalanceDto<T> BuildRemoteTrialBalanceUseCase<T>(TrialBalanceCommand command)
                                                                        where T : ITrialBalanceEntryDto {
      HttpApiClient http = CreateHttpApiClient();

      return http.PostAsync<ResponseModel<TrialBalanceDto<T>>>(command, "v2/financial-accounting/trial-balance")
                 .Result
                 .Data;
    }


    static private HttpApiClient CreateHttpApiClient() {
      return new HttpApiClient(TestingConstants.WEB_API_BASE_ADDRESS,
                               TimeSpan.FromSeconds(WEB_API_TIMEOUT_SECONDS));
    }

  }  // class BalanceEngineUseCaseProxy

}  // namespace Empiria.FinancialAccounting.Tests.BalanceEngine
