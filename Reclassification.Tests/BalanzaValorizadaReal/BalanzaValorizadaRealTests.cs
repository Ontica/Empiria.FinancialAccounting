/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reclassification Services                    Component : Test cases                            *
*  Assembly : FinancialAccounting.Reclassification.Tests   Pattern   : Unit tests                            *
*  Type     : BalanzaValorizadaRealTests                   License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Unit tests for BalanzaValorizadaReal report entries.                                           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Xunit;

using System;
using System.Collections.Generic;

using Empiria.FinancialAccounting.Reclassification;
using Empiria.FinancialAccounting.Reclassification.Adapters;
using Empiria.FinancialAccounting.Reclassification.Data;

namespace Empiria.FinancialAccounting.Tests.Reclassification {

  /// <summary>Unit tests for BalanzaValorizadaReal report entries.</summary>
  public class BalanzaValorizadaRealTests {

    #region Facts

    [Fact]
    public void Should_Get_BalanzaValorizadaReal() {

      DateTime fromDate = Convert.ToDateTime("01-01-2026");
      DateTime toDate = Convert.ToDateTime("01-02-2026");

      FixedList<AccountReclassifiedBalances> sut = ReclassifiedBalancesDataService.GetBalances(fromDate, toDate);

      Assert.NotNull(sut);
    }


    [Fact]
    public void Should_Get_BalanzaValorizadaPorMonedaReal() {

      DateTime fromDate = Convert.ToDateTime("01-01-2026");
      DateTime toDate = Convert.ToDateTime("01-02-2026");

      FixedList<AccountReclassifiedBalances> balanzaValorizadaReal = ReclassifiedBalancesDataService.GetBalances(fromDate, toDate);

      var balanzaEntries = MapToBalanzaReal(balanzaValorizadaReal);


      var sut = BalanzaEnMonedasMapper.Map(balanzaEntries);

      Assert.NotNull(sut);
    }

    #endregion Factsno

    #region Helpers

    private FixedList<BalanzaReal> MapToBalanzaReal(FixedList<AccountReclassifiedBalances> balance) {

      var idCuentaStandar = -1;
      BalanzaReal entryBalance = new BalanzaReal();
      List<BalanzaReal> balanzaEntries = new List<BalanzaReal>();

      foreach (var entry in balance) {
        if (entry.StdAccount.Id != idCuentaStandar) {
          balanzaEntries.Add(entryBalance);
          entryBalance = new BalanzaReal();

          entryBalance.CuentaEstandar = entry.StdAccount;
          idCuentaStandar = entry.StdAccount.Id;
        }

        CurrencyBalance currencyBalance = new CurrencyBalance();
        currencyBalance.InitialBalance = entry.InitialBalance;
        currencyBalance.Credits = entry.Credits;
        currencyBalance.Debits = entry.Debits;
        currencyBalance.Currency = entry.Currency;
        currencyBalance.FinalBalance = entry.FinalBalance;

        entryBalance.SaldosPorMoneda.Add(currencyBalance);

        CurrencyBalance currencyBalanceReal = new CurrencyBalance();
        currencyBalanceReal.InitialBalance = entry.RealInitialBalance;
        currencyBalanceReal.Credits = entry.RealCredits;
        currencyBalanceReal.Debits = entry.RealDebits;
        currencyBalanceReal.Currency = entry.RealCurrency;
        currencyBalanceReal.FinalBalance = entry.RealFinalBalance;

        entryBalance.SaldosPorMonedaReal.Add(currencyBalanceReal);
      }

      balanzaEntries.RemoveAt(0);

      return balanzaEntries.ToFixedList();
    }


    #endregion Helpers

  } // class BalanzaValorizadaRealTests

} // namespace Empiria.FinancialAccounting.Tests.Reclassification
