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
  internal class BalanzaValorizada {

    private readonly TrialBalanceCommand _command;

    public BalanzaValorizada(TrialBalanceCommand command) {
      _command = command;
    }


    internal TrialBalance Build() {
      var balanceHelper = new TrialBalanceHelper(_command);
      var helper = new ValorizedBalanceHelper(_command);

      List<TrialBalanceEntry> trialBalance = balanceHelper.GetPostingEntries().ToList();

      trialBalance = balanceHelper.GetSummaryToParentEntries(trialBalance.ToFixedList()).ToList();

      List<TrialBalanceEntry> summaryEntries = 
                               balanceHelper.GenerateSummaryEntries(trialBalance.ToFixedList());

      summaryEntries = helper.GetAccountList(trialBalance, summaryEntries);

      EmpiriaHashTable<TrialBalanceEntry> ledgerAccounts = helper.GetEntriesWithItemType(summaryEntries);

      List<TrialBalanceEntry> orderingBalance = 
                                helper.OrderingDollarizedBalance(ledgerAccounts.ToFixedList());

      FixedList<TrialBalanceEntry> valuedEntries = helper.ValuateToExchangeRate(
                                    orderingBalance.ToFixedList(), _command.InitialPeriod);

      List<ValuedTrialBalanceEntry> mergeBalancesToValuedBalances =
                                      helper.MergeTrialBalanceIntoValuedBalances(valuedEntries);

      List<ValuedTrialBalanceEntry> asignExchageRateAndTotalToBalances =
                                      helper.GetExchangeRateByValuedEntry(mergeBalancesToValuedBalances);

      var returnBalance = new FixedList<ITrialBalanceEntry>(
                                asignExchageRateAndTotalToBalances.Select(x => (ITrialBalanceEntry) x));

      return new TrialBalance(_command, returnBalance);
    }


    internal TrialBalance BuildBalanceInColumnsByCurrency() {
      var balanceHelper = new TrialBalanceHelper(_command);
      var helper = new ValorizedBalanceHelper(_command);

      List<TrialBalanceEntry> trialBalance = balanceHelper.GetPostingEntries().ToList();

      trialBalance = balanceHelper.GetSummaryToParentEntries(trialBalance.ToFixedList()).ToList();

      List<TrialBalanceEntry> summaryEntries = 
                                balanceHelper.GenerateSummaryEntries(trialBalance.ToFixedList());

      summaryEntries = helper.GetSummaryByDebtorCreditorEntries(summaryEntries);

      summaryEntries = helper.GetAccountList(trialBalance, summaryEntries);

      EmpiriaHashTable<TrialBalanceEntry> ledgerAccounts = 
                                          helper.GetLedgerAccountsListByCurrency(summaryEntries);

      List<TrialBalanceByCurrencyEntry> mergeBalancesToBalanceByCurrency =
                      helper.MergeTrialBalanceIntoBalanceByCurrency(ledgerAccounts.ToFixedList());

      var returnBalance = new FixedList<ITrialBalanceEntry>(
                            mergeBalancesToBalanceByCurrency.Select(x => (ITrialBalanceEntry) x));

      return new TrialBalance(_command, returnBalance);
    }

    
  } // class BalanzaValorizada

} // namespace Empiria.FinancialAccounting.BalanceEngine
