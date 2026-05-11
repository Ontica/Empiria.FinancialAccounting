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
using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.Tests;
using Xunit;

namespace Empiria.FinancialAccounting.Tests.BalanceEngine.BalanzaTradicional {

  /// <summary>Unit test cases for 'Balanza tradicional' report entries.</summary>
  public class BalanzaValorizadaRealTests {

    #region Initialization

    public BalanzaValorizadaRealTests() {
      TestsCommonMethods.Authenticate();
    }

    #endregion Initialization


    #region Facts

    [Fact]
    public void Should_Get_BalanzaValorizadaReal() {

      TestsCommonMethods.Authenticate();

      DateTime fromDate = Convert.ToDateTime("01-01-2026");
      DateTime toDate = Convert.ToDateTime("01-02-2026");

      var balanzaValorizadaReal = new BalanzaValorizadaReal();

      FixedList<BalanzaValorizadaReal> sut = balanzaValorizadaReal.GetBalance(fromDate, toDate);

      Assert.NotNull(sut);
    }

    #endregion Facts


    #region Helpers


    #endregion Helpers

  } // class BalanzaValorizadaRealTests

} // namespace Empiria.FinancialAccounting.Tests.BalanceEngine.BalanzaTradicional
