/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Exchange Rates                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Data Transfer Object                    *
*  Type     : ExchangeRateValuesDto                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Input/Output DTO that holds all exchange rates on a date for a given ExchangeRateType.         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Adapters {

  /// <summary>Input/Output DTO that holds all exchange rates on a date for a given ExchangeRateType.</summary>
  public class ExchangeRateValuesDto {

    public string ExchangeRateTypeUID {
      get; set;
    }

    public DateTime Date {
      get; set;
    } = ExecutionServer.DateMinValue;


    public ExchangeRateValue[] Values {
      get; set;
    } = new ExchangeRateValue[0];


    internal void EnsureValid() {
      Assertion.AssertObject(ExchangeRateTypeUID, "ExchangeRateTypeUID");

      Assertion.Assert(Date != ExecutionServer.DateMinValue,
          "Exchange rate date must be provided.");
      Assertion.AssertObject(Values, "Values array can not be null.");
      Assertion.Assert(Values.Length != 0, "Values array must have one or more values.");

      for (int i = 0; i < Values.Length; i++) {
        Assertion.AssertObject(Values[i].ToCurrencyUID,
                               $"Exchange rate currency is missed for values element {i}.");
        Assertion.Assert(Values[i].Value > 0,
                         $"Exchange rate value must be a positive decimal for values element {i}.");
      }
    }

  }  // public class ExchangeRateValuesDto



  public class ExchangeRateValue {

    public string ToCurrencyUID {
      get; set;
    }


    public string ToCurrency {
      get; set;
    }


    public decimal Value {
      get; set;
    }


    public bool HasValue {
      get {
        return Value != decimal.Zero;
      }
    }

  }  // class ExchangeRateFieldValue

}  // namespace Empiria.FinancialAccounting.Adapters
