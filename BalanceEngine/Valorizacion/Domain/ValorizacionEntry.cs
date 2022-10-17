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


    public decimal TotalBalance {
      get;
      internal set;
    }


    public decimal ValuedExchangeRate {
      get;
      internal set;
    } = 1;


    public ValorizationByCurrency ValuesByCurrency {
      get;
      internal set;
    } = new ValorizationByCurrency();


    public TrialBalanceItemType ItemType {
      get;
      internal set;
    }


    public DateTime LastChangeDate {
      get;
      internal set;
    }


    public DateTime ConsultingDate {
      get;
      internal set;
    }


    public bool HasParentPostingEntry {
      get; internal set;
    }


    public bool IsParentPostingEntry {
      get; internal set;
    }


    internal ValorizacionEntry MapToValorizedReport(TrialBalanceEntry entry, DateTime date) {

      this.ItemType = entry.ItemType;
      this.Account = entry.Account;
      this.Currency = entry.Currency;
      this.Sector = entry.Sector;

      this.LastChangeDate = entry.LastChangeDate;
      this.ConsultingDate = date;
      this.HasParentPostingEntry = entry.HasParentPostingEntry;
      this.IsParentPostingEntry = entry.IsParentPostingEntry;

      AssingValues(entry);

      return this;
    }




    internal void AssingValues(TrialBalanceEntry entry) {
      if (entry.Currency.Equals(Currency.USD)) {
        
        this.ValuesByCurrency.USD = entry.InitialBalance;
        this.ValuesByCurrency.ExchangeRateUSD = entry.ExchangeRate;
        this.ValuesByCurrency.LastExchangeRateUSD = entry.SecondExchangeRate;
      }
      if (entry.Currency.Equals(Currency.YEN)) {
        
        this.ValuesByCurrency.YEN = entry.InitialBalance;
        this.ValuesByCurrency.ExchangeRateYEN = entry.ExchangeRate;
        this.ValuesByCurrency.LastExchangeRateYEN = entry.SecondExchangeRate;
      }
      if (entry.Currency.Equals(Currency.EUR)) {

        this.ValuesByCurrency.EUR = entry.InitialBalance;
        this.ValuesByCurrency.ExchangeRateEUR = entry.ExchangeRate;
        this.ValuesByCurrency.LastExchangeRateEUR = entry.SecondExchangeRate;
      }
      if (entry.Currency.Equals(Currency.UDI)) {

        this.ValuesByCurrency.UDI = entry.InitialBalance;
        this.ValuesByCurrency.ExchangeRateUDI = entry.ExchangeRate;
        this.ValuesByCurrency.LastExchangeRateUDI = entry.SecondExchangeRate;
      }
    }

  } // class ValorizacionEntry


} // namespace Empiria.FinancialAccounting.BalanceEngine
