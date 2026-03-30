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

using Empiria.Collections;
using Empiria.Tests;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine;

namespace Empiria.FinancialAccounting.Tests.BalanceEngine.BalanzaBalorizadaPorDia {

  /// <summary>Unit test cases for 'Balanza tradicional' report entries.</summary>
  public class TrialBalanceValuedTests {

    private readonly EmpiriaHashTable<BalanzaTradicionalDto> _cache =
                                        new EmpiriaHashTable<BalanzaTradicionalDto>(16);

    #region Initialization

    public TrialBalanceValuedTests() {
      TestsCommonMethods.Authenticate();
    }

    #endregion Initialization


    #region Facts

    [Fact]
    public void Should_Get_TrialBalaceValuedEntries_By_Date() {

      DateTime fromDate = Convert.ToDateTime("01/01/2025");
      DateTime toDate = Convert.ToDateTime("31/01/2025");
      int exchangeRateTypeId = 46;

      var sut = TrialBalanceValued.GetTrialBalanceValuedTransactions(fromDate, toDate, exchangeRateTypeId);

      Assert.NotNull(sut);
    }

    #endregion Facts


    #region Helpers






    #endregion Helpers

  } // class TrialBalanceValuedTests

} // namespace Empiria.FinancialAccounting.Tests.BalanceEngine.BalanzaTradicional
