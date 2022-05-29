/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                 Component : Vouchers Importer                    *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll   Pattern   : Service provider                     *
*  Type     : InterfazUnicaImporter                         License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Provides services to import vouchers using a Banobras' InterfazUnica compatible structure.     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter.Adapters;

namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter {

  /// <summary>Provides services to import vouchers using a Banobras'
  /// InterfazUnica compatible data structure.</summary>
  internal class InterfazUnicaImporter {

    private readonly FixedList<EncabezadoDto> _vouchers;

    internal InterfazUnicaImporter(InterfazUnicaImporterCommand command) {
      Assertion.Require(command, "command");

      _vouchers = new FixedList<EncabezadoDto>(command.ENCABEZADOS);
    }


    internal ImportVouchersResult DryRunImport() {
      return GetDryRunResult();
    }


    internal ImportVouchersResult Import() {
      ImportVouchersResult result = this.DryRunImport();

      if (result.HasErrors) {
        return result;
      }

      try {
        foreach (EncabezadoDto voucher in _vouchers) {
          InterfazUnicaDataService.WriteEncabezado(voucher);

          foreach (MovimientoDto voucherEntry in voucher.MOVIMIENTOS) {
            InterfazUnicaDataService.WriteMovimiento(voucher, voucherEntry);
          }
        }
      } catch (Exception e) {
        EmpiriaLog.Error(e);
        throw;
      }

      return result;
    }


    #region Private methods

    private ImportVouchersResult GetDryRunResult() {
      var result = new ImportVouchersResult();

      result.VouchersCount = _vouchers.Count;

      return result;
    }

    #endregion Private methods

  }  // class InterfazUnicaImporter

}  //namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter
