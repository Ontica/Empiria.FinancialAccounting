/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Cash Ledger                                   Component : Adapters Layer                       *
*  Assembly : FinancialAccounting.CashLedger.dll            Pattern   : Output DTO                           *
*  Type     : CashTransactionHolder, Descriptor, Entry      License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Output DTO used to retrieve cash ledger transactions.                                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System;

namespace Empiria.FinancialAccounting.CashLedger.Adapters {


  /// <summary>Output holder DTO used for a cash transaction.</summary>
  public class CashTransactionHolderDto {

    public CashTransactionDescriptor Transaction {
      get; set;
    }

    public FixedList<CashTransactionEntryDto> Entries {
      get; set;
    }

  }  // class CashTransactionHolderDto


  /// <summary>Output DTO used to retrieve cash ledger transactions for use in lists.</summary>
  public class CashTransactionDescriptor {

    public long Id {
      get; set;
    }

    public string Number {
      get; set;
    }

    public string LedgerName {
      get; set;
    }

    public string Concept {
      get; set;
    }

    public string TransactionTypeName {
      get; set;
    }

    public string VoucherTypeName {
      get; set;
    }

    public string SourceName {
      get; set;
    }

    public DateTime AccountingDate {
      get; set;
    }

    public DateTime RecordingDate {
      get; set;
    }

    public string ElaboratedBy {
      get; set;
    }

    public string Status {
      get; set;
    }

    public string StatusName {
      get; set;
    }

  }  // class CashTransactionDescriptor


  /// <summary>Output DTO used to retrieve cash ledger transaction entries.</summary>
  public class CashTransactionEntryDto {

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

  }  // class CashTransactionEntryDto

}  // namespace Empiria.FinancialAccounting.CashLedger.Adapters
