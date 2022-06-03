/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Service provider                        *
*  Type     : TrialBalanceBuilder                        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Genera los datos para el reporte de balanzas tradicionales.                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;
using Empiria.Collections;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Genera los datos para el reporte de balanzas tradicionales.</summary>
  internal class TrialBalanceBuilder {

    private readonly TrialBalanceQuery _query;

    internal TrialBalanceBuilder(TrialBalanceQuery query) {
      _query = query;
    }


    internal TrialBalance Build() {
      var helper = new TrialBalanceHelper(_query);

      var startTime = DateTime.Now;

      EmpiriaLog.Debug($"START BalanzaTradicional: {startTime}");

      FixedList<TrialBalanceEntry> postingEntries = helper.GetPostingEntries();

      helper.SetSummaryToParentEntries(postingEntries);

      List<TrialBalanceEntry> summaryEntries = helper.GetCalculatedParentAccounts(postingEntries);

      EmpiriaLog.Debug($"AFTER GenerateSummaryEntries: {DateTime.Now.Subtract(startTime).TotalSeconds} seconds.");

      List<TrialBalanceEntry> postingEntriesMapped = helper.GetEntriesMappedForSectorization(
                                              postingEntries.ToList());

      List<TrialBalanceEntry> _postingEntries = helper.GetSummaryAccountEntriesAndSectorization(
                                                postingEntriesMapped);

      EmpiriaLog.Debug($"AFTER GetSummaryEntriesAndSectorization (postingEntries): {DateTime.Now.Subtract(startTime).TotalSeconds} seconds.");

      List<TrialBalanceEntry> summaryEntriesAndSectorization =
                              helper.GetSummaryAccountEntriesAndSectorization(summaryEntries);

      EmpiriaLog.Debug($"AFTER GetSummaryEntriesAndSectorization (summaryEntries): {DateTime.Now.Subtract(startTime).TotalSeconds} seconds.");

      List<TrialBalanceEntry> trialBalance = helper.CombineSummaryAndPostingEntries(
                                             summaryEntriesAndSectorization, _postingEntries.ToFixedList());

      EmpiriaLog.Debug($"AFTER CombineSummaryAndPostingEntries: {DateTime.Now.Subtract(startTime).TotalSeconds} seconds.");

      trialBalance = GetTrialBalanceType(trialBalance, postingEntries);

      EmpiriaLog.Debug($"AFTER GetTrialBalanceType: {DateTime.Now.Subtract(startTime).TotalSeconds} seconds.");

      helper.RestrictLevels(trialBalance);

      EmpiriaLog.Debug($"AFTER RestrictLevels: {DateTime.Now.Subtract(startTime).TotalSeconds} seconds.");

      var returnBalance = new FixedList<ITrialBalanceEntry>(
                              trialBalance.Select(x => (ITrialBalanceEntry) x));

      EmpiriaLog.Debug($"END BalanzaTradicional: {DateTime.Now.Subtract(startTime).TotalSeconds} seconds.");

      return new TrialBalance(_query, returnBalance);
    }


    #region Private methods


    private List<TrialBalanceEntry> GetTrialBalanceType(List<TrialBalanceEntry> trialBalance,
                                                        FixedList<TrialBalanceEntry> postingEntries) {
      if (!_query.IsOperationalReport) {

        trialBalance = GenerateTrialBalance(trialBalance, postingEntries);

      } else {
        trialBalance = GenerateOperationalBalance(trialBalance);
      }
      return trialBalance;
    }


    private List<TrialBalanceEntry> GenerateTrialBalance(List<TrialBalanceEntry> trialBalance,
                                     FixedList<TrialBalanceEntry> postingEntries) {
      var helper = new TrialBalanceHelper(_query);

      List<TrialBalanceEntry> returnedTrialBalance = new List<TrialBalanceEntry>();

      FixedList<TrialBalanceEntry> summaryGroupEntries = helper.GenerateTotalSummaryGroups(postingEntries);

      returnedTrialBalance = helper.CombineGroupEntriesAndPostingEntries(trialBalance, summaryGroupEntries);

      List<TrialBalanceEntry> summaryTotalDebtorCreditorEntries =
                              helper.GenerateTotalSummaryDebtorCreditor(postingEntries.ToList());

      returnedTrialBalance = helper.CombineDebtorCreditorAndPostingEntries(returnedTrialBalance,
                                                                   summaryTotalDebtorCreditorEntries);

      List<TrialBalanceEntry> summaryTotalCurrencies = helper.GenerateTotalSummaryCurrency(
                                                              summaryTotalDebtorCreditorEntries);

      returnedTrialBalance = helper.CombineCurrencyTotalsAndPostingEntries(returnedTrialBalance, summaryTotalCurrencies);

      List<TrialBalanceEntry> summaryTrialBalanceConsolidated = helper.GenerateTotalSummaryConsolidated(
                                                                     summaryTotalCurrencies);

      returnedTrialBalance = helper.CombineTotalConsolidatedAndAccountEntries(
                            returnedTrialBalance, summaryTrialBalanceConsolidated);

      returnedTrialBalance = helper.TrialBalanceWithSubledgerAccounts(returnedTrialBalance);

      return returnedTrialBalance;
    }


    private List<TrialBalanceEntry> GenerateOperationalBalance(List<TrialBalanceEntry> trialBalance) {
      var helper = new TrialBalanceHelper(_query);
      var totalByAccountEntries = new EmpiriaHashTable<TrialBalanceEntry>(trialBalance.Count);

      if (_query.ConsolidateBalancesToTargetCurrency == true) {

        foreach (var entry in trialBalance) {
          helper.SummaryByAccount(totalByAccountEntries, entry);
        }

        return totalByAccountEntries.ToFixedList().ToList();

      } else {

        return trialBalance;

      }
    }

    #endregion

  }  // class TrialBalanceBuilder

}  // namespace Empiria.FinancialAccounting.BalanceEngine
