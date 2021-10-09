/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                 Component : Vouchers Importer                    *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll   Pattern   : Information holder                   *
*  Type     : StandardVoucherHeader                         License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Holds the header of a voucher to be imported.                                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.Vouchers;

namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter {

  /// <summary>Holds the header of a voucher to be imported.</summary>
  internal class ToImportVoucherHeader {

    public string ImportationSet {
      get; internal set;
    }


    public string UniqueID {
      get; internal set;
    }


    public Ledger Ledger {
      get; internal set;
    }


    public string Concept {
      get; internal set;
    }


    public DateTime AccountingDate {
      get; internal set;
    }


    public DateTime RecordingDate {
      get; internal set;
    }


    public VoucherType VoucherType {
      get; internal set;
    }


    public TransactionType TransactionType {
      get; internal set;
    }


    public FunctionalArea FunctionalArea {
      get; internal set;
    }


    public Participant ElaboratedBy {
      get; internal set;
    }

    public FixedList<ToImportVoucherIssue> Issues {
      get; internal set;
    }

  }  // class StandardVoucherHeader

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter
