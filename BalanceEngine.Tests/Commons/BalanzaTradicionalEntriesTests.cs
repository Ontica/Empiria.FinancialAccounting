/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test cases                              *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Unit tests                              *
*  Type     : TrialBalanceValuedTests                    License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Unit test cases for 'Balanza tradicional' report entries.                                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System;

using Xunit;
using Empiria.Tests;
using Empiria.FinancialAccounting.BalanceEngine.Data;

namespace Empiria.FinancialAccounting.Tests.BalanceEngine.BalanzaBalorizadaPorDia {

  /// <summary>Unit test cases for 'Balanza tradicional' report entries.</summary>
  public class TrialBalanceValuedTests {

    #region Initialization

    public TrialBalanceValuedTests() {
      TestsCommonMethods.Authenticate();
    }

    #endregion Initialization

    #region Facts

    [Fact]
    public void Should_Get_TrialBalaceValuedEntries_By_Date() {

      DateTime fromDate = Convert.ToDateTime("01/01/2026");
      DateTime toDate = Convert.ToDateTime("31/01/2026");

      var sut = BalancesDataServiceV3.GetDailyMovements(fromDate, toDate);

      Assert.NotNull(sut);
    }

    #endregion Facts

    #region Helpers






    #endregion Helpers

  } // class TrialBalanceValuedTests

} // namespace Empiria.FinancialAccounting.Tests.BalanceEngine.BalanzaTradicional
