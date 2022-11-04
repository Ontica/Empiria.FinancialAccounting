/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Domain Layer                            *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Service provider                        *
*  Type     : FinancialReportCalculator                  License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Converts a plain text expression into an Expression object.                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.FinancialReports.Expressions {

  /// <summary>Converts a plain text expression into an Expression object.</summary>
  internal class ExpressionParser {

    private readonly string _plainText;

    public ExpressionParser(string plainText) {
      Assertion.Require(plainText, nameof(plainText));

      _plainText = plainText;
    }


    internal Expression Compile() {
      FunctionObject function = ParseFunctionObject();

      return new Expression(function);
    }


    private FunctionObject ParseFunctionObject() {
      FunctionToken token = ParseFunctionToken();

      FixedList<FunctionParameter> parameters = ParseFunctionParameters();

      return new FunctionObject(token, parameters);
    }


    private FunctionToken ParseFunctionToken() {
      return FunctionToken.SUM;
    }


    private FixedList<FunctionParameter> ParseFunctionParameters() {
      return new FixedList<FunctionParameter>();
    }


  }  // class ExpressionParser

}  // namespace Empiria.FinancialAccounting.FinancialReports.Expressions
