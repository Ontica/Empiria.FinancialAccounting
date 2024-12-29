/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Empiria Plain Object                    *
*  Type     : BalanzaDiferenciaDiariaMonedaEntry         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Represents an entry for a Balanza diferencia diaria por moneda.                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.BalanceEngine {


  /// <summary>Represents an entry for a Balanza diferencia diaria por moneda.</summary>
  internal class BalanzaDiferenciaDiariaMonedaEntry : BalanzaColumnasMonedaCommons, ITrialBalanceEntry {

    public decimal DomesticDailyBalance {
      get; internal set;
    }


    public decimal DollarDailyBalance {
      get; internal set;
    }


    public decimal YenDailyBalance {
      get; internal set;
    }


    public decimal EuroDailyBalance {
      get; internal set;
    }


    public decimal UdisDailyBalance {
      get; internal set;
    }


    public decimal ValorizedDailyDollarBalance {
      get; internal set;
    }


    public decimal ValorizedDailyYenBalance {
      get; internal set;
    }


    public decimal ValorizedDailyEuroBalance {
      get; internal set;
    }


    public decimal ValorizedDailyUdisBalance {
      get; internal set;
    }


    internal void SetDailyBalance(BalanzaDiferenciaDiariaMonedaEntry previousDayEntry) {
      this.DomesticDailyBalance = this.DomesticBalance - previousDayEntry.DomesticBalance;
      this.DollarDailyBalance = this.DollarBalance - previousDayEntry.DollarBalance;
      this.YenDailyBalance = this.YenBalance - previousDayEntry.YenBalance;
      this.EuroDailyBalance = this.EuroBalance - previousDayEntry.EuroBalance;
      this.UdisDailyBalance = this.UdisBalance - previousDayEntry.UdisBalance;
    }


    internal void SetValorizedDailyBalance() {
      this.ValorizedDailyDollarBalance = (this.ClosingExchangeRateForDollar - this.ExchangeRateForDollar) * DollarDailyBalance;
      this.ValorizedDailyYenBalance = (this.ClosingExchangeRateForYen - this.ExchangeRateForYen) * YenDailyBalance;
      this.ValorizedDailyEuroBalance = (this.ClosingExchangeRateForEuro - this.ExchangeRateForEuro) * EuroDailyBalance;
      this.ValorizedDailyUdisBalance = (this.ClosingExchangeRateForUdi - this.ExchangeRateForUdi) * UdisDailyBalance;
    }

  } // class BalanzaDiferenciaDiariaMonedaEntry

} // namespace Empiria.FinancialAccounting.BalanceEngine
