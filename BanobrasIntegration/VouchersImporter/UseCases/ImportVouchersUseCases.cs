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

  /// <summary>Use cases used for import vouchers and voucher entries from Databases, Excel and text files.</summary>
  public class ImportVouchersUseCases : UseCase {

    #region Constructors and parsers

    protected ImportVouchersUseCases() {
      // no-op
    }

    static public ImportVouchersUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<ImportVouchersUseCases>();
    }

    #endregion Constructors and parsers

    #region Database importers

    public ImportVouchersResult DryRunImportVouchersFromDatabase(ImportVouchersCommand command) {
      Assertion.AssertObject(command, "command");

      var importer = new DbVouchersImporter(command);

      return importer.DryRunImport();
    }


    public ImportVouchersResult ImportVouchersFromDatabase(ImportVouchersCommand command) {
      Assertion.AssertObject(command, "command");

      var importer = new DbVouchersImporter(command);

      return importer.Import();
    }

    #endregion Database importers


    #region Excel importers

    public ImportVouchersResult DryRunImportVouchersFromExcelFile(ImportVouchersCommand command,
                                                                  FileData excelFileData) {
      FileInfo excelFile = AssertParametersAreValidAndGetFileInfo(command, excelFileData);

      var importer = new ExcelVouchersImporter(command, excelFile);

      return importer.DryRunImport();
    }


    public ImportVouchersResult DryRunImportVoucherEntriesFromExcelFile(int voucherId,
                                                                        ImportVouchersCommand command,
                                                                        FileData excelFileData) {
      FileInfo excelFile = AssertParametersAreValidAndGetFileInfo(command, excelFileData);

      var voucher = Voucher.Parse(voucherId);

      var importer = new ExcelVouchersImporter(command, excelFile);

      return importer.DryRunImport(voucher);
    }


    public ImportVouchersResult ImportVouchersFromExcelFile(ImportVouchersCommand command, FileData excelFileData) {
      FileInfo excelFile = AssertParametersAreValidAndGetFileInfo(command, excelFileData);

      var importer = new ExcelVouchersImporter(command, excelFile);

      return importer.Import();
    }


    public ImportVouchersResult ImportVoucherEntriesFromExcelFile(int voucherId, ImportVouchersCommand command, FileData excelFileData) {
      FileInfo excelFile = AssertParametersAreValidAndGetFileInfo(command, excelFileData);

      var voucher = Voucher.Parse(voucherId);

      var importer = new ExcelVouchersImporter(command, excelFile);

      return importer.Import(voucher);
    }

    #endregion Excel importers


    #region Text file importers

    public ImportVouchersResult DryRunImportVouchersFromTextFile(ImportVouchersCommand command, FileData textFileData) {
      FileInfo textFile = AssertParametersAreValidAndGetFileInfo(command, textFileData);

      var importer = new TextFileImporter(command, textFile);

      return importer.DryRunImport();
    }


    public ImportVouchersResult ImportVouchersFromTextFile(ImportVouchersCommand command, FileData textFileData) {
      FileInfo textFile = AssertParametersAreValidAndGetFileInfo(command, textFileData);

      var importer = new TextFileImporter(command, textFile);

      return importer.Import();
    }

    #endregion Text file importers

    #region Helpers

    private FileInfo AssertParametersAreValidAndGetFileInfo(ImportVouchersCommand command, FileData textFileData) {
      Assertion.AssertObject(command, "command");
      Assertion.AssertObject(textFileData, "textFileData");

      return FileUtilities.SaveFile(textFileData);
    }

    #endregion Helpers

  }  // class ImportVouchersUseCases

}  // Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter.UseCases
