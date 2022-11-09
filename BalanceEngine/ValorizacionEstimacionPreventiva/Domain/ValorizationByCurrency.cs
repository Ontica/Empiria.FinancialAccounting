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


    public decimal LastUSD {
      get;
      internal set;
    }


    public decimal LastYEN {
      get;
      internal set;
    }


    public decimal LastEUR {
      get;
      internal set;
    }


    public decimal LastUDI {
      get;
      internal set;
    }


    public decimal CurrentUSD {
      get;
      internal set;
    }


    public decimal CurrentYEN {
      get;
      internal set;
    }


    public decimal CurrentEUR {
      get;
      internal set;
    }


    public decimal CurrentUDI {
      get;
      internal set;
    }


    public decimal ValuedEffectUSD {
      get;
      internal set;
    }


    public decimal ValuedEffectYEN {
      get;
      internal set;
    }


    public decimal ValuedEffectEUR {
      get;
      internal set;
    }


    public decimal ValuedEffectUDI {
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


  internal class ColumnsByCurrency {

    public bool USDColumn {
      get; internal set;
    }

    public bool YENColumn {
      get; internal set;
    }

    public bool EURColumn {
      get; internal set;
    }

    public bool UDIColumn {
      get; internal set;
    }

  } // class ColumnsByCurrency


} // namespace Empiria.FinancialAccounting.BalanceEngine
