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
  internal class BalanzaTradicionalBuilder {

    private readonly TrialBalanceQuery _query;

    internal BalanzaTradicionalBuilder(TrialBalanceQuery query) {
      _query = query;
    }


    internal TrialBalance Build() {
      var balanzaHelper = new BalanzaTradicionalHelper(_query);

      if (_query.TrialBalanceType == TrialBalanceType.Saldos) {
        _query.WithSubledgerAccount = true;
      }

      var startTime = DateTime.Now;

      EmpiriaLog.Debug($"START BalanzaTradicional: {startTime}");

      FixedList<TrialBalanceEntry> accountEntries = balanzaHelper.GetPostingEntries();

      FixedList<TrialBalanceEntry> parentAccounts = balanzaHelper.GetCalculatedParentAccounts(
                                                    accountEntries);

      EmpiriaLog.Debug($"AFTER GetCalculatedParentAccounts: {DateTime.Now.Subtract(startTime).TotalSeconds} seconds.");

      var helper = new TrialBalanceHelper(_query);

      helper.SetSummaryToParentEntries(accountEntries);

      List<TrialBalanceEntry> accountEntriesMapped = helper.GetEntriesMappedForSectorization(
                                                     accountEntries.ToList());

      List<TrialBalanceEntry> accountEntriesAndSectorization =
                              helper.GetSummaryAccountEntriesAndSectorization(accountEntriesMapped);

      EmpiriaLog.Debug($"AFTER GetSummaryAccountEntriesAndSectorization (postingEntries): {DateTime.Now.Subtract(startTime).TotalSeconds} seconds.");

      List<TrialBalanceEntry> parentAccountEntriesAndSectorization =
                              helper.GetSummaryAccountEntriesAndSectorization(parentAccounts.ToList());

      EmpiriaLog.Debug($"AFTER GetSummaryAccountEntriesAndSectorization (summaryEntries): {DateTime.Now.Subtract(startTime).TotalSeconds} seconds.");


      List<TrialBalanceEntry> balanzaTradicional = balanzaHelper.CombineParentsAndAccountEntries(
                                                   parentAccountEntriesAndSectorization,
                                                   accountEntriesAndSectorization);

      EmpiriaLog.Debug($"AFTER CombineSummaryAndPostingEntries: {DateTime.Now.Subtract(startTime).TotalSeconds} seconds.");

      balanzaTradicional = GenerateTotalsAndCombineWithAccountEntries(balanzaTradicional, accountEntries);

      EmpiriaLog.Debug($"AFTER GetTrialBalanceType: {DateTime.Now.Subtract(startTime).TotalSeconds} seconds.");

      helper.RestrictLevels(balanzaTradicional);

      EmpiriaLog.Debug($"AFTER RestrictLevels: {DateTime.Now.Subtract(startTime).TotalSeconds} seconds.");

      var returnBalance = new FixedList<ITrialBalanceEntry>(
                              balanzaTradicional.Select(x => (ITrialBalanceEntry) x));

      EmpiriaLog.Debug($"END BalanzaTradicional: {DateTime.Now.Subtract(startTime).TotalSeconds} seconds.");

      return new TrialBalance(_query, returnBalance);
    }


    #region Private methods


    private List<TrialBalanceEntry> GenerateTotalsAndCombineWithAccountEntries(
                                    List<TrialBalanceEntry> balanzaTradicional,
                                    FixedList<TrialBalanceEntry> accountEntries) {

      var helper = new TrialBalanceHelper(_query);
      var balanzaHelper = new BalanzaTradicionalHelper(_query);

      FixedList<TrialBalanceEntry> groupTotalsEntries = balanzaHelper.GenerateTotalGroupEntries(
                                                         accountEntries);

      List<TrialBalanceEntry> returnedBalance =
                              balanzaHelper.CombineTotalGroupEntriesAndAccountEntries(
                              balanzaTradicional, groupTotalsEntries);

      List<TrialBalanceEntry> totalDebtorCreditorEntries =
                              balanzaHelper.GenerateTotalDebtorCreditorsByCurrency(accountEntries.ToList());

      returnedBalance = balanzaHelper.CombineTotalDebtorCreditorsByCurrencyAndAccountEntries(
                        returnedBalance, totalDebtorCreditorEntries);

      List<TrialBalanceEntry> totalsByCurrency = balanzaHelper.GenerateTotalByCurrency(
                                                totalDebtorCreditorEntries);

      returnedBalance = balanzaHelper.CombineTotalsByCurrencyAndAccountEntries(
                        returnedBalance, totalsByCurrency);

      if (_query.ShowCascadeBalances) {
        List<TrialBalanceEntry> totalsConsolidatedByLedger =
                              balanzaHelper.GenerateTotalsConsolidatedByLedger(totalsByCurrency);

        returnedBalance = balanzaHelper.CombineTotalConsolidatedByLedgerAndAccountEntries(
                              returnedBalance, totalsConsolidatedByLedger);

      }

      List<TrialBalanceEntry> summaryTrialBalanceConsolidated = helper.GenerateTotalSummaryConsolidated(
                                                                     totalsByCurrency);

      returnedBalance = helper.CombineTotalConsolidatedAndPostingEntries(
                            returnedBalance, summaryTrialBalanceConsolidated);

      returnedBalance = helper.TrialBalanceWithSubledgerAccounts(returnedBalance);

      return returnedBalance;
    }


    #endregion

  }  // class BalanzaTradicional

}  // namespace Empiria.FinancialAccounting.BalanceEngine
