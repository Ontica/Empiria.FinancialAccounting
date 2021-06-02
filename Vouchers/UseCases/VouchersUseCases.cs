/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Vouchers Management                        Component : Use cases Layer                         *
*  Assembly : FinancialAccounting.Vouchers.dll           Pattern   : Use case interactor class               *
*  Type     : VouchersUseCases                           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases used to retrive and manage accounting vouchers.                                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.FinancialAccounting.Vouchers.Adapters;

namespace Empiria.FinancialAccounting.Vouchers.UseCases {

  /// <summary>Use cases used to retrive and manage accounting vouchers.</summary>
  public class VouchersUseCases : UseCase {

    #region Constructors and parsers

    protected VouchersUseCases() {
      // no-op
    }

    static public VouchersUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<VouchersUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases


    public FixedList<VoucherDescriptorDto> SearchVouchers(SearchVouchersCommand searchCommand) {
      Assertion.AssertObject(searchCommand, "searchCommand");

      searchCommand.EnsureIsValid();

      string filter = searchCommand.MapToFilterString();
      string sort = searchCommand.MapToSortString();

      FixedList<Voucher> list = Voucher.GetList(filter, sort, searchCommand.PageSize);

      return VoucherMapper.MapToVoucherDescriptor(list);
    }

    #endregion Use cases

  }  // class VouchersUseCases

}  // namespace Empiria.FinancialAccounting.Vouchers.UseCases
