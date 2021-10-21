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

      this.Validate();
    }


    public ToImportVoucherHeader Header {
      get;
    }


    public FixedList<ToImportVoucherEntry> Entries {
      get;
    }


    public FixedList<ToImportVoucherIssue> AllIssues {
      get {
        var issues = new List<ToImportVoucherIssue>();

        issues.AddRange(_issuesList);

        issues.AddRange(this.Header.Issues);

        foreach (var item in this.Entries) {
          issues.AddRange(item.Issues);
        }

        return issues.ToFixedList();
      }
    }


    public bool HasErrors {
      get {
        return _issuesList.Exists(x => x.Type == VoucherIssueType.Error) ||
               Header.Issues.Exists(x => x.Type == VoucherIssueType.Error) ||
               Entries.Exists(x => x.Issues.Exists(y => y.Type == VoucherIssueType.Error));
      }
    }


    public FixedList<ToImportVoucherIssue> Issues {
      get {
        return _issuesList.ToFixedList();
      }
    }


    internal void AddError(string description) {
      _issuesList.Add(new ToImportVoucherIssue(VoucherIssueType.Error,
                                               this.Header.ImportationSet,
                                               description));
    }


    internal void AddWarning(string description) {
      _issuesList.Add(new ToImportVoucherIssue(VoucherIssueType.Warning,
                                               this.Header.ImportationSet,
                                               description));
    }


    internal void Validate() {
      if (this.Entries.Count < 2) {
        AddError("La póliza no tiene movimientos suficientes.");
      }
    }

  }  // class ToImportVoucher

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter
