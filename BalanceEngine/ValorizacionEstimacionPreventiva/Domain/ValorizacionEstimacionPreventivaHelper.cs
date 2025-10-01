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


    internal void ValuateExchangeRateByCurrency(FixedList<TrialBalanceEntry> entries,
                                                DateTime date, bool isLastMonth = false) {

      if (_query.ValuateBalances || _query.InitialPeriod.UseDefaultValuation) {
        
        if (_query.InitialPeriod.ToDate.Year >= 2025) {
          ExchangeRateByCurrencyV2(entries, date, isLastMonth);

        } else {

          ExchangeRateByCurrencyV1(entries, date, isLastMonth);
        }
      }
    }


    internal void ExchangeRateByCurrencyV1(FixedList<TrialBalanceEntry> entries,
                                           DateTime date, bool isLastMonth) {

      DateTime toDate = GetToDateOrLastMonthDate(date, isLastMonth);

      var exchangeRateType = ExchangeRateType.Parse(ExchangeRateType.ValorizacionBanxico.UID);

      FixedList<ExchangeRate> exchangeRates = ExchangeRate.GetList(exchangeRateType, toDate);

      foreach (var entry in entries.Where(a => a.Currency.Distinct(Currency.MXN))) {
        var exchangeRate = exchangeRates.FirstOrDefault(a => a.FromCurrency.Equals(Currency.MXN) &&
                                                             a.ToCurrency.Equals(entry.Currency));

        Assertion.Require(exchangeRate, $"No se ha registrado el tipo de cambio para la " +
                                        $"moneda {entry.Currency.FullName} en la fecha proporcionada.");

        if (isLastMonth) {
          entry.SecondExchangeRate = exchangeRate.Value;
        } else {
          entry.ExchangeRate = exchangeRate.Value;
        }
      }
    }


    internal void ExchangeRateByCurrencyV2(FixedList<TrialBalanceEntry> entries,
                                           DateTime date, bool isLastMonth) {

      DateTime toDate = GetToDateOrLastMonthDate(date, isLastMonth);

      var trialBalanceHelper = new TrialBalanceHelper(_query);

      DateTime _toDateFlag = _query.InitialPeriod.ToDate;
      _query.InitialPeriod.ToDate = toDate;
      
      var exchangeRateFor = trialBalanceHelper.GetExchangeRateTypeForCurrencies(_query.InitialPeriod);

      foreach (var entry in entries.Where(a => a.Currency.Distinct(Currency.MXN))) {

        var exchangeRate = exchangeRateFor.ExchangeRateList.Find(
                            a => a.ToCurrency.Equals(entry.Currency) &&
                            a.FromCurrency.Code == exchangeRateFor.ValuateToCurrrencyUID);

        Assertion.Require(exchangeRate, $" {exchangeRateFor.InvalidExchangeRateTypeMsg()} " +
                                        $"para la moneda {entry.Currency.FullName} ");

        if (isLastMonth) {
          entry.SecondExchangeRate = exchangeRate.Value;
        } else {
          entry.ExchangeRate = exchangeRate.Value;
        }
      }
      _query.InitialPeriod.ToDate = _toDateFlag;
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
                                          DateTime date,
                                          bool isPreviousMonth = false) {

      if (accountEntries.Count == 0) {
        return new FixedList<ValorizacionEstimacionPreventivaEntry>();
      }

      var balanzaColumnasBuilder = new BalanzaColumnasMonedaBuilder(_query);

      ValuateExchangeRateByCurrency(accountEntries, date);

      FixedList<TrialBalanceEntry> accountsByCurrency =
          balanzaColumnasBuilder.BuildValorizacion(accountEntries);

      List<ValorizacionEstimacionPreventivaEntry> balanceByCurrency = MergeAccountsIntoAccountsByCurrency(
                                                    accountsByCurrency, date, isPreviousMonth);

      GetTotalValuedByAccount(balanceByCurrency);

      return balanceByCurrency.ToFixedList();
    }


    internal FixedList<ValorizacionEstimacionPreventivaEntry> GetAccountsByFilteredMonth() {

      DateTime fromDate = _query.InitialPeriod.FromDate;
      DateTime toDate = _query.InitialPeriod.ToDate;

      GetInitialDate(out int daysInMonth, out int totalMonths, out DateTime initialDate, out DateTime lastDate);

      var accountBalanceByMonth = new List<ValorizacionEstimacionPreventivaEntry>();

      for (int i = 1; i <= totalMonths; i++) {
        bool isPreviousMonth = false;

        if (i == totalMonths) {
          isPreviousMonth = true;
        }

        List<ValorizacionEstimacionPreventivaEntry> accountsByMonth =
              GetAccountsByMonth(initialDate, lastDate, isPreviousMonth);

        foreach (var account in accountsByMonth) {
          account.MonthPosition = i;
        }

        accountBalanceByMonth.AddRange(accountsByMonth);

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


    private void GetValueByMonth(ValorizacionEstimacionPreventivaEntry accountEntry,
                                 List<ValorizacionEstimacionPreventivaEntry> entriesByMonthList) {

      List<DateTime> dateRange = GetDateRange();

      int flagDateCount = 0;
      foreach (var date in dateRange) {

        var existEntryInMonth = entriesByMonthList.Find(a => a.ConsultingDate.Year == date.Year &&
                                                    a.ConsultingDate.Month == date.Month);

        if (existEntryInMonth != null) {

          flagDateCount += 1;

          if (flagDateCount == dateRange.Count) {
            GetValuedEffectDebitCredit(accountEntry, existEntryInMonth);
          }

          accountEntry.SetTotalField($"{GetMonthNameAndYear(existEntryInMonth.ConsultingDate)}",
                                     existEntryInMonth.TotalValued);

          accountEntry.TotalAccumulated += existEntryInMonth.TotalValued;

        } else {
          accountEntry.SetTotalField($"{GetMonthNameAndYear(date)}", 0.00M);
        }
      }

      accountEntry.SetTotalField($"{GetMonthNameAndYear(accountEntry.ConsultingDate)}",
                                 accountEntry.TotalValued);

      accountEntry.TotalAccumulated += accountEntry.TotalValued;

    }


    internal void GetValuedEffectDebitCredit(ValorizacionEstimacionPreventivaEntry entry,
                                             ValorizacionEstimacionPreventivaEntry preventivaEntry) {

      entry.ValuesByCurrency.ValuedEffectDebitUSD = preventivaEntry.ValuesByCurrency.PreviousValuedUSDDebit -
                                                      entry.ValuesByCurrency.ValuedUSDDebit;

      entry.ValuesByCurrency.ValuedEffectCreditUSD = preventivaEntry.ValuesByCurrency.PreviousValuedUSDCredit -
                                                     entry.ValuesByCurrency.ValuedUSDCredit;

      entry.ValuesByCurrency.ValuedEffectDebitYEN = preventivaEntry.ValuesByCurrency.PreviousValuedYENDebit -
                                                    entry.ValuesByCurrency.ValuedYENDebit;

      entry.ValuesByCurrency.ValuedEffectCreditYEN = preventivaEntry.ValuesByCurrency.PreviousValuedYENCredit -
                                                     entry.ValuesByCurrency.ValuedYENCredit;

      entry.ValuesByCurrency.ValuedEffectDebitEUR = preventivaEntry.ValuesByCurrency.PreviousValuedEURDebit -
                                                    entry.ValuesByCurrency.ValuedEURDebit;

      entry.ValuesByCurrency.ValuedEffectCreditEUR = preventivaEntry.ValuesByCurrency.PreviousValuedEURCredit -
                                                     entry.ValuesByCurrency.ValuedEURCredit;

      entry.ValuesByCurrency.ValuedEffectDebitUDI = preventivaEntry.ValuesByCurrency.PreviousValuedUDIDebit -
                                                    entry.ValuesByCurrency.ValuedUDIDebit;

      entry.ValuesByCurrency.ValuedEffectCreditUDI = preventivaEntry.ValuesByCurrency.PreviousValuedUDICredit -
                                                     entry.ValuesByCurrency.ValuedUDICredit;
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




    internal FixedList<ValorizacionEstimacionPreventivaEntry> MergeAccountsByMonth(
              FixedList<ValorizacionEstimacionPreventivaEntry> accountsByCurrency,
              FixedList<ValorizacionEstimacionPreventivaEntry> entriesByMonth) {

      if (accountsByCurrency.Count == 0 && entriesByMonth.Count == 0) {
        return new FixedList<ValorizacionEstimacionPreventivaEntry>();
      }

      var returnedAccountEntries = new List<ValorizacionEstimacionPreventivaEntry>(accountsByCurrency);

      foreach (var accountEntry in returnedAccountEntries) {

        var entriesByMonthList = entriesByMonth.Where(a => a.Account.Number == accountEntry.Account.Number)
                                               .OrderBy(a => a.ConsultingDate)
                                               .ToList();

        GetValueByMonth(accountEntry, entriesByMonthList);

      }

      return returnedAccountEntries.ToFixedList();
    }


    #endregion Public methods


    #region Helpers

    private List<ValorizacionEstimacionPreventivaEntry> GetAccountsByMonth(
                                          DateTime initialDate, DateTime lastDate, bool isPreviousMonth) {

      _query.InitialPeriod.FromDate = initialDate;
      _query.InitialPeriod.ToDate = lastDate;

      FixedList<TrialBalanceEntry> baseAccountEntries = BalancesDataService.GetTrialBalanceEntries(_query);

      FixedList<ValorizacionEstimacionPreventivaEntry> returnedAccounts =
              GetAccountsBalances(baseAccountEntries, lastDate, isPreviousMonth);

      return returnedAccounts.ToList();
    }


    private string GetMonthNameAndYear(DateTime date) {
      return $"{EmpiriaString.MonthName(date)}_{date.Year}";
    }


    private DateTime GetToDateOrLastMonthDate(DateTime date, bool isLastMonth) {

      DateTime toDate = date;

      if (isLastMonth /*|| date.Month == 1*/) {
        DateTime flagMonth = new DateTime(toDate.Year, toDate.Month, 1);
        DateTime lastMonth = flagMonth.AddDays(-1);
        toDate = lastMonth;
      }
      return toDate;
    }


    private List<ValorizacionEstimacionPreventivaEntry> MergeAccountsIntoAccountsByCurrency(
                                    FixedList<TrialBalanceEntry> accountEntries,
                                    DateTime date,
                                    bool isPreviousMonth) {

      if (accountEntries.Count == 0) {
        return new List<ValorizacionEstimacionPreventivaEntry>();
      }

      ValuateExchangeRateByCurrency(accountEntries, date, true);

      var returnedEntries = new List<ValorizacionEstimacionPreventivaEntry>();

      foreach (var usdEntry in accountEntries.Where(a => a.Currency.Equals(Currency.USD))) {

        returnedEntries.Add(new ValorizacionEstimacionPreventivaEntry()
                        .MapToValorizedReport(usdEntry, date, isPreviousMonth));

      }

      MergeDomesticIntoForeignBalances(returnedEntries, accountEntries, isPreviousMonth);

      MergeForeignBalancesByAccount(returnedEntries, accountEntries, date, isPreviousMonth);


      return returnedEntries.OrderBy(a => a.Account.Number).ToList();
    }


    private void MergeDomesticIntoForeignBalances(List<ValorizacionEstimacionPreventivaEntry> returnedEntries,
                                                  FixedList<TrialBalanceEntry> accountEntries,
                                                  bool isPreviousMonth) {

      foreach (var entry in accountEntries.Where(a => a.Currency.Equals(Currency.MXN))) {

        var existAccount = returnedEntries.Find(a => a.Account.Number == entry.Account.Number);

        if (existAccount != null) {
          existAccount.MXN = entry.CurrentBalance;
          existAccount.MXNDebit = entry.Debit;
          existAccount.MXNCredit = entry.Credit;
        } else {
          returnedEntries.Add(new ValorizacionEstimacionPreventivaEntry().MapToValorizedReport(entry,
                                                        _query.InitialPeriod.ToDate, isPreviousMonth));
        }

      } // foreach

    }


    private void MergeForeignBalancesByAccount(List<ValorizacionEstimacionPreventivaEntry> returnedEntries,
                                               FixedList<TrialBalanceEntry> accountEntries,
                                               DateTime date,
                                               bool isPreviousMonth) {

      foreach (var entry in accountEntries.Where(a => a.Currency.Distinct(Currency.MXN))) {

        var valorizacion = returnedEntries.Find(a => a.Account.Number == entry.Account.Number);

        if (valorizacion == null) {
          returnedEntries.Add(new ValorizacionEstimacionPreventivaEntry().MapToValorizedReport(entry,
                                                        _query.InitialPeriod.ToDate, isPreviousMonth));
        } else {

          valorizacion.AssingValues(entry, date, isPreviousMonth);

        }

      } // foreach

    }

    #endregion Helpers

  } // class ValorizacionEstimacionPreventivaHelper

} // namespace Empiria.FinancialAccounting.BalanceEngine
