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

      FixedList<TrialBalanceEntry> postingEntries = helper.GetPostingEntries();

      List<TrialBalanceEntry> summaryEntries = helper.GenerateSummaryEntries(postingEntries);

      List<TrialBalanceEntry> _postingEntries = helper.GetSummaryEntriesAndSectorization(
                                                postingEntries.ToList());

      List<TrialBalanceEntry> summaryEntriesAndSectorization =
                              helper.GetSummaryEntriesAndSectorization(summaryEntries);

      List<TrialBalanceEntry> trialBalance = helper.CombineSummaryAndPostingEntries(
                                             summaryEntriesAndSectorization, _postingEntries.ToFixedList());

      trialBalance = GetTrialBalanceType(trialBalance, postingEntries);

      trialBalance = helper.RestrictLevels(trialBalance);

      //var ensureIsValid = new EnsureBalanceValidations();

      //ensureIsValid.EnsureIsValid(trialBalance, postingEntries);

      var returnBalance = new FixedList<ITrialBalanceEntry>(
                              trialBalance.Select(x => (ITrialBalanceEntry) x));

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

      trialBalance = helper.TrialBalanceWithSubledgerAccounts(trialBalance);

      //trialBalance = helper.GenerateAverageBalance(trialBalance);

      return trialBalance;
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
