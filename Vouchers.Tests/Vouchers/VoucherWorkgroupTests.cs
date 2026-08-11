/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Ledger Management                          Component : Test cases                              *
*  Assembly : FinancialAccounting.Vouchers.Tests         Pattern   : Unit Test                               *
*  Type     : VoucherWorkgroupTests                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Unit tests for Voucher Workgroup.                                                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using Empiria.FinancialAccounting.Vouchers;
using Xunit;


namespace Empiria.FinancialAccounting.Tests {

  /// <summary>Unit tests for Voucher Workgroup.</summary>
  public class VoucherWorkgroupTests {

    #region Facts

    [Fact]
    public void Should_Get_All_VoucherWorkgroups() {
      var sut = VoucherWorkgroup.GetList();

      Assert.NotNull(sut);
      Assert.NotEmpty(sut);
    }


    [Fact]
    public void Should_Parse_All_VoucherWorkgroups() {
      var workGroupList = VoucherWorkgroup.GetList();

      foreach (var sut in workGroupList) {
        Assert.NotNull(sut.Name);
      }
    }


    [Fact]
    public void Should_Get_VoucherWorkgroup_Members() {
      var workGroupList = VoucherWorkgroup.GetList();

      foreach (var sut in workGroupList) {
        Assert.NotEmpty(sut.Members);
        Assert.NotNull(sut.Members);
      }
    }

    #endregion Facts

  }  // class VoucherWorkgroupTests

}  // namespace Empiria.FinancialAccounting.Tests
