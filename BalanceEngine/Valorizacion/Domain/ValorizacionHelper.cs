/* Empiria Financial *****************************************************************************************
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

    internal FixedList<ValorizacionEntry> GetAccountsWithCurrencies(
                                          FixedList<TrialBalanceEntry> accountEntries,
                                          DateTime date) {
      
      FixedList<TrialBalanceEntry> accountsByCurrency = GetAccountsByCurrency(accountEntries);

      ExchangeRateByCurrency(accountsByCurrency, date);

      List<ValorizacionEntry> balanceByCurrency = MergeAccountsIntoAccountsByCurrency(
                                                    accountsByCurrency, date);

      return balanceByCurrency.ToFixedList();
    }

    internal FixedList<ValorizacionEntry> GetAccountsByFilteredMonth() {

      DateTime fromDate = _query.InitialPeriod.FromDate;
      DateTime toDate = _query.InitialPeriod.ToDate;

      var returnedEntries = new List<ValorizacionEntry>();
      
      var totalMonths = Math.Abs((_query.InitialPeriod.ToDate.Month - _query.InitialPeriod.FromDate.Month) +
                        12 * (_query.InitialPeriod.ToDate.Year - _query.InitialPeriod.FromDate.Year));

      DateTime initialDate = _query.InitialPeriod.FromDate;
      var daysInMonth = DateTime.DaysInMonth(initialDate.Year, initialDate.Month);
      DateTime lastDate = new DateTime(initialDate.Year, initialDate.Month, daysInMonth);

      for (int i = 1; i <= totalMonths; i++) {

        List<ValorizacionEntry> accountsByMonth = GetAccountsByMonth(initialDate, lastDate);

        if (accountsByMonth.Count>0) {
          returnedEntries.AddRange(accountsByMonth);
        }

        initialDate = initialDate.AddMonths(1);
        daysInMonth = DateTime.DaysInMonth(initialDate.Year, initialDate.Month);
        lastDate = new DateTime(initialDate.Year, initialDate.Month, daysInMonth);

      }

      _query.InitialPeriod.FromDate = fromDate;
      _query.InitialPeriod.ToDate= toDate;

      return returnedEntries.ToFixedList();
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

      FixedList<ValorizacionEntry> returnedAccounts = GetAccountsWithCurrencies(baseAccountEntries, lastDate);

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
