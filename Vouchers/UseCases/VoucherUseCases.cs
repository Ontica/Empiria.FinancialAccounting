/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Vouchers Management                        Component : Use cases Layer                         *
*  Assembly : FinancialAccounting.Vouchers.dll           Pattern   : Use case interactor class               *
*  Type     : VoucherUseCases                            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases used to retrive and manage accounting vouchers.                                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.FinancialAccounting.Vouchers.Adapters;

namespace Empiria.FinancialAccounting.Vouchers.UseCases {

  /// <summary>Use cases used to retrive and manage accounting vouchers.</summary>
  public class VoucherUseCases : UseCase {

    #region Constructors and parsers

    protected VoucherUseCases() {
      // no-op
    }

    static public VoucherUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<VoucherUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases


    public VoucherDto GetVoucher(long voucherId) {
      Assertion.Assert(voucherId > 0, "Unrecognized voucherId.");

      Voucher voucher = Voucher.Parse(voucherId);

      return VoucherMapper.Map(voucher);
    }


    public VoucherEntryDto GetVoucherEntry(long voucherId, long voucherEntryId) {
      Assertion.Assert(voucherId > 0, "Unrecognized voucherId.");
      Assertion.Assert(voucherEntryId > 0, "Unrecognized voucherEntryId.");

      Voucher voucher = Voucher.Parse(voucherId);
      VoucherEntry voucherEntry = voucher.GetEntry(voucherEntryId);

      return VoucherMapper.MapEntry(voucherEntry);
    }

    public string GetVoucherAsHtmlString(long voucherId) {
      Assertion.Assert(voucherId > 0, "Unrecognized voucherId.");

      Voucher voucher = Voucher.Parse(voucherId);

      return "http://172.27.207.97/sicofin/files/vouchers/poliza.prueba.pdf";
    }


    public FixedList<VoucherDescriptorDto> SearchVouchers(SearchVouchersCommand searchCommand) {
      Assertion.AssertObject(searchCommand, "searchCommand");

      searchCommand.EnsureIsValid();

      string filter = searchCommand.MapToFilterString();
      string sort = searchCommand.MapToSortString();

      FixedList<Voucher> list = Voucher.GetList(filter, sort, searchCommand.PageSize);

      return VoucherMapper.MapToVoucherDescriptor(list);
    }

    #endregion Use cases

  }  // class VoucherUseCases

}  // namespace Empiria.FinancialAccounting.Vouchers.UseCases
