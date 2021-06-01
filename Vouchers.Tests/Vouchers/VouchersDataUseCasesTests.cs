/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Vouchers Management                          Component : Test cases                            *
*  Assembly : FinancialAccounting.BalanceEngine.Tests      Pattern   : Use cases tests                       *
*  Type     : VouchersDataUseCasesTests                    License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Test cases for vouchers related data.                                                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using Xunit;

using Empiria.FinancialAccounting.Vouchers.UseCases;

namespace Empiria.FinancialAccounting.Tests {

  /// <summary>Test cases for vouchers related data.</summary>
  public class VouchersDataUseCasesTests {

    #region Fields

    private readonly VouchersDataUseCases _usecases;

    #endregion Fields

    #region Initialization

    public VouchersDataUseCasesTests() {
      CommonMethods.Authenticate();

      _usecases = VouchersDataUseCases.UseCaseInteractor();
    }

    ~VouchersDataUseCasesTests() {
      _usecases.Dispose();
    }

    #endregion Initialization

    #region Facts

    [Fact]
    public void Should_Get_The_list_Of_Transaction_Types() {
      FixedList<NamedEntityDto> transactionTypes = _usecases.TransactionTypes();

      Assert.NotEmpty(transactionTypes);
    }


    [Fact]
    public void Should_Get_The_list_Of_Voucher_Types() {
      FixedList<NamedEntityDto> vouchersTypes = _usecases.VoucherTypes();

      Assert.NotEmpty(vouchersTypes);
    }


    #endregion Facts

  }  // class VouchersDataUseCasesTests

}  // namespace Empiria.FinancialAccounting.Tests
