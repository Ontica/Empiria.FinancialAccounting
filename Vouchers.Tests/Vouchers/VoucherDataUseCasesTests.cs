/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Vouchers Management                          Component : Test cases                            *
*  Assembly : FinancialAccounting.Vouchers.Tests           Pattern   : Use cases tests                       *
*  Type     : VoucherDataUseCasesTests                     License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Test cases for voucher related data.                                                           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Xunit;

using Empiria.FinancialAccounting.Vouchers.UseCases;

namespace Empiria.FinancialAccounting.Tests.Vouchers {

  /// <summary>Test cases for vouchers related data.</summary>
  public class VoucherDataUseCasesTests {

    #region Fields

    private readonly VoucherDataUseCases _usecases;

    #endregion Fields

    #region Initialization

    public VoucherDataUseCasesTests() {
      CommonMethods.Authenticate();

      _usecases = VoucherDataUseCases.UseCaseInteractor();
    }

    ~VoucherDataUseCasesTests() {
      _usecases.Dispose();
    }

    #endregion Initialization

    #region Facts


    [Fact]
    public void Should_Read_Entry_Event_Types() {
      FixedList<NamedEntityDto> eventTypes = _usecases.EventTypes();

      Assert.NotEmpty(eventTypes);
    }


    [Fact]
    public void Should_Read_Functional_Areas() {
      FixedList<NamedEntityDto> functionalAreas = _usecases.FunctionalAreas();

      Assert.NotEmpty(functionalAreas);
    }


    [Fact]
    public void Should_Read_Opened_Accounting_Dates() {
      FixedList<DateTime> accountingDates = _usecases.OpenedAccountingDates(TestingConstants.LEDGER_UID);

      Assert.NotEmpty(accountingDates);
    }


    [Fact]
    public void Should_Read_Transaction_Types() {
      FixedList<NamedEntityDto> transactionTypes = _usecases.TransactionTypes();

      Assert.NotEmpty(transactionTypes);
    }


    [Fact]
    public void Should_Read_Voucher_Types() {
      FixedList<NamedEntityDto> vouchersTypes = _usecases.VoucherTypes();

      Assert.NotEmpty(vouchersTypes);
    }


    [Fact]
    public void Should_Search_Accounts_for_Voucher() {
      int voucherId = TestingConstants.VOUCHER_ID;

      FixedList<NamedEntityDto> vouchers = _usecases.SearchAccountsForVoucher(voucherId, "impuestos");

      Assert.NotEmpty(vouchers);
    }


    #endregion Facts

  }  // class VoucherDataUseCasesTests

}  // namespace Empiria.FinancialAccounting.Tests.Vouchers
