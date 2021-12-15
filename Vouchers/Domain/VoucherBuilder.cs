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

    protected VoucherBuilder() {
      // no-op
    }

    protected VoucherSpecialCaseFields Fields {
      get;
      private set;
    }

    #region Abstract methods

    internal abstract Voucher BuildVoucher();

    #endregion Abstract methods

    static internal VoucherBuilder SelectBuilder(VoucherSpecialCaseFields fields) {
      VoucherBuilder builder;

      switch (fields.VoucherTypeUID) {
        case "CancelacionMovimientos":
          builder = SelectCancelacionMovimientosBuilder(fields);
          break;

        case "CancelacionCuentasResultados":
          builder = SelectCancelacionCuentasResultadosBuilder();
          break;

        case "NivelacionCuentasCompraventa":
          builder = SelectNivelacionCuentasCompraventaBuilder();
          break;

        default:
          throw Assertion.AssertNoReachThisCode($"Unrecognized voucher special case {fields.VoucherTypeUID}");
      }

      builder.Fields = fields;

      return builder;
    }


    static private VoucherBuilder SelectCancelacionCuentasResultadosBuilder() {
      throw new NotImplementedException();
    }


    static private VoucherBuilder SelectCancelacionMovimientosBuilder(VoucherSpecialCaseFields fields) {
      Ledger ledger = Ledger.Parse(fields.LedgerUID);

      Voucher voucher = Voucher.TryParse(ledger, fields.OnVoucherNumber);

      if (voucher == null) {
        Assertion.AssertNoReachThisCode(
          $"No existe la póliza '{fields.OnVoucherNumber}' en la contabilidad {ledger.FullName}."
        );
      }

      return new CancelacionMovimientosVoucherBuilder(voucher);
    }


    static private VoucherBuilder SelectNivelacionCuentasCompraventaBuilder() {
      throw new NotImplementedException();
    }

  }  // class VoucherBuilder

}  //  namespace Empiria.FinancialAccounting.Vouchers
