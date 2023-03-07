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

using Empiria.FinancialAccounting.FinancialConcepts;

namespace Empiria.FinancialAccounting.FinancialReports.Providers {

  /// <summary>Default arithmetic functions library.</summary>
  internal class FinancialFunctionsLibrary : BaseFunctionsLibrary {

    private readonly ExecutionContext _executionContext;

    public FinancialFunctionsLibrary(ExecutionContext executionContext) {
      _executionContext = executionContext;

      LoadFunctions();
    }


    private void LoadFunctions() {
      var functions = new[] {

        new Function(lexeme: "DEUDORAS_MENOS_ACREEDORAS", arity: 3,
                     calle: () => new DeudorasMenosAcreedorasFunction()),

        new Function(lexeme: "VALORES_CONCEPTO", arity: 1,
                     calle: () => new ValoresConceptoFunction(_executionContext.ConceptsCalculator)),

      };

      base.AddRange(functions);
    }

    /// <summary>Returns deudoras minus acreedoras, or acreedoras minus deudoras balance,
    /// depending on the concept code.</summary>
    sealed private class DeudorasMenosAcreedorasFunction : FunctionHandler {

      protected override object Evaluate() {

        FinancialConcept concept = GetObject<FinancialConcept>(Parameters[0]);
        decimal deudorasBalance = GetDecimal(Parameters[1]);
        decimal acreedorasBalance = GetDecimal(Parameters[2]);

        if (concept.Code.Contains(",")) {

          return deudorasBalance - acreedorasBalance;
        }

        if (concept.Code.StartsWith("2") || concept.Code.StartsWith("4") || concept.Code.StartsWith("5")) {
          return acreedorasBalance - deudorasBalance;

        } else {
          return deudorasBalance - acreedorasBalance;
        }
      }

    }  // DeudorasMenosAcreedorasFunction


    /// <summary>Returns deudoras minus acreedoras, or acreedoras minus deudoras balance,
    /// depending on the concept code.</summary>
    sealed private class ValoresConceptoFunction : FunctionHandler {

      private readonly FinancialConceptsCalculator _conceptsCalculator;

      public ValoresConceptoFunction(FinancialConceptsCalculator conceptsCalculator) {
        _conceptsCalculator = conceptsCalculator;
      }

      protected override object Evaluate() {

        int conceptId = Convert.ToInt32(GetDecimal(Parameters[0]));

        var financialConcept = FinancialConcept.Parse(conceptId);

        return _conceptsCalculator.Calculate(financialConcept);
      }

    }  // ValoresConceptoFunction

  }  // class FinancialFunctionsLibrary

}  // namespace Empiria.FinancialAccounting.FinancialReports.Providers
