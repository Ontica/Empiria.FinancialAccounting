/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Exchange Rates                             Component : Test cases                              *
*  Assembly : Empiria.FinancialAccounting.Tests.dll      Pattern   : Use cases tests                         *
*  Type     : ExchangeRatesUseCasesTests                 License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use case tests for exchange rates management.                                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Xunit;

using Empiria.FinancialAccounting.UseCases;
using Empiria.FinancialAccounting.Adapters;

namespace Empiria.FinancialAccounting.Tests {

  /// <summary>Use case tests for exchange rates management.</summary>
  public class ExchangeRatesUseCasesTests {

    #region Use cases initialization

    private readonly ExchangeRatesUseCases _usecases;

    public ExchangeRatesUseCasesTests() {
      CommonMethods.Authenticate();

      _usecases = ExchangeRatesUseCases.UseCaseInteractor();
    }

    ~ExchangeRatesUseCasesTests() {
      _usecases.Dispose();
    }

    #endregion Use cases initialization


    #region Facts

    [Fact]
    public void Should_Get_A_List_Of_Exchange_Rates_Types() {
      FixedList<ExchangeRateTypeDto> list = _usecases.GetExchangeRatesTypes();

      Assert.NotEmpty(list);
    }


    [Fact]
    public void Should_Get_Exchange_On_A_Date() {
      var DATE = new DateTime(2019, 08, 31);
      string exchangeRateTypeUID = "96c617f6-8ed9-47f3-8d2d-f1240e446e1d";

      FixedList<ExchangeRateDto> list = _usecases.GetExchangeRates(exchangeRateTypeUID, DATE);

      Assert.NotEmpty(list);

      Assert.All(list, x => { Assert.Equal(exchangeRateTypeUID, x.ExchangeRateType.UID); });
      Assert.All(list, x => { Assert.Equal(DATE, x.Date); });
      Assert.All(list, x => { Assert.True(x.Value > 0); });
    }

    #endregion Facts

  }  // class ExchangeRatesUseCasesTests

}  // namespace Empiria.FinancialAccounting.Tests
