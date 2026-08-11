/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Ledger Management                          Component : Test cases                              *
*  Assembly : FinancialAccounting.Vouchers.Tests         Pattern   : Unit Test                               *
*  Type     : VoucherSpecialCaseType                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Unit tests for Voucher Special Case Types.                                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using Empiria.FinancialAccounting.Vouchers;
using Xunit;


namespace Empiria.FinancialAccounting.Tests {

  /// <summary>Unit tests for SubLedger Types.</summary>
  public class VoucherSpecialCaseTypeTests {

    #region Facts

    [Fact]
    public void Should_Get_All_VoucherSpecialCaseType() {
      var sut = VoucherSpecialCaseType.GetList();

      Assert.NotNull(sut);
      Assert.NotEmpty(sut);
    }


    [Fact]
    public void Should_Parse_All_VoucherSpecialCaseType() {
      var rules = VoucherSpecialCaseType.GetList();

      foreach (var sut in rules) {
        Assert.NotNull(sut.Name);
      }
    }


    [Fact]
    public void Should_Parse_VoucherSpecialCaseType() {
      var sut = VoucherSpecialCaseType.GetList()
                 .Find(x => x.UID == "DepreciacionActivoFijo");

      Assert.NotNull(sut);
      Assert.Equal(59, sut.VoucherType.Id);
    }

    #endregion Facts

  }  // class VoucherSpecialCaseType

}  // namespace Empiria.FinancialAccounting.Tests
