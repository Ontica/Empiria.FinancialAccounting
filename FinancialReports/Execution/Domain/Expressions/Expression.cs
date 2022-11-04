/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Domain Layer                            *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Information Holder                      *
*  Type     : Expression                                 License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Performs data calculation over financial reports data.                                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.FinancialReports.Expressions {

  internal class Expression {

    internal Expression(FunctionObject function) {
      Assertion.Require(function, nameof(function));

      Function = function;
    }

    internal FunctionObject Function {
      get;
    }


    internal T Evaluate<T>(DynamicFields data) {
      decimal result = data.GetTotalField("monedaNacional") - data.GetTotalField("monedaExtranjera");

      return (T) (object) result;
    }

  }  // class Expression

}  // namespace Empiria.FinancialAccounting.FinancialReports.Expressions
