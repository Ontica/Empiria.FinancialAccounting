/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Empiria Plain Object                    *
*  Type     : BalanzaColumnasMonedaEntry                 License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Represents an entry for a Balanza en columnas por moneda entry.                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.BalanceEngine {


  /// <summary>Represents an entry for a Balanza en columnas por moneda entry.</summary>
  public class BalanzaColumnasMonedaEntry : BalanzaColumnasMonedaCommons, ITrialBalanceEntry {

    public Currency Currency {
      get; internal set;
    }


    public Sector Sector {
      get; internal set;
    }


    public decimal TotalValorized {
      get; internal set;
    }


    public string GroupName {
      get; internal set;
    } = string.Empty;


    public string GroupNumber {
      get; internal set;
    } = string.Empty;


    internal void SumToTotalValorized(bool valorizedBanalce = false) {

      if (valorizedBanalce) {

        TotalValorized += (
          this.DomesticBalance +
          this.ValorizedDollarBalance +
          this.ValorizedYenBalance +
          this.ValorizedEuroBalance +
          this.ValorizedUdisBalance
        );

      } else {

        TotalValorized += (
          this.DomesticBalance +
          this.DollarBalance +
          this.YenBalance +
          this.EuroBalance +
          this.UdisBalance
        );
      }   
    }

  } // BalanzaColumnasMonedaEntry


  public class BalanzaColumnasMonedaCommons {


    public TrialBalanceItemType ItemType {
      get; internal set;
    }


    public DateTime FromDate {
      get; internal set;
    }


    public DateTime ToDate {
      get; internal set;
    }


    public StandardAccount Account {
      get; internal set;
    }


    public decimal DomesticBalance {
      get; internal set;
    }


    public decimal DollarBalance {
      get; internal set;
    }


    public decimal YenBalance {
      get; internal set;
    }


    public decimal EuroBalance {
      get; internal set;
    }


    public decimal UdisBalance {
      get; internal set;
    }


    public decimal ValorizedDollarBalance {
      get; internal set;
    }


    public decimal ValorizedYenBalance {
      get; internal set;
    }


    public decimal ValorizedEuroBalance {
      get; internal set;
    }


    public decimal ValorizedUdisBalance {
      get; internal set;
    }


    public decimal ExchangeRateForDollar {
      get; internal set;
    }


    public decimal ExchangeRateForYen {
      get; internal set;
    }


    public decimal ExchangeRateForEuro {
      get; internal set;
    }


    public decimal ExchangeRateForUdi {
      get; internal set;
    }


    public decimal ClosingExchangeRateForDollar {
      get; internal set;
    }


    public decimal ClosingExchangeRateForYen {
      get; internal set;
    }


    public decimal ClosingExchangeRateForEuro {
      get; internal set;
    }


    public decimal ClosingExchangeRateForUdi {
      get; internal set;
    }

  } // class BalanzaColumnasMonedaCommons


} // namespace Empiria.FinancialAccounting.BalanceEngine
