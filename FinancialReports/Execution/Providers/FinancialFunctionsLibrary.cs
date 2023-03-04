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

        new Function(lexeme: "CALCULAR_ABONOS_R04", arity: 0,
                     calle: () => new CalcularAbonosR04()),

        new Function(lexeme: "CALCULAR_ABONOS_EFECTO_VALUACION_R04", arity: 0,
                     calle: () => new CalcularAbonosEfectoValuacionR04()),

        new Function(lexeme: "CALCULAR_CARGOS_R04", arity: 0,
                     calle: () => new CalcularCargosR04()),

        new Function(lexeme: "CALCULAR_CARGOS_EFECTO_VALUACION_R04", arity: 0,
                     calle: () => new CalcularCargosEfectoValuacionR04()),

        new Function(lexeme: "DEUDORAS_MENOS_ACREEDORAS", arity: 3,
                     calle: () => new DeudorasMenosAcreedorasFunction()),

        new Function(lexeme: "VALORES_CONCEPTO", arity: 1,
                     calle: () => new ValoresConceptoFunction(_executionContext.ConceptsCalculator)),

      };

      base.AddRange(functions);
    }


    sealed private class CalcularAbonosR04 : FunctionHandler {

      protected override object Evaluate() {
        IFinancialConceptValues _this = (IFinancialConceptValues) base.Data["this"];

        decimal abonosPesos = _this.GetTotalField("abonosPesos");
        decimal abonosMonExt = _this.GetTotalField("abonosMonExtVal");
        decimal abonosUDIs = _this.GetTotalField("abonosUDIsVal");

        _this.SetTotalField("totalMonedaNacional", abonosPesos);
        _this.SetTotalField("totalMonedaExtranjera", abonosMonExt);
        _this.SetTotalField("totalUdis", abonosUDIs);

        _this.SetTotalField("totalR04", abonosPesos + abonosMonExt + abonosUDIs);

        return _this;
      }

    }  // CalcularAbonosR04


    sealed private class CalcularAbonosEfectoValuacionR04 : FunctionHandler {

      protected override object Evaluate() {
        IFinancialConceptValues _this = (IFinancialConceptValues) base.Data["this"];

        decimal abonosMonExt = _this.GetTotalField("abonosEfectosValuacionMonExt");
        decimal abonosUDIs = _this.GetTotalField("abonosEfectosValuacionUDIs");

        _this.SetTotalField("totalMonedaNacional", 0);
        _this.SetTotalField("totalMonedaExtranjera", abonosMonExt);
        _this.SetTotalField("totalUdis", abonosUDIs);

        _this.SetTotalField("totalR04", abonosMonExt + abonosUDIs);

        return _this;
      }

    }  // class CalcularAbonosEfectoValuacionR04


    sealed private class CalcularCargosR04 : FunctionHandler {

      protected override object Evaluate() {
        IFinancialConceptValues _this = (IFinancialConceptValues) base.Data["this"];

        decimal cargosPesos = _this.GetTotalField("cargosPesos");
        decimal cargosMonExt = _this.GetTotalField("cargosMonExtVal");
        decimal cargosUDIs = _this.GetTotalField("cargosUDIsVal");

        _this.SetTotalField("totalMonedaNacional", cargosPesos);
        _this.SetTotalField("totalMonedaExtranjera", cargosMonExt);
        _this.SetTotalField("totalUdis", cargosUDIs);

        _this.SetTotalField("totalR04", cargosPesos + cargosMonExt + cargosUDIs);

        return _this;
      }

    }  // class CalcularCargosR04


    sealed private class CalcularCargosEfectoValuacionR04 : FunctionHandler {

      protected override object Evaluate() {
        IFinancialConceptValues _this = (IFinancialConceptValues) base.Data["this"];

        decimal cargosMonExt = _this.GetTotalField("cargosEfectosValuacionMonExt");
        decimal cargosUDIs = _this.GetTotalField("cargosEfectosValuacionUDIs");

        _this.SetTotalField("totalMonedaNacional", 0);
        _this.SetTotalField("totalMonedaExtranjera", cargosMonExt);
        _this.SetTotalField("totalUdis", cargosUDIs);

        _this.SetTotalField("totalR04", cargosMonExt + cargosUDIs);
        return _this;
      }

    }  //  class CalcularCargosEfectoValuacionR04


    /// <summary>Returns deudoras minus acreedoras, or acreedoras minus deudoras balance,
    /// depending on the concept code.</summary>
    sealed private class DeudorasMenosAcreedorasFunction : FunctionHandler {

      protected override object Evaluate() {

        FinancialConcept concept = GetObject<FinancialConcept>(Parameters[0]);
        decimal deudorasBalance = GetDecimal(Parameters[1]);
        decimal acreedorasBalance = GetDecimal(Parameters[2]);

        //if (!concept.Group.Tags.Contains("DeudorasMenosAcreedoras")) {

        //  return deudorasBalance - acreedorasBalance;
        //}

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

    }  // DeudorasMenosAcreedorasFunction

  }  // class FinancialFunctionsLibrary

}  // namespace Empiria.FinancialAccounting.FinancialReports.Providers
