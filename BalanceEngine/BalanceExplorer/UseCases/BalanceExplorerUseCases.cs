﻿/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Use cases Layer                         *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Use case interactor class               *
*  Type     : BalanceExplorerUseCases                    License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases used to build balances.                                                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Threading.Tasks;
using Empiria.Services;

using Empiria.FinancialAccounting.BalanceEngine.BalanceExplorer.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine.BalanceExplorer.UseCases {

  /// <summary>Use cases used to build balances.</summary>
  public class BalanceExplorerUseCases : UseCase {

    #region Constructors and parsers

    protected BalanceExplorerUseCases() {
      // no-op
    }

    static public BalanceExplorerUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<BalanceExplorerUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases


    public BalanceExplorerDto GetBalances(BalanceExplorerQuery query) {
      Assertion.Require(query, nameof(query));

      if (query.TrialBalanceType == TrialBalanceType.SaldosPorAuxiliarID) {

        query.SubledgerAccount = SubledgerAccount.Parse(query.SubledgerAccountID).Number;
      }

      var builder = new BalanceExplorerResultBuilder(query);

      BalanceExplorerResult balances = builder.Build();

      return BalanceExplorerMapper.Map(balances);
    }


    public async Task<BalanceExplorerDto> GetBalancesForExplorer(BalanceExplorerQuery query) {
      Assertion.Require(query, nameof(query));

      if (query.TrialBalanceType == TrialBalanceType.SaldosPorAuxiliarID) {
        
        query.SubledgerAccount = SubledgerAccount.Parse(query.SubledgerAccountID).Number;
      }

      var builder = new BalanceExplorerResultBuilder(query);

      BalanceExplorerResult balances = await Task.Run(() => builder.Build()).ConfigureAwait(false);

      return BalanceExplorerMapper.Map(balances);
    }

    #endregion Use cases

  } // class BalanceExplorerUseCases

} // namespace Empiria.FinancialAccounting.BalanceEngine.BalanceExplorer.UseCases
