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

    Saldos,

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

    internal TrialBalanceEngine(TrialBalanceCommand command) {
      Assertion.Require(command, "command");

      SetCommandDefaultValues(command);

      this.Command = command;
    }

    public TrialBalanceCommand Command {
      get;
    }

    internal TrialBalance BuildTrialBalance() {
      if (!this.Command.UseCache) {
        return GenerateTrialBalance();
      }

      string hash = TrialBalanceCache.GenerateHash(this.Command);

      TrialBalance trialBalance = TrialBalanceCache.TryGet(hash);
      if (trialBalance == null) {
        trialBalance = GenerateTrialBalance();
        TrialBalanceCache.Store(hash, trialBalance);
      }

      return trialBalance;
    }

    private TrialBalance GenerateTrialBalance() {
      switch (this.Command.TrialBalanceType) {

        case TrialBalanceType.AnaliticoDeCuentas:
          var builder = new AnaliticoDeCuentasBuilder(this.Command);

          var entries = builder.Build();

          FixedList<ITrialBalanceEntry> analyticBalance = entries.Select(x => (ITrialBalanceEntry) x)
                                                           .ToFixedList();

          return new TrialBalance(this.Command, analyticBalance);

        case TrialBalanceType.Balanza:

          var balanzaTradicional = new BalanzaTradicionalBuilder(this.Command);
          return balanzaTradicional.Build();

        case TrialBalanceType.Saldos:
        case TrialBalanceType.SaldosPorCuenta:

          var trialBalance = new TrialBalanceBuilder(this.Command);
          return trialBalance.Build();

        case TrialBalanceType.BalanzaEnColumnasPorMoneda:

          var balanzaPorMoneda = new BalanzaValorizada(this.Command);
          return balanzaPorMoneda.BuildBalanceInColumnsByCurrency();

        case TrialBalanceType.BalanzaValorizadaComparativa:

          var balanzaComparativa = new BalanzaComparativa(this.Command);
          return balanzaComparativa.Build();

        case TrialBalanceType.BalanzaDolarizada:

          var balanzaDolarizada = new BalanzaValorizada(this.Command);
          return balanzaDolarizada.Build();

        case TrialBalanceType.GeneracionDeSaldos:

          var saldosConAuxiliares = new SaldosPorAuxiliar(this.Command);
          return saldosConAuxiliares.BuildForBalancesGeneration();

        case TrialBalanceType.SaldosPorAuxiliar:

          var saldosPorAuxiliar = new SaldosPorAuxiliar(this.Command);
          return saldosPorAuxiliar.Build();

        case TrialBalanceType.BalanzaConContabilidadesEnCascada:

          var saldosPorCuentaYMayores = new BalanzaContabilidadesCascada(this.Command);
          return saldosPorCuentaYMayores.Build();

        default:
          throw Assertion.EnsureNoReachThisCode(
                    $"Unhandled trial balance type {this.Command.TrialBalanceType}.");
      }
    }


    private void SetCommandDefaultValues(TrialBalanceCommand command) {
      if (command.UseDefaultValuation) {
        command.InitialPeriod.UseDefaultValuation = true;
        command.FinalPeriod.UseDefaultValuation = true;
      }
    }

  } // class TrialBalanceEngine

} // namespace Empiria.FinancialAccounting.BalanceEngine
