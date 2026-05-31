/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reclassification Services                    Component : Test cases                            *
*  Assembly : FinancialAccounting.Reclassification.Tests   Pattern   : Unit tests                            *
*  Type     : AccountingOperationTypeTests                 License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Unit tests for Accounting operation types.                                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Xunit;

using Empiria.FinancialAccounting.Reclassification;

namespace Empiria.FinancialAccounting.Tests.Reclassification {

  /// <summary>Unit tests for Accounting operation types.</summary>
  public class AccountingOperationTypeTests {

    #region Facts

    [Fact]
    public void Should_Parse_All_Credit_Types() {
      var creditTypes = AccountingOperationType.GetList();

      foreach (var sut in creditTypes) {
        Assert.NotEmpty(sut.Name);
      }
    }


    [Fact]
    public void Should_Read_All_Credit_Types() {
      var sut = AccountingOperationType.GetList();

      Assert.NotNull(sut);
      Assert.NotEmpty(sut);
    }

    #endregion Facts

  } // class AccountingOperationTypeTests

} // namespace Empiria.FinancialAccounting.Tests.Reclassification
