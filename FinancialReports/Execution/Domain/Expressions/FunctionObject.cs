/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Domain Layer                            *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Information Holder                      *
*  Type     : FunctionObject                             License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Describes a function object that can be evaluated.                                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.FinancialReports.Expressions {


  /// <summary>Describes a function object that can be evaluated.</summary>
  internal class FunctionObject {

    internal FunctionObject(FunctionToken token, FixedList<FunctionParameter> parameters) {
      Assertion.Require(token, nameof(token));
      Assertion.Require(parameters, nameof(parameters));

      Definition = FunctionDefinition.Parse(token);

      Parameters = parameters;
    }

    public FunctionDefinition Definition {
      get;
    }

    public FixedList<FunctionParameter> Parameters {
      get;
    }


  }  // FunctionObject

}  // namespace Empiria.FinancialAccounting.FinancialReports.Expressions
