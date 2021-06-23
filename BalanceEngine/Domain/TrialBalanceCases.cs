/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Service provider                        *
*  Type     : TrialBalanceCases                          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides services to generate a trial balance.                                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine {

  internal class TrialBalanceCases {

    private readonly TrialBalanceCommand _command;

    public TrialBalanceCases(TrialBalanceCommand command) {
      _command = command;
    }

    internal TrialBalance BuildAnaliticoDeCuentas() {
      var helper = new TrialBalanceHelper(_command);

      FixedList<TrialBalanceEntry> postingEntries = helper.GetTrialBalanceEntries();

      postingEntries = helper.ValuateToExchangeRate(postingEntries);

      List<TrialBalanceEntry> summaryEntries = helper.GenerateSummaryEntries(postingEntries);

      List<TrialBalanceEntry> trialBalance = helper.CombineSummaryAndPostingEntries(summaryEntries,
                                                                                    postingEntries);

      trialBalance = helper.RestrictLevels(trialBalance);

      FixedList<ITrialBalanceEntry> twoColumnsBalance = helper.MergeAccountsIntoTwoColumnsByCurrency(trialBalance);

      return new TrialBalance(_command, twoColumnsBalance);
    }


    internal TrialBalance BuildSaldos() {
      var helper = new TrialBalanceHelper(_command);

      FixedList<TrialBalanceEntry> postingEntries = helper.GetTrialBalanceEntries();

      FixedList<ITrialBalanceEntry> returnBalance = postingEntries.Select(x => (ITrialBalanceEntry) x)
                                                                  .ToList().ToFixedList();

      return new TrialBalance(_command, returnBalance);
    }


    internal List<TrialBalanceEntry> BuildSaldosPorAuxiliar() {
      var helper = new TrialBalanceHelper(_command);

      List<TrialBalanceEntry> trialBalance = GetSummaryAndPostingEntries();

      List<TrialBalanceEntry> summarySubsidiaryEntries = helper.BalancesBySubsidiaryAccounts(trialBalance);

      trialBalance = helper.CombineTotalSubsidiaryEntriesWithSummaryAccounts(summarySubsidiaryEntries);

      return helper.RestrictLevels(trialBalance);
    }


    internal List<TrialBalanceEntry> BuildSaldosPorCuentaConDelegaciones() {
      var helper = new TrialBalanceHelper(_command);

      List<TrialBalanceEntry> trialBalance = GetSummaryAndPostingEntries();

      List<TrialBalanceEntry> summaryByAccountAndDelegations = helper.GenerateTotalByAccountAndDelegations(trialBalance);

      trialBalance = helper.CombineAccountsAndLedgers(summaryByAccountAndDelegations);

      return helper.RestrictLevels(trialBalance);
    }


    internal List<TrialBalanceEntry> BuildBalanzaTradicional() {
      var helper = new TrialBalanceHelper(_command);

      FixedList<TrialBalanceEntry> postingEntries = GetPostingEntries();

      List<TrialBalanceEntry> summaryEntries = helper.GenerateSummaryEntries(postingEntries);

      List<TrialBalanceEntry> trialBalance = helper.CombineSummaryAndPostingEntries(summaryEntries, postingEntries);

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

      trialBalance = helper.CombineTotalConsolidatedAndPostingEntries(trialBalance, summaryTrialBalanceConsolidated);

      return helper.RestrictLevels(trialBalance);
    }


    #region Helper methods

    private FixedList<TrialBalanceEntry> GetPostingEntries() {
      var helper = new TrialBalanceHelper(_command);

      FixedList<TrialBalanceEntry> postingEntries = helper.GetTrialBalanceEntries();

      if (_command.ValuateBalances) {
        postingEntries = helper.ValuateToExchangeRate(postingEntries);

        if (_command.ConsolidateBalancesToTargetCurrency) {
          postingEntries = helper.ConsolidateToTargetCurrency(postingEntries);
        }
      }
      return postingEntries;
    }


    private List<TrialBalanceEntry> GetSummaryAndPostingEntries() {
      var helper = new TrialBalanceHelper(_command);

      FixedList<TrialBalanceEntry> postingEntries = GetPostingEntries();

      List<TrialBalanceEntry> summaryEntries = helper.GenerateSummaryEntries(postingEntries);

      List<TrialBalanceEntry> trialBalance = helper.CombineSummaryAndPostingEntries(summaryEntries, postingEntries);

      return helper.RestrictLevels(trialBalance);
    }

    #endregion Helper methods

  }  // class TrialBalanceCases

}  // namespace Empiria.FinancialAccounting.BalanceEngine
