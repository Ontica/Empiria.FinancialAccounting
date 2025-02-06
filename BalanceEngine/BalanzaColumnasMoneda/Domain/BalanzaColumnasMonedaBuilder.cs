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
using Empiria.Time;

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

      if (Query.TrialBalanceType == TrialBalanceType.BalanzaEnColumnasPorMoneda) {

        return BuildBalanceInColumnByCurrency(accountEntries);

      } else if (Query.TrialBalanceType == TrialBalanceType.BalanzaDiferenciaDiariaPorMoneda) {

        return BuildDailyDifferenceBalanceV2(accountEntries);

      } else {
        throw Assertion.EnsureNoReachThisCode(
                    $"Unhandled trial balance type {Query.TrialBalanceType}.");
      }

    }


    internal FixedList<BalanzaColumnasMonedaEntry> BuildBalanceInColumnByCurrency(FixedList<TrialBalanceEntry> accountEntries) {

      if (accountEntries.Count == 0) {
        return new FixedList<BalanzaColumnasMonedaEntry>();
      }

      var balanceHelper = new TrialBalanceHelper(Query);
      var helper = new BalanzaColumnasMonedaHelper(Query);

      helper.ValuateEntriesToExchangeRate(accountEntries);

      balanceHelper.RoundDecimals(accountEntries);

      balanceHelper.SetSummaryToParentEntries(accountEntries);

      var parentAccountsEntries = balanceHelper.GetCalculatedParentAccounts(accountEntries.ToFixedList());

      List<TrialBalanceEntry> debtorAccounts = helper.GetSumFromCreditorToDebtorAccounts(
                                                      parentAccountsEntries);

      helper.CombineAccountEntriesAndDebtorAccounts(accountEntries.ToList(), debtorAccounts);

      List<TrialBalanceEntry> accountEntriesByCurrency =
                                helper.GetAccountEntriesByCurrency(debtorAccounts).ToList();
      
      balanceHelper.RestrictLevels(accountEntriesByCurrency.ToList());

      List<BalanzaColumnasMonedaEntry> balanceByCurrency =
                      helper.MergeTrialBalanceIntoBalanceByCurrency(accountEntriesByCurrency.ToFixedList());
      
      helper.GetTotalValorizedByAccount(balanceByCurrency);

      return balanceByCurrency.ToFixedList();
    }


    internal FixedList<BalanzaColumnasMonedaEntry> BuildDailyDifferenceBalanceV2(
                                                    FixedList<TrialBalanceEntry> accountEntries) {

      if (accountEntries.Count == 0) {
        return new FixedList<BalanzaColumnasMonedaEntry>();
      }

      var balanceHelper = new TrialBalanceHelper(Query);
      var helper = new BalanzaColumnasMonedaHelper(Query);

      helper.ValuateEntriesToExchangeRate(accountEntries);

      balanceHelper.RoundDecimals(accountEntries);

      balanceHelper.SetSummaryToParentEntries(accountEntries);

      var parentAccountsEntries = balanceHelper.GetCalculatedParentAccounts(accountEntries.ToFixedList());

      List<TrialBalanceEntry> debtorAccounts = helper.GetSumFromCreditorToDebtorAccounts(
                                                      parentAccountsEntries);

      helper.CombineAccountEntriesAndDebtorAccounts(accountEntries.ToList(), debtorAccounts);

      List<TrialBalanceEntry> accountEntriesByCurrency =
                                helper.GetAccountEntriesByCurrency(debtorAccounts).ToList();

      balanceHelper.RestrictLevels(accountEntriesByCurrency.ToList());

      helper.ValuateEntriesToExchangeRateByCurrency(accountEntriesByCurrency.ToFixedList());

      helper.ValuateEntriesToClosingExchangeRate(accountEntriesByCurrency.ToFixedList());

      List<BalanzaColumnasMonedaEntry> balanceByCurrency =
                      helper.MergeTrialBalanceIntoBalanceByCurrency(accountEntriesByCurrency.ToFixedList());

      return balanceByCurrency.ToFixedList();
    }


    internal FixedList<BalanzaColumnasMonedaEntry> BuildDailyDifferenceBalanceV1(
                                                    FixedList<TrialBalanceEntry> accountEntries) {

      if (accountEntries.Count == 0) {
        return new FixedList<BalanzaColumnasMonedaEntry>();
      }

      var balanceHelper = new TrialBalanceHelper(Query);
      var helper = new BalanzaColumnasMonedaHelper(Query);

      helper.ValuateEntriesToExchangeRate(accountEntries);

      helper.ValuateEntriesToClosingExchangeRate(accountEntries);

      balanceHelper.RoundDecimals(accountEntries);

      balanceHelper.SetSummaryToParentEntries(accountEntries);

      balanceHelper.RestrictLevels(accountEntries.ToList());

      List<BalanzaColumnasMonedaEntry> balanceByCurrency =
                      helper.MergeTrialBalanceIntoBalanceByCurrency(accountEntries.ToFixedList());

      return balanceByCurrency.ToFixedList();
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
