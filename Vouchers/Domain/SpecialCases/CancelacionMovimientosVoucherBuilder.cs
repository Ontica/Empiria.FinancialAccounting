/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Vouchers Management                        Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Vouchers.dll           Pattern   : Concrete Builder                        *
*  Type     : CancelacionMovimientosVoucherBuilder       License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Builds a voucher that cancels (reverses) the credits and debits included in another voucher.   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Vouchers.SpecialCases {

  /// <summary>Builds a voucher that cancels (reverses) the credits and debits
  /// included in another voucher.</summary>
  internal class CancelacionMovimientosVoucherBuilder : VoucherBuilder {

    private readonly Voucher _voucherToCancel;

    internal CancelacionMovimientosVoucherBuilder(Voucher voucherToCancel) {
      Assertion.AssertObject(voucherToCancel, "voucherToCancel");

      _voucherToCancel = voucherToCancel;
    }


    internal override Voucher BuildVoucher() {
      throw new NotImplementedException("CancelacionMovimientosVoucherBuilder");
    }


  }  // class CancelacionMovimientosVoucherBuilder

}  // namespace Empiria.FinancialAccounting.Vouchers.SpecialCases
