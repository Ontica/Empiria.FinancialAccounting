/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Cash Ledger                                   Component : Adapters Layer                       *
*  Assembly : FinancialAccounting.CashLedger.dll            Pattern   : Output DTO                           *
*  Type     : CashTransactionDescriptor                     License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Output DTO used to retrieve cash ledger transactions for use in lists.                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System;

namespace Empiria.FinancialAccounting.CashLedger.Adapters {

  /// <summary>Output DTO used to retrieve cash ledger transactions for use in lists.</summary>
  public class CashTransactionDescriptor {

    public long Id {
      get; internal set;
    }

    public string Number {
      get; internal set;
    }

    public string LedgerName {
      get; internal set;
    }

    public string Concept {
      get; internal set;
    }

    public string TransactionTypeName {
      get; internal set;
    }

    public string VoucherTypeName {
      get; internal set;
    }

    public string SourceName {
      get; internal set;
    }

    public DateTime AccountingDate {
      get; internal set;
    }

    public DateTime RecordingDate {
      get; internal set;
    }

    public string ElaboratedBy {
      get; internal set;
    }

    public string StatusName {
      get; internal set;
    }

  }  // class CashTransactionDescriptor

}  // namespace Empiria.FinancialAccounting.CashLedger.Adapters
