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

using Xunit;

using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Data;

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

    #endregion Facts

  } // class BalanzaValorizadaRealTests

} // namespace Empiria.FinancialAccounting.Tests.BalanceEngine.BalanzaTradicional
