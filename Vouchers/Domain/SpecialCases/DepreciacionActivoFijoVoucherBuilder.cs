/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Vouchers Management                        Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Vouchers.dll           Pattern   : Concrete Builder                        *
*  Type     : DepreciacionActivoFijoVoucherBuilder       License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Builds a voucher for fixed assets depreciation.                                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;

using Empiria.FinancialAccounting.FixedAssetsDepreciation;
using Empiria.FinancialAccounting.Vouchers.Adapters;

namespace Empiria.FinancialAccounting.Vouchers.SpecialCases {

  /// <summary>Builds a voucher for fixed assets depreciation.</summary>
  internal class DepreciacionActivoFijoVoucherBuilder : VoucherBuilder {

    internal DepreciacionActivoFijoVoucherBuilder(VoucherSpecialCaseFields fields) : base(fields) {
      // no-op
    }

    #region Abstract Implements

    protected override FixedList<VoucherEntryFields> BuildVoucherEntries() {

      FixedList<FixedAssetsDepreciationEntry> fixedAssetsToDepreciate = GetFixedAssetsToDepreciate();

      var tipos = fixedAssetsToDepreciate.Select(x => x.TipoActivoFijo)
                                         .Distinct()
                                         .ToFixedList();

      var voucherEntries = new List<VoucherEntryFields>();

      foreach (var tipo in tipos) {
        var activos = fixedAssetsToDepreciate.FindAll(x => x.DepreciacionPendienteRegistrar > 0 && x.TipoActivoFijo.UID == tipo.UID);

        if (activos.Count != 0) {

          decimal suma = activos.Sum(x => x.DepreciacionPendienteRegistrar);

          voucherEntries.Add(BuildVoucherEntry(tipo.ValorHistoricoAccount, suma, VoucherEntryType.Debit, SubledgerAccount.Empty));

          foreach (var activo in activos) {
            voucherEntries.Add(BuildVoucherEntry(tipo.DepreciacionAcumuladaAccount, activo.DepreciacionPendienteRegistrar,
                                                 VoucherEntryType.Credit, activo.AuxiliarHistorico));
          }

        }

        activos = fixedAssetsToDepreciate.FindAll(x => x.DepreciacionPendienteRegistrarDeLaRevaluacion > 0 && x.TipoActivoFijo.UID == tipo.UID);

        if (activos.Count != 0) {

          decimal suma = activos.Sum(x => x.DepreciacionPendienteRegistrarDeLaRevaluacion);

          voucherEntries.Add(BuildVoucherEntry(tipo.RevaluacionAccount, suma, VoucherEntryType.Debit, SubledgerAccount.Empty));

          foreach (var activo in activos) {
            voucherEntries.Add(BuildVoucherEntry(tipo.RevaluacionDepreciacionAcumuladaAccount, activo.DepreciacionPendienteRegistrarDeLaRevaluacion,
                                                 VoucherEntryType.Credit, activo.AuxiliarRevaluacion));
          }

        }

      }

      Assertion.Require(voucherEntries.Count >= 2,
                        "No hay activos fijos por depreciar a la fecha proporcionada.");

      return voucherEntries.ToFixedList();
    }


    #endregion Abstract Implements

    #region Helpers

    private FixedList<FixedAssetsDepreciationEntry> GetFixedAssetsToDepreciate() {
      var builder = new FixedAssetsDepreciationBuilder(base.Fields.CalculationDate, new[] { base.Ledger.UID });

      FixedList<FixedAssetsDepreciationEntry> list = builder.Build();

      return list.FindAll(x => x.DepreciacionPendienteRegistrar > 0 || x.DepreciacionPendienteRegistrarDeLaRevaluacion > 0);
    }


    private VoucherEntryFields BuildVoucherEntry(string accountNumber, decimal balance,
                                                 VoucherEntryType entryType,
                                                 SubledgerAccount subledgerAccount) {
      StandardAccount stdAccount = base.AccountsChart.GetStandardAccount(accountNumber);

      LedgerAccount ledgerAccount = base.Ledger.AssignAccount(stdAccount);

      return new VoucherEntryFields {
        Amount = balance,
        BaseCurrencyAmount = balance,
        CurrencyUID = base.Ledger.BaseCurrency.UID,
        SectorId = Sector.Empty.Id,
        SubledgerAccountId = subledgerAccount.Id,
        SubledgerAccountNumber = subledgerAccount.IsEmptyInstance ?
                                                    string.Empty : subledgerAccount.Number,
        StandardAccountId = stdAccount.Id,
        LedgerAccountId = ledgerAccount.Id,
        VoucherEntryType = entryType
      };
    }

    #endregion Helpers

  }  // class DepreciacionActivoFijoVoucherBuilder

}  // namespace Empiria.FinancialAccounting.Vouchers.SpecialCases
