/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Vouchers Management                          Component : Test cases                            *
*  Assembly : FinancialAccounting.Vouchers.Tests           Pattern   : Use cases tests                       *
*  Type     : VoucherUseCasesTests                         License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Test cases for accounting vouchers.                                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Xunit;

using Empiria.Tests;

using Empiria.FinancialAccounting.Vouchers.UseCases;
using Empiria.FinancialAccounting.Vouchers.Adapters;

namespace Empiria.FinancialAccounting.Tests.Vouchers {

  /// <summary>Test cases for accounting vouchers.</summary>
  public class VoucherUseCasesTests {

    #region Fields

    private readonly VoucherUseCases _usecases;

    #endregion Fields

    #region Initialization

    public VoucherUseCasesTests() {
      TestsCommonMethods.Authenticate();

      _usecases = VoucherUseCases.UseCaseInteractor();
    }

    ~VoucherUseCasesTests() {
      _usecases.Dispose();
    }

    #endregion Initialization

    #region Facts

    [Fact]
    public void Should_Read_A_Voucher() {
      VoucherDto voucherDto = _usecases.GetVoucher(TestingConstants.VOUCHER_ID);

      Assert.NotNull(voucherDto);

      Assert.Equal(TestingConstants.VOUCHER_ID, voucherDto.Id);
    }


    [Fact]
    public void Should_Search_Vouchers_With_Keywords() {
      var command = new SearchVouchersCommand {
        Keywords = "intereses"
      };

      FixedList<VoucherDescriptorDto> vouchers = _usecases.SearchVouchers(command);

      Assert.NotEmpty(vouchers);
    }

    #endregion Facts

  }  // class VouchersUseCasesTests

}  // namespace Empiria.FinancialAccounting.Tests.Vouchers
