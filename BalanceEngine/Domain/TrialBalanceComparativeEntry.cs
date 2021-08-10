﻿/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Empiria Plain Object                    *
*  Type     : TrialBalanceComparativeEntry               License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Represents an entry for a comparative balance between periods.                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;


namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Represents an entry for a comparative balance between periods.</summary>
  public class TrialBalanceComparativeEntry : ITrialBalanceEntry {

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

    public decimal FirstTotalBalance {
      get;
      internal set;
    }

    public decimal FirstExchangeRate {
      get;
      internal set;
    } = 1;

    public decimal FirstValorization {
      get;
      internal set;
    }

    public decimal Debit {
      get;
      internal set;
    } = 0;

    public decimal Credit {
      get;
      internal set;
    } = 0;

    public decimal SecondTotalBalance {
      get;
      internal set;
    }

    public decimal SecondExchangeRate {
      get;
      internal set;
    } = 1;

    public decimal SecondValorization {
      get;
      internal set;
    }

    public decimal Variation {
      get;
      internal set;
    }

    public decimal VariationByER {
      get;
      internal set;
    } = 1;

    public decimal RealVariation {
      get;
      internal set;
    }

    public decimal AverageBalance {
      get;
      internal set;
    }

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

  }
}
