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

    SaldosPorCuenta,

    SaldosPorAuxiliar,

  }


  public enum BalancesType {

    AllAccounts,

    WithCurrentBalance,

    WithCurrentBalanceOrMovements,

    WithMovements

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
      switch (this.Command.TrialBalanceType) {
        case TrialBalanceType.AnaliticoDeCuentas:
          return BuildAnaliticoDeCuentas();

        case TrialBalanceType.Balanza:
        case TrialBalanceType.BalanzaConAuxiliares:
        case TrialBalanceType.SaldosPorAuxiliar:
        case TrialBalanceType.SaldosPorCuenta:
          return BuildTraditionalTrialBalance();

        default:
          throw Assertion.AssertNoReachThisCode(
                $"Unhandled trial balance type {this.Command.TrialBalanceType}.");
      }
    }


    private TrialBalance BuildAnaliticoDeCuentas() {
      var helper = new TrialBalanceHelper(this.Command);

      FixedList<TrialBalanceEntry> postingEntries = helper.GetTrialBalanceEntries();

      postingEntries = helper.ValuateToExchangeRate(postingEntries);

      List<TrialBalanceEntry> summaryEntries = helper.GenerateSummaryEntries(postingEntries);

      FixedList<TrialBalanceEntry> trialBalance = helper.CombineSummaryAndPostingEntries(summaryEntries,
                                                                                         postingEntries);

      trialBalance = helper.RestrictLevels(trialBalance);

      FixedList<ITrialBalanceEntry> twoColumnsBalance = helper.MergeAccountsIntoTwoColumnsByCurrency(trialBalance);

      return new TrialBalance(Command, twoColumnsBalance);
    }


    private TrialBalance BuildTraditionalTrialBalance() {
      var helper = new TrialBalanceHelper(this.Command);

      FixedList<TrialBalanceEntry> postingEntries = helper.GetTrialBalanceEntries();

      if (Command.ValuateBalances) {
        postingEntries = helper.ValuateToExchangeRate(postingEntries);

        if (Command.ConsolidateBalancesToTargetCurrency) {
          postingEntries = helper.ConsolidateToTargetCurrency(postingEntries);
        }
      }

      List<TrialBalanceEntry> summaryEntries = helper.GenerateSummaryEntries(postingEntries);

      FixedList<TrialBalanceEntry> trialBalance = helper.CombineSummaryAndPostingEntries(summaryEntries,
                                                                                         postingEntries);
      trialBalance = helper.RestrictLevels(trialBalance);

      FixedList<ITrialBalanceEntry> returnBalance = trialBalance.Select(x => (ITrialBalanceEntry) x)
                                                                .ToList().ToFixedList();

      return new TrialBalance(Command, returnBalance);
    }

  } // class TrialBalanceEngine

} // namespace Empiria.FinancialAccounting.BalanceEngine
