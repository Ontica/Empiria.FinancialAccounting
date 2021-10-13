/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                 Component : Vouchers Importer                    *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll   Pattern   : Structurer                           *
*  Type     : ToImportVoucher                               License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Holds data for a voucher and its entries to be imported, regardless of its original source.    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Linq;

using System.Collections.Generic;

namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter {

  /// <summary>Holds data for a voucher and its entries to be imported,
  /// regardless of its original source.</summary>
  internal class ToImportVoucher {

    private readonly List<ToImportVoucherIssue> _issuesList = new List<ToImportVoucherIssue>();

    internal ToImportVoucher(ToImportVoucherHeader header,
                             FixedList<ToImportVoucherEntry> entries) {
      Assertion.AssertObject(header, "header");
      Assertion.AssertObject(entries, "entries");

      this.Header = header;
      this.Entries = entries;
    }


    public ToImportVoucherHeader Header {
      get;
    }


    public FixedList<ToImportVoucherEntry> Entries {
      get;
    }


    public FixedList<ToImportVoucherIssue> Issues {
      get {
        return _issuesList.ToFixedList();
      }
    }


    public bool HasErrors {
      get {
        return _issuesList.Exists(x => x.Type == VoucherIssueType.Error) ||
               Header.Issues.Exists(x => x.Type == VoucherIssueType.Error) ||
               Entries.Exists(x => x.Issues.Exists(y => y.Type == VoucherIssueType.Error));
      }
    }


    internal void AddIssue(VoucherIssueType type, string description) {
      _issuesList.Add(new ToImportVoucherIssue(type, description));
    }

  }  // class ToImportVoucher

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter
