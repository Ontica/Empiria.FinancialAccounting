/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Exchange Rates                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Data Transfer Object                    *
*  Type     : ExchangeRateTypeDto                        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO for exchange rate types.                                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Adapters {

  /// <summary>Output DTO for an exchange rate types.</summary>
  public class ExchangeRateTypeDto {

    internal ExchangeRateTypeDto() {
      // no-op
    }

    public string UID {
      get; internal set;
    }


    public string Name {
      get; internal set;
    }


    public FixedList<NamedEntityDto> Currencies {
      get; internal set;
    }

  }  // public class ExchangeRateTypeDto

}  // namespace Empiria.FinancialAccounting.Adapters
