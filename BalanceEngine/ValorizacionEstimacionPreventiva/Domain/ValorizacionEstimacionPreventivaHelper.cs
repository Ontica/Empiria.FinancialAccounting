/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Helper methods                          *
*  Type     : ValorizacionEstimacionPreventivaHelper     License   : Please read LICENSE.txt file            *
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
  internal class ValorizacionEstimacionPreventivaHelper {

    private readonly TrialBalanceQuery _query;

    internal ValorizacionEstimacionPreventivaHelper(TrialBalanceQuery query) {
      _query = query;
    }


    #region Public methods


    internal void ExchangeRateByCurrency(FixedList<TrialBalanceEntry> entries, DateTime date,
                                         bool isLastMonth = false) {
      DateTime toDate = date;

      if (isLastMonth) {
        DateTime flagMonth = new DateTime(toDate.Year, toDate.Month, 1);
        DateTime lastMonth = flagMonth.AddDays(-1);
        toDate = lastMonth;
      }

      var exchangeRateType = ExchangeRateType.Parse(ExchangeRateType.ValorizacionBanxico.UID);

      FixedList<ExchangeRate> exchangeRates = ExchangeRate.GetList(exchangeRateType, toDate);

      foreach (var entry in entries) {
        var exchangeRate = exchangeRates.FirstOrDefault(a => a.FromCurrency.Code == "01" &&
                                                             a.ToCurrency.Code == entry.Currency.Code);

        Assertion.Require(exchangeRate, $"No se ha registrado el tipo de cambio para la " +
                                        $"moneda {entry.Currency.FullName} con la fecha proporcionada.");

        if (isLastMonth) {
          entry.SecondExchangeRate = exchangeRate.Value;
        } else {
          entry.ExchangeRate = exchangeRate.Value;
        }
      }

    }


    internal FixedList<ValorizacionEstimacionPreventivaEntry> MergeAccountsByMonth(
                                          FixedList<ValorizacionEstimacionPreventivaEntry> accountsByCurrency,
                                          FixedList<ValorizacionEstimacionPreventivaEntry> accountsInfoByMonth) {

      if (accountsByCurrency.Count == 0 && accountsInfoByMonth.Count == 0) {
        return new FixedList<ValorizacionEstimacionPreventivaEntry>();
      }

      var returnedAccounts = new List<ValorizacionEstimacionPreventivaEntry>(accountsByCurrency);

      foreach (var account in returnedAccounts) {

        var months = accountsInfoByMonth.Where(a => a.Account.Number == account.Account.Number)
                                        .OrderBy(a => a.ConsultingDate)
                                        .ToList();

        GetValueByMonth(account, months);

      }

      return returnedAccounts.ToFixedList();
    }


    private void GetValueByMonth(ValorizacionEstimacionPreventivaEntry account, List<ValorizacionEstimacionPreventivaEntry> months) {

      var utility = new ValorizacionEstimacionPreventivaUtility();

      List<DateTime> dateRange = GetDateRange();

      foreach (var date in dateRange) {

        var existDate = months.Find(a => a.ConsultingDate.Year == date.Year &&
                                         a.ConsultingDate.Month == date.Month);

        if (existDate != null) {

          account.SetTotalField($"{utility.GetMonthNameAndYear(existDate.ConsultingDate)}", existDate.TotalValued);
          account.TotalAccumulated += existDate.TotalValued;

        } else {
          account.SetTotalField($"{utility.GetMonthNameAndYear(date)}", 0.00M);
        }
      }

      account.SetTotalField($"{utility.GetMonthNameAndYear(account.ConsultingDate)}", account.TotalValued);

      account.TotalAccumulated += account.TotalValued;

    }


    private List<DateTime> GetDateRange() {

      List<DateTime> dateRange = new List<DateTime>();

      GetInitialDate(out int daysInMonth, out int totalMonths,
                     out DateTime initialDate, out DateTime lastDate);

      for (int i = 1; i <= totalMonths; i++) {

        dateRange.Add(lastDate);

        initialDate = initialDate.AddMonths(1);
        daysInMonth = DateTime.DaysInMonth(initialDate.Year, initialDate.Month);
        lastDate = new DateTime(initialDate.Year, initialDate.Month, daysInMonth);

      }

      return dateRange;
    }


    internal FixedList<ValorizacionEstimacionPreventivaEntry> GetAccountsBalances(
                                          FixedList<TrialBalanceEntry> accountEntries,
                                          DateTime date) {

      if (accountEntries.Count==0) {
        return new FixedList<ValorizacionEstimacionPreventivaEntry>();
      }

      var balanzaColumnasBuilder = new BalanzaColumnasMonedaBuilder(_query);

      FixedList<TrialBalanceEntry> accountsByCurrency =
          balanzaColumnasBuilder.BuildValorizacion(accountEntries);

      ExchangeRateByCurrency(accountsByCurrency, date);

      List<ValorizacionEstimacionPreventivaEntry> balanceByCurrency = MergeAccountsIntoAccountsByCurrency(
                                                    accountsByCurrency, date);

      GetTotalValuedByAccount(balanceByCurrency);

      return balanceByCurrency.ToFixedList();
    }


    internal FixedList<ValorizacionEstimacionPreventivaEntry> GetAccountsByFilteredMonth() {

      DateTime fromDate = _query.InitialPeriod.FromDate;
      DateTime toDate = _query.InitialPeriod.ToDate;

      GetInitialDate(out int daysInMonth, out int totalMonths, out DateTime initialDate, out DateTime lastDate);

      var accountBalanceByMonth = new List<ValorizacionEstimacionPreventivaEntry>();

      for (int i = 1; i <= totalMonths; i++) {

        List<ValorizacionEstimacionPreventivaEntry> accountsByMonth = GetAccountsByMonth(initialDate, lastDate);

        if (accountsByMonth.Count > 0) {
          foreach (var account in accountsByMonth) {
            account.MonthPosition = i;
          }
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

      initialDate = new DateTime(_query.InitialPeriod.ToDate.Year, 1, 1);
      daysInMonth = DateTime.DaysInMonth(initialDate.Year, initialDate.Month);
      lastDate = new DateTime(initialDate.Year, initialDate.Month, daysInMonth);
      totalMonths = Math.Abs((_query.InitialPeriod.ToDate.Month - initialDate.Month) +
                              12 * (_query.InitialPeriod.ToDate.Year - initialDate.Year));

    }

    private void GetTotalValuedByAccount(List<ValorizacionEstimacionPreventivaEntry> returnedEntries) {

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


    internal List<ValorizacionEstimacionPreventivaEntry> MergeAccountsIntoAccountsByCurrency(
                                    FixedList<TrialBalanceEntry> accountEntries,
                                    DateTime date) {

      if (accountEntries.Count == 0) {
        return new List<ValorizacionEstimacionPreventivaEntry>();
      }

      ExchangeRateByCurrency(accountEntries, date, true);

      var returnedEntries = new List<ValorizacionEstimacionPreventivaEntry>();

      foreach (var usdEntry in accountEntries.Where(a => a.Currency.Equals(Currency.USD))) {

        returnedEntries.Add(new ValorizacionEstimacionPreventivaEntry().MapToValorizedReport(usdEntry, date));

      }

      MergeForeignBalancesByAccount(returnedEntries, accountEntries, date);

      return returnedEntries.OrderBy(a => a.Account.Number).ToList();
    }


    #endregion Public methods


    #region Private methods


    private List<ValorizacionEstimacionPreventivaEntry> GetAccountsByMonth(
                                          DateTime initialDate, DateTime lastDate) {

      _query.InitialPeriod.FromDate = initialDate;
      _query.InitialPeriod.ToDate = lastDate;

      FixedList<TrialBalanceEntry> baseAccountEntries = BalancesDataService.GetTrialBalanceEntries(_query);

      FixedList<ValorizacionEstimacionPreventivaEntry> returnedAccounts = GetAccountsBalances(baseAccountEntries, lastDate);

      return returnedAccounts.ToList();
    }


    private void MergeForeignBalancesByAccount(List<ValorizacionEstimacionPreventivaEntry> returnedEntries,
                                               FixedList<TrialBalanceEntry> accountEntries,
                                               DateTime date) {

      foreach (var entry in accountEntries) {

        var valorizacion = returnedEntries.Find(a => a.Account.Number == entry.Account.Number);

        if (valorizacion == null) {
          returnedEntries.Add(new ValorizacionEstimacionPreventivaEntry().MapToValorizedReport(entry,
                                                        _query.InitialPeriod.ToDate));
        } else {

          valorizacion.AssingValues(entry, date);

        }

      } // foreach

    }


    #endregion Private methods

  } // class ValorizacionEstimacionPreventivaHelper

} // namespace Empiria.FinancialAccounting.BalanceEngine
