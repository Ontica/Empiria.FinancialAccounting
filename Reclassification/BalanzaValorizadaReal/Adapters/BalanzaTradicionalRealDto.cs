/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reclassification Services                  Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Reclassification.dll   Pattern   : Output DTO                              *
*  Type     : BalanzaTradicionalRealDto                  License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO used to return a traditional trial balance with balances and real balances.         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.FinancialAccounting.Reclassification.Adapters {

  /// <summary>Output DTO used to return a traditional trial balance with balances and real balances.</summary>
  public class BalanzaTradicionalRealDto {

    public string AccountNo {
      get; internal set;
    }

    public string AccountName {
      get; internal set;
    }

    public string CurrencyCode {
      get; internal set;
    }

    public decimal InitialBalance {
      get; internal set;
    }

    public decimal Credits {
      get; internal set;
    }

    public decimal Debits {
      get; internal set;
    }

    public decimal FinalBalance {
      get; internal set;
    }

    public string RealCurrencyCode {
      get; internal set;
    }

    public decimal RealInitialBalance {
      get; internal set;
    }

    public decimal RealCredits {
      get; internal set;
    }

    public decimal RealDebits {
      get; internal set;
    }

    public decimal RealFinalBalance {
      get; internal set;
    }

  }  // class BalanzaValorizadaRealDto

} // namespace Empiria.FinancialAccounting.Reclassification.Adapters
