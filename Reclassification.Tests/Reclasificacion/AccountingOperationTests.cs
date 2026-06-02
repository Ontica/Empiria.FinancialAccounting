/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reclassification Services                    Component : Test cases                            *
*  Assembly : FinancialAccounting.Reclassification.Tests   Pattern   : Unit tests                            *
*  Type     : AccountingOperationTests    cccc             License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Unit tests for Accounting operation.cccccc                                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Xunit;

using Empiria.FinancialAccounting.Vouchers.Adapters;
using Empiria.FinancialAccounting.Vouchers.UseCases;
using Empiria.FinancialAccounting.Reclassification;


namespace Empiria.FinancialAccounting.Tests.Reclassification {

  /// <summary>Unit tests for Accounting operation.</summary>
  public class AccountingOperationTests {

    #region Facts

    [Fact]
    public void Should_Parse_All_AccountingOperation() {
      var operations = AccountingOperation.GetList();

      foreach (var sut in operations) {
        Assert.NotEmpty(sut.DebitAccount);
      }
    }


    [Fact]
    public void Should_Read_All_AccountingOperation() {
      var sut = AccountingOperation.GetList();

      Assert.NotNull(sut);
      Assert.NotEmpty(sut);
    }


    [Fact]
    public void ExportVoucherMovementesTest() {

      using (var usecases = VoucherUseCases.UseCaseInteractor()) {

        VoucherDto sut = usecases.GetVoucher(9195387);
        var op = new TransactionReclasificator(sut);

        op.GroupTransactions();


        Assert.NotNull(sut);
      }

    }

    #endregion Facts

  } // class AccountingOperationTests

} // namespace Empiria.FinancialAccounting.Tests.Reclassification
