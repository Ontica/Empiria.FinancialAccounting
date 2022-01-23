/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Vouchers Management                        Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Vouchers.dll           Pattern   : Concrete Builder                        *
*  Type     : CancelacionCuentasResultadosVoucherBuilder License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Builds a voucher that cancels the balances of profit and loss accounts                         *
*             at a given date (cuentas de resultados).                                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Vouchers.SpecialCases {

  /// <summary>Builds a voucher that cancels the balances of profit and loss
  /// accounts (cuentas de resultados) at a given date.</summary>
  internal class CancelacionCuentasResultadosVoucherBuilder : VoucherBuilder {

    internal CancelacionCuentasResultadosVoucherBuilder() {
      // no-op
    }


    internal override FixedList<string> DryRun() {
      throw new NotImplementedException("DryRun.CancelacionCuentasResultadosVoucherBuilder");
    }


    internal override Voucher GenerateVoucher() {
      throw new NotImplementedException("GenerateVoucher.CancelacionCuentasResultadosVoucherBuilder");
    }


  }  // class CancelacionCuentasResultadosVoucherBuilder

}  // namespace Empiria.FinancialAccounting.Vouchers.SpecialCases
