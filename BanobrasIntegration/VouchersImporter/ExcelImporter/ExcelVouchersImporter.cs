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


    internal ImportVouchersResult Import() {
      VoucherImporter voucherImporter = GetVoucherImporter();

      return voucherImporter.Import();
    }


    private VoucherImporter GetVoucherImporter() {
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

      AccountsChart accountsChart = _command.GetAccountsChart();

      EnsureEntriesHaveRequestedAccountsChart(allEntries, accountsChart);

      return allEntries.ToFixedList();
    }


    private void EnsureEntriesHaveRequestedAccountsChart(List<ExcelVoucherEntry> entries, AccountsChart accountsChart) {
      Assertion.Assert(entries.TrueForAll(x => x.AccountsChart.Equals(accountsChart)),
        $"El archivo Excel tiene pólizas o movimientos que no coresponden al catálogo de cuentas {accountsChart.Name}.");
    }

  }  // class ExcelVouchersImporter

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter
