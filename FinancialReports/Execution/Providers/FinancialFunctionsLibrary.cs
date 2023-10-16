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


        new Function(lexeme: "SALDO_CUENTA", arity: 1,
                     calle: () => new SaldoCuentaFunction(_executionContext.BalancesProvider)),


        new Function(lexeme: "TIPO_CAMBIO", arity: 1,
                     calle: () => new TipoCambioFunction(_executionContext.BuildQuery.ToDate)),


        new Function(lexeme: "VALOR_CONCEPTO", arity: 2,
                     calle: () => new ValorConceptoFunction(_executionContext.ConceptsCalculator)),


        new Function(lexeme: "VALOR_CONCEPTO_EXTERNO", arity: 3,
                     calle: () => new ValorConceptoExternoFunction(_executionContext)),


        new Function(lexeme: "VALORES_CONCEPTO", arity: 1,
                     calle: () => new ValoresConceptoFunction(_executionContext.ConceptsCalculator)),


        new Function(lexeme: "VALOR_EXTERNO", arity: 2,
                     calle: () => new ValorExternoFunction(_executionContext.ExternalValuesProvider)),


        new Function(lexeme: "VALORIZAR", arity: 2,
                     calle: () => new ValorizarFunction(_executionContext.BuildQuery.ToDate)),

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

    }  // class DeudorasMenosAcreedorasFunction



    /// <summary>Returns the current balance of an account.</summary>
    sealed private class SaldoCuentaFunction : FunctionHandler {

      private readonly AccountBalancesProvider _balancesProvider;

      public SaldoCuentaFunction(AccountBalancesProvider provider) {
        _balancesProvider = provider;
      }

      protected override object Evaluate() {

        string accountNumber = GetString(Parameters[0]);

        var balance = _balancesProvider.TryGetAccountBalances(accountNumber);

        if (balance != null) {
          return balance.CurrentBalanceForBalances;
        } else {
          EmpiriaLog.Trace($"No encontré la cuenta {accountNumber}.");
          return 0m;
        }

      }

    }  // class SaldoCuentaFunction



    /// <summary>Returns the current balance of an account.</summary>
    sealed private class TipoCambioFunction : FunctionHandler {

      private readonly ExchangeRatesProvider _exchangeRatesProvider;

      public TipoCambioFunction(DateTime date) {
        _exchangeRatesProvider = new ExchangeRatesProvider(date);
      }

      protected override object Evaluate() {

        var currency = Currency.Parse(GetString(Parameters[0]));

        return _exchangeRatesProvider.Convert(ExchangeRateType.ValorizacionBanxico, currency, 1m, 6);
      }

    }  // class TipoCambioFunction



    /// <summary>Returns a decimal value for a given field of a financial concept.</summary>
    sealed private class ValorConceptoFunction : FunctionHandler {

      private readonly FinancialConceptsCalculator _conceptsCalculator;

      public ValorConceptoFunction(FinancialConceptsCalculator conceptsCalculator) {
        _conceptsCalculator = conceptsCalculator;
      }

      protected override object Evaluate() {

        string variableID = GetString(Parameters[0]);

        string fieldName = GetString(Parameters[1]);

        var financialConcept = FinancialConcept.ParseWithVariableID(variableID);

        IFinancialConceptValues values = _conceptsCalculator.Calculate(financialConcept);

        return values.GetTotalField(fieldName);
      }

    }  // class ValorConceptoFunction



    /// <summary>Returns a full evaluated financial concept with all of its fields.</summary>
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

    }  // class ValoresConceptoFunction



    /// <summary>Returns a decimal value for a given field of a financial concept.</summary>
    sealed private class ValorConceptoExternoFunction : FunctionHandler {

      private readonly ExecutionContext _originalContext;

      public ValorConceptoExternoFunction(ExecutionContext originalContext) {
        _originalContext = originalContext;
      }

      protected override object Evaluate() {

        int reportTypeId = (int) GetDecimal(Parameters[0]);

        string variableID = GetString(Parameters[1]);

        string fieldName = GetString(Parameters[2]);

        var reportType = FinancialReportType.Parse(reportTypeId);

        ExecutionContext executionContext = _originalContext.CreateCopy(reportType);

        var financialConcept = FinancialConcept.ParseWithVariableID(variableID);

        IFinancialConceptValues values = executionContext.ConceptsCalculator.Calculate(financialConcept);

        return values.GetTotalField(fieldName);
      }

    }  // class ValorConceptoExternoFunction



    /// <summary>Returns a decimal value for a given field of a financial concept.</summary>
    sealed private class ValorExternoFunction : FunctionHandler {

      private readonly ExternalValuesProvider _provider;

      public ValorExternoFunction(ExternalValuesProvider provider) {
        _provider = provider;
      }

      protected override object Evaluate() {

        string externalVariableID = GetString(Parameters[0]);

        string fieldName = GetString(Parameters[1]);

        return _provider.GetValue(externalVariableID, fieldName);
      }

    }  // ValorConceptoFunction



    /// <summary>Returns a currency value converted to a given exchange rate.</summary>
    sealed private class ValorizarFunction: FunctionHandler {

      private readonly ExchangeRatesProvider _exchangeRatesProvider;

      public ValorizarFunction(DateTime date) {
        _exchangeRatesProvider = new ExchangeRatesProvider(date);
      }

      protected override object Evaluate() {
        decimal value = GetDecimal(Parameters[0]);
        var currency = Currency.Parse(GetString(Parameters[1]));

        return _exchangeRatesProvider.Convert(ExchangeRateType.ValorizacionBanxico, currency, value, 2);
      }

    }  // ValorizarFunction


  }  // class FinancialFunctionsLibrary

}  // namespace Empiria.FinancialAccounting.FinancialReports.Providers
