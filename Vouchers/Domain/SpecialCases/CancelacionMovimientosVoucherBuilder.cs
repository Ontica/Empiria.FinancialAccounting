/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Vouchers Management                        Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Vouchers.dll           Pattern   : Concrete Builder                        *
*  Type     : CancelacionMovimientosVoucherBuilder       License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Builds a voucher that manticancels the balances of profit and loss accounts                    *
*             at a given date (cuentas de resultados).                                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.Vouchers.Adapters;

namespace Empiria.FinancialAccounting.Vouchers.SpecialCases {

  /// <summary>Builds a voucher that cancels (reverses) the credits and debits
  /// included in another voucher.</summary>
  internal class CancelacionMovimientosVoucherBuilder : VoucherBuilder {

    public CancelacionMovimientosVoucherBuilder(VoucherSpecialCaseFields fields) : base(fields) {
      // no-op
    }


    internal override Voucher BuildVoucher() {
      throw new NotImplementedException("CancelacionMovimientosVoucherBuilder");
    }


  }  // class CancelacionMovimientosVoucherBuilder

}  // namespace Empiria.FinancialAccounting.Vouchers.SpecialCases
