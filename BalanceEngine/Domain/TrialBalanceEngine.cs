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
using System.Collections.Generic;
using System.Linq;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine {

  public enum TrialBalanceType {

    AnaliticoDeCuentas,

    Balanza,

    BalanzaConAuxiliares,

    Saldos,

    SaldosPorAuxiliar,

    SaldosPorCuenta,

    SaldosPorCuentaConDelegaciones

  }


  public enum BalancesType {

    AllAccounts,

    WithCurrentBalance,

    WithCurrentBalanceOrMovements,

    WithMovements

  }


  public enum TrialBalanceItemType {

    BalanceEntry,

    BalanceSummary,

    BalanceTotalGroupDebtor,

    BalanceTotalGroupCreditor,

    BalanceTotalDebtor,

    BalanceTotalCreditor,

    BalanceTotalCurrency,

    BalanceTotalConsolidated

  }


  /// <summary>Provides services to generate a trial balance.</summary>
  internal class TrialBalanceEngine {

    internal TrialBalanceEngine(TrialBalanceCommand command) {
      Assertion.AssertObject(command, "command");

      this.Command = command;
    }


    public TrialBalanceCommand Command {
      get;
    }


    internal TrialBalance BuildTrialBalance() {
      List<TrialBalanceEntry> trialBalance;

      var cases = new TrialBalanceCases(this.Command);

      switch (this.Command.TrialBalanceType) {

        case TrialBalanceType.AnaliticoDeCuentas:
          //trialBalance = cases.BuildAnaliticoDeCuentas();
          //break;
          throw Assertion.AssertNoReachThisCode("TrialBalanceType.AnaliticoDeCuentas");

        case TrialBalanceType.Balanza:
        case TrialBalanceType.BalanzaConAuxiliares:
        case TrialBalanceType.SaldosPorCuenta:
          trialBalance = cases.BuildBalanzaTradicional();
          break;

        case TrialBalanceType.Saldos:
          //trialBalance = cases.BuildSaldos();
          //break;
          throw Assertion.AssertNoReachThisCode("TrialBalanceType.Saldos");

        case TrialBalanceType.SaldosPorAuxiliar:
          trialBalance = cases.BuildSaldosPorAuxiliar();
          break;

        case TrialBalanceType.SaldosPorCuentaConDelegaciones:
          trialBalance = cases.BuildSaldosPorCuentaConDelegaciones();
          break;

        default:
          throw Assertion.AssertNoReachThisCode(
                    $"Unhandled trial balance type {this.Command.TrialBalanceType}.");
      }

      FixedList<ITrialBalanceEntry> returnBalance = trialBalance.Select(x => (ITrialBalanceEntry) x)
                                                                  .ToList().ToFixedList();

      return new TrialBalance(Command, returnBalance);
    }


  } // class TrialBalanceEngine

} // namespace Empiria.FinancialAccounting.BalanceEngine
