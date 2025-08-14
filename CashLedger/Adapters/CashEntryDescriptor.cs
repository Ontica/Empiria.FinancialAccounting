/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Cash Ledger                                   Component : Adapters Layer                       *
*  Assembly : FinancialAccounting.CashLedger.dll            Pattern   : Output DTO                           *
*  Type     : CashEntryDescriptor                           License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Output DTO used to retrieve cash ledger transaction entries for use in lists.                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System;

namespace Empiria.FinancialAccounting.CashLedger.Adapters {

  /// <summary>Output DTO used to retrieve cash ledger transaction entries for use in lists.</summary>
  public class CashEntryDescriptor {

    public long Id {
      get; set;
    }

    public string AccountNumber {
      get; set;
    }

    public string AccountName {
      get; set;
    }

    public string ParentAccountFullName {
      get; set;
    }

    public string SectorCode {
      get; set;
    }

    public string SubledgerAccountNumber {
      get; set;
    }

    public string SubledgerAccountName {
      get; set;
    }

    public string VerificationNumber {
      get; set;
    }

    public string ResponsibilityAreaCode {
      get; set;
    }

    public string ResponsibilityAreaName {
      get; set;
    }

    public string BudgetCode {
      get; set;
    }

    public string Description {
      get; set;
    }

    public DateTime Date {
      get; set;
    }

    public int CurrencyId {
      get; set;
    }

    public string CurrencyName {
      get; set;
    }

    public decimal ExchangeRate {
      get; set;
    }

    public decimal Debit {
      get; set;
    }

    public decimal Credit {
      get; set;
    }

    public int CashAccountId {
      get; set;
    }

    public long TransactionId {
      get; set;
    }

    public string TransactionNumber {
      get; set;
    }

    public string TransactionLedgerName {
      get; set;
    }

    public string TransactionConcept {
      get; set;
    }

    public DateTime TransactionAccountingDate {
      get; set;
    }

    public DateTime TransactionRecordingDate {
      get; set;
    }

  }  // class CashEntryDescriptor

}  // namespace Empiria.FinancialAccounting.CashLedger.Adapters
