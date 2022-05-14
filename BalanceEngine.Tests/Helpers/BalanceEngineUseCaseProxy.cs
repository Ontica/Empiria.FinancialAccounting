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

using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.UseCases;

namespace Empiria.FinancialAccounting.Tests.BalanceEngine {

  static class BalanceEngineUseCaseProxy {

    /// <summary>Use case proxy based on Http remote calls to TrialBalanceUseCases methods.</summary>
    static internal TrialBalanceDto BuildTrialBalance(TrialBalanceCommand command) {

      using (var usecase = TrialBalanceUseCases.UseCaseInteractor()) {
        return usecase.BuildTrialBalance(command);
      }

    }

  }  // class BalanceEngineUseCaseProxy

}  // namespace Empiria.FinancialAccounting.Tests.BalanceEngine
