/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Service provider                        *
*  Type     : BalanzaContabilidadesCascada               License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Genera los datos para el reporte de balanza con contabilidades en cascada.                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Genera los datos para el reporte de balanza con contabilidades en cascada.</summary>
  internal class BalanzaContabilidadesCascadaBuilder {

    private readonly TrialBalanceQuery _query;

    public BalanzaContabilidadesCascadaBuilder(TrialBalanceQuery query) {
      _query = query;
    }


    internal TrialBalance Build() {
      var trialBalanceHelper = new TrialBalanceHelper(_query);
      var helper = new BalanzaContabilidadesCascadaHelper(_query);

      FixedList<TrialBalanceEntry> accountEntries = trialBalanceHelper.GetAccountEntriesV2();

      if (accountEntries.Count == 0) {
        return new TrialBalance(_query, new FixedList<ITrialBalanceEntry>());
      }

      trialBalanceHelper.SetSummaryToParentEntries(accountEntries);

      List<TrialBalanceEntry> accountEntriesOrdered =
                              helper.OrderingAccountEntries(accountEntries);

      FixedList<TrialBalanceEntry> totalByAccountEntries =
                                   helper.GenerateTotalsByAccountAndLedger(accountEntriesOrdered);

      List<TrialBalanceEntry> accountEntriesWithTotals = helper.CombineTotalsWithAccountEntries(
                                                         accountEntries, totalByAccountEntries);

      List<TrialBalanceEntry> totalsByDebtorCreditor =
                                helper.GenerateTotalsByDebtorCreditor(accountEntriesOrdered);

      List<TrialBalanceEntry> accountEntriesAndTotalsByDebtorCreditor =
        helper.CombineTotalDebtorCreditorAndEntries(accountEntriesWithTotals, totalsByDebtorCreditor);

      FixedList<TrialBalanceEntry> totalsByCurrency = helper.GenerateTotalsByCurrency(
                                                            totalsByDebtorCreditor);

      List<TrialBalanceEntry> accountEntriesAndTotalsByCurrency =
        helper.CombineTotalsByCurrencyAndAccountEntries(accountEntriesAndTotalsByDebtorCreditor,
                                                        totalsByCurrency);

      TrialBalanceEntry totalReport = helper.GenerateTotalReport(totalsByCurrency);
      accountEntriesAndTotalsByCurrency.Add(totalReport);

      List<TrialBalanceEntry> accountEntriesAndAverageBalance = helper.GetAverageBalance(
                                                                accountEntriesAndTotalsByCurrency);

      trialBalanceHelper.RestrictLevels(accountEntriesAndAverageBalance);

      var returnBalance = new FixedList<ITrialBalanceEntry>(
                              accountEntriesAndAverageBalance.Select(x => (ITrialBalanceEntry) x));

      return new TrialBalance(_query, returnBalance);
    }



    #region Helper methods



    #endregion Helper methods

  }  // class BalanzaContabilidadesCascada

}  // namespace Empiria.FinancialAccounting.BalanceEngine
