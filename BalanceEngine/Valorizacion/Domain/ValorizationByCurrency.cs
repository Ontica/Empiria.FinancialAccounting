/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Empiria Plain Object                    *
*  Type     : ValorizationByCurrency                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Represents an object for a valorization by currencies by entry.                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Represents an object for a valorization by currencies by entry.</summary>
  internal class ValorizationByCurrency {

    public decimal USD {
      get;
      internal set;
    }


    public decimal YEN {
      get;
      internal set;
    }


    public decimal EUR {
      get;
      internal set;
    }


    public decimal UDI {
      get;
      internal set;
    }


    public decimal LastExchangeRateUSD {
      get;
      internal set;
    }


    public decimal LastExchangeRateYEN {
      get;
      internal set;
    }


    public decimal LastExchangeRateEUR {
      get;
      internal set;
    }


    public decimal LastExchangeRateUDI {
      get;
      internal set;
    }


    public decimal ExchangeRateUSD {
      get;
      internal set;
    }


    public decimal ExchangeRateYEN {
      get;
      internal set;
    }


    public decimal ExchangeRateEUR {
      get;
      internal set;
    }


    public decimal ExchangeRateUDI {
      get;
      internal set;
    }

  } // class ValorizationByCurrency


} // namespace Empiria.FinancialAccounting.BalanceEngine
