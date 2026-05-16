/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Use cases Layer                         *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Use case interactor class               *
*  Type     : BalanzaValorizadaRealUseCases              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases used to read Real valorize balance.                                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System;
using System.Collections.Generic;
using Empiria.DynamicData;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.Data;
using Empiria.Services;

namespace Empiria.FinancialAccounting.BalanceEngine.UseCases {

  /// <summary>Use cases used to read Real valorize balance.</summary>
  public class BalanzaValorizadaRealUseCases : UseCase {

    #region Constructors and parsers

    protected BalanzaValorizadaRealUseCases() {
      // no-op
    }

    static public BalanzaValorizadaRealUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<BalanzaValorizadaRealUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases

    public DynamicDto<BalanzaValorizadaRealDto> BuildBalances(DateTime fromDate, DateTime toDate) {
      Assertion.Require(fromDate, nameof(fromDate));
      Assertion.Require(toDate, nameof(toDate));

      FixedList<BalanzaValorizadaReal> balances =
                            BalanzaValorizadaRealDataService.GetBalances(fromDate, toDate);

      return BalanzaValorizadaRealMapper.Map(balances);
    }


    public DynamicDto<BalanzaRealDto> BuildBalanceReal(DateTime fromDate, DateTime toDate) {
      Assertion.Require(fromDate, nameof(fromDate));
      Assertion.Require(toDate, nameof(toDate));

      FixedList<BalanzaValorizadaReal> balances =
                            BalanzaValorizadaRealDataService.GetBalances(fromDate, toDate);
      FixedList<BalanzaReal> balanzaReal = MapToBalanzaReal(balances);

      return BalanzaRealMapper.Map(balanzaReal);
    }

    #endregion

    #region Helpers

    private FixedList<BalanzaReal> MapToBalanzaReal(FixedList<BalanzaValorizadaReal> balance) {
      var idCuentaStandar = -1;
      BalanzaReal entryBalance = new BalanzaReal();
      List<BalanzaReal> balanzaEntries = new List<BalanzaReal>();

      foreach (var entry in balance) {
        if (entry.CuentaEstandar.Id != idCuentaStandar) {
          balanzaEntries.Add(entryBalance);
          entryBalance = new BalanzaReal();

          entryBalance.CuentaEstandar = entry.CuentaEstandar;
          idCuentaStandar = entry.CuentaEstandar.Id;
        }

        CurrencyBalance currencyBalance = new CurrencyBalance();
        currencyBalance.InitialBalance = entry.SaldoInicial;
        currencyBalance.Credits = entry.Haber;
        currencyBalance.Debits = entry.Debe;
        currencyBalance.Currency = entry.Moneda;
        currencyBalance.FinalBalance = entry.SaldoFinal;

        entryBalance.SaldosPorMoneda.Add(currencyBalance);

        CurrencyBalance currencyBalanceReal = new CurrencyBalance();
        currencyBalanceReal.InitialBalance = entry.SaldoInicialReal;
        currencyBalanceReal.Credits = entry.HaberMonedaReal;
        currencyBalanceReal.Debits = entry.DebeMonedaReal;
        currencyBalanceReal.Currency = entry.MonedaReal;
        currencyBalanceReal.FinalBalance = entry.SaldoFinalReal;

        entryBalance.SaldosPorMonedaReal.Add(currencyBalanceReal);
      }

      balanzaEntries.RemoveAt(0);

      return balanzaEntries.ToFixedList();
    }

    #endregion Helpers

  } // class BalanzaValorizadaRealUseCases

} // namespace Empiria.FinancialAccounting.BalanceEngine.UseCases
