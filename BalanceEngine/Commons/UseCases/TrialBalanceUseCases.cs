/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Use cases Layer                         *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Use case interactor class               *
*  Type     : TrialBalanceUseCases                       License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases used to build trial balances.                                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Threading.Tasks;

using Empiria.Services;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine.UseCases {

  /// <summary>Use cases used to build trial balances.</summary>
  public class TrialBalanceUseCases : UseCase {

    #region Constructors and parsers

    protected TrialBalanceUseCases() {
      // no-op
    }

    static public TrialBalanceUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<TrialBalanceUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases

    public async Task<AnaliticoDeCuentasDto> BuildAnaliticoDeCuentas(TrialBalanceCommand command) {
      Assertion.AssertObject(command, "command");

      Assertion.Assert(command.TrialBalanceType == TrialBalanceType.AnaliticoDeCuentas,
                       "command.TrialBalanceType must be 'AnaliticoDeCuentas'.");

      var trialBalanceEngine = new TrialBalanceEngine(command);

      TrialBalance trialBalance = await Task.Run(() => trialBalanceEngine.BuildTrialBalance())
                                            .ConfigureAwait(false);

      return AnaliticoDeCuentasMapper.Map(trialBalance);
    }


    public TrialBalanceDto BuildTrialBalance(TrialBalanceCommand command) {
      Assertion.AssertObject(command, "command");

      var trialBalanceEngine = new TrialBalanceEngine(command);

      TrialBalance trialBalance = trialBalanceEngine.BuildTrialBalance();

      return TrialBalanceMapper.Map(trialBalance);
    }

    #endregion Use cases

  } // class TrialBalanceUseCases

} // Empiria.FinancialAccounting.BalanceEngine.UseCases
