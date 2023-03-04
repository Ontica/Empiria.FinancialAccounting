/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Empiria Plain Object                    *
*  Type     : ValorizacionEstimacionPreventivaEntry      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Represents an entry for a valorized report entry.                                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Represents an entry for a valorized report entry.</summary>
  internal class ValorizacionEstimacionPreventivaEntry : DynamicFields, ITrialBalanceEntry {


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


    public decimal MXN {
      get; internal set;
    }


    public decimal MXNDebit {
      get; internal set;
    }


    public decimal MXNCredit {
      get; internal set;
    }


    public DebtorCreditorType DebtorCreditor {
      get; internal set;
    } = DebtorCreditorType.Deudora;


    public decimal ValuedExchangeRate {
      get;
      internal set;
    } = 1;


    public ValorizationByCurrency ValuesByCurrency {
      get;
      internal set;
    } = new ValorizationByCurrency();


    //public List<ValuesByMonth> ValuesByMonth {
    //  get;
    //  internal set;
    //} = new List<ValuesByMonth>();


    public decimal TotalValued {
      get;
      internal set;
    }


    public decimal TotalAccumulated {
      get;
      internal set;
    }


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


    public int MonthPosition {
      get;
      internal set;
    }


    public bool HasParentPostingEntry {
      get; internal set;
    }


    public bool IsParentPostingEntry {
      get; internal set;
    }


    internal ValorizacionEstimacionPreventivaEntry MapToValorizedReport(TrialBalanceEntry entry, DateTime date) {

      this.ItemType = entry.ItemType;
      this.Account = entry.Account;
      this.Currency = entry.Currency;
      this.Sector = entry.Sector;
      this.DebtorCreditor = entry.DebtorCreditor;

      this.LastChangeDate = entry.LastChangeDate;
      this.ConsultingDate = date;
      this.HasParentPostingEntry = entry.HasParentPostingEntry;
      this.IsParentPostingEntry = entry.IsParentPostingEntry;

      AssingValues(entry, date);

      return this;
    }


    internal void AssingValues(TrialBalanceEntry entry, DateTime date) {
      decimal balance = entry.InitialBalance;
      
      if (date.Month == 1) {
        balance = entry.CurrentBalance;

      }

      if (entry.Currency.Equals(Currency.USD)) {

        this.ValuesByCurrency.USD = balance;
        this.ValuesByCurrency.ExchangeRateUSD = entry.ExchangeRate;
        this.ValuesByCurrency.LastExchangeRateUSD = entry.SecondExchangeRate;
        this.ValuesByCurrency.LastUSD = balance * entry.SecondExchangeRate;
        this.ValuesByCurrency.ValuedUSD = balance * entry.ExchangeRate;
        this.ValuesByCurrency.ValuedEffectUSD = this.ValuesByCurrency.LastUSD - this.ValuesByCurrency.ValuedUSD;

        this.ValuesByCurrency.USDDebit = entry.Debit;
        this.ValuesByCurrency.ValuedUSDDebit = entry.Debit * entry.ExchangeRate;

        this.ValuesByCurrency.USDCredit = entry.Credit;
        this.ValuesByCurrency.ValuedUSDCredit = entry.Credit * entry.ExchangeRate;

      }
      if (entry.Currency.Equals(Currency.YEN)) {

        this.ValuesByCurrency.YEN = balance;
        this.ValuesByCurrency.ExchangeRateYEN = entry.ExchangeRate;
        this.ValuesByCurrency.LastExchangeRateYEN = entry.SecondExchangeRate;
        this.ValuesByCurrency.LastYEN = balance * entry.SecondExchangeRate;
        this.ValuesByCurrency.ValuedYEN = balance * entry.ExchangeRate;
        this.ValuesByCurrency.ValuedEffectYEN = this.ValuesByCurrency.LastYEN - this.ValuesByCurrency.ValuedYEN;

        this.ValuesByCurrency.YENDebit = entry.Debit;
        this.ValuesByCurrency.ValuedYENDebit = entry.Debit * entry.ExchangeRate;

        this.ValuesByCurrency.YENCredit = entry.Credit;
        this.ValuesByCurrency.ValuedYENCredit = entry.Credit * entry.ExchangeRate;
      }
      if (entry.Currency.Equals(Currency.EUR)) {

        this.ValuesByCurrency.EUR = balance;
        this.ValuesByCurrency.ExchangeRateEUR = entry.ExchangeRate;
        this.ValuesByCurrency.LastExchangeRateEUR = entry.SecondExchangeRate;
        this.ValuesByCurrency.LastEUR = balance * entry.SecondExchangeRate;
        this.ValuesByCurrency.ValuedEUR = balance * entry.ExchangeRate;
        this.ValuesByCurrency.ValuedEffectEUR = this.ValuesByCurrency.LastEUR - this.ValuesByCurrency.ValuedEUR;

        this.ValuesByCurrency.EURDebit = entry.Debit;
        this.ValuesByCurrency.ValuedEURDebit = entry.Debit * entry.ExchangeRate;

        this.ValuesByCurrency.EURCredit = entry.Credit;
        this.ValuesByCurrency.ValuedEURCredit = entry.Credit * entry.ExchangeRate;
      }
      if (entry.Currency.Equals(Currency.UDI)) {

        this.ValuesByCurrency.UDI = balance;
        this.ValuesByCurrency.ExchangeRateUDI = entry.ExchangeRate;
        this.ValuesByCurrency.LastExchangeRateUDI = entry.SecondExchangeRate;
        this.ValuesByCurrency.LastUDI = balance * entry.SecondExchangeRate;
        this.ValuesByCurrency.ValuedUDI = balance * entry.ExchangeRate;
        this.ValuesByCurrency.ValuedEffectUDI = this.ValuesByCurrency.LastUDI - this.ValuesByCurrency.ValuedUDI;

        this.ValuesByCurrency.UDIDebit = entry.Debit;
        this.ValuesByCurrency.ValuedUDIDebit = entry.Debit * entry.ExchangeRate;

        this.ValuesByCurrency.UDICredit = entry.Credit;
        this.ValuesByCurrency.ValuedUDICredit = entry.Credit * entry.ExchangeRate;
      }
    }

  } // class ValorizacionEstimacionPreventivaEntry


} // namespace Empiria.FinancialAccounting.BalanceEngine
