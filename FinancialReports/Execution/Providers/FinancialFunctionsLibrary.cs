/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Providers                               *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Empiria Expressions Functions Library   *
*  Type     : FinancialFunctionsLibrary                  License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Financial functions library.                                                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Expressions;
using Empiria.Expressions.Execution;

namespace Empiria.FinancialAccounting.FinancialReports.Providers {

  /// <summary>Default arithmetic functions library.</summary>
  internal class FinancialFunctionsLibrary : BaseFunctionsLibrary {

    private FinancialFunctionsLibrary() {
      LoadFunctions();
    }


    static public FinancialFunctionsLibrary Instance {
      get {
        return new FinancialFunctionsLibrary();
      }
    }


    private void LoadFunctions() {
      var functions = new[] {
        new Function(lexeme: "DEUDORAS_MENOS_ACREEDORAS", arity: 3,
                     calle: () => new DeudorasMenosAcreedorasFunction()),
      };

      base.AddRange(functions);
    }


    /// <summary>Returns deudoras minus acreedoras, or acreedoras minus deudoras balance,
    /// depending on the concept code.</summary>
    sealed private class DeudorasMenosAcreedorasFunction : FunctionHandler {

      protected override object Evaluate() {

        string conceptCode = GetString(Parameters[0]);

        decimal deudorasBalance = GetDecimal(Parameters[1]);
        decimal acreedorasBalance = GetDecimal(Parameters[2]);

        if (conceptCode.StartsWith("2") || conceptCode.StartsWith("4") || conceptCode.StartsWith("5")) {
          return acreedorasBalance - deudorasBalance;

        } else {
          return deudorasBalance - acreedorasBalance;
        }
      }

    }  // DeudorasMenosAcreedorasFunction

  }  // class FinancialFunctionsLibrary

}  // namespace Empiria.FinancialAccounting.FinancialReports.Providers
