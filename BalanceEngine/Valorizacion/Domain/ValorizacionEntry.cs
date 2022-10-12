/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Empiria Plain Object                    *
*  Type     : ValorizacionEntry                          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Represents an entry for a valorized report entry.                                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Represents an entry for a valorized report entry.</summary>
  internal class ValorizacionEntry {

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

    public decimal InitialBalance {
      get;
      internal set;
    }

    public decimal CurrentBalance {
      get;
      internal set;
    }

    public decimal ValuedEffects {
      get;
      internal set;
    }

    public decimal TotalValued {
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

    public decimal ValuedExchangeRate {
      get;
      internal set;
    } = 1;

    public TrialBalanceItemType ItemType {
      get; internal set;
    }

    public bool HasParentPostingEntry {
      get; internal set;
    }

    public bool IsParentPostingEntry {
      get; internal set;
    }

    internal void Sum(ValorizacionEntry entry) {
      
    }

  } // class ValorizacionEntry

} // namespace Empiria.FinancialAccounting.BalanceEngine
