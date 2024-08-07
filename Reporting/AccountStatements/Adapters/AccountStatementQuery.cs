﻿/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                         Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Reporting.dll          Pattern   : Query payload                           *
*  Type     : AccountStatementQuery                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Query payload used to build account statements.                                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Newtonsoft.Json;

using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.BalanceExplorer.Adapters;

namespace Empiria.FinancialAccounting.Reporting.AccountStatements.Adapters {


  public enum AccountStatementOrder {

    AccountingDate,

    Amount,

    RecordingDate,

    SubledgerAccount,

    VoucherNumber

  }


  public enum AccountStatementOrderType {

    Ascending,

    Descending

  }


  /// <summary>Query payload used to build account statement.</summary>
  public class AccountStatementQuery {

    [JsonProperty("Query")]
    public BalanceExplorerQuery BalancesQuery {
      get; set;
    } = new BalanceExplorerQuery();


    public AccountStatementQueryEntry Entry {
      get; set;
    } = new AccountStatementQueryEntry();


    public AccountStatementSortOrder OrderBy {
      get; set;
    } = new AccountStatementSortOrder();

  } // class AccountStatementQuery


  public class AccountStatementSortOrder {

    public AccountStatementOrder SortType {
      get; set;
    } = AccountStatementOrder.AccountingDate;


    public AccountStatementOrderType OrderType {
      get; set;
    } = AccountStatementOrderType.Ascending;

  }


  public class AccountStatementQueryEntry {

    public TrialBalanceItemType ItemType {
      get; set;
    } = TrialBalanceItemType.Entry;


    public string LedgerUID {
      get; set;
    }  = string.Empty;


    public string LedgerNumber {
      get; set;
    } = string.Empty;


    public string LedgerName {
      get; set;
    } = string.Empty;


    public string CurrencyCode {
      get; set;
    } = string.Empty;


    public string AccountNumber {
      get; set;
    } = string.Empty;


    public string AccountNumberForBalances {
      get; set;
    } = string.Empty;


    public string AccountName {
      get; set;
    } = string.Empty;


    public string SubledgerAccountNumber {
      get; set;
    } = string.Empty;


    public string SectorCode {
      get; set;
    } = string.Empty;


    public decimal InitialBalance {
      get; set;
    }


    public decimal CurrentBalanceForBalances {
      get; set;
    }


    public DateTime LastChangeDateForBalances {
      get; set;
    } = DateTime.Now;


    public string DebtorCreditor {
      get; set;
    }

  }  // class AccountStatementEntry

} // namespace Empiria.FinancialAccounting.Reporting.AccountStatements.Adapters
