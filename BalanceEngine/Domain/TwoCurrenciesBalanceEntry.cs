/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Empiria Plain Object                    *
*  Type     : TwoCurrenciesBalanceEntry                  License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Represents an entry for a two columns balance entry.                                           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Represents an entry for a two columns balance entry.</summary>
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

    public decimal AverageBalance {
      get;
      internal set;
    } = 0;

    public string GroupName {
      get; internal set;
    } = string.Empty;

    public string GroupNumber {
      get; internal set;
    } = string.Empty;

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

  } // class TwoCurrenciesBalanceEntry

} // namespace Empiria.FinancialAccounting.BalanceEngine
