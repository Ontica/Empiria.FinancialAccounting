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
      switch (this.Command.TrialBalanceType) {

        case TrialBalanceType.AnaliticoDeCuentas:
          return BuildAnaliticoDeCuentas();

        case TrialBalanceType.Balanza:
        case TrialBalanceType.BalanzaConAuxiliares:
        case TrialBalanceType.SaldosPorAuxiliar:
        case TrialBalanceType.SaldosPorCuenta:
        case TrialBalanceType.SaldosPorCuentaConDelegaciones:
          return BuildTraditionalTrialBalance();

        case TrialBalanceType.Saldos:
          return BuildSaldos();

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

      List<TrialBalanceEntry> trialBalance = helper.CombineSummaryAndPostingEntries(summaryEntries,
                                                                                    postingEntries);

      trialBalance = helper.RestrictLevels(trialBalance);

      FixedList<ITrialBalanceEntry> twoColumnsBalance = helper.MergeAccountsIntoTwoColumnsByCurrency(trialBalance);

      return new TrialBalance(Command, twoColumnsBalance);
    }


    private TrialBalance BuildSaldos() {
      var helper = new TrialBalanceHelper(this.Command);

      FixedList<TrialBalanceEntry> postingEntries = helper.GetTrialBalanceEntries();

      FixedList<ITrialBalanceEntry> returnBalance = postingEntries.Select(x => (ITrialBalanceEntry) x)
                                                                  .ToList().ToFixedList();

      return new TrialBalance(Command, returnBalance);
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

      List<TrialBalanceEntry> trialBalance = helper.CombineSummaryAndPostingEntries(summaryEntries,
                                                                                    postingEntries);

      if (Command.TrialBalanceType == TrialBalanceType.SaldosPorAuxiliar) {

        List<TrialBalanceEntry> summarySubsidiaryEntries = helper.BalancesBySubsidiaryAccounts(trialBalance);

        trialBalance = helper.CombineTotalSubsidiaryEntriesWithSummaryAccounts(summarySubsidiaryEntries);

      } else if (Command.TrialBalanceType == TrialBalanceType.SaldosPorCuentaConDelegaciones) {

        List<TrialBalanceEntry> summaryByAccountAndDelegations = helper.GenerateTotalByAccountAndDelegations(trialBalance);

        trialBalance = helper.CombineAccountsAndLedgers(summaryByAccountAndDelegations);

      } else {

        FixedList<TrialBalanceEntry> summaryGroupEntries = helper.GenerateTotalSummaryGroup(postingEntries);

        trialBalance = helper.CombineGroupEntriesAndPostingEntries(trialBalance, summaryGroupEntries);

        List<TrialBalanceEntry> summaryTotalDebtorCreditorEntries =
                                helper.GenerateTotalSummaryDebtorCreditor(postingEntries.ToList());

        trialBalance = helper.CombineDebtorCreditorAndPostingEntries(
                              trialBalance, summaryTotalDebtorCreditorEntries);

        List<TrialBalanceEntry> summaryTotalCurrencies = helper.GenerateTotalSummaryCurrency(
                                                           summaryTotalDebtorCreditorEntries);

        trialBalance = helper.CombineCurrencyAndPostingEntries(trialBalance, summaryTotalCurrencies);

        List<TrialBalanceEntry> summaryTrialBalanceConsolidated = helper.GenerateTotalSummaryConsolidated(
                                                                         summaryTotalCurrencies);

        trialBalance = helper.CombineTotalConsolidatedAndPostingEntries(trialBalance,
                                                                        summaryTrialBalanceConsolidated);
      }


      trialBalance = helper.RestrictLevels(trialBalance);

      FixedList<ITrialBalanceEntry> returnBalance = trialBalance.Select(x => (ITrialBalanceEntry) x)
                                                                  .ToList().ToFixedList();

      return new TrialBalance(Command, returnBalance);
    }

  } // class TrialBalanceEngine

} // namespace Empiria.FinancialAccounting.BalanceEngine
