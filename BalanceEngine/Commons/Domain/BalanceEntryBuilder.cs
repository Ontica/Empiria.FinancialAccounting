/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Information Holder                      *
*  Type     : BalanceEntryBuilder                        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Contains the entries of a balance.                                                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Linq;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.Data;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Contains the entries of a balance.</summary>
  internal class BalanceEntryBuilder {

    #region Constructors and parsers

    private readonly TrialBalanceQuery _query;

    internal BalanceEntryBuilder(TrialBalanceQuery query) {
      _query = query;
    }

    #endregion Constructors and parsers

    internal FixedList<BalanceEntry> GetBalanceEntries() {

      FixedList<BalanceEntry> entries = BalanceEntryDataService.GetBalanceEntries(_query);

      EntriesToExchangeRate(entries.FindAll(x=>x.Currency != Currency.MXN));

      return entries;
    }


    static internal FixedList<BalanceEntry> GetBalancesByAccount(string accountNumber,
                                                                  FixedList<BalanceEntry> balanceEntries) {

      var returned = balanceEntries.FindAll(x => x.Account.Number.StartsWith(accountNumber)).ToFixedList();
      return returned;
    }


    static internal FixedList<BalanceEntry> GetBalancesByAccountAndSector(string accountNumber,
                                             string sectorCode, FixedList<BalanceEntry> balanceEntries) {

      if (sectorCode == "00") {
        return balanceEntries.FindAll(x => x.Account.Number.StartsWith(accountNumber)).ToFixedList();

      } else {
        return balanceEntries.FindAll(x => x.Account.Number.StartsWith(accountNumber) &&
                                              x.Sector.Code == sectorCode).ToFixedList();
      }
    }


    static internal decimal GetSumByME(FixedList<BalanceEntry> filtered) {
      return filtered.FindAll(x => x.Currency != Currency.MXN && x.Currency != Currency.UDI)
                     .Sum(x => x.CurrentBalance);
    }


    static internal decimal GetSumByMN(FixedList<BalanceEntry> filtered) {
      return filtered.FindAll(x => x.Currency == Currency.MXN || x.Currency == Currency.UDI)
                     .Sum(x => x.CurrentBalance);

    }

    #region Private methods

    internal void EntriesToExchangeRate(FixedList<BalanceEntry> entries) {

      var exchangeRateType = ExchangeRateType.Parse(ExchangeRateType.ValorizacionBanxico.UID);
      FixedList<ExchangeRate> exchangeRates = ExchangeRate.GetList(exchangeRateType,
                                                                   _query.InitialPeriod.ToDate);
      foreach (var entry in entries) {

        var exchangeRate = exchangeRates.Find(
                            a => a.ToCurrency.Equals(entry.Currency) &&
                            a.FromCurrency.Code == Currency.MXN.Code);

        Assertion.Require(exchangeRate, $"No se ha registrado el tipo de cambio para la " +
                                        $"moneda {entry.Currency.FullName} en la fecha proporcionada.");

        entry.MultiplyBy(exchangeRate.Value);
      }
    }

    
    #endregion Private methods

  } // class BalanceEntryBuilder

} // namespace Empiria.FinancialAccounting.BalanceEngine
