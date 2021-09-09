/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Empiria Plain Object                    *
*  Type     : ValuedTrialBalanceEntry                  License   : Please read LICENSE.txt file              *
*                                                                                                            *
*  Summary  : Represents an entry for a valued balance entry.                                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Represents an entry for a valued balance entry.</summary>
  public class ValuedTrialBalanceEntry : ITrialBalanceEntry {

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

    public decimal TotalBalance {
      get;
      internal set;
    }

    public decimal ExchangeRate {
      get;
      internal set;
    }

    public decimal TotalEquivalence {
      get;
      internal set;
    }

    public string GroupName {
      get; internal set;
    } = string.Empty;

    public string GroupNumber {
      get; internal set;
    } = string.Empty;

    public TrialBalanceItemType ItemType {
      get;
      internal set;
    }

    internal void Sum(ValuedTrialBalanceEntry entry) {
      this.TotalEquivalence += entry.TotalEquivalence;
      this.ExchangeRate = entry.ExchangeRate;
    }

  } // class ValuedTrialBalanceEntry

} // namespace Empiria.FinancialAccounting.BalanceEngine
