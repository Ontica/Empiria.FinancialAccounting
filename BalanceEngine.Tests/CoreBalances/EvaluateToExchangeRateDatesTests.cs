/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test cases                              *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Use cases tests                         *
*  Type     : EvaluateToExchangeRateDatesTests           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Test cases to evaluate exchange rate dates.                                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System;
using Empiria.FinancialAccounting;
using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Xunit;

namespace Empiria.Tests.FinancialAccounting.BalanceEngine {

  /// <summary>Test cases to evaluate exchange rate dates.</summary>
  public class EvaluateToExchangeRateDatesTests {

    [Theory]
    [InlineData("2024-12-01", "2025-01-01")]
    [InlineData("2024-12-15", "2025-01-03")]
    [InlineData("2024-12-10", "2025-01-05")]
    [InlineData("2024-12-10", "2025-01-31")]
    [InlineData("2024-05-01", "2025-10-31")]
    [InlineData("2024-01-01", "2025-06-01")]
    [InlineData("2024-01-01", "2025-06-02")]
    [InlineData("2025-01-01", "2025-02-02")]
    [InlineData("2025-01-01", "2025-02-09")]
    [InlineData("2025-05-01", "2025-06-01")]
    [InlineData("2025-05-01", "2025-06-08")]
    [InlineData("2025-05-01", "2025-05-01")]
    [InlineData("2025-05-31", "2025-05-31")]
    [InlineData("2025-06-01", "2025-06-01")]
    [InlineData("2025-05-31", "2025-06-01")]
    public void Should_Have_Same_Entries(string fromDate, string toDate) {

      var exchangeRateFor = new ExchangeRateForCurrencies(DateTime.Parse(fromDate), DateTime.Parse(toDate));

      exchangeRateFor.GetDefaultExchangeRate();

      Assert.True(exchangeRateFor != null);
      Assert.True(exchangeRateFor.ExchangeRateList.Count > 0, $"No se encontraron registros de " +
                                                              $"tipos de cambio para las fechas del: {fromDate} " +
                                                              $"al: {toDate}, ");
    }

  } // class EvaluateToExchangeRateDatesTests

} // namespace Empiria.Tests.FinancialAccounting.BalanceEngine
