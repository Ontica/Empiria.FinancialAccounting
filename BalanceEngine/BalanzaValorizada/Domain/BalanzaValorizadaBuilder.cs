/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Service provider                        *
*  Type     : BalanzaValorizada                          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Genera los datos para el reporte de balanza valorizada en dolares.                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;

using Empiria.Collections;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Genera los datos para el reporte de balanza valorizada en dolares.</summary>
  internal class BalanzaValorizadaBuilder {

    private readonly TrialBalanceQuery _query;

    internal BalanzaValorizadaBuilder(TrialBalanceQuery query) {
      _query = query;
    }


    internal TrialBalance Build() {
      var balanceHelper = new TrialBalanceHelper(_query);
      var helper = new BalanzaValorizadaHelper(_query);

      List<TrialBalanceEntry> trialBalance = balanceHelper.GetPostingEntries().ToList();

      // Special case summaries y posting al mismo tiempo.
      balanceHelper.SetSummaryToParentEntries(trialBalance);

      List<TrialBalanceEntry> summaryEntries =
                               balanceHelper.GetCalculatedParentAccounts(trialBalance.ToFixedList());

      summaryEntries = helper.GetAccountList(trialBalance, summaryEntries);

      EmpiriaHashTable<TrialBalanceEntry> ledgerAccounts = helper.GetEntriesWithItemType(summaryEntries);

      List<TrialBalanceEntry> orderingBalance =
                                helper.OrderingDollarizedBalance(ledgerAccounts.ToFixedList());

      FixedList<TrialBalanceEntry> valuedEntries = helper.ValuateToExchangeRate(
                                                   orderingBalance.ToFixedList());

      List<BalanzaValorizadaEntry> mergeBalancesToValuedBalances =
                                      helper.MergeTrialBalanceIntoValuedBalances(valuedEntries);

      List<BalanzaValorizadaEntry> asignExchageRateAndTotalToBalances =
                                      helper.GetExchangeRateByValuedEntry(mergeBalancesToValuedBalances);

      var returnBalance = new FixedList<ITrialBalanceEntry>(
                                asignExchageRateAndTotalToBalances.Select(x => (ITrialBalanceEntry) x));

      return new TrialBalance(_query, returnBalance);
    }


    internal TrialBalance BuildBalanceInColumnsByCurrency() {
      var balanceHelper = new TrialBalanceHelper(_query);
      var helper = new BalanzaValorizadaHelper(_query);

      List<TrialBalanceEntry> trialBalance = balanceHelper.GetPostingEntries().ToList();

      balanceHelper.SetSummaryToParentEntries(trialBalance);

      List<TrialBalanceEntry> parentAccountsEntries =
                                balanceHelper.GetCalculatedParentAccounts(trialBalance.ToFixedList());

      parentAccountsEntries = helper.GetSummaryByDebtorCreditorEntries(parentAccountsEntries);

      parentAccountsEntries = helper.GetAccountList(trialBalance, parentAccountsEntries);

      EmpiriaHashTable<TrialBalanceEntry> ledgerAccounts =
                                          helper.GetLedgerAccountsListByCurrency(parentAccountsEntries);

      List<TrialBalanceByCurrencyEntry> mergeBalancesToBalanceByCurrency =
                      helper.MergeTrialBalanceIntoBalanceByCurrency(ledgerAccounts.ToFixedList());

      var returnBalance = new FixedList<ITrialBalanceEntry>(
                            mergeBalancesToBalanceByCurrency.Select(x => (ITrialBalanceEntry) x));

      return new TrialBalance(_query, returnBalance);
    }


  } // class BalanzaValorizada

} // namespace Empiria.FinancialAccounting.BalanceEngine
