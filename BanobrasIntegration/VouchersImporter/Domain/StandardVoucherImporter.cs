/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                 Component : Vouchers Importer                    *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll   Pattern   : Service provider                     *
*  Type     : StandardVoucherImporter                       License   : Please read LICENSE.txt file         *
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
  internal class StandardVoucherImporter {

    private readonly ImportVouchersCommand _command;
    private readonly StandardVouchersStructure _standardStructure;

    #region Public methods

    internal StandardVoucherImporter(ImportVouchersCommand command,
                                     StandardVouchersStructure standardStructure) {
      _command = command;
      _standardStructure = standardStructure;
    }


    internal ImportVouchersResult DryRunImport() {
      FixedList<ImportVouchersTotals> voucherTotals = GetVouchersTotals();

      var voucherErrors = new List<NamedEntityDto>();
      var voucherWarnings = new List<NamedEntityDto>();

      var result = new ImportVouchersResult();

      result.VoucherTotals = voucherTotals;
      result.Errors = voucherErrors.ToFixedList();
      result.Warnings = voucherWarnings.ToFixedList();

      return result;
    }

    #endregion Public methods

    #region Private methods

    private FixedList<ImportVouchersTotals> GetVouchersTotals() {
      return new FixedList<ImportVouchersTotals>();
    }

    #endregion Private methods

  }  // class StandardVoucherImporter

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter
