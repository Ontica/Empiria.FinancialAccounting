/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Empiria Plain Object                    *
*  Type     : BalanzaColumnasMonedaEntry                 License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Represents an entry for a Balanza en columnas por moneda entry.                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.BalanceEngine {


  /// <summary>Represents an entry for a Balanza en columnas por moneda entry.</summary>
  public class BalanzaColumnasMonedaEntry : ITrialBalanceEntry {

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

    public decimal DomesticBalance {
      get;
      internal set;
    }

    public decimal DollarBalance {
      get;
      internal set;
    }

    public decimal YenBalance {
      get;
      internal set;
    }

    public decimal EuroBalance {
      get;
      internal set;
    }

    public decimal UdisBalance {
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

    internal void Sum(BalanzaDolarizadaEntry entry) {

    }

  } // BalanzaColumnasMonedaEntry

} // namespace Empiria.FinancialAccounting.BalanceEngine
