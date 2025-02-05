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
    private DateTime fromDateFlag;
    private DateTime toDateFlag;
    internal BalanzaDiferenciaDiariaMonedaBuilder(TrialBalanceQuery query) {
      Query = query;
    }

    #endregion Constructors and parsers


    #region Public methods


    internal FixedList<BalanzaDiferenciaDiariaMonedaEntry> Build() {

      FixedList<BalanzaColumnasMonedaEntry> balanzaColumnas = GetBalanzaColumnasMoneda();

      FixedList<BalanzaColumnasMonedaEntry> entriesByAccountAndDate = GetOrderingByAccountAndDate(
                                                                      balanzaColumnas);

      FixedList<BalanzaDiferenciaDiariaMonedaEntry> diffByDayEntries =
        MergeBalanceByCurrencyIntoCurrencyDiffByDay(entriesByAccountAndDate);

      CalculateDifferenceByDayEntries(diffByDayEntries);

      FixedList<BalanzaDiferenciaDiariaMonedaEntry> entriesByDateAndAccount =
        GetOrderingByDateAndAccount(diffByDayEntries);

      return entriesByDateAndAccount;
    }


    private FixedList<BalanzaDiferenciaDiariaMonedaEntry> GetOrderingByDateAndAccount(
      FixedList<BalanzaDiferenciaDiariaMonedaEntry> diffByDayEntries) {

      var orderingEntries = new List<BalanzaDiferenciaDiariaMonedaEntry>(diffByDayEntries);

      return orderingEntries.OrderBy(x => x.ToDate)
                            .ThenBy(x => x.Account.Number).ToFixedList();
    }

    #endregion Public methods


    #region Private methods


    private void CalculateDifferenceByDayEntries(
      FixedList<BalanzaDiferenciaDiariaMonedaEntry> diffByDayEntries) {

      DateTime previousDate = DateTime.MinValue;

      foreach (var entry in diffByDayEntries) {

        if (entry.FromDate < Query.InitialPeriod.FromDate) {
          entry.SetDailyBalance(new BalanzaDiferenciaDiariaMonedaEntry());
          
        } else if (previousDate != DateTime.MinValue && previousDate < entry.ToDate) {

          var calendar = EmpiriaCalendar.Default;
          
          DateTime lastWorkingDateInMonth =
            calendar.LastWorkingDateWithinMonth(entry.ToDate.Year, entry.ToDate.Month);

          var previousDayEntry = diffByDayEntries.Find(x => x.ToDate == previousDate &&
                                                       x.Account.Number == entry.Account.Number);
          if (entry.ToDate == lastWorkingDateInMonth) {
            entry.SetDailyBalance(new BalanzaDiferenciaDiariaMonedaEntry());

          } else if (previousDayEntry != null) {
            entry.SetDailyBalance(previousDayEntry);
            entry.SetValorizedDailyBalance();
          }
        }
        previousDate = entry.ToDate;

      }
    }


    private FixedList<BalanzaColumnasMonedaEntry> GetBalanzaColumnasMoneda() {

      FixedList<DateTime> workingDays = GetWorkingDaysRange();
      var balanzaColumnasList = new List<BalanzaColumnasMonedaEntry>();

      foreach (var dateFilter in workingDays) {

        Query.AssignPeriodByWorkingDate(dateFilter);
        var balanzaColumnasBuilder = new BalanzaColumnasMonedaBuilder(this.Query);
        List<BalanzaColumnasMonedaEntry> balanzaColumnas = balanzaColumnasBuilder.Build().ToList();
        balanzaColumnasList.AddRange(balanzaColumnas);

      }
      this.Query.InitialPeriod.FromDate = fromDateFlag;
      this.Query.InitialPeriod.ToDate = toDateFlag;

      return balanzaColumnasList.ToFixedList();
    }


    private FixedList<BalanzaColumnasMonedaEntry> GetOrderingByAccountAndDate(
                                                  FixedList<BalanzaColumnasMonedaEntry> balanzaColumnas) {

      var orderingEntries = new List<BalanzaColumnasMonedaEntry>(balanzaColumnas);
      return orderingEntries.OrderBy(x => x.Account.Number)
                            .ThenBy(x => x.ToDate).ToFixedList();
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


    private BalanzaDiferenciaDiariaMonedaEntry MapToDiffByDayEntry(BalanzaColumnasMonedaEntry x) {

      var entry = new BalanzaDiferenciaDiariaMonedaEntry();
      entry.MapFromBalanceByColumnEntry(x);

      return entry;
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
