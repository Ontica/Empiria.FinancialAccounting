/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Providers                               *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Service provider                        *
*  Type     : ExpressionEvaluationProvider               License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides runtime textual expressions evaluation services.                                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.Expressions;

namespace Empiria.FinancialAccounting.FinancialReports.Providers {

/// <summary>Provides runtime textual expressions evaluation services.</summary>
  internal class ExpressionEvaluationProvider {

    internal ExpressionEvaluationProvider() {

    }


    internal T Evaluate<T>(string textExpression) {

      var expression = new Expression(textExpression);

      return expression.Evaluate<T>();
    }


    internal T Evaluate<T>(string textExpression, IDictionary<string, object> values) {

      var expression = new Expression(textExpression);

      return expression.Evaluate<T>(values);
    }


  }  // class ExpressionEvaluationProvider

}  // namespace Empiria.FinancialAccounting.FinancialReports.Providers
