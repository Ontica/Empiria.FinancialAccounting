﻿/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Helper methods                          *
*  Type     : ValorizacionHelper                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Helper methods to build reporte de valorización.                                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.Data;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Helper methods to build reporte de valorización.</summary>
  internal class ValorizacionHelper {

    private readonly TrialBalanceQuery _query;

    internal ValorizacionHelper(TrialBalanceQuery query) {
      _query = query;
    }


    #region Public methods


    private List<string> GetMemberFields(FixedList<ValorizacionEntry> accountsList) {
      var rootAccount = accountsList.First();

      List<string> members = new List<string>();

      if (rootAccount != null) {
        members.AddRange(rootAccount.GetDynamicMemberNames());
      }

      return members;
    }


    internal void ExchangeRateByCurrency(FixedList<TrialBalanceEntry> entries, DateTime date, bool isLastMonth = false) {

      DateTime toDate = date;

      if (isLastMonth == true) {
        DateTime flagMonth = new DateTime(toDate.Year, toDate.Month, 1);
        DateTime lastMonth = flagMonth.AddDays(-1);
        toDate = lastMonth;
      }

      var exchangeRateType = ExchangeRateType.Parse(ExchangeRateType.ValorizacionBanxico.UID);
      FixedList<ExchangeRate> exchangeRates = ExchangeRate.GetList(exchangeRateType, toDate);

      foreach (var entry in entries) {
        var exchangeRate = exchangeRates.FirstOrDefault(a => a.FromCurrency.Code == "01" &&
                                                             a.ToCurrency.Code == entry.Currency.Code);

        if (isLastMonth == true) {
          entry.SecondExchangeRate = exchangeRate.Value;
        } else {
          entry.ExchangeRate = exchangeRate.Value;
        }
      }

    }


    internal FixedList<ValorizacionEntry> MergeAccountsByMonth(
                                          FixedList<ValorizacionEntry> accountsByCurrency,
                                          FixedList<ValorizacionEntry> accountsInfoByMonth) {

      if (accountsByCurrency.Count == 0 && accountsInfoByMonth.Count == 0) {
        return new FixedList<ValorizacionEntry>();
      }

      var returnedAccounts = new List<ValorizacionEntry>(accountsByCurrency);
      
      foreach (var account in returnedAccounts) {
        
        var months = accountsInfoByMonth.FindAll(a => a.Account.Number == account.Account.Number)
                                        .OrderBy(a => a.ConsultingDate);
        //TODO AGREGAR MESES EN LOS QUE NO HUBO MOVIMIENTO CON 00.00 PORQUE SALEN NULL
        // EJEMPLO 3.02.01.02.01.01.06.01 DE ENERO-JUNIO 2022
        
        GetValueByMonth(account, months);

      }

      return returnedAccounts.ToFixedList();
    }

    
    private void GetValueByMonth(ValorizacionEntry account, IOrderedEnumerable<ValorizacionEntry> months) {

      var utility = new ValorizacionUtility();

      //List<DateTime> dateRange = GetDateRange();

      foreach (var month in months) {

        account.SetTotalField($"{utility.GetMonthNameAndYear(month.ConsultingDate)}", month.TotalValued);
        account.TotalAccumulated += month.TotalValued;
      }

      account.SetTotalField($"{utility.GetMonthNameAndYear(account.ConsultingDate)}", account.TotalValued);

      account.TotalAccumulated += account.TotalValued;

    }


    private List<DateTime> GetDateRange() {
      throw new NotImplementedException();
    }


    internal FixedList<ValorizacionEntry> GetAccountsBalances(
                                          FixedList<TrialBalanceEntry> accountEntries,
                                          DateTime date) {

      if (accountEntries.Count==0) {
        return new FixedList<ValorizacionEntry>();
      }

      FixedList<TrialBalanceEntry> accountsByCurrency = GetAccountsByCurrency(accountEntries);

      ExchangeRateByCurrency(accountsByCurrency, date);

      List<ValorizacionEntry> balanceByCurrency = MergeAccountsIntoAccountsByCurrency(
                                                    accountsByCurrency, date);

      GetTotalValuedByAccount(balanceByCurrency);

      return balanceByCurrency.ToFixedList();
    }

    internal FixedList<ValorizacionEntry> GetAccountsByFilteredMonth() {

      DateTime fromDate = _query.InitialPeriod.FromDate;
      DateTime toDate = _query.InitialPeriod.ToDate;

      GetInitialDate(out int daysInMonth, out int totalMonths, out DateTime initialDate, out DateTime lastDate);

      var accountBalanceByMonth = new List<ValorizacionEntry>();

      for (int i = 1; i <= totalMonths; i++) {

        List<ValorizacionEntry> accountsByMonth = GetAccountsByMonth(initialDate, lastDate);

        if (accountsByMonth.Count > 0) {
          accountBalanceByMonth.AddRange(accountsByMonth);
        }

        initialDate = initialDate.AddMonths(1);
        daysInMonth = DateTime.DaysInMonth(initialDate.Year, initialDate.Month);
        lastDate = new DateTime(initialDate.Year, initialDate.Month, daysInMonth);
      }

      _query.InitialPeriod.FromDate = fromDate;
      _query.InitialPeriod.ToDate = toDate;

      return accountBalanceByMonth.ToFixedList();
    }

    private void GetInitialDate(out int daysInMonth, out int totalMonths,
                                out DateTime initialDate, out DateTime lastDate) {

      initialDate = _query.InitialPeriod.FromDate;
      daysInMonth = DateTime.DaysInMonth(initialDate.Year, initialDate.Month);
      lastDate = new DateTime(initialDate.Year, initialDate.Month, daysInMonth);
      totalMonths = Math.Abs((_query.InitialPeriod.ToDate.Month - _query.InitialPeriod.FromDate.Month) +
                              12 * (_query.InitialPeriod.ToDate.Year - _query.InitialPeriod.FromDate.Year));

    }

    private void GetTotalValuedByAccount(List<ValorizacionEntry> returnedEntries) {

      foreach (var account in returnedEntries) {
        account.TotalValued = account.ValuesByCurrency.ValuedEffectUSD +
                               account.ValuesByCurrency.ValuedEffectYEN +
                               account.ValuesByCurrency.ValuedEffectEUR +
                               account.ValuesByCurrency.ValuedEffectUDI;
      }

    }


    internal FixedList<TrialBalanceEntry> GetAccountsByCurrency(FixedList<TrialBalanceEntry> accountEntries) {
      var trialBalanceHelper = new TrialBalanceHelper(_query);
      var balanzaColumnasHelper = new BalanzaColumnasMonedaHelper(_query);

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


    internal List<ValorizacionEntry> MergeAccountsIntoAccountsByCurrency(
                                    FixedList<TrialBalanceEntry> accountEntries,
                                    DateTime date) {

      if (accountEntries.Count == 0) {
        return new List<ValorizacionEntry>();
      }

      ExchangeRateByCurrency(accountEntries, date, true);

      var returnedEntries = new List<ValorizacionEntry>();

      foreach (var usdEntry in accountEntries.Where(a => a.Currency.Equals(Currency.USD))) {

        returnedEntries.Add(new ValorizacionEntry().MapToValorizedReport(usdEntry, date));

      }

      MergeForeignBalancesByAccount(returnedEntries, accountEntries);

      return returnedEntries.OrderBy(a => a.Account.Number).ToList();
    }


    #endregion Public methods


    #region Private methods


    private List<ValorizacionEntry> GetAccountsByMonth(
                                          DateTime initialDate, DateTime lastDate) {

      _query.InitialPeriod.FromDate = initialDate;
      _query.InitialPeriod.ToDate = lastDate;

      FixedList<TrialBalanceEntry> baseAccountEntries = BalancesDataService.GetTrialBalanceEntries(_query);

      FixedList<ValorizacionEntry> returnedAccounts = GetAccountsBalances(baseAccountEntries, lastDate);

      return returnedAccounts.ToList();
    }


    private void MergeForeignBalancesByAccount(List<ValorizacionEntry> returnedEntries,
                                               FixedList<TrialBalanceEntry> accountEntries) {

      foreach (var entry in accountEntries) {

        var valorizacion = returnedEntries.Find(a => a.Account.Number == entry.Account.Number);

        if (valorizacion == null) {
          returnedEntries.Add(new ValorizacionEntry().MapToValorizedReport(entry,
                                                        _query.InitialPeriod.ToDate));
        } else {

          valorizacion.AssingValues(entry);

        }

      } // foreach

    }


    #endregion Private methods

  } // class ValorizacionHelper

} // namespace Empiria.FinancialAccounting.BalanceEngine
