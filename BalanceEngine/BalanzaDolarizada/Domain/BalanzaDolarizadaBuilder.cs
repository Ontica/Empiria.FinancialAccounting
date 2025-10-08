/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Service provider                        *
*  Type     : BalanzaDolarizadaBuilder                   License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Genera los datos para el reporte de balanza valorizada en dolares.                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;

using Empiria.Collections;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.Data;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Genera los datos para el reporte de balanza valorizada en dolares.</summary>
  internal class BalanzaDolarizadaBuilder {

    private readonly TrialBalanceQuery Query;

    internal BalanzaDolarizadaBuilder(TrialBalanceQuery query) {
      Query = query;
    }


    internal FixedList<BalanzaDolarizadaEntry> Build() {

      FixedList<TrialBalanceEntry> baseAccountEntries = BalancesDataService.GetTrialBalanceEntries(Query);

      return Build(baseAccountEntries);
    }


    internal FixedList<BalanzaDolarizadaEntry> Build(FixedList<TrialBalanceEntry> accountEntries) {
      
      if (accountEntries.Count == 0) {
        return new FixedList<BalanzaDolarizadaEntry>();
      }

      var helper = new BalanzaDolarizadaHelper(Query);

      List<TrialBalanceEntry> parentAccountEntries = GetSummaryParentAccountEntries(accountEntries);

      helper.GetAccountList(accountEntries, parentAccountEntries);

      List<TrialBalanceEntry> accountEntriesWithItemType = 
                              helper.AssignItemTypeToAccountEntries(parentAccountEntries);

      List<TrialBalanceEntry> foreignAccountEntries = 
                              helper.GetForeignAccountEntries(accountEntriesWithItemType);

      List<TrialBalanceEntry> valuedAccountEntries = helper.GetDollarizedExchangeRate(
                                                   foreignAccountEntries);

      List<BalanzaDolarizadaEntry> balanzaDolarizada =
                                   helper.MergeAccountsIntoBalanzaDolarizada(valuedAccountEntries);
      
      List<BalanzaDolarizadaEntry> returnedAccountEntries =
                                   helper.GetExchangeRateByAccountEntry(balanzaDolarizada);

      return returnedAccountEntries.ToFixedList();
    }


    private List<TrialBalanceEntry> GetSummaryParentAccountEntries(
                                    FixedList<TrialBalanceEntry> accountEntries) {

      var trialBalanceHelper = new TrialBalanceHelper(Query);

      trialBalanceHelper.RoundDecimals(accountEntries);

      // Special case summaries y posting al mismo tiempo.
      trialBalanceHelper.SetSummaryToParentEntries(accountEntries);

      List<TrialBalanceEntry> parentAccounts =trialBalanceHelper.GetCalculatedParentAccounts(
                                                                  accountEntries.ToFixedList());

      return parentAccounts;

    }
  } // class BalanzaDolarizadaBuilder

} // namespace Empiria.FinancialAccounting.BalanceEngine
