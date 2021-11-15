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

using Empiria.FinancialAccounting.Vouchers;

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


    #region Standard voucher importation

    public ImportVouchersResult DryRunStandardVoucherImportation(VoucherImportationCommand command) {
      Assertion.AssertObject(command, "command");

      var importer = new StandardVoucherImporter(command);

      return importer.DryRunImport();
    }

    public ImportVouchersResult StandardVoucherImportation(VoucherImportationCommand command) {
      Assertion.AssertObject(command, "command");

      var importer = new StandardVoucherImporter(command);

      return importer.Import();
    }

    #endregion Standard voucher importation


    #region InterfazUnica structure importation

    public ImportVouchersResult ImportVouchersFromInterfazUnica(InterfazUnicaImporterCommand command,
                                                                bool dryRun) {
      Assertion.AssertObject(command, "command");

      var importer = new InterfazUnicaImporter(command);

      if (dryRun) {
        return importer.DryRunImport();
      } else {
        return importer.Import();
      }
    }

    #endregion InterfazUnica structure importation


    #region Database importers

    public ImportVouchersResult ImportVouchersFromDatabase(ImportVouchersCommand command) {
      Assertion.AssertObject(command, "command");

      var importer = DbVouchersImporter.Instance;

      if (importer.IsRunning) {
        Assertion.AssertFail("El importador de pólizas ya está en ejecución.");
      }

      importer.Start(command)
              .ConfigureAwait(false);

      return importer.GetImportVouchersResult();
    }


    public ImportVouchersResult StatusOfImportVouchersFromDatabase() {
      return DbVouchersImporter.Instance.GetImportVouchersResult();
    }


    public ImportVouchersResult StopImportVouchersFromDatabase() {
      var importer = DbVouchersImporter.Instance;

      if (!importer.IsRunning) {
        Assertion.AssertFail("El importador de pólizas no está en ejecución.");
      }

      importer.Stop();

      return importer.GetImportVouchersResult();
    }


    #endregion Database importers

    #region Excel importers

    public ImportVouchersResult DryRunImportVouchersFromExcelFile(ImportVouchersCommand command,
                                                                  FileData excelFileData) {
      FileInfo excelFile = AssertParametersAreValidAndGetFileInfo(command, excelFileData);

      PrepareCommandForImportTextFile(command);

      var importer = new ExcelVouchersImporter(command, excelFile);

      ImportVouchersResult result = importer.DryRunImport();

      return result;
    }


    public ImportVouchersResult DryRunImportVoucherEntriesFromExcelFile(int voucherId,
                                                                        ImportVouchersCommand command,
                                                                        FileData excelFileData) {
      FileInfo excelFile = AssertParametersAreValidAndGetFileInfo(command, excelFileData);

      PrepareCommandForImportTextFile(command);

      var voucher = Voucher.Parse(voucherId);

      var importer = new ExcelVouchersImporter(command, excelFile);

      return importer.DryRunImport(voucher);
    }


    public ImportVouchersResult ImportVouchersFromExcelFile(ImportVouchersCommand command, FileData excelFileData) {
      FileInfo excelFile = AssertParametersAreValidAndGetFileInfo(command, excelFileData);

      PrepareCommandForImportTextFile(command);

      var importer = new ExcelVouchersImporter(command, excelFile);

      return importer.Import();
    }


    public ImportVouchersResult ImportVoucherEntriesFromExcelFile(int voucherId, ImportVouchersCommand command, FileData excelFileData) {
      FileInfo excelFile = AssertParametersAreValidAndGetFileInfo(command, excelFileData);

      PrepareCommandForImportTextFile(command);

      var voucher = Voucher.Parse(voucherId);

      var importer = new ExcelVouchersImporter(command, excelFile);

      return importer.Import(voucher);
    }


    #endregion Excel importers

    #region Text file importers

    public ImportVouchersResult DryRunImportVouchersFromTextFile(ImportVouchersCommand command, FileData textFileData) {
      FileInfo textFile = AssertParametersAreValidAndGetFileInfo(command, textFileData);

      PrepareCommandForImportTextFile(command);

      var importer = new TextFileImporter(command, textFile);

      return importer.DryRunImport();
    }


    public ImportVouchersResult ImportVouchersFromTextFile(ImportVouchersCommand command,
                                                           FileData textFileData) {
      FileInfo textFile = AssertParametersAreValidAndGetFileInfo(command, textFileData);

      PrepareCommandForImportTextFile(command);

      var importer = new TextFileImporter(command, textFile);

      return importer.Import();
    }


    #endregion Text file importers

    #region Helpers

    private FileInfo AssertParametersAreValidAndGetFileInfo(ImportVouchersCommand command,
                                                            FileData textFileData) {
      Assertion.AssertObject(command, "command");
      Assertion.AssertObject(textFileData, "textFileData");

      return FileUtilities.SaveFile(textFileData);
    }


    private void PrepareCommandForImportTextFile(ImportVouchersCommand command) {
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
