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

      FixedList<BalanzaDiferenciaDiariaMonedaEntry> balanceDifferenceByDayEntries =
        MergeBalanceByCurrencyIntoCurrencyDiffByDay(entriesByAccountAndDate);

      return balanceDifferenceByDayEntries;
    }


    private FixedList<BalanzaDiferenciaDiariaMonedaEntry> MergeBalanceByCurrencyIntoCurrencyDiffByDay(
      FixedList<BalanzaColumnasMonedaEntry> entriesByAccountAndDate) {

      var mappedItems = entriesByAccountAndDate.Select((x) =>
                          MapToDiffByDayEntry((BalanzaColumnasMonedaEntry) x));

      return new FixedList<BalanzaDiferenciaDiariaMonedaEntry>(mappedItems);
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


    private FixedList<BalanzaColumnasMonedaEntry> GetOrderByAccountAndDateEntries(
                                                  FixedList<BalanzaColumnasMonedaEntry> balanzaColumnas) {

      var orderingEntries = new List<BalanzaColumnasMonedaEntry>(balanzaColumnas);
      
      return orderingEntries.OrderBy(x => x.Account.Number)
                            .ThenBy(x => x.ToDate).ToFixedList();
    }

    #endregion Public methods


    #region Private methods

    private FixedList<BalanzaColumnasMonedaEntry> GetBalanzaColumnasMoneda(FixedList<DateTime> workingDays) {

      var balanzaColumnasList = new List<BalanzaColumnasMonedaEntry>();

      foreach (var date in workingDays) {
        this.Query.InitialPeriod.FromDate = date;
        this.Query.InitialPeriod.ToDate = date;
        this.Query.InitialPeriod.ValuateToCurrrencyUID = string.Empty;
        this.Query.UseDefaultValuation = false;

        var balanzaColumnasBuilder = new BalanzaColumnasMonedaBuilder(this.Query);
        List<BalanzaColumnasMonedaEntry> balanzaColumnas = balanzaColumnasBuilder.Build().ToList();

        balanzaColumnasList.AddRange(balanzaColumnas.Where(x => x.ItemType == TrialBalanceItemType.Entry));
      }

      return balanzaColumnasList.ToFixedList();
    }


    private FixedList<DateTime> GetWorkingDaysRange() {

      List<DateTime> workingDays = new List<DateTime>();
      for (DateTime i = this.Query.InitialPeriod.FromDate; i <= this.Query.InitialPeriod.ToDate; i = i.AddDays(1)) {

        var calendar = EmpiriaCalendar.Default;
        if (calendar.IsWorkingDate(i)) {
          workingDays.Add(i);
        }
      }

      if (workingDays.Count == 0) {
        throw Assertion.EnsureNoReachThisCode($"There must be at least one working day.");
      }

      return workingDays.ToFixedList();
    }

    #endregion Private methods




  } // class BalanzaDiferenciaDiariaMonedaBuilder

} // namespace Empiria.FinancialAccounting.BalanceEngine
