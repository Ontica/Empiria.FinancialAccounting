/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                 Component : Vouchers Importer                    *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll   Pattern   : Command Controller                   *
*  Type     : ExcelFileHandler                              License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Provides read services for vouchers data imported from Excel files.                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.IO;

using Empiria.Office;

namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter {

  internal class ExcelFileHandler {

    private readonly Spreadsheet _excelFile;

    public ExcelFileHandler(FileInfo excelFileInfo) {
      _excelFile = Spreadsheet.Open(excelFileInfo.FullName);
    }


    #region Public methods

    public void Close() {
      _excelFile.Close();
    }


    public FixedList<ExcelVoucherEntry> GetWorksheetEntries(string worksheetName) {
      Assertion.Require(worksheetName, "worksheetName");

      _excelFile.SelectWorksheet(worksheetName);

      List<ExcelVoucherEntry> entriesList = new List<ExcelVoucherEntry>(32);

      int endRow = VoucherDataEndRow();

      int worksheetSection = 1;

      string voucherConcept = String.Empty;

      for (int currentRow = 1; currentRow <= endRow; currentRow++) {

        if (IsHeaderRow(currentRow)) {
          worksheetSection = currentRow;
          voucherConcept = GetVoucherConcept(currentRow);

        } else if (IsVoucherEntryRow(currentRow)) {
          ExcelVoucherEntry entry = TryGetExcelRowVoucherEntry(worksheetName, worksheetSection,
                                                               currentRow, voucherConcept);

          if (entry != null) {
            entriesList.Add(entry);
          }

        } else {
          // no-op

        }
      }
      return entriesList.ToFixedList();
    }


    public string[] SelectCandidateWorksheets() {
      var worksheets = new List<string>();

      foreach (var worksheetName in _excelFile.Worksheets()) {
        if (_excelFile.IsWorksheetHidden(worksheetName)) {
          continue;
        }

        _excelFile.SelectWorksheet(worksheetName);

        if (IsCandidateWorksheet()) {
          worksheets.Add(worksheetName);
        }
      }
      return worksheets.ToArray();
    }

    #endregion Public methods


    #region Private methods

    private string GetVoucherConcept(int row) {
      if (_excelFile.IsNotEmpty($"H{row}")) {
        return _excelFile.ReadCellValue<string>($"H{row}");
      }
      if (_excelFile.IsNotEmpty($"I{row}")) {
        return _excelFile.ReadCellValue<string>($"I{row}");
      }
      if (_excelFile.IsNotEmpty($"J{row}")) {
        return _excelFile.ReadCellValue<string>($"J{row}");
      }
      return "PÓLIZA IMPORTADA DE EXCEL SIN CONCEPTO";
    }


    private int VoucherDataEndRow() {
      int nextRowToCheck = 1;

      while (true) {
        if (IsVoucherEntryRow(nextRowToCheck)) {
          nextRowToCheck = nextRowToCheck + 1;
        } else if (IsVoucherEntryRow(nextRowToCheck + 1)) {
          nextRowToCheck = nextRowToCheck + 2;
        } else if (IsVoucherEntryRow(nextRowToCheck + 2)) {
          nextRowToCheck = nextRowToCheck + 3;
        } else {
          break;
        }
      }
      return nextRowToCheck - 1;
    }


    private ExcelVoucherEntry TryGetExcelRowVoucherEntry(string worksheetName, int worksheetSection,
                                                         int row, string concept) {

      decimal debit = _excelFile.ReadCellValue<decimal>($"E{row}", 0);
      decimal credit = _excelFile.ReadCellValue<decimal>($"F{row}", 0);

      debit = Math.Round(debit, 2);
      credit = Math.Round(credit, 2);

      if (debit == 0 && credit == 0) {    // skip rows with zeros
        return null;
      }

      var entry = new ExcelVoucherEntry(worksheetName, worksheetSection, row);

      entry.SetConcept(concept);
      entry.SetBaseAccount(_excelFile.ReadCellValue<string>($"A{row}"));
      entry.SetSubaccountWithSector(_excelFile.ReadCellValue<string>($"B{row}"));
      entry.SetCurrencyCode(_excelFile.ReadCellValue<string>($"C{row}"));
      entry.SetResponsibilityAreaCode(_excelFile.ReadCellValue<string>($"D{row}"));
      entry.SetDebitOrCredit(debit, credit);
      entry.SetSubledgerAccount(_excelFile.ReadCellValue<string>($"G{row}", string.Empty));
      entry.SetExchangeRate(_excelFile.ReadCellValue<decimal>($"H{row}", 1));
      // entry.SetEventCode(_excelFile.ReadCellValue<int>($"I{row}", 0)); TODO Enabled for SIC System Créditos

      return entry;
    }


    private bool IsCandidateWorksheet() {
      return this.VoucherDataEndRow() > 2;
    }


    private bool IsVoucherEntryRow(int row) {
      if (row == 1) {
        return false;
      }

      if (_excelFile.IsEmpty($"A{row}", true) ||
          _excelFile.IsEmpty($"B{row}", true) ||
          _excelFile.IsEmpty($"C{row}", true) ||
          _excelFile.IsEmpty($"D{row}", true)) {
        return false;
      }

      // Cuenta de mayor
      string cellValue = _excelFile.ReadCellValue<string>($"A{row}");

      if (cellValue.Length != 4 && cellValue.Length != 1) {
        return false;
      }

      // Subcuenta rellena con ceros y con clave de sector, sin separadores
      cellValue = _excelFile.ReadCellValue<string>($"B{row}");

      if (cellValue.Length != 14 && cellValue.Length != 22) {
        return false;
      }

      // Clave de la moneda en dos posiciones
      cellValue = _excelFile.ReadCellValue<string>($"C{row}");

      if (cellValue.Length != 2) {
        return false;
      }

      // Clave del mayor en seis posiciones
      cellValue = _excelFile.ReadCellValue<string>($"D{row}");

      if (cellValue.Length != 6) {
        return false;
      }

      return true;
    }


    private bool IsHeaderRow(int row) {
      if (row == 1) {
        return true;
      }

      if (_excelFile.IsEmpty($"A{row}", true) &&
          _excelFile.IsEmpty($"B{row}", true) &&
          _excelFile.IsEmpty($"C{row}", true) &&
          _excelFile.IsEmpty($"D{row}", true) &&
          _excelFile.IsEmpty($"E{row}", true) &&
          _excelFile.IsEmpty($"F{row}", true) &&
          _excelFile.IsEmpty($"G{row}", true) &&
          _excelFile.IsNotEmpty($"H{row}")) {
        return true;
      }

      return false;
    }

    #endregion Private methods

  }

} // namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter
