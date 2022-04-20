/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Exchange Rates                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Data Transfer Object                    *
*  Type     : ExchangeRateFields                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Input DTO used to store an exchange rate.                                                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Adapters {

  /// <summary>Input DTO used to store an exchange rate.</summary>
  public class ExchangeRateFields {


    public string ExchangeRateTypeUID {
      get; set;
    }


    public DateTime Date {
      get; set;
    } = ExecutionServer.DateMinValue;


    public string FromCurrencyUID {
      get; set;
    }


    public string ToCurrencyUID {
      get; set;
    }


    public decimal Value {
      get; set;
    }


    internal void EnsureValid() {
      Assertion.AssertObject(ExchangeRateTypeUID, "ExchangeRateTypeUID");
      Assertion.Assert(Date != ExecutionServer.DateMinValue, "Exchange rate date must be provided.");
      Assertion.AssertObject(FromCurrencyUID, "FromCurrencyUID");
      Assertion.AssertObject(ToCurrencyUID, "ToCurrencyUID");
      Assertion.Assert(Value > 0, "Exchange rate value must be a positive decimal.");
    }


  }  // public class ExchangeRateFields

}  // namespace Empiria.FinancialAccounting.Adapters
