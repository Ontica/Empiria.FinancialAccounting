﻿/* Empiria Financial *****************************************************************************************
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
  public class AnaliticoDeCuentasEntry : ITrialBalanceEntry {

    public TrialBalanceItemType ItemType {
      get; internal set;
    }


    public Ledger Ledger {
      get; internal set;
    }


    public Currency Currency {
      get; internal set;
    }


    public StandardAccount Account {
      get; internal set;
    }


    public string AccountMark {
      get {
        if (this.ItemType != TrialBalanceItemType.Entry) {
          return string.Empty;
        }
        return IsParentPostingEntry ? "**" : "*";
      }
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
      get {
        return DomesticBalance + ForeignBalance;
      }
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


    public int Level {
      get {
        return this.Account.Level;
      }
    }


    public bool IsParentPostingEntry {
      get; internal set;
    }

    internal void Sum(AnaliticoDeCuentasEntry entry) {
      this.DomesticBalance += entry.DomesticBalance;
      this.ForeignBalance += entry.ForeignBalance;
      this.ExchangeRate = entry.ExchangeRate;
    }


    internal void ResetBalances() {
      this.DomesticBalance = 0;
      this.InitialBalance = 0;
      this.Debit = 0;
      this.Credit = 0;
      this.AverageBalance = 0;
    }
  } // class AnalyticBalanceEntry

} // namespace Empiria.FinancialAccounting.BalanceEngine
