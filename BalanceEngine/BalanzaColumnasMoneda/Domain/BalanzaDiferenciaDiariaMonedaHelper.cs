/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Helper methods                          *
*  Type     : BalanzaDiferenciaDiariaMonedaHelper        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Helper methods to build balanza diferencia diaria por moneda.                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;
using Empiria.FinancialAccounting.AccountsLists;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.Data;
using Empiria.Time;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Helper methods to build balanza diferencia diaria por moneda.</summary>
  internal class BalanzaDiferenciaDiariaMonedaHelper {

    #region Constructors and parsers

    private readonly TrialBalanceQuery Query;
    private DateTime fromDateFlag;
    private DateTime toDateFlag;

    internal BalanzaDiferenciaDiariaMonedaHelper(TrialBalanceQuery query) {
      Query = query;
      fromDateFlag = this.Query.InitialPeriod.FromDate;
      toDateFlag = this.Query.InitialPeriod.ToDate;
    }

    #endregion Constructors and parsers


    #region Public methods

    internal void AssignTagsToEntries(
                  FixedList<BalanzaDiferenciaDiariaMonedaEntry> entriesByDateAndAccount) {

      FixedList<string> accountsNumberList = entriesByDateAndAccount.SelectDistinct(x => x.Account.Number);

      var tagsList = AccountClassificatorList.Parse("BitacoraCuentasValorizacion");

      foreach (var accountNumber in accountsNumberList) {

        FixedList<BalanzaDiferenciaDiariaMonedaEntry> entries = entriesByDateAndAccount.FindAll(
                                                                x => x.Account.Number == accountNumber);

        for (int i = 0; i < entries.Count; i++) {

          entries[i].ERI = tagsList.TryGetAccountValue(accountNumber, "ERI") ?? "";
          entries[i].ComplementDescription = tagsList.TryGetAccountValue(accountNumber, "Completo") ?? "";
          entries[i].ComplementDetail = tagsList.TryGetAccountValue(accountNumber, "Detallado") ?? "";
          entries[i].CategoryType = tagsList.TryGetAccountValue(accountNumber, "Rubro") ?? "";
        }
      }
    }


    internal void CalculateBalancesForEntries(
      FixedList<BalanzaDiferenciaDiariaMonedaEntry> diffByDayEntries) {

      DateTime previousDate = DateTime.MinValue;

      foreach (var entry in diffByDayEntries) {

        if (entry.FromDate < Query.InitialPeriod.FromDate) {
          entry.SetDailyBalance(new BalanzaDiferenciaDiariaMonedaEntry());
          entry.SetValorizedDailyBalance();

        } else if (previousDate != DateTime.MinValue && previousDate < entry.ToDate) {

          var previousDayEntry = diffByDayEntries.Find(x => x.ToDate == previousDate &&
                                                       x.Account.Number == entry.Account.Number);
          if (previousDayEntry != null) {
            entry.SetDailyBalance(previousDayEntry);
            entry.SetValorizedDailyBalance();
          }
        }
        previousDate = entry.ToDate;
      }
    }


    internal FixedList<BalanzaColumnasMonedaEntry> GetBalanceInColumnByCurrency() {

      List<DateTime> workingDays = GetWorkingDates();

      var balancesInColumnByCurrency = new List<BalanzaColumnasMonedaEntry>();

      foreach (var dateFilter in workingDays) {

        Query.AssignDefaultDateAndValuation(dateFilter);

        List<BalanzaColumnasMonedaEntry> balanzaColumnas = BuildAccountEntries().ToList();
        balancesInColumnByCurrency.AddRange(balanzaColumnas);
      }
      this.Query.InitialPeriod.FromDate = fromDateFlag;
      this.Query.InitialPeriod.ToDate = toDateFlag;

      return balancesInColumnByCurrency.ToFixedList();
    }


    internal FixedList<BalanzaColumnasMonedaEntry> GetOrderingByAccountThenDate(
                                                  FixedList<BalanzaColumnasMonedaEntry> balanzaColumnas) {

      var orderingEntries = new List<BalanzaColumnasMonedaEntry>(balanzaColumnas);
      return orderingEntries.OrderBy(x => x.Account.Number)
                            .ThenBy(x => x.ToDate).ToFixedList();
    }


    internal FixedList<BalanzaDiferenciaDiariaMonedaEntry> GetOrderingByDateThenAccount(
      FixedList<BalanzaDiferenciaDiariaMonedaEntry> diffByDayEntries) {

      var orderingEntries = new List<BalanzaDiferenciaDiariaMonedaEntry>(diffByDayEntries);

      return orderingEntries.OrderBy(x => x.ToDate)
                            .ThenBy(x => x.Account.Number).ToFixedList();
    }


    internal FixedList<BalanzaDiferenciaDiariaMonedaEntry> MergeBalanceByCurrencyIntoCurrencyDiffByDay(
      FixedList<BalanzaColumnasMonedaEntry> entriesByAccountAndDate) {

      var mappedItems = entriesByAccountAndDate.Select((x) =>
                        MapToDifferenceByDayEntry((BalanzaColumnasMonedaEntry) x));

      return new FixedList<BalanzaDiferenciaDiariaMonedaEntry>(mappedItems);
    }

    #endregion Public methods


    #region Private methods

    private FixedList<BalanzaColumnasMonedaEntry> BuildAccountEntries() {

      FixedList<TrialBalanceEntry> baseAccountEntries = BalancesDataService.GetTrialBalanceEntries(Query);

      return BuildBalanceInColumnByCurrency(baseAccountEntries);
    }


    private FixedList<BalanzaColumnasMonedaEntry> BuildBalanceInColumnByCurrency(
                                                    FixedList<TrialBalanceEntry> accountEntries) {
      if (accountEntries.Count == 0) {
        return new FixedList<BalanzaColumnasMonedaEntry>();
      }

      var balanceInColumnHelper = new BalanzaColumnasMonedaHelper(Query);
      var balanceHelper = new TrialBalanceHelper(Query);

      balanceInColumnHelper.ValuateEntriesToExchangeRate(accountEntries);
      balanceHelper.RoundDecimals(accountEntries);
      balanceHelper.SetParentPostingFlags(accountEntries);

      var parentAccountsEntries = balanceHelper.GetCalculatedParentAccounts(accountEntries.ToFixedList());

      List<TrialBalanceEntry> debtorAccounts = balanceInColumnHelper.GetSumFromCreditorToDebtorAccounts(
                                                      parentAccountsEntries);

      balanceInColumnHelper.CombineAccountEntriesAndDebtorAccounts(accountEntries.ToList(), debtorAccounts);

      List<TrialBalanceEntry> accountEntriesByCurrency =
                                balanceInColumnHelper.GetAccountEntriesByCurrency(debtorAccounts).ToList();

      ValuateEntriesToExchangeRateByCurrency(accountEntriesByCurrency.ToFixedList());

      ValuateEntriesToClosingExchangeRate(accountEntriesByCurrency.ToFixedList(), fromDateFlag);

      List<BalanzaColumnasMonedaEntry> balanceByCurrency =
                      balanceInColumnHelper.MergeTrialBalanceIntoBalanceByCurrency(
                                              accountEntriesByCurrency.ToFixedList());

      return balanceByCurrency.ToFixedList();
    }


    private DateTime GetLastWorkingDateFromPreviousMonth() {
      var calendar = EmpiriaCalendar.Default;

      var previousMonth = Query.InitialPeriod.FromDate.AddMonths(-1);

      return calendar.LastWorkingDateWithinMonth(previousMonth.Year, previousMonth.Month);
    }


    private List<DateTime> GetWorkingDates() {

      List<DateTime> workingDays = new List<DateTime>();

      workingDays.Add(GetLastWorkingDateFromPreviousMonth());

      var balanceHelper = new TrialBalanceHelper(Query);

      workingDays.AddRange(balanceHelper.GetWorkingDaysRange());

      return workingDays;
    }


    private BalanzaDiferenciaDiariaMonedaEntry MapToDifferenceByDayEntry(BalanzaColumnasMonedaEntry x) {

      var entry = new BalanzaDiferenciaDiariaMonedaEntry();
      entry.MapFromBalanceByColumnEntry(x);

      return entry;
    }


    internal void ValuateEntriesToClosingExchangeRate(
      FixedList<TrialBalanceEntry> entries, DateTime fromDateFlag) {

      DateTime dateForLastWorkingDate = Query.InitialPeriod.FromDate < fromDateFlag ?
                                 fromDateFlag :
                                 Query.InitialPeriod.FromDate;

      var calendar = EmpiriaCalendar.Default;
      var lastWorkingDayInMonth = calendar.LastWorkingDateWithinMonth(
                                    dateForLastWorkingDate.Year, dateForLastWorkingDate.Month);

      var exchangeRateType = ExchangeRateType.Parse(ExchangeRateType.ValorizacionBanxico.UID);
      FixedList<ExchangeRate> exchangeRates = ExchangeRate.GetList(
                                                exchangeRateType, lastWorkingDayInMonth);

      foreach (var entry in entries) {

        entry.AssignClosingExchangeRateValueByCurrency(exchangeRates);
      }
    }


    internal void ValuateEntriesToExchangeRateByCurrency(FixedList<TrialBalanceEntry> entries) {

      var balanceInColumnHelper = new BalanzaColumnasMonedaHelper(Query);
      FixedList<ExchangeRate> exchangeRates = balanceInColumnHelper.GetExchangeRateList(false);

      foreach (var entry in entries) {

        entry.AssignExchangeRateValueByCurrency(exchangeRates);
      }
    }

    #endregion Private methods

  } // class BalanzaDiferenciaDiariaMonedaHelper

} // namespace Empiria.FinancialAccounting.BalanceEngine
