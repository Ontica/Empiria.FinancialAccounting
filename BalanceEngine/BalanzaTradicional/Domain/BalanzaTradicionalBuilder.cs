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

      FixedList<TrialBalanceEntry> accountEntries = balanzaHelper.GetPostingEntries();

      FixedList<TrialBalanceEntry> parentAccounts = balanzaHelper.GetCalculatedParentAccounts(
                                                    accountEntries);

      var trialBalanceHelper = new TrialBalanceHelper(_query);

      trialBalanceHelper.SetSummaryToParentEntries(accountEntries);

      List<TrialBalanceEntry> accountEntriesMapped = trialBalanceHelper.GetEntriesMappedForSectorization(
                                                     accountEntries.ToList());

      List<TrialBalanceEntry> accountEntriesAndSectorization =
        trialBalanceHelper.GetSummaryAccountEntriesAndSectorization(accountEntriesMapped);

      List<TrialBalanceEntry> parentAccountEntriesAndSectorization =
        trialBalanceHelper.GetSummaryAccountEntriesAndSectorization(parentAccounts.ToList());


      List<TrialBalanceEntry> parentsAndAccountEntries = balanzaHelper.CombineParentsAndAccountEntries(
                                                         parentAccountEntriesAndSectorization,
                                                         accountEntriesAndSectorization);

      List<TrialBalanceEntry> balanzaTradicional = GenerateTotalsAndCombineWithAccountEntries(
                                                   parentsAndAccountEntries, accountEntries);

      trialBalanceHelper.RestrictLevels(balanzaTradicional);

      var returnBalance = new FixedList<ITrialBalanceEntry>(
                              balanzaTradicional.Select(x => (ITrialBalanceEntry) x));

      return new TrialBalance(_query, returnBalance);
    }


    #region Private methods


    private List<TrialBalanceEntry> GenerateTotalsAndCombineWithAccountEntries(
                                    List<TrialBalanceEntry> balanceEntries,
                                    FixedList<TrialBalanceEntry> accountEntries) {

      var helper = new BalanzaTradicionalHelper(_query);

      FixedList<TrialBalanceEntry> groupTotalsEntries = helper.GenerateTotalGroupEntries(
                                                         accountEntries);

      List<TrialBalanceEntry> totalGroupAndAccountEntries =
                              helper.CombineTotalGroupEntriesAndAccountEntries(
                              balanceEntries, groupTotalsEntries);

      List<TrialBalanceEntry> totalDebtorCreditorEntries =
                              helper.GenerateTotalDebtorCreditorsByCurrency(accountEntries.ToList());

      List<TrialBalanceEntry> totalDebtorCreditorsAndAccountEntries = 
                              helper.CombineTotalDebtorCreditorsByCurrencyAndAccountEntries(
                              totalGroupAndAccountEntries, totalDebtorCreditorEntries);

      List<TrialBalanceEntry> totalsByCurrency = helper.GenerateTotalByCurrency(
                                                totalDebtorCreditorEntries);

      List<TrialBalanceEntry> totalByCurrencyAndAccountEntries = 
                              helper.CombineTotalsByCurrencyAndAccountEntries(
                              totalDebtorCreditorsAndAccountEntries, totalsByCurrency);

      if (_query.ShowCascadeBalances) {
        List<TrialBalanceEntry> totalsConsolidatedByLedger =
                              helper.GenerateTotalsConsolidatedByLedger(totalsByCurrency);

        totalByCurrencyAndAccountEntries = helper.CombineTotalConsolidatedByLedgerAndAccountEntries(
                                           totalByCurrencyAndAccountEntries, totalsConsolidatedByLedger);
      }

      List<TrialBalanceEntry> returnedBalance = new List<TrialBalanceEntry>(totalByCurrencyAndAccountEntries);

      TrialBalanceEntry totalConsolidated = helper.GenerateTotalConsolidated(totalsByCurrency);
      returnedBalance.Add(totalConsolidated);

      return returnedBalance;
    }


    #endregion

  }  // class BalanzaTradicional

}  // namespace Empiria.FinancialAccounting.BalanceEngine
