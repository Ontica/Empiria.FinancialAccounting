/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Service provider                        *
*  Type     : SaldosPorCuentaBuilder                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Genera los datos para el reporte de saldos por cuenta.                                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;
using Empiria.Collections;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Genera los datos para el reporte de saldos por cuenta.</summary>
  internal class SaldosPorCuentaBuilder {

    private readonly TrialBalanceQuery _query;

    internal SaldosPorCuentaBuilder(TrialBalanceQuery query) {
      _query = query;
    }


    internal TrialBalance Build() {
      var helper = new TrialBalanceHelper(_query);


      if (_query.TrialBalanceType == TrialBalanceType.Saldos) {
        _query.WithSubledgerAccount = true;
      }

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

      trialBalance = GenerateTrialBalance(trialBalance, postingEntries);

      EmpiriaLog.Debug($"AFTER GetTrialBalanceType: {DateTime.Now.Subtract(startTime).TotalSeconds} seconds.");

      helper.RestrictLevels(trialBalance);

      EmpiriaLog.Debug($"AFTER RestrictLevels: {DateTime.Now.Subtract(startTime).TotalSeconds} seconds.");

      var returnBalance = new FixedList<ITrialBalanceEntry>(
                              trialBalance.Select(x => (ITrialBalanceEntry) x));

      EmpiriaLog.Debug($"END BalanzaTradicional: {DateTime.Now.Subtract(startTime).TotalSeconds} seconds.");

      return new TrialBalance(_query, returnBalance);
    }


    #region Private methods


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

    
    #endregion

  } // class SaldosPorCuentaBuilder

} // namespace Empiria.FinancialAccounting.BalanceEngine
