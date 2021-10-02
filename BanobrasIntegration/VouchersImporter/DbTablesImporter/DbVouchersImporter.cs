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
      FixedList<Encabezado> encabezados = DbVouchersImporterDataService.GetEncabezados();
      FixedList<Movimiento> movimientos = DbVouchersImporterDataService.GetMovimientos();

      var structurer = new DbVouchersStructurer(encabezados, movimientos);

      StandardVouchersStructure standardStructure = structurer.GetStandardStructure();

      var voucherImporter = new StandardVoucherImporter(_command, standardStructure);

      return voucherImporter.DryRunImport();
    }


    internal ImportVouchersResult Import() {
      ImportVouchersResult result = DryRunImport();

      return result;
    }

    #endregion Public methods

  }  // class DbVouchersImporter

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter
