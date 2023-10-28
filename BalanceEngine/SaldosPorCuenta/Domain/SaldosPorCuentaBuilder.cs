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
      var trialBalanceHelper = new TrialBalanceHelper(_query);

      FixedList<TrialBalanceEntry> accountEntries = trialBalanceHelper.GetAccountEntries();

      if (accountEntries.Count == 0) {
        return new TrialBalance(_query, new FixedList<ITrialBalanceEntry>());
      }

      List<TrialBalanceEntry> parentAccounts = helper.GetCalculatedParentAccounts(accountEntries);


      trialBalanceHelper.SetSummaryToParentEntries(accountEntries);

      List<TrialBalanceEntry> accountEntriesMapped = trialBalanceHelper.GetEntriesMappedForSectorization(
                                              accountEntries.ToList());

      List<TrialBalanceEntry> _accountEntries = trialBalanceHelper.GetSummaryAccountsAndSectorization(
                                                accountEntriesMapped);

      List<TrialBalanceEntry> parentEntriesAndSectorization =
                              trialBalanceHelper.GetSummaryAccountsAndSectorization(parentAccounts);

      List<TrialBalanceEntry> parentsAndAccountEntries = helper.CombineSummaryAndPostingEntries(
                                             parentEntriesAndSectorization, _accountEntries.ToFixedList());

      parentsAndAccountEntries = GenerateTotalsForBalances(parentsAndAccountEntries, accountEntries);

      trialBalanceHelper.RestrictLevels(parentsAndAccountEntries);

      var returnBalance = new FixedList<ITrialBalanceEntry>(
                              parentsAndAccountEntries.Select(x => (ITrialBalanceEntry) x));

      return new TrialBalance(_query, returnBalance);
    }


    #region Private methods


    private List<TrialBalanceEntry> GenerateTotalsForBalances(List<TrialBalanceEntry> parentsAccountEntries,
                                     FixedList<TrialBalanceEntry> accountEntries) {

      if (accountEntries.Count == 0) {
        return parentsAccountEntries;
      }

      var helper = new SaldosPorCuentaHelper(_query);

      List<TrialBalanceEntry> parentsAndAccountEntries = new List<TrialBalanceEntry>(parentsAccountEntries);

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

      TrialBalanceEntry totalConsolidated = helper.GenerateTotalConsolidated(totalsByCurrency);

      if (totalConsolidated != null) {
        totalsByCurrencyAndAccountEntries.Add(totalConsolidated);
      }

      List<TrialBalanceEntry> returnedBalances = helper.AccountEntriesWithSubledgerAccounts(
                                                       totalsByCurrencyAndAccountEntries);

      return returnedBalances;
    }


    #endregion

  } // class SaldosPorCuentaBuilder

} // namespace Empiria.FinancialAccounting.BalanceEngine
