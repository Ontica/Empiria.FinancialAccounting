/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Vouchers Management                        Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Vouchers.dll           Pattern   : Builder                                 *
*  Type     : VoucherBuilder                             License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Abstract class used to create special case vouchers.                                           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.Vouchers.Adapters;

using Empiria.FinancialAccounting.Vouchers.SpecialCases;

namespace Empiria.FinancialAccounting.Vouchers {

  /// <summary>Abstract class used to create special case vouchers.</summary>
  internal abstract class VoucherBuilder {

    private readonly VoucherSpecialCaseFields _fields;

    protected VoucherBuilder(VoucherSpecialCaseFields fields) {
      _fields = fields;
    }


    internal abstract Voucher BuildVoucher();


    static internal VoucherBuilder SelectBuilder(VoucherSpecialCaseFields fields) {
      switch (fields.VoucherTypeUID) {
        case "CancelacionMovimientos":
          return new CancelacionMovimientosVoucherBuilder(fields);

        case "CancelacionCuentasResultados":
          return new CancelacionCuentasResultadosVoucherBuilder(fields);

        case "NivelacionCuentasCompraventa":
          return new NivelacionCuentasCompraventaVoucherBuilder(fields);

        default:
          throw Assertion.AssertNoReachThisCode($"Unrecognized voucher special case {fields.VoucherTypeUID}");
      }
    }

  }  // class VoucherBuilder

}  //  namespace Empiria.FinancialAccounting.Vouchers
