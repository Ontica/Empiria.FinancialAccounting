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
using System.Linq;

using Empiria.Data;

namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter {

  /// <summary>Data methods used to read and write data for vouchers importation data tables.</summary>
  static internal class VoucherImporterDataService {

    static internal void StoreVoucher(ToImportVoucher voucher) {
      // no-op
    }

    static internal void StoreVoucherIssues(ToImportVoucher voucher) {
      var issuesList = voucher.Issues;

      foreach (var issue in issuesList) {
        StoreVoucherIssues(voucher, issue, 'V');
      }

      issuesList = voucher.Header.Issues;

      foreach (var issue in issuesList) {
        StoreVoucherIssues(voucher, issue, 'H');
      }

      foreach (var entry in voucher.Entries) {
        issuesList = entry.Issues;

        foreach (var issue in issuesList) {
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
