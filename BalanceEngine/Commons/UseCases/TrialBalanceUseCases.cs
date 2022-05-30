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
using Empiria.FinancialAccounting.BalanceEngine.Data;

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

    public async Task<AnaliticoDeCuentasDto> BuildAnaliticoDeCuentas(TrialBalanceQuery query) {
      Assertion.Require(query, nameof(query));

      Assertion.Require(query.TrialBalanceType == TrialBalanceType.AnaliticoDeCuentas,
                       "query.TrialBalanceType must be 'AnaliticoDeCuentas'.");

      var builder = new AnaliticoDeCuentasBuilder(query);

      FixedList<TrialBalanceEntry> baseAccountEntries = BalancesDataService.GetTrialBalanceEntries(query);

      FixedList<AnaliticoDeCuentasEntry> entries = await Task.Run(() => builder.Build(baseAccountEntries))
                                                             .ConfigureAwait(false);

      return AnaliticoDeCuentasMapper.Map(query, entries);
    }


    public async Task<BalanzaTradicionalDto> BuildBalanzaTradicional(TrialBalanceQuery query) {
      Assertion.Require(query, nameof(query));

      Assertion.Require(query.TrialBalanceType == TrialBalanceType.Balanza,
                       "query.TrialBalanceType must be 'Balanza'.");

      var builder = new BalanzaTradicionalBuilder(query);
      TrialBalance entries = await Task.Run(() => builder.Build()).ConfigureAwait(false);

      return BalanzaTradicionalMapper.Map(entries);
    }


    public TrialBalanceDto BuildTrialBalance(TrialBalanceQuery query) {
      Assertion.Require(query, nameof(query));

      var trialBalanceEngine = new TrialBalanceEngine(query);

      TrialBalance trialBalance = trialBalanceEngine.BuildTrialBalance();

      return TrialBalanceMapper.Map(trialBalance);
    }


    #endregion Use cases

  } // class TrialBalanceUseCases

} // Empiria.FinancialAccounting.BalanceEngine.UseCases
