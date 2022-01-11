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


  /// <summary>Provides services to generate a trial balance.</summary>
  internal class TrialBalanceEngine {

    internal TrialBalanceEngine(TrialBalanceCommand command) {
      Assertion.AssertObject(command, "command");

      command.Prepare();

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
          var analiticoDeCuentas = new AnaliticoDeCuentas(this.Command);

          return analiticoDeCuentas.Build();

        case TrialBalanceType.Balanza:
        case TrialBalanceType.Saldos:
        case TrialBalanceType.SaldosPorCuenta:

          var balanzaTradicional = new BalanzaTradicional(this.Command);
          return balanzaTradicional.Build();

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

          var saldosPorCuentaYMayores = new SaldosPorCuentaYMayores(this.Command);
          return saldosPorCuentaYMayores.Build();

        default:
          throw Assertion.AssertNoReachThisCode(
                    $"Unhandled trial balance type {this.Command.TrialBalanceType}.");
      }
    }

  } // class TrialBalanceEngine

} // namespace Empiria.FinancialAccounting.BalanceEngine
