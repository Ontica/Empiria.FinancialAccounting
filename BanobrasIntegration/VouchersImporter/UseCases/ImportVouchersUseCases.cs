/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                 Component : Vouchers Importer                    *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll   Pattern   : Use case interactor class            *
*  Type     : ImportVouchersUseCases                        License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Use cases used for import vouchers and voucher entries from Databases, Excel and text files.   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.IO;

using Empiria.Services;
using Empiria.Storage;

using Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter.Adapters;

namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter.UseCases {

  /// <summary>Use cases used for import vouchers and voucher entries from Databases,
  /// Excel and text files.</summary>
  public class ImportVouchersUseCases : UseCase {

    #region Constructors and parsers

    protected ImportVouchersUseCases() {
      // no-op
    }

    static public ImportVouchersUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<ImportVouchersUseCases>();
    }

    #endregion Constructors and parsers

    #region Importers

    public ImportVouchersResult ImportVouchersFromExcelFile(ImportVouchersCommand command,
                                                            InputFile excelFileData, bool dryRun) {
      FileInfo excelFile = AssertParametersAreValidAndGetFileInfo(command, excelFileData);

      PrepareCommandForFileImportation(command);

      var importer = new ExcelVouchersImporter(command, excelFile);

      if (dryRun) {
        return importer.DryRunImport();
      } else {
        return importer.Import();
      }
    }


    public ImportVouchersResult ImportVouchersFromInterfazUnica(InterfazUnicaImporterCommand command,
                                                                bool dryRun) {
      Assertion.Require(command, "command");

      var importer = new InterfazUnicaImporter(command);

      if (dryRun) {
        return importer.DryRunImport();
      } else {
        return importer.Import();
      }
    }


    public ImportVouchersResult ImportVouchersFromTextFile(ImportVouchersCommand command,
                                                           InputFile textFileData, bool dryRun) {
      FileInfo textFile = AssertParametersAreValidAndGetFileInfo(command, textFileData);

      PrepareCommandForFileImportation(command);

      var importer = new TextFileImporter(command, textFile);

      if (dryRun) {
        return importer.DryRunImport();
      } else {
        return importer.Import();
      }
    }


    #endregion Importers

    #region Helpers

    private FileInfo AssertParametersAreValidAndGetFileInfo(ImportVouchersCommand command,
                                                            InputFile fileData) {
      Assertion.Require(command, "command");
      Assertion.Require(fileData, "fileData");

      return FileUtilities.SaveFile(fileData);
    }


    private void PrepareCommandForFileImportation(ImportVouchersCommand command) {
      if (command.TransactionTypeUID.Length == 0) {
        command.TransactionTypeUID = "d7b175e7-33e8-4abf-8554-dab648af9384";
      }
      if (command.VoucherTypeUID.Length == 0) {
        command.VoucherTypeUID = "32279ad5-ad9f-46ff-80d8-2463366e3b7a";
      }
    }


    #endregion Helpers

  }  // class ImportVouchersUseCases

}  // Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter.UseCases
