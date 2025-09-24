/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Vouchers Management                        Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Vouchers.dll           Pattern   : Builder                                 *
*  Type     : VoucherBuilder                             License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Abstract class used to create special case vouchers.                                           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.FinancialAccounting.Vouchers.Adapters;
using Empiria.FinancialAccounting.Vouchers.SpecialCases;

namespace Empiria.FinancialAccounting.Vouchers {

  /// <summary>Abstract class used to create special case vouchers.</summary>
  internal abstract class VoucherBuilder {

    #region Constructors and parsers

    protected VoucherBuilder(VoucherSpecialCaseFields fields) {
      Assertion.Require(fields, nameof(fields));

      this.Fields = fields;
      this.SpecialCaseType = VoucherSpecialCaseType.Parse(fields.VoucherTypeUID);
      this.Fields.VoucherTypeUID = this.SpecialCaseType.VoucherType.UID;
      this.Fields.TransactionTypeUID = TransactionType.Automatic.UID;
      this.AccountsChart = AccountsChart.Parse(this.Fields.AccountsChartUID);
      this.Ledger = Ledger.Parse(this.Fields.LedgerUID);
    }


    static internal VoucherBuilder CreateBuilder(VoucherSpecialCaseFields fields) {
      switch (fields.VoucherTypeUID) {

        case "DepreciacionActivoFijo":
          return new DepreciacionActivoFijoVoucherBuilder(fields);

        case "CancelacionMovimientos":
          return new CancelacionMovimientosVoucherBuilder(fields);

        case "CancelacionCuentasResultados":
          return new CancelacionCuentasResultadosVoucherBuilder(fields);

        case "NivelacionCuentasCompraventa":
          return new NivelacionCuentasCompraventaVoucherBuilder(fields);

        case "CancelacionSaldosEncerrados":
          fields.Concept = "Traspaso de saldos por modificación al catálogo de cuentas";
          fields.CalculationDate = fields.AccountingDate;

          Assertion.Require(fields.AccountingDate >= fields.DatePeriod.ToDate,
            $"La fecha de afectación {fields.AccountingDate:dd/MMM/yyyy} debe ser igual " +
            $"o posterior a la fecha final del período de cancelación de los saldos encerrados " +
            $"{fields.DatePeriod.ToDate:dd/MMM/yyyy}.");

          return new CancelacionSaldosEncerradosVoucherBuilder(fields);

        default:
          throw Assertion.EnsureNoReachThisCode($"Unrecognized voucher special case {fields.VoucherTypeUID}.");
      }
    }

    #endregion Constructors and parsers

    #region Fields

    protected AccountsChart AccountsChart {
      get;
    }

    protected Ledger Ledger {
      get;
    }

    protected VoucherSpecialCaseFields Fields {
      get;
    }


    public VoucherSpecialCaseType SpecialCaseType {
      get;
    }


    #endregion Fields

    #region Abstract methods

    protected abstract FixedList<VoucherEntryFields> BuildVoucherEntries();


    internal virtual FixedList<string> DryRun() {
      FixedList<VoucherEntryFields> entries = BuildVoucherEntries();

      return ImplementsDryRun(entries);
    }


    internal virtual Voucher GenerateVoucher() {
      FixedList<VoucherEntryFields> entries = BuildVoucherEntries();

      FixedList<string> issues = this.ImplementsDryRun(entries);

      Assertion.Require(issues.Count == 0,
        $"There were one or more issues generating '{SpecialCaseType.Name}' voucher: " +
        EmpiriaString.ToString(issues));

      var voucher = new Voucher(Fields, entries);

      voucher.SaveAll();

      return voucher;
    }



    internal bool TryGenerateVoucher(out Voucher voucher) {
      try {
        voucher = GenerateVoucher();
        return true;

      } catch {
        voucher = null;
        return false;
      }
    }


    private FixedList<string> ImplementsDryRun(FixedList<VoucherEntryFields> entries) {
      var validator = new VoucherValidator(Ledger.Parse(Fields.LedgerUID),
                                           Fields.AccountingDate, SpecialCaseType.SkipEntriesValidation);

      return validator.Validate(entries);
    }

    #endregion Abstract methods

  }  // class VoucherBuilder

}  //  namespace Empiria.FinancialAccounting.Vouchers
