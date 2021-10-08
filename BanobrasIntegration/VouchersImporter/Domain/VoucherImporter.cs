/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                 Component : Vouchers Importer                    *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll   Pattern   : Service provider                     *
*  Type     : VoucherImporter                               License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Performs voucher importation tasks from a standard structure adapted from distinct sources.    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter.Adapters;

namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter {

  /// <summary>Performs voucher importation tasks from a standard structure
  /// adapted from distinct sources.</summary>
  internal class VoucherImporter {

    private readonly ImportVouchersCommand _command;
    private readonly ToImportVouchersList _toImportVouchersList;

    #region Public methods

    internal VoucherImporter(ImportVouchersCommand command,
                             ToImportVouchersList toImportVouchersList) {
      Assertion.AssertObject(command, "command");
      Assertion.AssertObject(toImportVouchersList, "toImportVouchersList");

      _command = command;
      _toImportVouchersList = toImportVouchersList;
    }


    internal ImportVouchersResult DryRunImport() {
      var result = new ImportVouchersResult();

      result.VoucherTotals = GetImportVoucherTotals();

      result.Errors = GetImportErrors();
      result.Warnings = GetImportWarnings();

      return result;
    }


    internal ImportVouchersResult Import() {
      ImportVouchersResult result = this.DryRunImport();

      if (result.HasErrors) {
        return result;
      }

      return result;
    }


    #endregion Public methods

    #region Private methods

    private FixedList<NamedEntityDto> GetImportErrors() {
      throw new NotImplementedException();
    }


    private FixedList<NamedEntityDto> GetImportWarnings() {
      throw new NotImplementedException();
    }


    private FixedList<ImportVouchersTotals> GetImportVoucherTotals() {
      throw new NotImplementedException();
    }


    #endregion Private methods

  }  // class StandardVoucherImporter

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter
