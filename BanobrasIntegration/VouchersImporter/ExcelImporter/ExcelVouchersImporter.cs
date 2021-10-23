/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                 Component : Vouchers Importer                    *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll   Pattern   : Command Controller                   *
*  Type     : ExcelVouchersImporter                         License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Provides services to import vouchers using predefined Excel files.                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.IO;

using Empiria.FinancialAccounting.Vouchers;

using Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter.Adapters;

namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter {

  /// <summary>Provides services to import vouchers using predefined Excel files.</summary>
  internal class ExcelVouchersImporter {

    private readonly ImportVouchersCommand _command;
    private readonly FixedList<ExcelVoucherEntry> _excelFileEntries;

    internal ExcelVouchersImporter(ImportVouchersCommand command, FileInfo excelFile) {
      Assertion.AssertObject(command, "command");
      Assertion.AssertObject(excelFile, "excelFile");

      _command = command;

      _excelFileEntries = ReadExcelContent(excelFile);
    }


    internal ImportVouchersResult DryRunImport() {
      VoucherImporter voucherImporter = GetVoucherImporter();

      return voucherImporter.DryRunImport();
    }


    internal ImportVouchersResult DryRunImport(Voucher voucher) {
      VoucherImporter voucherImporter = GetVoucherImporter(voucher);

      return voucherImporter.DryRunImport();
    }


    internal ImportVouchersResult Import() {
      VoucherImporter voucherImporter = GetVoucherImporter();

      return voucherImporter.Import();
    }


    internal ImportVouchersResult Import(Voucher voucher) {
      VoucherImporter voucherImporter = GetVoucherImporter(voucher);

      return voucherImporter.Import();
    }


    private VoucherImporter GetVoucherImporter() {
      var structurer = new ExcelFileStructurer(_command, _excelFileEntries);

      FixedList<ToImportVoucher> toImport = structurer.GetToImportVouchersList();

      return new VoucherImporter(_command, toImport);
    }


    private VoucherImporter GetVoucherImporter(Voucher voucher) {
      var structurer = new ExcelFileStructurer(_command, _excelFileEntries);

      FixedList<ToImportVoucher> toImport = structurer.GetToImportVouchersList();

      return new VoucherImporter(_command, toImport);
    }


    private FixedList<ExcelVoucherEntry> ReadExcelContent(FileInfo excelFileInfo) {
      var excelFile = new ExcelFileHandler(excelFileInfo);

      var allEntries = new List<ExcelVoucherEntry>();

      string[] worksheets = excelFile.SelectCandidateWorksheets();

      Assertion.Assert(worksheets.Length > 0,
                      "El archivo Excel NO tiene información de pólizas para importar en ninguna de sus hojas");

      foreach (var worksheet in worksheets) {
        FixedList<ExcelVoucherEntry> worksheetEntries = excelFile.GetWorksheetEntries(worksheet);

        allEntries.AddRange(worksheetEntries);
      }

      return allEntries.ToFixedList();
    }

  }  // class ExcelVouchersImporter

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter
