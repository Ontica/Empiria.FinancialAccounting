/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Providers                               *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Service provider                        *
*  Type     : RuntimeCompiler                            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides runtime scripts execution and expressions evaluation services.                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.Expressions;

namespace Empiria.FinancialAccounting.FinancialReports.Providers {

  /// <summary>Provides runtime scripts execution and expressions evaluation services.</summary>
  internal class RuntimeCompiler {

    private readonly LexicalGrammar _grammar;

    internal RuntimeCompiler() {
      _grammar = LexicalGrammar.CreateFromDefault();

      _grammar.LoadLibrary(FinancialFunctionsLibrary.Instance);
    }


    internal T EvaluateExpression<T>(string textExpression) {

      var expression = new Expression(_grammar, textExpression);

      return expression.Evaluate<T>();
    }


    internal T EvaluateExpression<T>(string textExpression, IDictionary<string, object> inputValues) {

      var expression = new Expression(_grammar, textExpression);

      return expression.Evaluate<T>(inputValues);
    }


    internal T ExecuteScript<T>(string script, IDictionary<string, object> inputValues) {
      throw new NotImplementedException();

      // var script = new Script(_grammar, script);

      // return script.Execute<T>(inputValues);
    }

  }  // class RuntimeCompiler

}  // namespace Empiria.FinancialAccounting.FinancialReports.Providers
