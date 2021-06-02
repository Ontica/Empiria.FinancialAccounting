/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Vouchers Management                          Component : Test cases                            *
*  Assembly : FinancialAccounting.Vouchers.Tests           Pattern   : Use cases tests                       *
*  Type     : SearchVouchersUseCasesTests                  License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Test cases for search vouchers.                                                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using Xunit;

using Empiria.FinancialAccounting.Vouchers.UseCases;
using Empiria.FinancialAccounting.Vouchers.Adapters;

namespace Empiria.FinancialAccounting.Tests {

  /// <summary>Test cases for search vouchers.</summary>
  public class SearchVouchersUseCasesTests {

    #region Fields

    private readonly VouchersUseCases _usecases;

    #endregion Fields

    #region Initialization

    public SearchVouchersUseCasesTests() {
      CommonMethods.Authenticate();

      _usecases = VouchersUseCases.UseCaseInteractor();
    }

    ~SearchVouchersUseCasesTests() {
      _usecases.Dispose();
    }

    #endregion Initialization

    #region Facts

    [Fact]
    public void SearchVouchersWithKeywords() {
      var command = new SearchVouchersCommand {
        Keywords = "intereses"
      };

      FixedList<VoucherDescriptorDto> vouchers = _usecases.SearchVouchers(command);

      Assert.NotEmpty(vouchers);
    }

    #endregion Facts

  }  // class SearchVouchersUseCasesTests

}  // namespace Empiria.FinancialAccounting.Tests
