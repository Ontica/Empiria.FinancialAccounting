/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                 Component : Vouchers Importer                    *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll   Pattern   : Command Controller                   *
*  Type     : DbVouchersImporter                            License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Performs voucher importation tasks from a central database.                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter.Adapters;

namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter {

  /// <summary>Performs voucher importation tasks from a central database.</summary>
  internal class DbVouchersImporter {

    private readonly ImportVouchersCommand _command;

    internal DbVouchersImporter(ImportVouchersCommand command) {
      Assertion.AssertObject(command, "command");

      _command = command;
    }

    #region Public methods

    internal ImportVouchersResult DryRunImport() {
      ToImportVouchersList toImportVouchersList = GetVouchersToBeImported();

      var voucherImporter = new VoucherImporter(_command, toImportVouchersList);

      return voucherImporter.DryRunImport();
    }


    internal ImportVouchersResult Import() {
      ToImportVouchersList standardStructure = GetVouchersToBeImported();

      var voucherImporter = new VoucherImporter(_command, standardStructure);

      return voucherImporter.Import();
    }


    #endregion Public methods


    private ToImportVouchersList GetVouchersToBeImported() {
      FixedList<Encabezado> encabezados = DbVouchersImporterDataService.GetEncabezados();
      FixedList<Movimiento> movimientos = DbVouchersImporterDataService.GetMovimientos();

      var structurer = new DbVouchersStructurer(encabezados, movimientos);

      return structurer.GetToImportVouchersList();
    }


  }  // class DbVouchersImporter

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter
