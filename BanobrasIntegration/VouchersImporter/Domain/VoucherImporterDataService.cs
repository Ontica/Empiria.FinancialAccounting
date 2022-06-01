/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                 Component : Vouchers Importer                    *
*  Assembly : FinancialAccounting.BalanceEngine.dll         Pattern   : Data Service                         *
*  Type     : DbVouchersImporterDataService                 License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Data methods used to read and write data for vouchers importation data tables.                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter {

  /// <summary>Data methods used to read and write data for vouchers importation data tables.</summary>
  static internal class VoucherImporterDataService {

    static internal void StoreVoucher(ToImportVoucher voucher) {
      // no-op
    }

    static internal void StoreVoucherIssues(ToImportVoucher voucher) {
      foreach (var issue in voucher.AllIssues) {
        StoreVoucherIssues(voucher, issue, 'V');
      }

      foreach (var issue in voucher.Header.Issues) {
        StoreVoucherIssues(voucher, issue, 'H');
      }

      foreach (var entry in voucher.Entries) {
        foreach (var issue in entry.Issues) {
          StoreVoucherIssues(voucher, issue, 'E');
        }
      }
    }


    static private void StoreVoucherIssues(ToImportVoucher voucher,
                                           ToImportVoucherIssue issue,
                                           char issueType) {
      EmpiriaLog.Info(issue.Description);
    }

  }  // class DbVouchersImporterDataService

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter
