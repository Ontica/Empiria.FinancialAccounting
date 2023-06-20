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


    internal ValorizacionEstimacionPreventivaEntry MapToValorizedReport(
            TrialBalanceEntry entry, DateTime date, bool isPreviousMonth) {

      this.ItemType = entry.ItemType;
      this.Account = entry.Account;
      this.Currency = entry.Currency;
      this.Sector = entry.Sector;
      this.DebtorCreditor = entry.DebtorCreditor;

      this.LastChangeDate = entry.LastChangeDate;
      this.ConsultingDate = date;
      this.HasParentPostingEntry = entry.HasParentPostingEntry;
      this.IsParentPostingEntry = entry.IsParentPostingEntry;

      AssingValues(entry, date, isPreviousMonth);

      return this;
    }


    internal void AssingValues(TrialBalanceEntry entry, DateTime date, bool isPreviousMonth = false) {
      decimal balance = entry.InitialBalance; //entry.CurrentBalance;

      //if (date.Month == 1) {
      //  balance = entry.InitialBalance;
      //}

      if (entry.Currency.Equals(Currency.MXN)) {
        this.MXN = entry.CurrentBalance;
        this.MXNDebit = entry.Debit;
        this.MXNCredit = entry.Credit;
      }

      if (entry.Currency.Equals(Currency.USD)) {

        this.ValuesByCurrency.USD = balance;
        this.ValuesByCurrency.ExchangeRateUSD = entry.ExchangeRate;
        this.ValuesByCurrency.LastExchangeRateUSD = entry.SecondExchangeRate;
        this.ValuesByCurrency.LastUSD = Math.Round((balance * entry.SecondExchangeRate), 2);
        this.ValuesByCurrency.ValuedUSD = Math.Round((balance * entry.ExchangeRate), 2);
        this.ValuesByCurrency.ValuedEffectUSD = this.ValuesByCurrency.LastUSD -
                                                this.ValuesByCurrency.ValuedUSD;

        this.ValuesByCurrency.USDDebit = entry.Debit;
        this.ValuesByCurrency.ValuedUSDDebit = Math.Round((entry.Debit * entry.ExchangeRate),2);

        this.ValuesByCurrency.USDCredit = entry.Credit;
        this.ValuesByCurrency.ValuedUSDCredit = Math.Round((entry.Credit * entry.ExchangeRate),2);

        if (isPreviousMonth) {
          this.ValuesByCurrency.PreviousValuedUSDDebit = Math.Round((entry.Debit * entry.SecondExchangeRate), 2);
          this.ValuesByCurrency.PreviousValuedUSDCredit = Math.Round((entry.Credit * entry.SecondExchangeRate), 2);
        }

      }
      if (entry.Currency.Equals(Currency.YEN)) {

        this.ValuesByCurrency.YEN = balance;
        this.ValuesByCurrency.ExchangeRateYEN = entry.ExchangeRate;
        this.ValuesByCurrency.LastExchangeRateYEN = entry.SecondExchangeRate;
        this.ValuesByCurrency.LastYEN = Math.Round((balance * entry.SecondExchangeRate),2);
        this.ValuesByCurrency.ValuedYEN = Math.Round((balance * entry.ExchangeRate),2);
        this.ValuesByCurrency.ValuedEffectYEN = this.ValuesByCurrency.LastYEN -
                                                this.ValuesByCurrency.ValuedYEN;

        this.ValuesByCurrency.YENDebit = entry.Debit;
        this.ValuesByCurrency.ValuedYENDebit = Math.Round((entry.Debit * entry.ExchangeRate),2);

        this.ValuesByCurrency.YENCredit = entry.Credit;
        this.ValuesByCurrency.ValuedYENCredit = Math.Round((entry.Credit * entry.ExchangeRate),2);

        if (isPreviousMonth) {
          this.ValuesByCurrency.PreviousValuedYENDebit = Math.Round((entry.Debit * entry.SecondExchangeRate),2);
          this.ValuesByCurrency.PreviousValuedYENCredit = Math.Round((entry.Credit * entry.SecondExchangeRate),2);
        }

      }
      if (entry.Currency.Equals(Currency.EUR)) {

        this.ValuesByCurrency.EUR = balance;
        this.ValuesByCurrency.ExchangeRateEUR = entry.ExchangeRate;
        this.ValuesByCurrency.LastExchangeRateEUR = entry.SecondExchangeRate;
        this.ValuesByCurrency.LastEUR = Math.Round((balance * entry.SecondExchangeRate),2);
        this.ValuesByCurrency.ValuedEUR = Math.Round((balance * entry.ExchangeRate),2);
        this.ValuesByCurrency.ValuedEffectEUR = this.ValuesByCurrency.LastEUR -
                                                this.ValuesByCurrency.ValuedEUR;

        this.ValuesByCurrency.EURDebit = entry.Debit;
        this.ValuesByCurrency.ValuedEURDebit = Math.Round((entry.Debit * entry.ExchangeRate),2);

        this.ValuesByCurrency.EURCredit = entry.Credit;
        this.ValuesByCurrency.ValuedEURCredit = Math.Round((entry.Credit * entry.ExchangeRate),2);

        if (isPreviousMonth) {
          this.ValuesByCurrency.PreviousValuedEURDebit = Math.Round((entry.Debit * entry.SecondExchangeRate),2);
          this.ValuesByCurrency.PreviousValuedEURCredit = Math.Round((entry.Credit * entry.SecondExchangeRate),2);
        }

      }
      if (entry.Currency.Equals(Currency.UDI)) {

        this.ValuesByCurrency.UDI = balance;
        this.ValuesByCurrency.ExchangeRateUDI = entry.ExchangeRate;
        this.ValuesByCurrency.LastExchangeRateUDI = entry.SecondExchangeRate;
        this.ValuesByCurrency.LastUDI = Math.Round((balance * entry.SecondExchangeRate),2);
        this.ValuesByCurrency.ValuedUDI = Math.Round((balance * entry.ExchangeRate),2);
        this.ValuesByCurrency.ValuedEffectUDI = this.ValuesByCurrency.LastUDI -
                                                this.ValuesByCurrency.ValuedUDI;

        this.ValuesByCurrency.UDIDebit = entry.Debit;
        this.ValuesByCurrency.ValuedUDIDebit = Math.Round((entry.Debit * entry.ExchangeRate),2);

        this.ValuesByCurrency.UDICredit = entry.Credit;
        this.ValuesByCurrency.ValuedUDICredit = Math.Round((entry.Credit * entry.ExchangeRate),2);

        if (isPreviousMonth) {
          this.ValuesByCurrency.PreviousValuedUDIDebit = Math.Round((entry.Debit * entry.SecondExchangeRate),2);
          this.ValuesByCurrency.PreviousValuedUDICredit = Math.Round((entry.Credit * entry.SecondExchangeRate),2);
        }

      }
    }

  } // class ValorizacionEstimacionPreventivaEntry


} // namespace Empiria.FinancialAccounting.BalanceEngine
