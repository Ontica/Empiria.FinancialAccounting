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

  public class TwoCurrenciesBalanceEntry : ITrialBalanceEntry {

    public Ledger Ledger {
      get;
      internal set;
    }

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


    public int AccountId {
      get;
      internal set;
    }


    public int SubledgerAccountId {
      get;
      internal set;
    }

    public decimal DomesticBalance {
      get;
      internal set;
    }

    public decimal ForeignBalance {
      get;
      internal set;
    }

    public decimal TotalBalance {
      get;
      internal set;
    }

    public decimal ExchangeRate {
      get;
      internal set;
    } = 1;

    public string GroupName {
      get; internal set;
    } = string.Empty;

    public string GroupNumber {
      get; internal set;
    } = string.Empty;

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


    public string LastChangeDate {
      get; internal set;
    } = string.Empty;


    public DebtorCreditorType DebtorCreditor {
      get; internal set;
    } = DebtorCreditorType.Deudora;


    public TrialBalanceItemType ItemType {
      get;
      internal set;
    }


    public int Level {
      get {
        return EmpiriaString.CountOccurences(Account.Number, '-') + 1;
      }
    }


    internal void Sum(TwoCurrenciesBalanceEntry entry) {
      this.DomesticBalance += entry.DomesticBalance;
      this.ForeignBalance += entry.ForeignBalance;
      this.TotalBalance += entry.TotalBalance;
      this.ExchangeRate = entry.ExchangeRate;
    }
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


    [DataField("FECHA_ULTIMO_MOVIMIENTO")]
    public DateTime LastChangeDate {
      get; internal set;
    }


    public decimal ExchangeRate {
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
    } = TrialBalanceItemType.BalanceEntry;


    public DebtorCreditorType DebtorCreditor {
      get; internal set;
    } = DebtorCreditorType.Deudora;


    public int SubledgerAccountIdParent {
      get; internal set;
    } = -1;


    public string SubledgerAccountNumber {
      get; internal set;
    } = string.Empty;


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
        return EmpiriaString.CountOccurences(Account.Number, '-') + 1;
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
    }


    internal TwoCurrenciesBalanceEntry MapToTwoColumnsBalanceEntry() {
      return new TwoCurrenciesBalanceEntry {
        Account = this.Account,
        SubledgerAccountId = this.SubledgerAccountId,
        Ledger = this.Ledger,
        Currency = this.Currency,
        ItemType = this.ItemType,
        Sector = this.Sector,
        DebtorCreditor = this.Account.DebtorCreditor
        //LastChangeDate = this.LastChangeDate.ToString("dd-MM-yyyy")
      };
    }


  } // class TrialBalance



} // namespace Empiria.FinancialAccounting.BalanceEngine
