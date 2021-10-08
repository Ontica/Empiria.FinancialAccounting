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

namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter {

  /// <summary>Holds data for a voucher and its entries to be imported,
  /// regardless of its original source.</summary>
  internal class ToImportVoucher {

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
      get;
    }

  }  // class ToImportVoucher

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter
