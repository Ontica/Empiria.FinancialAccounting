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

      List<TrialBalanceEntry> _accountEntries = trialBalanceHelper.GetSummaryAccountEntriesAndSectorization(
                                                accountEntriesMapped);

      List<TrialBalanceEntry> parentEntriesAndSectorization =
                              trialBalanceHelper.GetSummaryAccountEntriesAndSectorization(parentAccounts);

      List<TrialBalanceEntry> parentsAndAccountEntries = helper.CombineSummaryAndPostingEntries(
                                             parentEntriesAndSectorization, _accountEntries.ToFixedList());

      parentsAndAccountEntries = GenerateTotalsForBalances(parentsAndAccountEntries, accountEntries);

      trialBalanceHelper.RestrictLevels(parentsAndAccountEntries);

      var returnBalance = new FixedList<ITrialBalanceEntry>(
                              parentsAndAccountEntries.Select(x => (ITrialBalanceEntry) x));

      return new TrialBalance(_query, returnBalance);
    }


    #region Private methods


    private List<TrialBalanceEntry> GenerateTotalsForBalances(List<TrialBalanceEntry> entriesList,
                                     FixedList<TrialBalanceEntry> accountEntries) {

      var trialBalanceHelper = new TrialBalanceHelper(_query);
      var helper = new SaldosPorCuentaHelper(_query);

      List<TrialBalanceEntry> parentsAndAccountEntries = new List<TrialBalanceEntry>(entriesList);

      List<TrialBalanceEntry> summaryTotalDebtorCreditorEntries =
                              helper.GenerateTotalsDebtorOrCreditor(accountEntries);

      List<TrialBalanceEntry> totalDebtorCreditorAndAccountEntries = 
                              helper.CombineDebtorCreditorAndPostingEntries(
                              parentsAndAccountEntries, summaryTotalDebtorCreditorEntries);

      List<TrialBalanceEntry> totalsByCurrency = helper.GenerateTotalsByCurrency(
                                                              summaryTotalDebtorCreditorEntries);

      List<TrialBalanceEntry> totalsByCurrencyAndAccountEntries =
                              helper.CombineCurrencyTotalsAndPostingEntries(
                              totalDebtorCreditorAndAccountEntries, totalsByCurrency);

      List<TrialBalanceEntry> summaryTrialBalanceConsolidated =
                              trialBalanceHelper.GenerateTotalSummaryConsolidated(totalsByCurrency);

      List<TrialBalanceEntry> totalConsolidatedAndAccountEntries =
                              trialBalanceHelper.CombineTotalConsolidatedAndAccountEntries(
                              totalsByCurrencyAndAccountEntries, summaryTrialBalanceConsolidated);

      List<TrialBalanceEntry> returnedAccountEntries = trialBalanceHelper.TrialBalanceWithSubledgerAccounts(
                                                       totalConsolidatedAndAccountEntries);

      return returnedAccountEntries;
    }

    
    #endregion

  } // class SaldosPorCuentaBuilder

} // namespace Empiria.FinancialAccounting.BalanceEngine
