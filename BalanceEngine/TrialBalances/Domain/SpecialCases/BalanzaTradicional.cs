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
using Empiria.Collections;
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


      if (_command.TrialBalanceType == TrialBalanceType.Saldos) {
        _command.WithSubledgerAccount = true;
      }

      var startTime = DateTime.Now;

      EmpiriaLog.Debug($"START BalanzaTradicional: {startTime}");

      FixedList<TrialBalanceEntry> postingEntries = helper.GetPostingEntries();

      postingEntries = helper.GetSummaryToParentEntries(postingEntries);

      List<TrialBalanceEntry> summaryEntries = helper.GenerateSummaryEntries(postingEntries);

      EmpiriaLog.Debug($"AFTER GenerateSummaryEntries: {DateTime.Now.Subtract(startTime).TotalSeconds} seconds.");

      List<TrialBalanceEntry> _postingEntries = helper.GetSummaryEntriesAndSectorization(
                                                postingEntries.ToList());

      EmpiriaLog.Debug($"AFTER GetSummaryEntriesAndSectorization (postingEntries): {DateTime.Now.Subtract(startTime).TotalSeconds} seconds.");

      List<TrialBalanceEntry> summaryEntriesAndSectorization =
                              helper.GetSummaryEntriesAndSectorization(summaryEntries);

      EmpiriaLog.Debug($"AFTER GetSummaryEntriesAndSectorization (summaryEntries): {DateTime.Now.Subtract(startTime).TotalSeconds} seconds.");

      List<TrialBalanceEntry> trialBalance = helper.CombineSummaryAndPostingEntries(
                                             summaryEntriesAndSectorization, _postingEntries.ToFixedList());

      EmpiriaLog.Debug($"AFTER CombineSummaryAndPostingEntries: {DateTime.Now.Subtract(startTime).TotalSeconds} seconds.");

      trialBalance = GetTrialBalanceType(trialBalance, postingEntries);

      EmpiriaLog.Debug($"AFTER GetTrialBalanceType: {DateTime.Now.Subtract(startTime).TotalSeconds} seconds.");

      trialBalance = helper.RestrictLevels(trialBalance);

      EmpiriaLog.Debug($"AFTER RestrictLevels: {DateTime.Now.Subtract(startTime).TotalSeconds} seconds.");

      var returnBalance = new FixedList<ITrialBalanceEntry>(
                              trialBalance.Select(x => (ITrialBalanceEntry) x));

      EmpiriaLog.Debug($"END BalanzaTradicional: {DateTime.Now.Subtract(startTime).TotalSeconds} seconds.");

      return new TrialBalance(_command, returnBalance);
    }


    #region Private methods


    private List<TrialBalanceEntry> GetTrialBalanceType(List<TrialBalanceEntry> trialBalance,
                                                        FixedList<TrialBalanceEntry> postingEntries) {
      if (!_command.IsOperationalReport) {

        trialBalance = GenerateTrialBalance(trialBalance, postingEntries);

      } else {
        trialBalance = GenerateOperationalBalance(trialBalance);
      }
      return trialBalance;
    }


    private List<TrialBalanceEntry> GenerateTrialBalance(List<TrialBalanceEntry> trialBalance,
                                     FixedList<TrialBalanceEntry> postingEntries) {
      var helper = new TrialBalanceHelper(_command);

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

      List<TrialBalanceEntry> summaryTotalConsolidatedByLedger =
                              helper.GenerateTotalSummaryConsolidatedByLedger(summaryTotalCurrencies);

      returnedTrialBalance = helper.CombineTotalConsolidatedByLedgerAndPostingEntries(
                            returnedTrialBalance, summaryTotalConsolidatedByLedger);

      List<TrialBalanceEntry> summaryTrialBalanceConsolidated = helper.GenerateTotalSummaryConsolidated(
                                                                     summaryTotalCurrencies);

      returnedTrialBalance = helper.CombineTotalConsolidatedAndPostingEntries(
                            returnedTrialBalance, summaryTrialBalanceConsolidated);

      returnedTrialBalance = helper.TrialBalanceWithSubledgerAccounts(returnedTrialBalance);

      return returnedTrialBalance;
    }

    private List<TrialBalanceEntry> GenerateOperationalBalance(List<TrialBalanceEntry> trialBalance) {
      var helper = new TrialBalanceHelper(_command);
      var totalByAccountEntries = new EmpiriaHashTable<TrialBalanceEntry>(trialBalance.Count);

      if (_command.ConsolidateBalancesToTargetCurrency == true) {

        foreach (var entry in trialBalance) {
          helper.SummaryByAccount(totalByAccountEntries, entry);
        }

        return totalByAccountEntries.ToFixedList().ToList();

      } else {

        return trialBalance;

      }
    }

    #endregion

  }  // class BalanzaTradicional

}  // namespace Empiria.FinancialAccounting.BalanceEngine
