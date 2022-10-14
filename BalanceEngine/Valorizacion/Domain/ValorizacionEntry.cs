/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Empiria Plain Object                    *
*  Type     : ValorizacionEntry                          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Represents an entry for a valorized report entry.                                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Represents an entry for a valorized report entry.</summary>
  internal class ValorizacionEntry : ITrialBalanceEntry {


    public Currency Currency {
      get;
      internal set;
    }


    public StandardAccount Account {
      get;
      internal set;
    }


    public Sector Sector {
      get;
      internal set;
    }


    public decimal USD {
      get;
      internal set;
    }


    public decimal YEN {
      get;
      internal set;
    }


    public decimal EUR {
      get;
      internal set;
    }


    public decimal UDI {
      get;
      internal set;
    }


    public decimal LastExchangeRateUSD {
      get;
      internal set;
    }


    public decimal LastExchangeRateYEN {
      get;
      internal set;
    }


    public decimal LastExchangeRateEUR {
      get;
      internal set;
    }


    public decimal LastExchangeRateUDI {
      get;
      internal set;
    }


    public decimal ExchangeRateUSD {
      get;
      internal set;
    }


    public decimal ExchangeRateYEN {
      get;
      internal set;
    }


    public decimal ExchangeRateEUR {
      get;
      internal set;
    }


    public decimal ExchangeRateUDI {
      get;
      internal set;
    }


    public decimal TotalBalance {
      get;
      internal set;
    }


    public decimal ValuedExchangeRate {
      get;
      internal set;
    } = 1;


    public TrialBalanceItemType ItemType {
      get; internal set;
    }


    public bool HasParentPostingEntry {
      get; internal set;
    }


    public bool IsParentPostingEntry {
      get; internal set;
    }


    internal ValorizacionEntry MapToValorizedReport(TrialBalanceEntry entry) {

      this.ItemType = entry.ItemType;
      this.Account = entry.Account;
      this.Currency = entry.Currency;
      this.Sector = entry.Sector;

      this.HasParentPostingEntry = entry.HasParentPostingEntry;
      this.IsParentPostingEntry = entry.IsParentPostingEntry;

      AssingValues(entry);

      return this;
    }




    internal void AssingValues(TrialBalanceEntry entry) {
      if (entry.Currency.Equals(Currency.USD)) {
        this.USD = entry.InitialBalance;
        this.ExchangeRateUSD = entry.ExchangeRate;
        this.LastExchangeRateUSD = entry.SecondExchangeRate;
      }
      if (entry.Currency.Equals(Currency.YEN)) {
        this.YEN = entry.InitialBalance;
        this.ExchangeRateYEN = entry.ExchangeRate;
        this.LastExchangeRateYEN = entry.SecondExchangeRate;
      }
      if (entry.Currency.Equals(Currency.EUR)) {
        this.EUR = entry.InitialBalance;
        this.ExchangeRateEUR = entry.ExchangeRate;
        this.LastExchangeRateEUR = entry.SecondExchangeRate;
      }
      if (entry.Currency.Equals(Currency.UDI)) {
        this.UDI = entry.InitialBalance;
        this.ExchangeRateUDI = entry.ExchangeRate;
        this.LastExchangeRateUDI = entry.SecondExchangeRate;
      }
    }

  } // class ValorizacionEntry


} // namespace Empiria.FinancialAccounting.BalanceEngine
