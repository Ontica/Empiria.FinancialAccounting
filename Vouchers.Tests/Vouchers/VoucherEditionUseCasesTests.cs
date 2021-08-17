/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Vouchers Management                          Component : Test cases                            *
*  Assembly : FinancialAccounting.Vouchers.Tests           Pattern   : Use cases tests                       *
*  Type     : VoucherEditionUseCasesTests                  License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Test cases for vouchers and postings edition.                                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using Xunit;

using Empiria.FinancialAccounting.Vouchers.Adapters;
using Empiria.FinancialAccounting.Vouchers.UseCases;

namespace Empiria.FinancialAccounting.Tests.Vouchers {

  /// <summary>Test cases for vouchers and postings edition.</summary>
  public class VoucherEditionUseCasesTests {

    #region Fields

    private readonly VoucherEditionUseCases _usecases;

    #endregion Fields

    #region Initialization

    public VoucherEditionUseCasesTests() {
      CommonMethods.Authenticate();

      _usecases = VoucherEditionUseCases.UseCaseInteractor();
    }

    ~VoucherEditionUseCasesTests() {
      _usecases.Dispose();
    }

    #endregion Initialization

    #region Facts


    [Fact]
    public void Should_Create_A_Voucher() {
      VoucherFields fields = null;

      VoucherDto voucher = _usecases.CreateVoucher(fields);

      Assert.NotNull(voucher);
    }


    [Fact]
    public void Should_Delete_A_Voucher() {
      int voucherId = 0;

      _usecases.DeleteVoucher(voucherId);
    }


    [Fact]
    public void Should_Update_A_Voucher() {
      int voucherId = 0;
      VoucherFields fields = null;

      VoucherDto voucher = _usecases.UpdateVoucher(voucherId, fields);

      Assert.NotNull(voucher);
    }


    #endregion Facts

  }  // class VoucherEditionUseCasesTests

}  // namespace Empiria.FinancialAccounting.Tests.Vouchers
