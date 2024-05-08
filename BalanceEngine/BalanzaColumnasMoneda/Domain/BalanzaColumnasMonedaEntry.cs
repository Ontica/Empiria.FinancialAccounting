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
  public class BalanzaColumnasMonedaEntry : ITrialBalanceEntry {

    public Currency Currency {
      get; internal set;
    }


    public StandardAccount Account {
      get; internal set;
    }


    public Sector Sector {
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


    public decimal TotalValorized {
      get; internal set;
    }


    public string GroupName {
      get; internal set;
    } = string.Empty;


    public string GroupNumber {
      get; internal set;
    } = string.Empty;


    public TrialBalanceItemType ItemType {
      get;
      internal set;
    }


    internal void Sum(BalanzaDolarizadaEntry entry) {

    }


    internal void MultiplyByValorizedValue(decimal value) {
      this.ValorizedDollarBalance *= value;
      this.ValorizedYenBalance *= value;
      this.ValorizedEuroBalance *= value;
      this.ValorizedUdisBalance *= value;
    }


    internal void GetValorizedValueByCurrency() {
      this.DollarBalance = this.ValorizedDollarBalance;
      this.YenBalance = this.ValorizedYenBalance;
      this.EuroBalance = this.ValorizedEuroBalance;
      this.UdisBalance = this.ValorizedUdisBalance;
    }


    internal void SumToTotalValorized() {
      TotalValorized += (
        this.DomesticBalance +
        this.ValorizedDollarBalance +
        this.ValorizedYenBalance +
        this.ValorizedEuroBalance +
        this.ValorizedUdisBalance
      );
    }
  } // BalanzaColumnasMonedaEntry

} // namespace Empiria.FinancialAccounting.BalanceEngine
