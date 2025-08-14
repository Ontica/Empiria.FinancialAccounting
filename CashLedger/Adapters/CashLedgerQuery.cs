/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Cash Ledger                                   Component : Adapters Layer                       *
*  Assembly : FinancialAccounting.CashLedger.dll            Pattern   : Query DTO                            *
*  Type     : CashLedgerQuery                               License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Input query DTO used to retrieve cash ledger transactions.                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System;

using Empiria.StateEnums;

namespace Empiria.FinancialAccounting.CashLedger.Adapters {

  public enum CashAccountStatus {

    CashAccountWaiting = -2,

    NoCashAccount = -1,

    CashAccountPending = 0,

    WithCashAccount = 1,

    All = 255

  }

  /// <summary>Input query DTO used to retrieve cash ledger transactions.</summary>
  public class CashLedgerQuery {

    public DateTime FromAccountingDate {
      get; set;
    } = ExecutionServer.DateMinValue;


    public DateTime ToAccountingDate {
      get; set;
    } = ExecutionServer.DateMaxValue;


    public TransactionStatus TransactionStatus {
      get; set;
    } = TransactionStatus.All;


    public CashAccountStatus CashAccountStatus {
      get; set;
    } = CashAccountStatus.All;


    public string Keywords {
      get; set;
    } = string.Empty;


    public string Concept {
      get; set;
    } = string.Empty;


    public string AccountingLedgerUID {
      get; set;
    } = string.Empty;


    public string[] CashAccounts {
      get; set;
    } = new string[0];


    public DateTime FromRecordingDate {
      get; set;
    } = ExecutionServer.DateMinValue;


    public DateTime ToRecordingDate {
      get; set;
    } = ExecutionServer.DateMaxValue;


    public string[] VoucherAccounts {
      get; set;
    } = new string[0];


    public string[] SubledgerAccounts {
      get;
      set;
    } = new string[0];


    public string[] VerificationNumbers {
      get; set;
    } = new string[0];


    public string TransactionTypeUID {
      get; set;
    } = string.Empty;


    public string VoucherTypeUID {
      get; set;
    } = string.Empty;


    public string SourceUID {
      get; set;
    } = string.Empty;


    public string OrderBy {
      get; set;
    } = string.Empty;


    public int PageSize {
      get; set;
    } = 10000;


    public string PartyUID {
      get; set;
    } = string.Empty;


    public string ProjectUID {
      get; set;
    } = string.Empty;


    [Newtonsoft.Json.JsonProperty(PropertyName = "ProjectTypeUID")]
    public string ProjectTypeCategoryUID {
      get; set;
    } = string.Empty;


    public string ProjectAccountUID {
      get; set;
    } = string.Empty;


    public string EntriesKeywords {
      get; set;
    } = string.Empty;

  }  // class CashLedgerQuery

}  // namespace Empiria.FinancialAccounting.CashLedger.Adapters
