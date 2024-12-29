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
using Empiria.FinancialAccounting.BalanceEngine.Data;

namespace Empiria.FinancialAccounting.BalanceEngine {


  /// <summary>Genera los datos para el reporte de balanza en columnas por moneda.</summary>
  internal class BalanzaColumnasMonedaBuilder {

    private readonly TrialBalanceQuery Query;

    internal BalanzaColumnasMonedaBuilder(TrialBalanceQuery query) {
      Query = query;
    }


    internal FixedList<BalanzaColumnasMonedaEntry> Build() {

      FixedList<TrialBalanceEntry> baseAccountEntries = BalancesDataService.GetTrialBalanceEntries(Query);

      return Build(baseAccountEntries);
    }


    internal FixedList<BalanzaColumnasMonedaEntry> Build(FixedList<TrialBalanceEntry> accountEntries) {

      if (accountEntries.Count == 0) {
        return new FixedList<BalanzaColumnasMonedaEntry>();
      }

      var trialBalanceHelper = new TrialBalanceHelper(Query);
      var helper = new BalanzaColumnasMonedaHelper(Query);

      helper.ValuateEntriesToExchangeRate(accountEntries);

      helper.ValuateEntriesToClosingExchangeRate(accountEntries);

      trialBalanceHelper.RoundDecimals(accountEntries);

      trialBalanceHelper.SetSummaryToParentEntries(accountEntries);

      List<TrialBalanceEntry> accountEntriesByCurrency = GetAccountEntriesByTrialBalanceType(
                                                                  accountEntries.ToFixedList());

      var balanceHelper = new TrialBalanceHelper(Query);
      balanceHelper.RestrictLevels(accountEntriesByCurrency.ToList());

      List<BalanzaColumnasMonedaEntry> balanceByCurrency =
                      helper.MergeTrialBalanceIntoBalanceByCurrency(accountEntriesByCurrency.ToFixedList());
      
      helper.GetTotalValorizedByAccount(balanceByCurrency);

      return balanceByCurrency.ToFixedList();
    }


    private List<TrialBalanceEntry> GetAccountEntriesByTrialBalanceType(
                                    FixedList<TrialBalanceEntry> accountEntries) {

      if (Query.TrialBalanceType == TrialBalanceType.BalanzaEnColumnasPorMoneda) {
        
        var trialBalanceHelper = new TrialBalanceHelper(Query);
        var helper = new BalanzaColumnasMonedaHelper(Query);

        var parentAccountsEntries = trialBalanceHelper.GetCalculatedParentAccounts(accountEntries.ToFixedList());

        List<TrialBalanceEntry> debtorAccounts = helper.GetSumFromCreditorToDebtorAccounts(
                                                        parentAccountsEntries);

        helper.CombineAccountEntriesAndDebtorAccounts(accountEntries.ToList(), debtorAccounts);

        return helper.GetAccountEntriesByCurrency(debtorAccounts).ToList();
      }

      return new List<TrialBalanceEntry>(accountEntries);
    }


    public FixedList<TrialBalanceEntry> BuildValorizacion(FixedList<TrialBalanceEntry> accountEntries) {
      var trialBalanceHelper = new TrialBalanceHelper(Query);
      var balanzaColumnasHelper = new BalanzaColumnasMonedaHelper(Query);

      trialBalanceHelper.RoundDecimals(accountEntries);

      trialBalanceHelper.SetSummaryToParentEntries(accountEntries);

      List<TrialBalanceEntry> parentAccountsEntries = trialBalanceHelper.GetCalculatedParentAccounts(
                                                                          accountEntries.ToFixedList());

      List<TrialBalanceEntry> debtorAccounts = balanzaColumnasHelper.GetSumFromCreditorToDebtorAccounts(
                                                        parentAccountsEntries);

      balanzaColumnasHelper.CombineAccountEntriesAndDebtorAccounts(accountEntries.ToList(), debtorAccounts);

      FixedList<TrialBalanceEntry> accountEntriesByCurrency =
                                          balanzaColumnasHelper.GetAccountEntriesByCurrency(debtorAccounts);

      return accountEntriesByCurrency;
    }


  } // class BalanzaColumnasMonedaBuilder

} // namespace Empiria.FinancialAccounting.BalanceEngine
