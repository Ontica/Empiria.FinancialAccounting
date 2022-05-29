/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                 Component : Vouchers Importer                    *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll   Pattern   : Service provider                     *
*  Type     : DbVoucherImporterEngine                       License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Provides services used to control the database vouchers importation engine.                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter.Adapters;

namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter.UseCases {

  /// <summary>Provides services used to control the database vouchers importation engine.</summary>
  public class DbVoucherImporterEngine : Service {

    #region Constructors and parsers

    protected DbVoucherImporterEngine() {
      // no-op
    }

    static public DbVoucherImporterEngine ServiceInteractor() {
      return Service.CreateInstance<DbVoucherImporterEngine>();
    }

    #endregion Constructors and parsers

    #region Importation engine methods

    public ImportVouchersResult Status() {
      return DbVouchersImporter.Instance.GetImportVouchersResult();
    }


    public ImportVouchersResult Start(ImportVouchersCommand command) {
      Assertion.Require(command, "command");

      var importer = DbVouchersImporter.Instance;

      Assertion.Require(!importer.IsRunning, "El importador de pólizas ya está en ejecución.");

      importer.Start(command)
              .ConfigureAwait(false);

      return importer.GetImportVouchersResult();
    }


    public ImportVouchersResult Stop() {
      var importer = DbVouchersImporter.Instance;

      Assertion.Require(importer.IsRunning, "El importador de pólizas no está en ejecución.");

      importer.Stop();

      return importer.GetImportVouchersResult();
    }


    #endregion Importation engine methods

  }  // class DbVoucherImporterEngine

}  // Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter.UseCases
