/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Builder                                 *
*  Type     : BalanceExplorerResultBuilder               License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Builds BalanceExplorerResult instances.                                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.BalanceEngine.BalanceExplorer.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine.BalanceExplorer {

  /// <summary>Builds BalanceExplorerResult instances.</summary>
  internal class BalanceExplorerResultBuilder {

    internal BalanceExplorerResultBuilder(BalanceExplorerQuery query){
      Assertion.Require(query, nameof(query));

      Query = query;
    }


    public BalanceExplorerQuery Query {
      get;
    }


    internal BalanceExplorerResult Build() {
      if (!this.Query.UseCache) {
        return BuildBalanceResult();
      }

      string hash = TrialBalanceCache.GenerateBalanceHash(this.Query);

      BalanceExplorerResult balances = TrialBalanceCache.TryGetBalances(hash);

      if (balances == null) {
        balances = BuildBalanceResult();

        TrialBalanceCache.StoreBalances(hash, balances);
      }

      return balances;
    }


    private BalanceExplorerResult BuildBalanceResult() {
      switch (Query.TrialBalanceType) {

        case TrialBalanceType.SaldosPorAuxiliarConsultaRapida:
          var saldosPorAuxiliar = new SaldosPorAuxiliarBuilder(Query);
          return saldosPorAuxiliar.Build();

        case TrialBalanceType.SaldosPorCuentaConsultaRapida:
          var saldosPorCuenta = new SaldosPorCuentaBuilder(Query);
          return saldosPorCuenta.Build();

        default:
          throw Assertion.EnsureNoReachThisCode(
                    $"Unhandled trial balance type {this.Query.TrialBalanceType}.");
      }
    }

  } // class BalanceConstructor

} // Empiria.FinancialAccounting.BalanceEngine.BalanceExplorer
