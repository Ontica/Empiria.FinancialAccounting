/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Service provider                        *
*  Type     : BalanzaColumnasMonedaBuilder               License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Genera los datos para el reporte de balanza en columnas por moneda.                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;
using Empiria.Collections;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine {


  /// <summary>Genera los datos para el reporte de balanza en columnas por moneda.</summary>
  internal class BalanzaColumnasMonedaBuilder {

    private readonly TrialBalanceQuery Query;

    internal BalanzaColumnasMonedaBuilder(TrialBalanceQuery query) {
      Query = query;
    }


    internal TrialBalance Build() {

      var balanceHelper = new TrialBalanceHelper(Query);
      var helper = new BalanzaColumnasMonedaHelper(Query);

      List<TrialBalanceEntry> accountEntries = balanceHelper.GetPostingEntries().ToList();

      balanceHelper.SetSummaryToParentEntries(accountEntries);

      List<TrialBalanceEntry> parentAccountsEntries =
                                balanceHelper.GetCalculatedParentAccounts(accountEntries.ToFixedList());

      List<TrialBalanceEntry> debtorAccounts = helper.GetSumFromCreditorToDebtorAccounts(
                                                        parentAccountsEntries);

      helper.CombineAccountEntriesAndDebtorAccounts(accountEntries, debtorAccounts);

      FixedList<TrialBalanceEntry> accountEntriesByCurrency =
                                          helper.GetAccountEntriesByCurrency(debtorAccounts);

      List<BalanzaColumnasMonedaEntry> mergeBalancesToBalanceByCurrency =
                      helper.MergeTrialBalanceIntoBalanceByCurrency(accountEntriesByCurrency);

      var returnBalance = new FixedList<ITrialBalanceEntry>(
                            mergeBalancesToBalanceByCurrency.Select(x => (ITrialBalanceEntry) x));

      return new TrialBalance(Query, returnBalance);
    }


  } // class BalanzaColumnasMonedaBuilder

} // namespace Empiria.FinancialAccounting.BalanceEngine
