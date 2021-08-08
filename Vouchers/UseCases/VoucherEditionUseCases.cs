﻿/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Vouchers Management                        Component : Use cases Layer                         *
*  Assembly : FinancialAccounting.Vouchers.dll           Pattern   : Use case interactor class               *
*  Type     : VoucherEditionUseCases                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases used to edit vouchers and their postings.                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.FinancialAccounting.Vouchers.Adapters;

namespace Empiria.FinancialAccounting.Vouchers.UseCases {

  /// <summary>Use cases used to edit vouchers and their postings.</summary>
  public class VoucherEditionUseCases : UseCase {

    #region Constructors and parsers

    protected VoucherEditionUseCases() {
      // no-op
    }

    static public VoucherEditionUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<VoucherEditionUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases


    public VoucherDto AppendEntry(int voucherId, VoucherEntryFields fields) {
      Assertion.AssertObject(fields, "fields");

      var voucher = Voucher.Parse(voucherId);

      var voucherEntry = voucher.AppendEntry(fields);

      voucherEntry.Save();

      return VoucherMapper.Map(voucher);
    }


    public VoucherDto CloseVoucher(int voucherId) {
      throw new NotImplementedException();
    }


    public VoucherDto CreateVoucher(VoucherFields fields) {
      Assertion.AssertObject(fields, "fields");

      fields.EnsureIsValid();

      var voucher = new Voucher(fields);

      voucher.Save();

      return VoucherMapper.Map(voucher);
    }


    public VoucherDto DeleteEntry(int voucherId, int voucherEntryId) {
      var voucher = Voucher.Parse(voucherId);

      VoucherEntry entry = voucher.GetEntry(voucherEntryId);

      voucher.DeleteEntry(entry);

      return VoucherMapper.Map(voucher);
    }


    public void DeleteVoucher(int voucherId) {
      var voucher = Voucher.Parse(voucherId);

      voucher.Delete();
    }


    public VoucherDto UpdateVoucher(int voucherId, VoucherFields fields) {
      Assertion.AssertObject(fields, "fields");

      fields.EnsureIsValid();

      var voucher = Voucher.Parse(voucherId);

      voucher.Update(fields);

      voucher.Save();

      return VoucherMapper.Map(voucher);
    }


    public VoucherDto UpdateEntry(int voucherId, int voucherEntryId,
                                  VoucherEntryFields fields) {
      throw new NotImplementedException();
    }


    #endregion Use cases

  }  // class VoucherEditionUseCases

}  // namespace Empiria.FinancialAccounting.Vouchers.UseCases
