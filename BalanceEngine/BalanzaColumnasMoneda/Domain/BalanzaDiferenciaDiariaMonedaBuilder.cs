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
using Empiria.Time;

namespace Empiria.FinancialAccounting.BalanceEngine {


  /// <summary>Genera los datos para el reporte de balanza diferencia diaria por moneda.</summary>
  internal class BalanzaDiferenciaDiariaMonedaBuilder {

    #region Constructors and parsers

    private readonly TrialBalanceQuery Query;

    internal BalanzaDiferenciaDiariaMonedaBuilder(TrialBalanceQuery query) {
      Query = query;
    }

    #endregion Constructors and parsers


    #region Public methods

    internal FixedList<BalanzaDiferenciaDiariaMonedaEntry> BuildBalanceByColumnValorized() {

      FixedList<DateTime> workingDays = GetWorkingDaysRange();

      FixedList<BalanzaColumnasMonedaEntry> balanzaColumnas = GetBalanzaColumnasMoneda(workingDays);

      FixedList<BalanzaColumnasMonedaEntry> entriesByAccountAndDate = GetOrderByAccountAndDateEntries(
                                                                      balanzaColumnas);

      FixedList<BalanzaDiferenciaDiariaMonedaEntry> diffByDayEntries =
        MergeBalanceByCurrencyIntoCurrencyDiffByDay(entriesByAccountAndDate);

      CalculateDifferenceByDayEntries(diffByDayEntries);

      return diffByDayEntries;
    }

    #endregion Public methods


    #region Private methods

    private void CalculateDifferenceByDayEntries(
      FixedList<BalanzaDiferenciaDiariaMonedaEntry> diffByDayEntries) {

      foreach (var entry in diffByDayEntries.Where(x => x.ToDate.Month == Query.InitialPeriod.ToDate.Month)) {

        var previousDayEntry = diffByDayEntries.Find(x => x.Account.Number == entry.Account.Number &&
                                                  x.ToDate == entry.ToDate.AddDays(-1));
        if (previousDayEntry != null) {
          entry.DomesticDailyBalance = entry.DomesticBalance - previousDayEntry.DomesticBalance;
          entry.DollarDailyBalance = entry.DollarBalance - previousDayEntry.DollarBalance;
          entry.YenDailyBalance = entry.YenBalance - previousDayEntry.YenBalance;
          entry.EuroDailyBalance = entry.EuroBalance - previousDayEntry.EuroBalance;
          entry.UdisDailyBalance = entry.UdisBalance - previousDayEntry.UdisBalance;
        }
      }
    }


    private FixedList<BalanzaColumnasMonedaEntry> GetBalanzaColumnasMoneda(FixedList<DateTime> workingDays) {

      var fromDateFlag = this.Query.InitialPeriod.FromDate;
      var toDateFlag = this.Query.InitialPeriod.ToDate;
      var balanzaColumnasList = new List<BalanzaColumnasMonedaEntry>();

      foreach (var dateFilter in workingDays) {
        this.Query.InitialPeriod.FromDate = dateFilter;
        this.Query.InitialPeriod.ToDate = dateFilter;
        this.Query.InitialPeriod.ValuateToCurrrencyUID = string.Empty;
        this.Query.UseDefaultValuation = false;

        var balanzaColumnasBuilder = new BalanzaColumnasMonedaBuilder(this.Query);
        List<BalanzaColumnasMonedaEntry> balanzaColumnas = balanzaColumnasBuilder.Build().ToList();

        balanzaColumnasList.AddRange(balanzaColumnas.Where(x => x.ItemType == TrialBalanceItemType.Entry));
      }
      this.Query.InitialPeriod.FromDate = fromDateFlag;
      this.Query.InitialPeriod.ToDate = toDateFlag;

      return balanzaColumnasList.ToFixedList();
    }


    private FixedList<BalanzaColumnasMonedaEntry> GetOrderByAccountAndDateEntries(
                                                  FixedList<BalanzaColumnasMonedaEntry> balanzaColumnas) {

      var orderingEntries = new List<BalanzaColumnasMonedaEntry>(balanzaColumnas);

      return orderingEntries.OrderBy(x => x.Account.Number)
                            .ThenBy(x => x.ToDate).ToFixedList();
    }


    private FixedList<DateTime> GetWorkingDaysRange() {

      List<DateTime> workingDays = new List<DateTime>();

      var calendar = EmpiriaCalendar.Default;
      var previousMonth = this.Query.InitialPeriod.ToDate.AddMonths(-1);
      var lastWorkingDatePreviousMonth = calendar.LastWorkingDateWithinMonth(
                                          previousMonth.Year, previousMonth.Month);

      workingDays.Add(lastWorkingDatePreviousMonth);

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


    private BalanzaDiferenciaDiariaMonedaEntry MapToDiffByDayEntry(BalanzaColumnasMonedaEntry x) {

      return new BalanzaDiferenciaDiariaMonedaEntry() {
        FromDate = x.FromDate,
        ToDate = x.ToDate,
        Account = x.Account,
        DomesticBalance = x.DomesticBalance,
        DollarBalance = x.DollarBalance,
        YenBalance = x.YenBalance,
        EuroBalance = x.EuroBalance,
        UdisBalance = x.UdisBalance,
        ValorizedDollarBalance = x.ValorizedDollarBalance,
        ValorizedYenBalance = x.ValorizedYenBalance,
        ValorizedEuroBalance = x.ValorizedEuroBalance,
        ValorizedUdisBalance = x.ValorizedUdisBalance,
        ExchangeRateForDollar = x.ExchangeRateForDollar,
        ExchangeRateForYen = x.ExchangeRateForYen,
        ExchangeRateForEuro = x.ExchangeRateForEuro,
        ExchangeRateForUdi = x.ExchangeRateForUdi
      };
    }


    private FixedList<BalanzaDiferenciaDiariaMonedaEntry> MergeBalanceByCurrencyIntoCurrencyDiffByDay(
      FixedList<BalanzaColumnasMonedaEntry> entriesByAccountAndDate) {

      var mappedItems = entriesByAccountAndDate.Select((x) =>
                          MapToDiffByDayEntry((BalanzaColumnasMonedaEntry) x));

      return new FixedList<BalanzaDiferenciaDiariaMonedaEntry>(mappedItems);
    }

    #endregion Private methods

  } // class BalanzaDiferenciaDiariaMonedaBuilder

} // namespace Empiria.FinancialAccounting.BalanceEngine
