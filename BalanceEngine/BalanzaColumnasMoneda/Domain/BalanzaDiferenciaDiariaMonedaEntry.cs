/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Empiria Plain Object                    *
*  Type     : BalanzaDiferenciaDiariaMonedaEntry         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Represents an entry for a Balanza diferencia diaria por moneda.                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.BalanceEngine {


  /// <summary>Represents an entry for a Balanza diferencia diaria por moneda.</summary>
  internal class BalanzaDiferenciaDiariaMonedaEntry : BalanzaColumnasMonedaCommons, ITrialBalanceEntry {

    public decimal DomesticDailyBalance {
      get; internal set;
    }


    public decimal DollarDailyBalance {
      get; internal set;
    }


    public decimal YenDailyBalance {
      get; internal set;
    }


    public decimal EuroDailyBalance {
      get; internal set;
    }


    public decimal UdisDailyBalance {
      get; internal set;
    }

  } // class BalanzaDiferenciaDiariaMonedaEntry

} // namespace Empiria.FinancialAccounting.BalanceEngine
