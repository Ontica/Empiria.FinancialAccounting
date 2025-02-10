/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Service provider                        *
*  Type     : BalanzaDiferenciaDiariaMonedaBuilder       License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Genera los datos para el reporte de balanza diferencia diaria por moneda.                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DocumentFormat.OpenXml.Spreadsheet;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.Data;
using Empiria.Time;

namespace Empiria.FinancialAccounting.BalanceEngine {


  /// <summary>Genera los datos para el reporte de balanza diferencia diaria por moneda.</summary>
  internal class BalanzaDiferenciaDiariaMonedaBuilder {

    #region Constructors and parsers

    private readonly TrialBalanceQuery Query;
    private DateTime fromDateFlag;
    private DateTime toDateFlag;
    internal BalanzaDiferenciaDiariaMonedaBuilder(TrialBalanceQuery query) {
      Query = query;
    }

    #endregion Constructors and parsers


    #region Public methods

    internal FixedList<BalanzaDiferenciaDiariaMonedaEntry> Build() {

      FixedList<BalanzaColumnasMonedaEntry> balanzaColumnas = GetBalanceInColumnByCurrency();

      FixedList<BalanzaColumnasMonedaEntry> entriesByAccountAndDate = GetOrderingByAccountThenDate(
                                                                      balanzaColumnas);

      FixedList<BalanzaDiferenciaDiariaMonedaEntry> diffByDayEntries =
        MergeBalanceByCurrencyIntoCurrencyDiffByDay(entriesByAccountAndDate);

      CalculateBalancesForEntries(diffByDayEntries);

      FixedList<BalanzaDiferenciaDiariaMonedaEntry> entriesByDateAndAccount =
        GetOrderingByDateThenAccount(diffByDayEntries);

      return entriesByDateAndAccount;
    }

    #endregion Public methods


    #region Private methods

    private FixedList<BalanzaColumnasMonedaEntry> BuildAccountEntries() {

      FixedList<TrialBalanceEntry> baseAccountEntries = BalancesDataService.GetTrialBalanceEntries(Query);

      return BuildBalanceInColumnByCurrencyV2(baseAccountEntries);
    }


    internal FixedList<BalanzaColumnasMonedaEntry> BuildBalanceInColumnByCurrencyV1(
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


    private FixedList<BalanzaColumnasMonedaEntry> BuildBalanceInColumnByCurrencyV2(
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

      helper.ValuateEntriesToExchangeRateByCurrency(accountEntriesByCurrency.ToFixedList());

      helper.ValuateEntriesToClosingExchangeRate(accountEntriesByCurrency.ToFixedList());

      List<BalanzaColumnasMonedaEntry> balanceByCurrency =
                      helper.MergeTrialBalanceIntoBalanceByCurrency(accountEntriesByCurrency.ToFixedList());

      return balanceByCurrency.ToFixedList();
    }


    private void CalculateBalancesForEntries(
      FixedList<BalanzaDiferenciaDiariaMonedaEntry> diffByDayEntries) {

      DateTime previousDate = DateTime.MinValue;

      foreach (var entry in diffByDayEntries) {

        if (entry.FromDate < Query.InitialPeriod.FromDate) {
          entry.SetDailyBalance(new BalanzaDiferenciaDiariaMonedaEntry());

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


    private FixedList<BalanzaColumnasMonedaEntry> GetBalanceInColumnByCurrency() {

      FixedList<DateTime> workingDays = GetWorkingDaysRange();
      var balanceInColumnByCurrencyList = new List<BalanzaColumnasMonedaEntry>();

      foreach (var dateFilter in workingDays) {

        Query.AssignPeriodByWorkingDate(dateFilter);
        var balanzaColumnasBuilder = new BalanzaColumnasMonedaBuilder(this.Query);
        List<BalanzaColumnasMonedaEntry> balanzaColumnas = BuildAccountEntries().ToList();
        balanceInColumnByCurrencyList.AddRange(balanzaColumnas);
      }
      this.Query.InitialPeriod.FromDate = fromDateFlag;
      this.Query.InitialPeriod.ToDate = toDateFlag;

      return balanceInColumnByCurrencyList.ToFixedList();
    }


    private FixedList<BalanzaColumnasMonedaEntry> GetOrderingByAccountThenDate(
                                                  FixedList<BalanzaColumnasMonedaEntry> balanzaColumnas) {

      var orderingEntries = new List<BalanzaColumnasMonedaEntry>(balanzaColumnas);
      return orderingEntries.OrderBy(x => x.Account.Number)
                            .ThenBy(x => x.ToDate).ToFixedList();
    }


    private FixedList<BalanzaDiferenciaDiariaMonedaEntry> GetOrderingByDateThenAccount(
      FixedList<BalanzaDiferenciaDiariaMonedaEntry> diffByDayEntries) {

      var orderingEntries = new List<BalanzaDiferenciaDiariaMonedaEntry>(diffByDayEntries);

      return orderingEntries.OrderBy(x => x.ToDate)
                            .ThenBy(x => x.Account.Number).ToFixedList();
    }


    private FixedList<DateTime> GetWorkingDaysRange() {

      fromDateFlag = this.Query.InitialPeriod.FromDate;
      toDateFlag = this.Query.InitialPeriod.ToDate;

      List<DateTime> workingDays = new List<DateTime>();

      var calendar = EmpiriaCalendar.Default;
      var previousMonth = this.Query.InitialPeriod.FromDate.AddMonths(-1);
      
      workingDays.Add(calendar.LastWorkingDateWithinMonth(previousMonth.Year, previousMonth.Month));

      for (DateTime dateCount = this.Query.InitialPeriod.FromDate;
           dateCount <= this.Query.InitialPeriod.ToDate; dateCount = dateCount.AddDays(1)) {

        if (calendar.IsWorkingDate(dateCount)) {
          workingDays.Add(dateCount);
        }
      }

      if (workingDays.Count == 0) {
        throw Assertion.EnsureNoReachThisCode($"There must be at least one working day.");
      }

      return workingDays.ToFixedList();
    }


    private BalanzaDiferenciaDiariaMonedaEntry MapToDifferenceByDayEntry(BalanzaColumnasMonedaEntry x) {

      var entry = new BalanzaDiferenciaDiariaMonedaEntry();
      entry.MapFromBalanceByColumnEntry(x);

      return entry;
    }


    private FixedList<BalanzaDiferenciaDiariaMonedaEntry> MergeBalanceByCurrencyIntoCurrencyDiffByDay(
      FixedList<BalanzaColumnasMonedaEntry> entriesByAccountAndDate) {

      var mappedItems = entriesByAccountAndDate.Select((x) =>
                        MapToDifferenceByDayEntry((BalanzaColumnasMonedaEntry) x));

      return new FixedList<BalanzaDiferenciaDiariaMonedaEntry>(mappedItems);
    }

    #endregion Private methods

  } // class BalanzaDiferenciaDiariaMonedaBuilder

} // namespace Empiria.FinancialAccounting.BalanceEngine
