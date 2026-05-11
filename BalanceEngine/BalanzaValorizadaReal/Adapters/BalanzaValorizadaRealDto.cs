/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Data Transfer Object                    *
*  Type     : BalanzaValorizadaRealDto                   License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO used to return a Balanza en columnas por moneda.                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {

  public class BalanzaValorizadaRealDto {

    public string StandarAccount {
      get; internal set;
    }

    public string AccountName {
      get; internal set;
    }

    public string CurrencyCode {
      get; internal set;
    }

    public string CurrencyName {
      get; internal set;
    }

    public decimal InitialBalance {
      get; internal set;
    }

    public decimal Credit {
      get; internal set;
    }

    public decimal Debit {
      get; internal set;
    }

    public string RealCurrencyCode {
      get; internal set;
    }

    public string RealCurrencyName {
      get; internal set;
    }

    public decimal RealInitialBalance {
      get; internal set;
    }

    public decimal RealCredit {
      get; internal set;
    }

    public decimal RealDebit {
      get; internal set;
    }

  }  // class BalanzaValorizadaRealDto

} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
