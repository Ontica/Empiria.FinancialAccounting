/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Builder                                 *
*  Type     : BalanzaValorizadaBuilder                   License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Builds BalanzaValorizada entries to determine daily exchange rate profit or loss variation.    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System.Collections.Generic;
using System.Linq;
using Empiria.DynamicData;
using Empiria.Time;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Builds BalanzaValorizada entries to determine daily exchange rate profit or loss variation.</summary>
  internal class BalanzaValorizadaBuilder {

    private readonly FixedList<TrialBalanceEntry> _balances;
    private readonly FixedList<MovimientosPorDia> _entries;
    private readonly FixedList<ExchangeRate> _exchangeRates;

    public BalanzaValorizadaBuilder(FixedList<TrialBalanceEntry> balances,
                                    FixedList<MovimientosPorDia> entries,
                                    FixedList<ExchangeRate> exchangeRates) {
      _balances = balances;
      _entries = entries;
      _exchangeRates = exchangeRates;
    }


    internal FixedList<BalanzaValorizadaEntry> Build(TimePeriod period) {

      var list = new List<BalanzaValorizadaEntry>(_balances.Count);

      foreach (var balance in _balances.FindAll(x => x.Account.Role != AccountRole.Sumaria &&
                                                     !x.HasSector)) {

        if (balance.Currency.Equals(Currency.MXN)) {
          BalanzaValorizadaEntry mxnEntry = BuildMxnEntry(balance, period);

          list.Add(mxnEntry);

        } else {
          FixedList<BalanzaValorizadaEntry> entries = BuildEntries(balance, period);

          list.AddRange(entries);
        }
      }

      return list.ToFixedList();
    }

    private BalanzaValorizadaEntry BuildMxnEntry(TrialBalanceEntry balance, TimePeriod period) {

      var entries = _entries.FindAll(x => x.CuentaEstandar.Equals(balance.Account) &&
                                          x.Moneda.Equals(Currency.MXN));

      var entry = new BalanzaValorizadaEntry(balance, period.EndTime,
                                             balance.InitialBalance,
                                             entries.Sum(x => x.Cargos),
                                             entries.Sum(x => x.Abonos));
      return entry;
    }

    private FixedList<BalanzaValorizadaEntry> BuildEntries(TrialBalanceEntry balance, TimePeriod period) {

      var dailyExchangeRates = _exchangeRates.FindAll(x => period.StartTime <= x.Date && x.Date <= period.EndTime &&
                                                           x.ToCurrency.Equals(balance.Currency) &&
                                                           x.ExchangeRateType.Equals(ExchangeRateType.Diario));

      var banxicoExchangeRates = _exchangeRates.FindAll(x => x.Date <= period.EndTime &&
                                                             x.ToCurrency.Equals(balance.Currency) &&
                                                             x.ExchangeRateType.Equals(ExchangeRateType.ValorizacionBanxico));

      var list = new List<BalanzaValorizadaEntry>(dailyExchangeRates.Count);


      decimal accumulatedBalance = balance.InitialBalance;

      ExchangeRate lastExchangeRate = null;

      for (var date = period.StartTime; date <= period.EndTime; date = date.AddDays(1)) {

        ExchangeRate exchangeRate;

        exchangeRate = dailyExchangeRates.Find(x => x.Date == date);

        if (date.Day == 1) {
          lastExchangeRate = banxicoExchangeRates.FindLast(x => x.Date < date);
        }

        if (exchangeRate == null) {
          continue;
        }

        var dateEntry = _entries.Find(x => x.CuentaEstandar.Equals(balance.Account) &&
                                           x.Moneda.Equals(balance.Currency) &&
                                           x.FechaAfectacion == date);

        var entry = new BalanzaValorizadaEntry(balance, date, accumulatedBalance,
                                               dateEntry, exchangeRate, lastExchangeRate);

        list.Add(entry);

        lastExchangeRate = exchangeRate;

        if (dateEntry != null) {
          if (balance.Account.DebtorCreditor == DebtorCreditorType.Deudora) {
            accumulatedBalance = accumulatedBalance + dateEntry.Cargos - dateEntry.Abonos;
          } else {
            accumulatedBalance = accumulatedBalance - dateEntry.Cargos + dateEntry.Abonos;
          }
        }
      }

      return list.ToFixedList();
    }


    internal FixedList<DataTableColumn> GetColumns() {
      return new DataTableColumn[] {
        new DataTableColumn("numeroCuenta", "Cuenta", "text-nowrap"),
        new DataTableColumn("nombreCuenta", "Descripción", "text"),
        new DataTableColumn("codigoMoneda", "Moneda", "text"),
        new DataTableColumn("fechaAfectacion", "Fecha", "date"),
        new DataTableColumn("saldoInicial", "Saldo inicial MO", "decimal"),
        new DataTableColumn("cargos", "Cargos MO", "decimal"),
        new DataTableColumn("abonos", "Abonos MO", "decimal"),
        new DataTableColumn("saldoFinal", "Saldo final MO", "decimal"),
        new DataTableColumn("tipoCambio", "T. Cambio", "decimal", 6),
        new DataTableColumn("saldoInicialMXN", "Saldo inicial MXN", "decimal", 8),
        new DataTableColumn("cargosMXN", "Cargos MXN", "decimal", 8),
        new DataTableColumn("abonosMXN", "Abonos MXN", "decimal", 8),
        new DataTableColumn("saldoFinalMXN", "Saldo final MXN", "decimal", 8),
        new DataTableColumn("utilidadCambiaria", "Utilidad cambiaria", "decimal", 8),
      }.ToFixedList();
    }

  }  // class BalanzaValorizadaBuilder

}  // namespace Empiria.FinancialAccounting.BalanceEngine
