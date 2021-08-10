/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Service provider                        *
*  Type     : BalanzaTradicional                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Genera los datos para el reporte de balanzas tradicionales.                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Genera los datos para el reporte de balanzas tradicionales.</summary>
  internal class BalanzaTradicional {

    private readonly TrialBalanceCommand _command;

    public BalanzaTradicional(TrialBalanceCommand command) {
      _command = command;
    }


    internal TrialBalance Build() {
      var helper = new TrialBalanceHelper(_command);

      FixedList<TrialBalanceEntry> postingEntries = helper.GetPostingEntries();

      List<TrialBalanceEntry> summaryEntries = helper.GenerateSummaryEntries(postingEntries);

      List<TrialBalanceEntry> trialBalance = helper.CombineSummaryAndPostingEntries(
                                                     summaryEntries, postingEntries);

      FixedList<TrialBalanceEntry> summaryGroupEntries = helper.GenerateTotalSummaryGroups(postingEntries);

      trialBalance = helper.CombineGroupEntriesAndPostingEntries(trialBalance, summaryGroupEntries);

      List<TrialBalanceEntry> summaryTotalDebtorCreditorEntries =
                              helper.GenerateTotalSummaryDebtorCreditor(postingEntries.ToList());

      trialBalance = helper.CombineDebtorCreditorAndPostingEntries(trialBalance,
                                                                   summaryTotalDebtorCreditorEntries);

      List<TrialBalanceEntry> summaryTotalCurrencies = helper.GenerateTotalSummaryCurrency(
                                                              summaryTotalDebtorCreditorEntries);

      trialBalance = helper.CombineCurrencyTotalsAndPostingEntries(trialBalance, summaryTotalCurrencies);

      List<TrialBalanceEntry> summaryTotalConsolidatedByLedger =
                              helper.GenerateTotalSummaryConsolidatedByLedger(summaryTotalCurrencies);

      trialBalance = helper.CombineTotalConsolidatedByLedgerAndPostingEntries(
                            trialBalance, summaryTotalConsolidatedByLedger);

      List<TrialBalanceEntry> summaryTrialBalanceConsolidated = helper.GenerateTotalSummaryConsolidated(
                                                                     summaryTotalCurrencies);

      trialBalance = helper.CombineTotalConsolidatedAndPostingEntries(
                            trialBalance, summaryTrialBalanceConsolidated);

      trialBalance = helper.GenerateAverageBalance(trialBalance, _command.InitialPeriod);

      trialBalance = helper.RestrictLevels(trialBalance);

      var returnBalance = new FixedList<ITrialBalanceEntry>(trialBalance.Select(x => (ITrialBalanceEntry) x));

      return new TrialBalance(_command, returnBalance);
    }

  }  // class BalanzaTradicional

}  // namespace Empiria.FinancialAccounting.BalanceEngine
