/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Exchange Rates                             Component : Test cases                              *
*  Assembly : Empiria.FinancialAccounting.Tests.dll      Pattern   : Domain tests                            *
*  Type     : ExchangeRatesTests                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use case tests for accounting ledger book management.                                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Xunit;

namespace Empiria.FinancialAccounting.Tests {

  /// <summary>Use case tests for accounting ledger book management.</summary>
  public class ExchangeRatesTests {

    #region Facts

    [Fact]
    public void Should_Get_The_List_Of_Exchange_Rate_Types() {
      FixedList<ExchangeRateType> list = ExchangeRateType.GetList();

      Assert.NotEmpty(list);
    }


    [Fact]
    public void Should_Get_A_List_Of_Exchange_Rates_For_Type_In_A_Date() {
      FixedList<ExchangeRateType> exchangeRateTypes = ExchangeRateType.GetList();

      var EXCHANGE_RATE_TYPE = exchangeRateTypes[2];
      DateTime DATE = new DateTime(2021, 04, 30);

      FixedList<ExchangeRate> list = ExchangeRate.GetList(EXCHANGE_RATE_TYPE, DATE);

      Assert.NotEmpty(list);

      Assert.All(list, x => { Assert.Equal(DATE, x.Date); });
      Assert.All(list, x => { Assert.Equal(EXCHANGE_RATE_TYPE, x.ExchangeRateType); });
      Assert.All(list, x => { Assert.True(x.Value > 0); });
    }


    [Fact]
    public void Should_Get_A_List_Of_Exchange_Rates_In_A_Date() {
      DateTime DATE = new DateTime(2017, 08, 31);

      FixedList<ExchangeRate> list = ExchangeRate.GetList(DATE);

      Assert.NotEmpty(list);

      Assert.All(list, x => { Assert.Equal(DATE, x.Date); });
      Assert.All(list, x => { Assert.True(x.Value > 0); });
    }


    [Fact]
    public void Should_Parse_The_Empty_Exchange_Rate_Type() {
      ExchangeRateType empty = ExchangeRateType.Empty;

      Assert.NotNull(empty);
    }


    #endregion Facts

  }  // class ExchangeRatesTests

}  // namespace Empiria.FinancialAccounting.Tests
