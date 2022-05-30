/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Service provider                        *
*  Type     : BalanceConstructor                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides services to generate a balance.                                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.FinancialAccounting.BalanceEngine.BalanceExplorer.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Provides services to generate a balance.</summary>
  internal class BalanceConstructor {

    internal BalanceConstructor(BalancesQuery query){
      Assertion.Require(query, nameof(query));

      Query = query;
    }


    public BalancesQuery Query {
      get;
    }


    internal Balances BuildBalances() {
      if (!this.Query.UseCache) {
        return GenerateBalance();
      }

      string hash = TrialBalanceCache.GenerateBalanceHash(this.Query);

      Balances balances = TrialBalanceCache.TryGetBalances(hash);

      if (balances == null) {
        balances = GenerateBalance();
        TrialBalanceCache.StoreBalances(hash, balances);
      }

      return balances;
    }


    internal Balances GenerateBalance() {
      switch (Query.TrialBalanceType) {

        case TrialBalanceType.SaldosPorAuxiliarConsultaRapida:
          var saldosPorAuxiliar = new SaldosPorAuxiliarConsultaRapida(Query);
          return saldosPorAuxiliar.Build();

        case TrialBalanceType.SaldosPorCuentaConsultaRapida:
          var saldosPorCuenta = new SaldosPorCuentaConsultaRapida(Query);
          return saldosPorCuenta.Build();

        default:
          throw Assertion.EnsureNoReachThisCode(
                    $"Unhandled trial balance type {this.Query.TrialBalanceType}.");
      }
    }

  } // class BalanceConstructor

} // Empiria.FinancialAccounting.BalanceEngine.Domain
