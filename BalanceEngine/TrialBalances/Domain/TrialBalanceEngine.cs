/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Service provider                        *
*  Type     : TrialBalanceEngine                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides services to generate a trial balance.                                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine {

  public enum TrialBalanceType {

    AnaliticoDeCuentas,

    Balanza,

    BalanzaConContabilidadesEnCascada,

    BalanzaEnColumnasPorMoneda,

    BalanzaValorizadaComparativa,

    BalanzaDolarizada,

    GeneracionDeSaldos,

    SaldosPorAuxiliar,

    SaldosPorCuenta,

    SaldosPorAuxiliarConsultaRapida,

    SaldosPorCuentaConsultaRapida

  }


  public enum BalancesType {

    AllAccounts,

    WithCurrentBalance,

    WithCurrentBalanceOrMovements,

    WithMovements

  }


  public enum TrialBalanceItemType {

    Entry,

    Summary,

    Group,

    Total,

    BalanceTotalGroupDebtor,

    BalanceTotalGroupCreditor,

    BalanceTotalDebtor,

    BalanceTotalCreditor,

    BalanceTotalCurrency,

    BalanceTotalConsolidatedByLedger,

    BalanceTotalConsolidated

  }


  public enum FileReportVersion {
    V1,

    V2,

    V3
  }


  /// <summary>Provides services to generate a trial balance.</summary>
  internal class TrialBalanceEngine {

    internal TrialBalanceEngine(TrialBalanceQuery query) {
      Assertion.Require(query, nameof(query));

      SetQueryDefaultValues(query);

      this.Query = query;
    }

    public TrialBalanceQuery Query {
      get;
    }

    internal TrialBalance BuildTrialBalance() {
      if (!this.Query.UseCache) {
        return GenerateTrialBalance();
      }

      string hash = TrialBalanceCache.GenerateHash(this.Query);

      TrialBalance trialBalance = TrialBalanceCache.TryGetTrialBalance(hash);
      if (trialBalance == null) {
        trialBalance = GenerateTrialBalance();
        TrialBalanceCache.StoreTrialBalance(hash, trialBalance);
      }

      return trialBalance;
    }

    private TrialBalance GenerateTrialBalance() {
      switch (this.Query.TrialBalanceType) {

        case TrialBalanceType.AnaliticoDeCuentas:
          
          var builder = new AnaliticoDeCuentasBuilder(this.Query);
          var entries = builder.Build();
          FixedList<ITrialBalanceEntry> analyticBalance = entries.Select(x => (ITrialBalanceEntry) x)
                                                           .ToFixedList();

          return new TrialBalance(this.Query, analyticBalance);

        case TrialBalanceType.Balanza:

          var balanzaTradicional = new BalanzaTradicionalBuilder(this.Query);
          return balanzaTradicional.Build();

        case TrialBalanceType.SaldosPorCuenta:
          
          var saldosPorCuenta = new SaldosPorCuentaBuilder(this.Query);
          return saldosPorCuenta.Build();
          

        case TrialBalanceType.BalanzaEnColumnasPorMoneda:

          var balanzaPorMoneda = new BalanzaValorizada(this.Query);
          return balanzaPorMoneda.BuildBalanceInColumnsByCurrency();

        case TrialBalanceType.BalanzaValorizadaComparativa:

          var balanzaComparativa = new BalanzaComparativa(this.Query);
          return balanzaComparativa.Build();

        case TrialBalanceType.BalanzaDolarizada:

          var balanzaDolarizada = new BalanzaValorizada(this.Query);
          return balanzaDolarizada.Build();

        case TrialBalanceType.GeneracionDeSaldos:

          var saldosConAuxiliares = new SaldosPorAuxiliarBuilder(this.Query);
          return saldosConAuxiliares.BuildForBalancesGeneration();

        case TrialBalanceType.SaldosPorAuxiliar:

          var saldosPorAuxiliar = new SaldosPorAuxiliarBuilder(this.Query);
          return saldosPorAuxiliar.Build();

        case TrialBalanceType.BalanzaConContabilidadesEnCascada:

          var saldosPorCuentaYMayores = new BalanzaContabilidadesCascada(this.Query);
          return saldosPorCuentaYMayores.Build();

        default:
          throw Assertion.EnsureNoReachThisCode(
                    $"Unhandled trial balance type {this.Query.TrialBalanceType}.");
      }
    }


    private void SetQueryDefaultValues(TrialBalanceQuery query) {
      if (query.UseDefaultValuation) {
        query.InitialPeriod.UseDefaultValuation = true;
        query.FinalPeriod.UseDefaultValuation = true;
      }
    }

  } // class TrialBalanceEngine

} // namespace Empiria.FinancialAccounting.BalanceEngine
