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
      var helper = new SaldosPorCuentaHelper(_query);

      FixedList<TrialBalanceEntry> accountEntries = helper.GetAccountEntries();

      List<TrialBalanceEntry> parentAccounts = helper.GetCalculatedParentAccounts(accountEntries);

      var trialBalanceHelper = new TrialBalanceHelper(_query);

      trialBalanceHelper.SetSummaryToParentEntries(accountEntries);

      List<TrialBalanceEntry> accountEntriesMapped = trialBalanceHelper.GetEntriesMappedForSectorization(
                                              accountEntries.ToList());

      List<TrialBalanceEntry> _postingEntries = trialBalanceHelper.GetSummaryAccountEntriesAndSectorization(
                                                accountEntriesMapped);

      List<TrialBalanceEntry> summaryEntriesAndSectorization =
                              trialBalanceHelper.GetSummaryAccountEntriesAndSectorization(parentAccounts);

      List<TrialBalanceEntry> trialBalance = helper.CombineSummaryAndPostingEntries(
                                             summaryEntriesAndSectorization, _postingEntries.ToFixedList());

      trialBalance = GenerateTrialBalance(trialBalance, accountEntries);

      trialBalanceHelper.RestrictLevels(trialBalance);

      var returnBalance = new FixedList<ITrialBalanceEntry>(
                              trialBalance.Select(x => (ITrialBalanceEntry) x));

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
