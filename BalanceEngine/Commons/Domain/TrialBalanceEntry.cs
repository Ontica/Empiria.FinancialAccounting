/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Empiria Plain Object                    *
*  Type     : TrialBalanceEntry                          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Represents an entry for a trial balance.                                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.FinancialAccounting.Adapters;
using Microsoft.Extensions.Logging;

namespace Empiria.FinancialAccounting.BalanceEngine {

  public interface ITrialBalanceEntry {

  }

  /// <summary>Represents an entry for a trial balance.</summary>
  public class TrialBalanceEntry : ITrialBalanceEntry {

    #region Constructors and parsers

    internal TrialBalanceEntry() {
      // Required by Empiria Framework.
    }

    #endregion Constructors and parsers

    public TrialBalanceItemType ItemType {
      get;
      internal set;
    } = TrialBalanceItemType.Entry;


    [DataField("ID_MAYOR", ConvertFrom = typeof(decimal))]
    public Ledger Ledger {
      get;
      internal set;
    }

    [DataField("ID_MONEDA", ConvertFrom = typeof(decimal))]
    public Currency Currency {
      get;
      internal set;
    }


    [DataField("ID_CUENTA_ESTANDAR", ConvertFrom = typeof(long))]
    public StandardAccount Account {
      get;
      internal set;
    }


    [DataField("ID_SECTOR", ConvertFrom = typeof(long))]
    public Sector Sector {
      get;
      internal set;
    }


    [DataField("ID_CUENTA_AUXILIAR", ConvertFrom = typeof(decimal))]
    public int SubledgerAccountId {
      get;
      internal set;
    }


    [DataField("SALDO_ANTERIOR")]
    public decimal InitialBalance {
      get;
      internal set;
    }


    [DataField("DEBE")]
    public decimal Debit {
      get;
      internal set;
    }


    [DataField("HABER")]
    public decimal Credit {
      get;
      internal set;
    }


    [DataField("SALDO_ACTUAL")]
    public decimal CurrentBalance {
      get;
      internal set;
    }


    [DataField("SALDO_PROMEDIO")]
    public decimal AverageBalance {
      get;
      internal set;
    }


    [DataField("FECHA_ULTIMO_MOVIMIENTO")]
    public DateTime LastChangeDate {
      get; internal set;
    }


    public decimal ExchangeRate {
      get;
      internal set;
    } = 1;


    public decimal USDExchangeRate {
      get; internal set;
    }


    public decimal YENExchangeRate {
      get; internal set;
    }


    public decimal EURExchangeRate {
      get; internal set;
    }


    public decimal UDIExchangeRate {
      get; internal set;
    }


    public decimal SecondExchangeRate {
      get; internal set;
    } = 1;


    public decimal USDSecondExchangeRate {
      get; internal set;
    }


    public decimal YENSecondExchangeRate {
      get; internal set;
    }


    public decimal EURSecondExchangeRate {
      get; internal set;
    }


    public decimal UDISecondExchangeRate {
      get; internal set;
    }


    public decimal ValorizedCurrentBalance {
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


    public DebtorCreditorType DebtorCreditor {
      get; internal set;
    } = DebtorCreditorType.Deudora;
    

    public int SubledgerAccountIdParent {
      get; internal set;
    } = -1;


    public string SubledgerAccountNumber {
      get; internal set;
    } = string.Empty;


    public int SubledgerNumberOfDigits {
      get; internal set;
    } = 0;


    public bool HasSector {
      get {
        return this.Sector.Code != "00";
      }
    }


    public int Level {
      get {
        return this.Account.Level;
      }
    }


    public bool IsSummaryForAnalytics {
      get; set;
    } = false;


    public bool HasParentPostingEntry {
      get; set;
    } = false;


    public bool IsParentPostingEntry {
      get; set;
    } = false;


    public bool IsFlattenedAccount {
      get; internal set;
    }


    internal TrialBalanceEntry CreatePartialCopy() {
      return new TrialBalanceEntry {
        Account = this.Account,
        Ledger = this.Ledger,
        Currency = this.Currency,
        Sector = this.Sector,
        SubledgerAccountId = this.SubledgerAccountId,
        InitialBalance = this.InitialBalance,
        Debit = this.Debit,
        Credit = this.Credit,
        CurrentBalance = this.CurrentBalance,
        GroupNumber = this.GroupNumber,
        GroupName = this.GroupName,
        ItemType = this.ItemType,
        ExchangeRate = this.ExchangeRate,
        HasParentPostingEntry = this.HasParentPostingEntry,
        IsParentPostingEntry = this.IsParentPostingEntry
      };
    }


    internal void AssignClosingExchangeRateValueByCurrency(FixedList<ExchangeRate> exchangeRates) {

      this.USDSecondExchangeRate = exchangeRates.Find(x => x.ToCurrency.Equals(Currency.USD) &&
                                                      x.FromCurrency.Code.Equals(Currency.MXN.Code)).Value;
      this.YENSecondExchangeRate = exchangeRates.Find(x => x.ToCurrency.Equals(Currency.YEN) &&
                                                      x.FromCurrency.Code.Equals(Currency.MXN.Code)).Value;
      this.EURSecondExchangeRate = exchangeRates.Find(x => x.ToCurrency.Equals(Currency.EUR) &&
                                                      x.FromCurrency.Code.Equals(Currency.MXN.Code)).Value;
      this.UDISecondExchangeRate = exchangeRates.Find(x => x.ToCurrency.Equals(Currency.UDI) &&
                                                      x.FromCurrency.Code.Equals(Currency.MXN.Code)).Value;
    }


    internal void AssignExchangeRateValueByCurrency(FixedList<ExchangeRate> exchangeRates) {

      this.USDExchangeRate = exchangeRates.Find(x => x.ToCurrency.Equals(Currency.USD) &&
                                                x.FromCurrency.Code.Equals(Currency.MXN.Code)).Value;
      this.YENExchangeRate = exchangeRates.Find(x => x.ToCurrency.Equals(Currency.YEN) &&
                                                x.FromCurrency.Code.Equals(Currency.MXN.Code)).Value;
      this.EURExchangeRate = exchangeRates.Find(x => x.ToCurrency.Equals(Currency.EUR) &&
                                                x.FromCurrency.Code.Equals(Currency.MXN.Code)).Value;
      this.UDIExchangeRate = exchangeRates.Find(x => x.ToCurrency.Equals(Currency.UDI) &&
                                                x.FromCurrency.Code.Equals(Currency.MXN.Code)).Value;
    }


    internal TrialBalanceEntry MapFromFlatAccountToTrialBalanceEntry(FlatAccountDto flatAccount,
                                                                     Ledger ledger) {
      var entry = new TrialBalanceEntry();
      entry.Account = StandardAccount.Parse(flatAccount.StandardAccountId);
      entry.Ledger = ledger;
      entry.Currency = flatAccount.Currency;
      entry.Sector = flatAccount.Sector;
      entry.SubledgerAccountId = -1;
      entry.DebtorCreditor = flatAccount.DebtorCreditor;
      entry.IsFlattenedAccount = true;

      return entry;
    }


    internal AnaliticoDeCuentasEntry MapToAnalyticBalanceEntry() {
      return new AnaliticoDeCuentasEntry {
        Account = this.Account,
        SubledgerAccountId = this.SubledgerAccountId,
        SubledgerAccountNumber = this.SubledgerAccountNumber,
        Ledger = this.Ledger,
        Currency = this.Currency,
        ItemType = this.ItemType,
        Sector = this.Sector,
        DebtorCreditor = this.DebtorCreditor,
        IsParentPostingEntry = this.IsParentPostingEntry,
        LastChangeDate = this.LastChangeDate
      };
    }


    internal BalanzaColumnasMonedaEntry MapToBalanceByCurrencyEntry(DateTime fromDate, DateTime toDate) {
      BalanzaColumnasMonedaEntry balanceByCurrencyEntry = new BalanzaColumnasMonedaEntry();

      balanceByCurrencyEntry.ItemType = this.ItemType;
      balanceByCurrencyEntry.Currency = this.Currency;
      balanceByCurrencyEntry.Account = this.Account;
      balanceByCurrencyEntry.Sector = this.Sector;
      balanceByCurrencyEntry.FromDate = fromDate;
      balanceByCurrencyEntry.ToDate = toDate;

      if (Currency.Equals(Currency.MXN)) {
        balanceByCurrencyEntry.DomesticBalance = this.CurrentBalance;
      }
      if (Currency.Equals(Currency.USD)) {
        balanceByCurrencyEntry.DollarBalance = this.CurrentBalance;
        balanceByCurrencyEntry.ValorizedDollarBalance = this.ValorizedCurrentBalance;
      }
      if (Currency.Equals(Currency.YEN)) {
        balanceByCurrencyEntry.YenBalance = this.CurrentBalance;
        balanceByCurrencyEntry.ValorizedYenBalance = this.ValorizedCurrentBalance;
      }
      if (Currency.Equals(Currency.EUR)) {
        balanceByCurrencyEntry.EuroBalance = this.CurrentBalance;
        balanceByCurrencyEntry.ValorizedEuroBalance = this.ValorizedCurrentBalance;
      }
      if (Currency.Equals(Currency.UDI)) {
        balanceByCurrencyEntry.UdisBalance = this.CurrentBalance;
        balanceByCurrencyEntry.ValorizedUdisBalance = this.ValorizedCurrentBalance;
      }

      balanceByCurrencyEntry.ExchangeRateForDollar = this.USDExchangeRate;
      balanceByCurrencyEntry.ExchangeRateForYen = this.YENExchangeRate;
      balanceByCurrencyEntry.ExchangeRateForEuro = this.EURExchangeRate;
      balanceByCurrencyEntry.ExchangeRateForUdi = this.UDIExchangeRate;

      balanceByCurrencyEntry.ClosingExchangeRateForDollar = this.USDSecondExchangeRate;
      balanceByCurrencyEntry.ClosingExchangeRateForYen = this.YENSecondExchangeRate;
      balanceByCurrencyEntry.ClosingExchangeRateForEuro = this.EURSecondExchangeRate;
      balanceByCurrencyEntry.ClosingExchangeRateForUdi = this.UDISecondExchangeRate;

      return balanceByCurrencyEntry;
    }


    internal BalanzaComparativaEntry MapToComparativeBalanceEntry() {
      return new BalanzaComparativaEntry {
        Ledger = this.Ledger,
        Currency = this.Currency,
        Sector = this.Sector,
        Account = this.Account,
        SubledgerAccountId = this.SubledgerAccountId,
        DebtorCreditor = this.DebtorCreditor,
        Debit = this.Debit,
        Credit = this.Credit,
        FirstTotalBalance = this.InitialBalance,
        FirstExchangeRate = Math.Round(this.ExchangeRate, 6),
        FirstValorization = InitialBalance * this.ExchangeRate,
        SecondTotalBalance = this.CurrentBalance,
        SecondExchangeRate = Math.Round(this.SecondExchangeRate, 6),
        SecondValorization = this.CurrentBalance * this.SecondExchangeRate,
        AverageBalance = this.AverageBalance,
        LastChangeDate = this.LastChangeDate
      };
    }


    internal BalanzaDolarizadaEntry MapToValuedBalanceEntry() {
      return new BalanzaDolarizadaEntry {
        Currency = this.Currency,
        Account = this.Account,
        Sector = this.Sector,
        TotalBalance = this.CurrentBalance,
        ExchangeRate = this.ExchangeRate,
        TotalEquivalence = this.CurrentBalance,
        ItemType = this.ItemType,
        HasParentPostingEntry = this.HasParentPostingEntry,
        IsParentPostingEntry = this.IsParentPostingEntry
      };
    }


    internal void MultiplyBy(decimal value) {
      this.InitialBalance *= value;
      this.Debit *= value;
      this.Credit *= value;
      this.CurrentBalance *= value;
      this.ExchangeRate = value;
    }


    internal void MultiplyByValorizedValue(decimal value) {
      this.ValorizedCurrentBalance = this.CurrentBalance * value;
      this.ExchangeRate = value;
    }


    internal void Sum(TrialBalanceEntry entry) {
      this.InitialBalance += entry.InitialBalance;
      this.Credit += entry.Credit;
      this.Debit += entry.Debit;
      this.CurrentBalance += entry.CurrentBalance;
      this.ExchangeRate = entry.ExchangeRate;
      this.SecondExchangeRate = entry.SecondExchangeRate;
      this.AverageBalance += entry.AverageBalance;
      this.ValorizedCurrentBalance += entry.ValorizedCurrentBalance;
    }


  } // class TrialBalance

} // namespace Empiria.FinancialAccounting.BalanceEngine
