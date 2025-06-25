﻿/* Empiria Financial *****************************************************************************************
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

    BalanzaDiferenciaDiariaPorMoneda,

    BalanzaValorizadaComparativa,

    BalanzaDolarizada,

    GeneracionDeSaldos,

    SaldosPorAuxiliar,

    SaldosPorCuenta,

    SaldosPorAuxiliarConsultaRapida,

    SaldosPorAuxiliarID,

    SaldosPorCuentaConsultaRapida,

    ValorizacionEstimacionPreventiva

  }


  public enum BalancesType {

    AllAccounts,

    AllAccountsInCatalog,

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
          
          var balanzaColumnasBuilder = new BalanzaColumnasMonedaBuilder(this.Query);
          var balanzaColumnasEntries = balanzaColumnasBuilder.Build();
          FixedList<ITrialBalanceEntry> balanzaColumnas = balanzaColumnasEntries.Select(x =>
                                                            (ITrialBalanceEntry) x).ToFixedList();

          return new TrialBalance(this.Query, balanzaColumnas);

        case TrialBalanceType.BalanzaDiferenciaDiariaPorMoneda:
          Assertion.Require(Query.BalancesType != BalancesType.AllAccounts,
          "La opción 'Todas las cuentas', está temporalmente fuera de servicio para la balanza de diferencia diaria por moneda origen.");

          var balanzaDifDiariaBuilder = new BalanzaDiferenciaDiariaMonedaBuilder(this.Query);
          var balanzaDifDiariaEntries = balanzaDifDiariaBuilder.Build();
          FixedList<ITrialBalanceEntry> balanzaDifDiaria = balanzaDifDiariaEntries.Select(x =>
                                                            (ITrialBalanceEntry) x).ToFixedList();

          return new TrialBalance(this.Query, balanzaDifDiaria);

        case TrialBalanceType.BalanzaValorizadaComparativa:
          Assertion.Require(Query.BalancesType != BalancesType.AllAccounts,
          "La opción 'Todas las cuentas', está temporalmente fuera de servicio para la balanza valorizada comparativa.");

          var comparativaBuilder = new BalanzaComparativaBuilder(this.Query);
          var balanza = comparativaBuilder.Build();
          FixedList<ITrialBalanceEntry> balanzaComparativa = balanza.Select(x => (ITrialBalanceEntry) x)
                                                                    .ToFixedList();

          return new TrialBalance(this.Query, balanzaComparativa);

        case TrialBalanceType.BalanzaDolarizada:
          Assertion.Require(Query.BalancesType != BalancesType.AllAccounts,
          "La opción 'Todas las cuentas', está temporalmente fuera de servicio para la balanza dolarizada.");

          var dolarizadaEntries = new BalanzaDolarizadaBuilder(this.Query).Build();

          FixedList<ITrialBalanceEntry> balanzaDolarizada = dolarizadaEntries.Select(x =>
                                                            (ITrialBalanceEntry) x).ToFixedList();

          return new TrialBalance(this.Query, balanzaDolarizada);

        case TrialBalanceType.GeneracionDeSaldos:

          var saldosConAuxiliares = new SaldosPorAuxiliarBuilder(this.Query);

          return saldosConAuxiliares.BuildForBalancesGeneration();

        case TrialBalanceType.SaldosPorAuxiliar:

          var saldosPorAuxiliar = new SaldosPorAuxiliarBuilder(this.Query);

          return saldosPorAuxiliar.Build();

        case TrialBalanceType.BalanzaConContabilidadesEnCascada:
          Assertion.Require(Query.BalancesType != BalancesType.AllAccounts,
          "La opción 'Todas las cuentas', está temporalmente fuera de servicio para la balanza con contabilidades en cascada.");

          var saldosPorCuentaYMayores = new BalanzaContabilidadesCascadaBuilder(this.Query);

          return saldosPorCuentaYMayores.Build();

        case TrialBalanceType.ValorizacionEstimacionPreventiva:
          var build = new ValorizacionEstimacionPreventivaBuilder(this.Query).Build();

          FixedList<ITrialBalanceEntry> valorizacion = build.Select(x => (ITrialBalanceEntry) x)
                                                            .ToFixedList();

          return new TrialBalance(this.Query, valorizacion);

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
