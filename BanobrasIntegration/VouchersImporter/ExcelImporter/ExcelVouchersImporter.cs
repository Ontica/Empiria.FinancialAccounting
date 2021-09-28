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
using System.IO;

using Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter.Adapters;
using Empiria.FinancialAccounting.Vouchers;
using Empiria.FinancialAccounting.Vouchers.Adapters;
using Empiria.FinancialAccounting.Vouchers.UseCases;

namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter {

  /// <summary>Provides services to import vouchers using predefined Excel files.</summary>
  internal class ExcelVouchersImporter {

    private readonly ImportVouchersCommand _command;
    private readonly FileInfo _excelFile;

    internal ExcelVouchersImporter(ImportVouchersCommand command, FileInfo excelFile) {
      Assertion.AssertObject(command, "command");
      Assertion.AssertObject(excelFile, "excelFile");

      _command = command;
      _excelFile = excelFile;
    }


    internal ImportVouchersResult DryRunImport() {
      return ImportVouchersResult.Default;
    }


    internal ImportVouchersResult DryRunImport(Voucher voucher) {
      return ImportVouchersResult.Default;
    }


    internal ImportVouchersResult Import() {
      return ImportVouchersResult.Default;
    }


    internal ImportVouchersResult Import(Voucher voucher) {
      try {
        var importer = new ExcelFileStructurer(voucher, _excelFile);

        FixedList<VoucherEntryFields> entries = importer.GetVoucherEntries();

        using (var usecases = VoucherEditionUseCases.UseCaseInteractor()) {
          usecases.AppendEntries(voucher.Id, entries);
        }

      } catch (Exception e) {
        EmpiriaLog.Error(e);
        throw;
      }

      return ImportVouchersResult.Default;
    }

  }  // class ExcelVouchersImporter

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter
