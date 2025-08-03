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
      get; internal set;
    }

    public FixedList<CashTransactionEntryDto> Entries {
      get; internal set;
    }

  }  // class CashTransactionHolderDto


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

    public string Status {
      get; internal set;
    }

    public string StatusName {
      get; internal set;
    }

  }  // class CashTransactionDescriptor


  /// <summary>Output DTO used to retrieve cash ledger transaction entries.</summary>
  public class CashTransactionEntryDto {

    public long Id {
      get; internal set;
    }

    public string AccountNumber {
      get; internal set;
    }

    public string AccountName {
      get; internal set;
    }

    public string ParentAccountFullName {
      get; internal set;
    }

    public string SectorCode {
      get; internal set;
    }

    public string SubledgerAccountNumber {
      get; internal set;
    }

    public string SubledgerAccountName {
      get; internal set;
    }

    public string VerificationNumber {
      get; internal set;
    }

    public string ResponsibilityAreaCode {
      get; internal set;
    }

    public string ResponsibilityAreaName {
      get; internal set;
    }

    public string BudgetCode {
      get; internal set;
    }

    public string Description {
      get; internal set;
    }

    public DateTime Date {
      get; internal set;
    }

    public int CurrencyId {
      get; internal set;
    }

    public string CurrencyName {
      get; internal set;
    }

    public decimal ExchangeRate {
      get; internal set;
    }

    public decimal Debit {
      get; internal set;
    }

    public decimal Credit {
      get; internal set;
    }

    public int CashAccountId {
      get; internal set;
    }

  }  // class CashTransactionEntryDto

}  // namespace Empiria.FinancialAccounting.CashLedger.Adapters
