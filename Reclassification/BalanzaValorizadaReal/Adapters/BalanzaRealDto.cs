/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reclassification Services                  Component : Adapters Layer                          *
*  Assembly : FinancialAccounting.Reclassification.dll   Pattern   : Output DTO                              *
*  Type     : BalanzaEnColumnasRealDto                   License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO used to return a BalanzaEnColumnasRealDto.                                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {

  /// <summary>Output DTO used to return a BalanzaEnColumnasRealDto.</summary>
  public class BalanzaEnColumnasRealDto {

    public string AccountNo {
      get; internal set;
    }

    public string AccountName {
      get; internal set;
    }

    public decimal DomesticFinalBalance {
      get; internal set;
    }


    public decimal DollarFinalBalance {
      get; internal set;
    }


    public decimal YenFinalBalance {
      get; internal set;
    }


    public decimal EuroFinalBalance {
      get; internal set;
    }


    public decimal UdisFinalBalance {
      get; internal set;
    }

    public decimal DomesticRealFinalBalance {
      get; internal set;
    }


    public decimal DollarRealFinalBalance {
      get; internal set;
    }


    public decimal YenRealFinalBalance {
      get; internal set;
    }


    public decimal EuroRealFinalBalance {
      get; internal set;
    }


    public decimal UdisRealFinalBalance {
      get; internal set;
    }

  }  // class BalanzaEnColumnasRealDto

} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
