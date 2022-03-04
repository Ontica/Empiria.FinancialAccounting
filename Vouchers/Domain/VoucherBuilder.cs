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

    #region Constructors and parsers

    protected VoucherBuilder() {
      // no-op
    }


    static internal VoucherBuilder CreateBuilder(VoucherSpecialCaseFields fields) {
      VoucherBuilder builder;

      switch (fields.VoucherTypeUID) {

        case "CancelacionMovimientos":
          builder = CreateCancelacionMovimientosBuilder(fields);
          break;

        case "CancelacionCuentasResultados":
          builder = CreateCancelacionCuentasResultadosBuilder();
          break;

        case "NivelacionCuentasCompraventa":
          builder = CreateNivelacionCuentasCompraventaBuilder();
          break;

        default:
          throw Assertion.AssertNoReachThisCode($"Unrecognized voucher special case {fields.VoucherTypeUID}.");
      }

      builder.Fields = fields;
      builder.SpecialCaseType = VoucherSpecialCaseType.Parse(fields.VoucherTypeUID);

      builder.Fields.VoucherTypeUID = builder.SpecialCaseType.VoucherType.UID;
      builder.Fields.TransactionTypeUID = TransactionType.Automatic.UID;

      return builder;
    }

    #endregion Constructors and parsers


    #region Fields

    protected VoucherSpecialCaseFields Fields {
      get;
      private set;
    }


    public VoucherSpecialCaseType SpecialCaseType {
      get;
      private set;
    }


    #endregion Fields

    #region Abstract methods

    internal abstract FixedList<string> DryRun();

    internal abstract Voucher GenerateVoucher();

    internal bool TryGenerateVoucher(Ledger ledger, out Voucher voucher) {
      this.Fields.LedgerUID = ledger.UID;

      try {
        voucher = GenerateVoucher();
        return true;

      } catch {
        voucher = null;
        return false;
      }
    }


    #endregion Abstract methods

    #region Factory methods

    static private VoucherBuilder CreateCancelacionCuentasResultadosBuilder() {
      throw new NotImplementedException();
    }


    static private VoucherBuilder CreateCancelacionMovimientosBuilder(VoucherSpecialCaseFields fields) {
      Ledger ledger = Ledger.Parse(fields.LedgerUID);

      Voucher voucherToCancel = Voucher.TryParse(ledger, fields.OnVoucherNumber);

      if (voucherToCancel == null) {
        Assertion.AssertNoReachThisCode(
          $"La póliza '{fields.OnVoucherNumber}' no existe en la contabilidad {ledger.FullName}."
        );
      }

      return new CancelacionMovimientosVoucherBuilder(voucherToCancel);
    }


    static private VoucherBuilder CreateNivelacionCuentasCompraventaBuilder() {
      return new NivelacionCuentasCompraventaVoucherBuilder();
    }

    #endregion Factory methods

  }  // class VoucherBuilder

}  //  namespace Empiria.FinancialAccounting.Vouchers
