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


    public decimal SecondExchangeRate {
      get;
      internal set;
    } = 1;


    public string GroupName {
      get; internal set;
    } = string.Empty;


    public string GroupNumber {
      get; internal set;
    } = string.Empty;


    public TrialBalanceItemType ItemType {
      get;
      internal set;
    } = TrialBalanceItemType.Entry;


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


    public bool NotHasSector {
      get {
        return !HasSector;
      }
    }


    public int Level {
      get {
        return this.Account.Level;
      }
    }


    internal void MultiplyBy(decimal value) {
      this.InitialBalance *= value;
      this.Debit *= value;
      this.Credit *= value;
      this.CurrentBalance *= value;
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
    }


    internal void SumOrSubstractIfDebtorOrCreditor(TrialBalanceEntry entry) {
      this.InitialBalance += entry.InitialBalance;
      this.CurrentBalance += entry.CurrentBalance;
      this.Credit += entry.Credit;
      this.Debit += entry.Debit;

      this.ExchangeRate = entry.ExchangeRate;
      this.SecondExchangeRate = entry.SecondExchangeRate;
      this.AverageBalance += entry.AverageBalance;
      //if (entry.DebtorCreditor == DebtorCreditorType.Deudora) {
      //  this.InitialBalance += entry.InitialBalance;
      //  this.CurrentBalance += entry.CurrentBalance;
      //} else {
      //  this.InitialBalance -= entry.InitialBalance;
      //  this.CurrentBalance -= entry.CurrentBalance;
      //}
    }


    internal TwoCurrenciesBalanceEntry MapToTwoColumnsBalanceEntry() {
      return new TwoCurrenciesBalanceEntry {
        Account = this.Account,
        SubledgerAccountId = this.SubledgerAccountId,
        SubledgerAccountNumber = this.SubledgerAccountNumber,
        SubledgerNumberOfDigits = this.SubledgerNumberOfDigits,
        Ledger = this.Ledger,
        Currency = this.Currency,
        ItemType = this.ItemType,
        Sector = this.Sector,
        DebtorCreditor = this.Account.DebtorCreditor,
        LastChangeDate = this.LastChangeDate
      };
    }


    internal TrialBalanceComparativeEntry MapToComparativeBalanceEntry() {
      return new TrialBalanceComparativeEntry {
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


    internal ValuedTrialBalanceEntry MapToValuedBalanceEntry() {
      return new ValuedTrialBalanceEntry {
        Currency = this.Currency,
        Account = this.Account,
        Sector = this.Sector,
        TotalBalance = this.CurrentBalance,
        ExchangeRate = this.ExchangeRate,
        TotalEquivalence = this.CurrentBalance,
        ItemType = this.ItemType
      };
    }


    internal TrialBalanceByCurrencyEntry MapToBalanceByCurrencyEntry() {
      TrialBalanceByCurrencyEntry trialBalanceByCurrencyEntry = new TrialBalanceByCurrencyEntry();
      trialBalanceByCurrencyEntry.Currency = this.Currency;
      trialBalanceByCurrencyEntry.Account = this.Account;
      trialBalanceByCurrencyEntry.Sector = this.Sector;
      if (Currency.Code == "01") {
        trialBalanceByCurrencyEntry.DomesticBalance = this.CurrentBalance;
      }
      if (Currency.Code == "02") {
        trialBalanceByCurrencyEntry.DollarBalance = this.CurrentBalance;
      }
      if (Currency.Code == "06") {
        trialBalanceByCurrencyEntry.YenBalance = this.CurrentBalance;
      }
      if (Currency.Code == "27") {
        trialBalanceByCurrencyEntry.EuroBalance = this.CurrentBalance;
      }
      if (Currency.Code == "44") {
        trialBalanceByCurrencyEntry.UdisBalance = this.CurrentBalance;
      }
      trialBalanceByCurrencyEntry.ItemType = this.ItemType;

      return trialBalanceByCurrencyEntry;
    }


  } // class TrialBalance

} // namespace Empiria.FinancialAccounting.BalanceEngine
