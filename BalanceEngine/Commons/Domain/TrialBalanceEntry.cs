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


    internal void MultiplyBy(decimal value) {
      this.InitialBalance *= value;
      this.Debit *= value;
      this.Credit *= value;
      this.CurrentBalance *= value;
      this.ExchangeRate = value;
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

    internal void Sum(TrialBalanceEntry entry) {
      this.InitialBalance += entry.InitialBalance;
      this.Credit += entry.Credit;
      this.Debit += entry.Debit;
      this.CurrentBalance += entry.CurrentBalance;
      this.ExchangeRate = entry.ExchangeRate;
      this.SecondExchangeRate = entry.SecondExchangeRate;
      this.AverageBalance += entry.AverageBalance;
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


    internal BalanzaColumnasMonedaEntry MapToBalanceByCurrencyEntry() {
      BalanzaColumnasMonedaEntry trialBalanceByCurrencyEntry = new BalanzaColumnasMonedaEntry();

      trialBalanceByCurrencyEntry.ItemType = this.ItemType;
      trialBalanceByCurrencyEntry.Currency = this.Currency;
      trialBalanceByCurrencyEntry.Account = this.Account;
      trialBalanceByCurrencyEntry.Sector = this.Sector;

      if (Currency.Equals(Currency.MXN)) {
        trialBalanceByCurrencyEntry.DomesticBalance = this.CurrentBalance;
      }
      if (Currency.Equals(Currency.USD)) {
        trialBalanceByCurrencyEntry.DollarBalance = this.CurrentBalance;
      }
      if (Currency.Equals(Currency.YEN)) {
        trialBalanceByCurrencyEntry.YenBalance = this.CurrentBalance;
      }
      if (Currency.Equals(Currency.EUR)) {
        trialBalanceByCurrencyEntry.EuroBalance = this.CurrentBalance;
      }
      if (Currency.Equals(Currency.UDI)) {
        trialBalanceByCurrencyEntry.UdisBalance = this.CurrentBalance;
      }

      return trialBalanceByCurrencyEntry;
    }


  } // class TrialBalance

} // namespace Empiria.FinancialAccounting.BalanceEngine
