/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reclassification Services                  Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Reclassification.dll   Pattern   : Output DTO                              *
*  Type     : BalanzaAnaliticaOperacionesDto             License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO used to return an analytical operations trial balance.                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.FinancialAccounting.Reclassification.Adapters {

  /// <summary>Output DTO used to return an analytical operations trial balance.</summary>
  public class BalanzaAnaliticaOperacionesDto {

    public string OperationType {
      get; internal set;
    }

    public string AccountNo {
      get; internal set;
    }

    public string AccountName {
      get; internal set;
    }

    public string CurrencyCode {
      get; internal set;
    }

    public decimal Credits {
      get; internal set;
    }

    public decimal Debits {
      get; internal set;
    }

  }  // class BalanzaAnaliticaOperacionesDto

} // namespace Empiria.FinancialAccounting.Reclassification.Adapters
