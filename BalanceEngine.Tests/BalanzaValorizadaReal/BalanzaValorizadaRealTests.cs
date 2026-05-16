/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test cases                              *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Unit tests                              *
*  Type     : BalanzaValorizadaRealTests                 License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Unit test cases for 'Balanza tradicional' report entries.                                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System;
using System.Collections.Generic;
using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.Data;
using Xunit;

namespace Empiria.FinancialAccounting.Tests.BalanceEngine.BalanzaValorizadaRealTests {

  /// <summary>Unit test cases for 'Balanza valorizada real' report entries.</summary>
  public class BalanzaValorizadaRealTests {

    #region Facts

    [Fact]
    public void Should_Get_BalanzaValorizadaReal() {

      DateTime fromDate = Convert.ToDateTime("01-01-2026");
      DateTime toDate = Convert.ToDateTime("01-02-2026");

      FixedList<BalanzaValorizadaReal> sut = BalanzaValorizadaRealDataService.GetBalances(fromDate, toDate);

      Assert.NotNull(sut);
    }


    [Fact]
    public void Should_Get_BalanzaValorizadaPorMonedaReal() {

      DateTime fromDate = Convert.ToDateTime("01-01-2026");
      DateTime toDate = Convert.ToDateTime("01-02-2026");

      FixedList<BalanzaValorizadaReal> balanzaValorizadaReal = BalanzaValorizadaRealDataService.GetBalances(fromDate, toDate);

      var balanzaEntries = MapToBalanzaReal(balanzaValorizadaReal);


      var sut = BalanzaRealMapper.Map(balanzaEntries);

      Assert.NotNull(sut);
    }

    #endregion Factsno

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


  } // class BalanzaValorizadaRealTests

} // namespace Empiria.FinancialAccounting.Tests.BalanceEngine.BalanzaTradicional
