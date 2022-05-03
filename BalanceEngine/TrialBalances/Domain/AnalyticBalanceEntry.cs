/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Information Holder                      *
*  Type     : AnalyticBalanceEntry                       License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Represents an entry for a two columns balance entry.                                           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Represents an entry for a two columns balance entry.</summary>
  public class AnalyticBalanceEntry : ITrialBalanceEntry {

    public Ledger Ledger {
      get; internal set;
    }


    public Currency Currency {
      get; internal set;
    }


    public StandardAccount Account {
      get; internal set;
    }


    public Sector Sector {
      get; internal set;
    }


    public int AccountId {
      get; internal set;
    }


    public int SubledgerAccountId {
      get; internal set;
    }


    public decimal InitialBalance {
      get; internal set;
    }


    public decimal Debit {
      get; internal set;
    }


    public decimal Credit {
      get; internal set;
    }


    public decimal DomesticBalance {
      get; internal set;
    }


    public decimal ForeignBalance {
      get; internal set;
    }


    public decimal TotalBalance {
      get; internal set;
    }


    public decimal ExchangeRate {
      get; internal set;
    } = 1;


    public decimal AverageBalance {
      get; internal set;
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


    public DateTime LastChangeDate {
      get; internal set;
    }


    public DebtorCreditorType DebtorCreditor {
      get; internal set;
    } = DebtorCreditorType.Deudora;


    public TrialBalanceItemType ItemType {
      get; internal set;
    }


    public int Level {
      get {
        return this.Account.Level;
      }
    }


    public bool IsParentPostingEntry {
      get; internal set;
    }

    internal void Sum(AnalyticBalanceEntry entry) {
      this.DomesticBalance += entry.DomesticBalance;
      this.ForeignBalance += entry.ForeignBalance;
      this.TotalBalance += entry.TotalBalance;
      this.ExchangeRate = entry.ExchangeRate;
    }


  } // class AnalyticBalanceEntry

} // namespace Empiria.FinancialAccounting.BalanceEngine
