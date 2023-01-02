/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                            Component : Test cases                            *
*  Assembly : FinancialAccounting.FinancialReports.Tests   Pattern   : Use cases tests                       *
*  Type     : ExpressionEvaluationProviderTests            License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Test cases for the expression evaluation provider.                                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Xunit;

using System.Collections.Generic;

using Empiria.FinancialAccounting.FinancialReports.Providers;

namespace Empiria.FinancialAccounting.Tests.FinancialReports.Providers {

  /// <summary>Test cases for financial concepts statements.</summary>
  public class ExpressionEvaluationProviderTests {

    #region Theories


    [Theory]
    [InlineData("DEUDORAS_MENOS_ACREEDORAS(1000, 350, 200)", 150)]
    [InlineData("DEUDORAS_MENOS_ACREEDORAS(2000, 1400, 1500)", 100)]
    [InlineData("DEUDORAS_MENOS_ACREEDORAS(1000, a, b)", 510.1)]
    [InlineData("DEUDORAS_MENOS_ACREEDORAS(2000, a, b)", -510.10)]
    [InlineData("DEUDORAS_MENOS_ACREEDORAS(1000, 2000 - b, 2000 - a)", 510.10)]
    [InlineData("DEUDORAS_MENOS_ACREEDORAS(2000, a + b, 1500)", -1051.00)]
    public void Should_Evaluate_Deudoras_Menos_Acreedoras(string textExpression, decimal expected) {
      var expression = new ExpressionEvaluationProvider();

      var data = new Dictionary<string, object> {
        { "a", 1530.55 },
        { "b", 1020.45 }
      };

      decimal sut = expression.Evaluate<decimal>(textExpression, data);

      Assert.Equal(expected, sut);
    }

    #endregion Theories

  }  // class ExpressionEvaluationProviderTests

} // namespace Empiria.FinancialAccounting.Tests.FinancialReports.Scripts
