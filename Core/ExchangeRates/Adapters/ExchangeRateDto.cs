/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Exchange Rates                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Data Transfer Object                    *
*  Type     : ExchangeRateDto                            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO for an exchange rate.                                                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Adapters {

  /// <summary>Output DTO for an exchange rate.</summary>
  public class ExchangeRateDto {

    internal ExchangeRateDto() {
      // no-op
    }

    public int Id {
      get; internal set;
    }

    public NamedEntityDto ExchangeRateType {
      get; internal set;
    }


    public DateTime Date {
      get; internal set;
    }


    public NamedEntityDto FromCurrency {
      get; internal set;
    }


    public NamedEntityDto ToCurrency {
      get; internal set;
    }


    public decimal Value {
      get; internal set;
    }

  }  // public class ExchangeRateDto


  /// <summary>Minimal output DTO for exchange rates, typically used in lists.</summary>
  public class ExchangeRateDescriptorDto {

    internal ExchangeRateDescriptorDto() {
      // no-op
    }

    public int Id {
      get; internal set;
    }

    public string ExchangeRateType {
      get; internal set;
    }

    public DateTime Date {
      get; internal set;
    }


    public string Currency {
      get; internal set;
    }


    public decimal Value {
      get; internal set;
    }

  }  // class ExchangeRateDescriptorDto


}  // namespace Empiria.FinancialAccounting.Adapters
