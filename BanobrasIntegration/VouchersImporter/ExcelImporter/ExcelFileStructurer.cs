/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                Component : Vouchers Importer                     *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll  Pattern   : Structurer                            *
*  Type     : ExcelFileStructurer                          License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Provides information structure services for vouchers contained in Excel Files.                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.IO;

using Empiria.Office;

using Empiria.FinancialAccounting.Vouchers;
using Empiria.FinancialAccounting.Vouchers.Adapters;
using System.Collections.Generic;

namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter {

  /// <summary>Provides information structure services for vouchers contained in Excel Files.</summary>
  internal class ExcelFileStructurer {

    private readonly Voucher _voucher;
    private readonly Spreadsheet _excel;

    internal ExcelFileStructurer(Voucher voucher, FileInfo excelFile) {
      Assertion.AssertObject(voucher, "voucher");
      Assertion.AssertObject(excelFile, "excelFile");

      _voucher = voucher;
      _excel = Spreadsheet.Open(excelFile.FullName);
    }


    internal FixedList<VoucherEntryFields> GetVoucherEntries() {
      string worksheetName = TryGetDataWorksheet();

      if (worksheetName == null) {
        Assertion.AssertFail("No encontré ninguna hoja en el archivo Excel con " +
                             "información de movimientos de pólizas.");
      }

      FixedList<VoucherEntryFields> entries = ParseVoucherEntries(worksheetName);

      return entries;
    }


    internal ExcelVoucherEntry ProcessExcelRow(Spreadsheet _excel, int row) {
      var sourceData = new ExcelToImportVoucherEntry();

      sourceData.BaseAccount = _excel.ReadCellValue<string>($"A{row}");
      sourceData.Subaccount = _excel.ReadCellValue<string>($"B{row}");
      sourceData.CurrencyCode = _excel.ReadCellValue<string>($"C{row}");
      sourceData.Area = _excel.ReadCellValue<string>($"D{row}");
      sourceData.Debit = _excel.ReadCellValue<decimal>($"E{row}", 0);
      sourceData.Credit = _excel.ReadCellValue<decimal>($"F{row}", 0);
      sourceData.SubledgerAccount = _excel.ReadCellValue<string>($"G{row}", string.Empty);
      sourceData.ExchangeRate = _excel.ReadCellValue<decimal>($"H{row}", 0);

      return new ExcelVoucherEntry(_voucher, sourceData);
    }

    private FixedList<VoucherEntryFields> ParseVoucherEntries(string worksheetName) {
      _excel.SelectWorksheet(worksheetName);

      List<VoucherEntryFields> entriesList = new List<VoucherEntryFields>(16);

      int row = 2;
      while (true) {
        if (_excel.HasNotCellValue($"A{row}")) {
          break;
        }

        ExcelVoucherEntry excelRow = ProcessExcelRow(_excel, row);

        VoucherEntryFields entry = excelRow.MapToVoucherEntryFields();

        entriesList.Add(entry);

        row++;
      }

      return entriesList.ToFixedList();
    }


    private string TryGetDataWorksheet() {
      foreach (var worksheetName in _excel.Worksheets()) {
        if (WorksheetHasEntriesData(worksheetName)) {
          return worksheetName;
        }
      }
      return null;
    }

    private bool WorksheetHasEntriesData(string worksheetName) {
      _excel.SelectWorksheet(worksheetName);

      int row = 2;

      if (_excel.HasNotCellValue($"A{row}")) {
        return false;
      }
      if (_excel.HasNotCellValue($"B{row}")) {
        return false;
      }
      if (_excel.HasNotCellValue($"C{row}")) {
        return false;
      }
      if (_excel.HasNotCellValue($"D{row}")) {
        return false;
      }

      return true;
    }

  }  // class VoucherEntriesFileImporter

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter
