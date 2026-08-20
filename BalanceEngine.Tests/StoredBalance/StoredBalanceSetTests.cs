/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test cases                              *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Unit tests                              *
*  Type     : StoredBalanceSetTests                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Unit test cases for StoredBalanceSet.                                                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using Xunit;

using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.StateEnums;


namespace Empiria.FinancialAccounting.Tests.BalanceEngine {

  /// <summary>nit test cases for StoredBalanceSet.</summary>
  public class StoredBalanceSetTests {
    #region Facts

    [Fact]
    public void Should_Get_All_StoredBalanceSet() {
      var sut = StoredBalanceSet.GetList<StoredBalanceSet>();

      Assert.NotNull(sut);
      Assert.NotEmpty(sut);
    }


    [Fact]
    public void Should_Parse_All_StoredBalanceSet() {
      var storedBalanceSetList = StoredBalanceSet.GetList<StoredBalanceSet>();

      foreach (var sut in storedBalanceSetList) {
        Assert.NotNull(sut.Name);
      }
    }

    [Fact]
    public void Should_Delete_StoredBalanceSet() {
      var sut = StoredBalanceSet.Parse(6106);

      sut.Delete();

      Assert.Equal(EntityStatus.Deleted, sut.Status);
    }

    #endregion Facts


  } // class StoredBalanceSetTests

} // namespace Empiria.FinancialAccounting.Tests.BalanceEngine
